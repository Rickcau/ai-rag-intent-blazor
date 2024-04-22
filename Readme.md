# api-ai-rag-intent-blazor

## Summary
In this example we leverage a Blazor Web App as the front which leverages our Azure Function for AI Chat Services.  Big thanks to Bill Reiss for the Blazor App (Web Assembly).  We will also be adding a Blazor Server Side Web App and additional features over time.

## Goal
The goal is to provide a super simple UI (somthing other than React, NextJS or others), hence is why we decided on Blazor. We will make use of **Intent Recognition** in conjunction with RAG (Retreval Augmentation Generation) using Semantic Kernel (1.4.0) an Isolated Azure Function as well as Speech Service.  It leverages all the common patterns e.g. dependecy injection, SK Plugins/Functions, AutoInvoke etc. 

## Patterns 
- RAG (Retreval Augmentation Generation) using Semantic + Vector Seach
- [Intent Classification / Recognition](./Intent.md) <- I highly recommend reading this.
- SK Functions & Plugins

## Technologies
- Azure Function (Isolated) as REST Endpoint
- Semantic Kernel (1.4.0)
- Azure AI Search (Semantic + Vector)
- Azure Speech Service
- Dependency Injection
- Azure Open AI

## Bicep Deployment and Scripts
If you would like to deploy the resouces for this example you can use the .bicep templates, just read the notess in the files to understand how to use them.

You can use the UploadDocument.ps1 to upload the employee_handbook.pdf to the Storage Account Container.  The CreateIndex.ps1 script is not complete yet so you will need to refactor it to get it working.  For now, you can simply use the **Import and Vectorize** option from the Azure Portal to build the index that will be used in this example.

### Request Body
The function expects you to pass a JSON body with the following information:

        ~~~
              {
                 "userId": "stevesmith@contoso.com",
                 "sessionId": "12345678",
                 "tenantId": "00001",
                 "prompt": "Can you tell what my healtcare benefits are for Northwinds"
              }
        ~~~

The client that is calling the Function will pass these values in and later they can be used to store/retreive prompt history. In a future version I will add support to storing/retreving of ChatHistory using CosmosDB.

## Steps to deploy the Azure Resouces needed for this example
### Create an Azure Resouce Group
1. Open a Terminal Windows in Visual Studio and run the following command
   
   ~~~
       az group create --name AIRagIntentResourceGroup --location westus
   ~~~

2. Open the .bicep files and read the instructions for details on how to deploy the resouces using the templates.
