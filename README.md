# Proposal for Cash Flow Management Service

## Introduction

With the aim of helping a merchant manage their daily cash flow and track their debit and credit transactions, we propose a Cash Flow Management Service. This service will allow the merchant to input their transactions and generate a consolidated daily balance report.

## Solution Description

Our Cash Flow Management Service will provide the following functionalities:

- Transaction Management: The service will enable the merchant to input their daily transactions, including debits and credits.
- Consolidated Daily Report: The service will generate a consolidated daily balance report, displaying the daily transactions and the merchant's balance.

## Requirements

1. Frontend web development team in React technologies.
2. Backend web development team in C# technology.
3. Desktop development team in C# technology.

## Technical Requirements

- 1 Azure subscription for Active Directory (AD).
- 1 AWS subscription for hosting the web solution.
- 1 DevOps organization for versioning source code and managing deployments in the solution's environments.
- 2 computers for development and testing of the POS application.

## Solution Design

For the proposed Cash Flow Management Service, we will use a combination of SQL Server, AWS, and C# to provide a scalable and reliable solution.

## Architecture Overview

The solution will consist of three main components:

1. Frontend: A web-based application that allows the merchant to input their daily transactions.
2. Backend: A set of APIs written in C# that will handle the merchant's transactions and generate consolidated daily reports.
3. POS: A local application installed on the point of sale computer.

## Technical Details

