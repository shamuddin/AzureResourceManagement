# 🌐 Azure Resource Management via ASP.NET Core Web API

Welcome to the **Azure Resource Management** repository! 🚀 This project demonstrates how to automate the creation and management of Azure resources using an ASP.NET Core Web API, leveraging .NET 8.0 for seamless and efficient cloud operations.

## ✨ Overview
This repository provides a comprehensive implementation of an ASP.NET Core Web API that interacts with Azure Resource Manager (ARM) to programmatically create and manage Azure services. The project simplifies cloud resource management and streamlines infrastructure provisioning.

### Key Features
- 🔧 **Automated Azure Resource Creation**: Effortlessly create resources such as Azure SQL Server, App Services, and more.
- ⚡ **Seamless Integration**: Built with ASP.NET Core 8.0 for high performance and reliability.
- 🌱 **Extensible and Scalable**: Designed to support additional Azure services and configurations.
- 🔒 **Secure Practices**: Implements best practices for secure and efficient interactions with Azure APIs.

## 📋 Prerequisites
Ensure you have the following before starting:

1. **Azure Subscription**: A valid Azure account and subscription.
2. **Development Environment**:
   - 🛠️ .NET 8.0 SDK
   - 🖥️ Visual Studio 2022 or any compatible IDE
3. **Azure CLI**: For managing Azure resources and configuring your environment.

## 📚 Reference Articles
To get the most out of this project, check out these essential articles:

- [📝 Building an ASP.NET Core Web API (.NET 8.0) for Seamless Azure Service Management Automation](https://medium.com/@shamuddin/building-an-asp-net-core-web-api-net8-0-for-seamless-azure-service-management-automation-f7ba9c276ae0) (Prerequisite)
- [📝 How to Create an Azure SQL Server Using Azure Server Management via ASP.NET Core Web API](https://medium.com/@shamuddin/how-to-create-an-azure-sql-server-using-azure-server-management-via-asp-net-core-web-api-54eeec87cf38) (Follow-up Guide)

## 🚀 Getting Started
Follow these steps to set up and run the project:

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/shamuddin/AzureResourceManagement.git
cd AzureResourceManagement
```

### 2️⃣ Configure the Application
Update the `appsettings.json` file with your Azure credentials and subscription details:

```json
{
  "Azure": {
    "TenantId": "<your-tenant-id>",
    "ClientId": "<your-client-id>",
    "ClientSecret": "<your-client-secret>",
    "SubscriptionId": "<your-subscription-id>"
  }
}
```

### 3️⃣ Run the Application
Use the following commands to build and run the project:

```bash
dotnet build
dotnet run
```

Your API will be available at `https://localhost:5001` or `http://localhost:5000`. 🌟

## 🛠️ How It Works
This project uses Azure SDKs to interact with Azure Resource Manager, allowing you to create, update, and manage resources programmatically. Explore the Medium articles for in-depth guides and examples on implementing specific resources like Azure SQL Server.

## 🤝 Contributing
We welcome contributions! If you have ideas for improving this project or adding new features, feel free to open an issue or submit a pull request. 💡

## 📄 License
This project is licensed under the [MIT License](LICENSE). Feel free to use and modify it as needed.

## 📬 Contact
For questions, feedback, or collaboration opportunities, feel free to reach out:
- [GitHub](https://github.com/shamuddin) 🌟
- [Medium Articles](https://medium.com/@shamuddin) ✍️
