# Exploring-Azure-Container-Apps
 
This book offers a hands-on, practical application built using a microservices architecture pattern. The application centers around an expense management solution, enabling external users (employees) to submit expenses, which are then reviewed by internal users (administrators). The solution will consist of multiple microservices, each with specific capabilities, demonstrating how ACA, Dapr, and KEDA can simplify microservice-based architectures. The diagram below shows the architecture of the solution that we will build throughout the book's chapters(see figure 1-4). The main components of the solution are as follows:

ACA Web App - Frontend is a simple ASP.NET Blazor Server app that accepts requests from external users to manage their expenses, and it allows internal users to review submitted expenses. It invokes the component "ACA WebAPI-BFF" endpoints via HTTP or gRPC.

ACA Web API - BFF is a minimal backend Web API which contains the business logic of expenses management service, data storage, data retrieval, and publishing messages to Azure Service Bus Topic.

ACA Processor - Backend is a minimal Web API that functions as an event-driven backend processor. It is responsible for sending emails to expense owners based on messages received from an Azure Service Bus Topic when there are changes in the expense status. Additionally, it handles events from external systems using Dapr Input bindings and triggers external events through Dapr Output bindings.

ACA Meilisearch - Service is a minimal backend Web API which hosts the open source lightweight search engine (Meilisearch). It is responsible to store submitted expenses of external users and allow full text search for internal users.

ACA Job Indexer - Service is a console application deployed as an ACA Job. It is responsible for indexing all submitted expenses and any changes to them into the search engine.

ACA Job Scheduled - Service is a console application deployed as an ACA Job, triggered by a CRON schedule (a nightly job). This service is responsible for creating aggregates of the submitted expenses and storing the aggregated data in Azure Redis Cache for future use by a reporting dashboard.

Autoscaling rules using KEDA are configured in the "ACA Processor - Backend" service to scale out/in replicas based on the number of messages in the Azure Service Bus Topic.

Azure Container Registry is used to build and host container images and deploy images from ACR to ACA.

Application Insights and Azure Log Analytics are used for Monitoring, Observability, and distributed tracings of ACA.

Authentication and authorization are applied on the ACA Web App - Frontend to control who can access the solution and control the available features of the signed in user.