![image](https://user-images.githubusercontent.com/19389281/227325768-965a7fd2-5caf-4b86-ac93-3b7c89fd7a71.png)

## POS Desktop

The POS will be an application installed on the operator's computer or mobile device, allowing uninterrupted operation in case of internet signal issues. All transactions will be stored locally until the internet connection is reestablished, at which point they will be synchronized with the management platform. This application will be built using Electron.js and will reuse modules already developed in the frontend.

## POS Database

The database will be SQL-based. We will use SQL Express to provide a scalable and reliable database solution. We will also use local folders to store any files or documents associated with the merchant's transactions.

## Frontend

The frontend will be a web-based application that allows the merchant to access reports, create products, categories, and manage their sales. We will use a combination of HTML, CSS, and JavaScript to build the frontend. We will also use a frontend framework like Angular or React to provide a responsive and user-friendly interface.

## Backend

The backend will consist of a set of APIs written in C#. These APIs will handle the merchant's transactions and generate consolidated daily reports. We will use a microservices architecture to ensure scalability and flexibility. We will also use Lambda Functions to provide serverless computing and ensure cost efficiency.

## Cloud Database

The database will be a DynamoDB hosted on AWS. We will use DynamoDB to provide a scalable and reliable database solution. We will also use Amazon S3 to store any files or documents associated with the merchant's transactions.

## Integration

The frontend and backend will communicate through REST APIs. The backend APIs will store the merchant's transactions in DynamoDB. The backend APIs will also generate consolidated daily reports, which will be stored in Amazon S3. The frontend will retrieve the daily reports from S3 and display them to the merchant.

The POS will instantly synchronize with the cloud whenever there is an internet connection. When the signal fails, the information will be retained until the internet connection is restored.

## Deployment

The solution will be deployed on AWS. We will use S3 to host the frontend's static files and the backend's APIs. We will also use Amazon to host DynamoDB. Amazon S3 will be used to store any files or documents associated with the merchant's transactions.

The POS will be deployed as an executable. Upon the first launch, the cloud information will be automatically transferred to the POS, eliminating the need to recreate records already created online.

# Backend API

The proposed Cash Flow Management Service will provide a set of APIs to handle the merchant's transactions and generate consolidated daily reports. Here are the details of the endpoints we will provide for the MVP:

- POST /publish

    This endpoint will allow the merchant to add, update, or delete a transaction. The request body must contain a message with the transaction details, such as the transaction type (debit or credit), amount, and description. The endpoint will return the request ID. Upon completion, the subscribing client will receive a notification with the same ID containing the operation result.

## Integration

The solution provides a set of APIs to handle the merchant's transactions and generate consolidated daily reports. These endpoints will allow the merchant to effectively manage their daily cash flow. By using these APIs, other systems can add, retrieve, update, and delete transactions, as well as retrieve consolidated daily reports for specific dates.

# Overview

![image](https://user-images.githubusercontent.com/19389281/227305160-c5e61089-664c-4a2e-a423-a3c1b8b6a662.png)

# Microservices

Miro Board:
[Link to Miro Board](https://miro.com/app/board/uXjVMaXQeaY=/?share_link_id=411079338218)

![image](https://user-images.githubusercontent.com/19389281/227304927-39c6f3d1-d73f-4755-8b64-c7f0a6deef1a.png)

# Deliverables

## Introduction

This project was proposed as part of a selection process for the position of architect with the goal of modernizing monolithic applications and implementing best practices in resilience, security, performance, maintainability, and future readiness. The developed project deals with a merchant's daily transactions, who needs to manage their cash flow and daily closure.

Although not all requested deliverables have been provided for this evaluation, depending on trade-offs, the architecture does not always encompass all activities at first, varying greatly according to the architect's short, medium, and long-term vision. This work was developed to exemplify the skills required for the architect position. The most relevant activities of this project were elaborated in accordance with the requirements of the position.

Like all architectural work, methodologies and base resources have been developed so that the project can be delivered to development teams using common practices but following established standards. These artifacts developed can be scaled and distributed independently into microservices in an event-driven serverless platform architecture, without affecting effort, deadlines, code clarity, and maintenance complexity of the development team.

It is important to note that although there are characteristics of Over Engineering, this delivered test is also a basis for the proposal to modernize the current monolithic application, mentioned in the interview, considering similar experiences gained in other projects.

## LGPD / Privacy

We have adopted strict security measures to ensure the privacy and protection of personal data in compliance with LGPD. In addition, we have implemented best practices for monitoring and logging, allowing the operations team to quickly identify and resolve issues without compromising the privacy and protection of personal data in use in the application at the time of inspection.

## Unit Tests

Unit tests have been developed as one of the practices to ensure the quality of the code produced. Unit tests are a type of automated test that verifies the behavior of a code unit, such as a function or method, in isolation from the rest of the system. They are important to ensure that each part of the system is functioning correctly and in accordance with the established requirements.

## Black Box Testing

In addition to unit tests, Black Box tests ensure that the internal integration of the developed modules is appropriate before conducting integration tests. This test examines code units as a whole. Black Box tests should be automatically executed to identify if the development is ensuring adequate end-to-end behavior, without being susceptible to availability issues of the microservices, as in the case of integration tests. These tests can be used by build pipelines to prevent check-in policies and code coverage defined from being impacted when an external service is unreachable.

## Code Coverage

To ensure the quality of tests in new developments, a minimum code coverage of 80% has been defined. This means that at least 80% of the code needs to be tested to ensure that the application has an adequate level of test coverage. This approach allows the team to have more confidence in the software and ensures that potential errors are identified early, reducing the cost and time required for corrections.

Code coverage will be automatically assessed by the pipeline with each code delivery to the source control. If coverage is below 80%, the code delivery will be rejected, and the developer and their lead will be notified.

## CI/CD Pipelines

The CI/CD pipeline implemented in this project has been configured to be multi-environment, multi-project, flexible, and secure, ensuring that the process of delivering features adheres to established best practices. Each environment (DEV, UAT, PRD) has variable groups to make the pipeline generic and easy to use with other environments. In addition, the pipeline uses Azure Vault to store secrets, which are obtained at runtime from the pipeline and automatically inserted into the solution's appsettings, eliminating the need for manual intervention, exposure of secrets, and the use of third-party tools for the task. This approach ensures greater security and simplicity in the feature delivery process.

## Branching

To manage the development of microservices and ensure code quality, the use of Gitflow has been established. In addition, some specific adaptations of the solution, such as the implementation of serverless, unit tests, black box tests, and code coverage, have been added. These branching policies ensure a more organized and efficient workflow for development and feature delivery.

## Check-In

To maintain code quality and integrity, it has been established that code check-in is only allowed on the feature and hotfix branches. This prevents the mixing of untested or incomplete code with the main version of the software. Feature branches allow developers to work on new features without affecting the main version of the software, while hotfix branches are used to fix critical issues in the current version. These check-in policies ensure code quality and minimize potential conflicts during the continuous integration process.

## Testing

In this project, the following testing policies will be adopted:

- Unit tests and black box tests will be automatically executed after the completion of feature development.
- Unit tests will ensure that the system is reliably modular and expedite the identification of technical failures.
- Black box tests will be implemented without querying external data to identify if development is ensuring appropriate end-to-end behavior. To achieve this, all external data must be mocked according to the structure provided in the solution.

## Microservices

Microservices are an important part of an event-based architecture, and as such, they need to be developed with patterns and consistency in mind. Predictability is crucial because microservices can add extra layers of complexity to the architecture, and a lack of predictability can lead to scalability and performance issues. One way to standardize the behavior of microservices is to define standards for the HTTP status codes they return. This helps ensure that developers of other services can handle responses consistently and predictably.

Expected Responses:

- 200 - Ok: Indicates that the request was successful, and the response content is as expected.
- 400 - Bad Request: Indicates that the client's request is invalid or malformed and cannot be processed by the server.
- 401 - Unauthorized: Indicates that the client is not authenticated and does not have permission to access the requested resource.
- 403 - Forbidden: Indicates that the client does not have permission to access the requested resource, even if authenticated.
- 500 - Internal Server Error: Indicates that an internal server error occurred while processing the request.

Additionally, another way to ensure standardization of the objects that microservices return is to package them in a standardized way to ensure consistency.

## Development Patterns

For the development of this solution, DRY, YAGNI, SOLID, SAGA have been used. It is important to mention that this solution will benefit from a modular and scalable design, following architectural best practices such as Separation of Concerns (SoC), which allows each module of the application to have a single well-defined responsibility. Additionally, the solution was built using software design patterns that favor code reuse, simplicity
