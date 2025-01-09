using AzureManagementAPI.Models;
using AzureManagementAPI.Service;
using AzureManagementAPI.Service.Helper;
using AzureManagementAPI.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

#region AzureServices
builder.Services.Configure<AzureAdConfig>(builder.Configuration.GetSection("AzureADAppConfig"));
builder.Services.Configure<AzureResourceConfig>(builder.Configuration.GetSection("AzureResourceConfig"));
builder.Services.AddScoped<IGetAccessToken, GetAccessToken>();
builder.Services.AddScoped<IAzureSQLServer, AzureSQLServer>();
builder.Services.AddScoped<IAzureResourceGroup, AzureResourceGroup>();
builder.Services.AddScoped<IAzureStorageAccount, AzureStorageAccount>();
#endregion

#region ADConfig
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureADAppConfig"));
#endregion

builder.Services.AddEndpointsApiExplorer();

string appTitle = "Azure Service Management";
string appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(1);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = appTitle, Version = appVersion });

    // Set the comments path for the Swagger JSON and UI.
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    string tenantId = builder.Configuration["AzureADAppConfig:TenantId"];
    string scopes = builder.Configuration["DefaultAccessTokenScopes"];

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Description = "Azure AAD Authentication",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { scopes, "Access API" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },
                        new[] { scopes }
                    }
                });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsApi",
        builder => builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});


var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseSwagger();

app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", $"{appTitle} {appVersion}");
    o.OAuthClientId(builder.Configuration["AzureADAppConfig:ClientId"]);
    o.OAuthUsePkce();
    o.OAuthScopeSeparator(" ");
    o.DocumentTitle = appTitle;
});


app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsApi");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();