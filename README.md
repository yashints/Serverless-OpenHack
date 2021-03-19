# Serverless Open Hack Challenge

This repo contains the code for ratings API which should be implemented using Azure Functions.

## Pre-requisites

* VS Code
* Azure Function Extension
* Azure Function Tools
* Dotnet Core 3.1

## Running locally

Pull down the code locally, then create a `local.appsettings.json` and use below template to fill in the values:

```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "YOUR_AZURE_STORAGE_CONNECTION_STRING",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "ProductAPIURL": "https://serverlessohproduct.trafficmanager.net/api/GetProduct?productId={0}",
        "UserAPIURL": "https://serverlessohuser.trafficmanager.net/api/GetUser?userId={0}"
    }
}
```

Replace the value of the `AzureWebJobsStorage` with your storage account connection string and run the the function using:

```bash
func start --build
```