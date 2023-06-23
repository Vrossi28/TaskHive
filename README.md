# Task Hive Server

![](https://img.shields.io/badge/Updated-June%20%204,%202023-lightgrey.svg)
[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0) [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](#contributing)

#### [Task Hive](https://taskhive.live "Task Hive") is a project management tool that provides options for creating, updating, and deleting workspaces and tasks. Users can manage workspaces by adding team members, sharing files, and exchanging messages. Tasks can be created, assigned to team members, and tracked for progress.

## Content

- [About](#about)
  - [Tech stack](#tech-stack)
  - [Workspaces](#within-workspaces-its-possible-to)
  - [Tasks](#within-tasks-its-possible-to)
- [Test environment](#how-to-run-test-environment)
  - [Pre-requisites](#pre-requisites)
  - [Setting up the Web API](#1---setting-up-the-web-api)
    - [Generating a dev certificate](#11---generating-a-dev-certificate)
  - [Running the containers](#2---running-the-containers)
  - [How to check the documentation](#3---how-to-check-the-documentation)
- [Development environment](#how-to-setup-development-environment)
  - [Database](#database)
    - [Local Database](#local-database-option)
    - [Container Database](#container-database-option)
  - [Running the project](#running-the-project)
  - [Changes in database](#changes-in-database)
    - [Create migration](#create-migration)
    - [Update database](#update-database)
- [Contributing](#contributing)
  - [Commits](#commits)
  - [Issues](#issue)
- [Contributors](#contributors)

## About

Task Hive Server is the backend of Task Hive, a project management software created by me for research purpose.
**You are most welcome to contribute for this project** :grinning:

### Tech Stack

- .NET Core 6.0
- SQL Server
- Amazon Web Services S3
- Hangfire
- SignalR
- Docker
- Jenkins
- Google OAuth

For complete architecture details please check my LinkedIn post: [LinkedIn](https://www.linkedin.com/posts/vinicius-rossi-br_devops-software-cloud-activity-7067198947294900225-oSgQ?utm_source=share&utm_medium=member_desktop, "LinkedIn")

### Within workspaces it's possible to:

- Create tasks.
- Invite external users to your workspace.
- Invite users which are already members of your company but not members of your workspace.
- Modify user roles to manage what can be done by a user.
- Manage task start time, end time and progress using Gantt View.

### Within tasks it's possible to:

- Assign tasks for another user inside your workspace.
- Log time spent in a task.
- Modify task type.
- Modify task status.
- Resolve a task.
- Attach files.
- Download attached files.
- Delete attached files.
- Create child tasks.
- Modify estimated time for a task to be completed.
- Add comments to a task.

# How to run test environment

#### Pre-requisites

- Docker properly installed.
- Pull all project files.
- .NET 6.0 (For generate dev certificate)

### 1 - Setting up the Web API

### 1.1 - Generating a dev certificate
Run the following commands in order to create a dev certificate which will be used to allow HTTPS and subsequently, Google Authentication. Replace `$CREDENTIAL_PLACEHOLDER$` with a desired password for you certificate.

```bash
dotnet dev-certs https -ep "$env:USERPROFILE\.aspnet\https\aspnetapp.pfx"  -p $CREDENTIAL_PLACEHOLDER$
dotnet dev-certs https --trust
```

Make sure to properly configure the following environment variables at `docker-compose.yml` to allow you to properly upload files to your Amazon S3 bucket, to receive email notifications and authenticate with Google:

- ASPNETCORE\_Kestrel\_\_Certificates\_\_Default\_\_Password=**DEVCERTIFICATEPASSWORD**
- AwsConfiguration\_\_AwsAccessKey=**AWSACCESSKEY**
- AwsConfiguration\_\_AwsSecretKey=**AWSSECRETKEY**
- AwsConfiguration\_\_AwsBucketName=**AWSBUCKETNAME**
- EmailService\_\_SenderEmail=**senderemail@outlook.com**
- EmailService\_\_SenderPassword=**senderpassword**
- EmailService\_\_WebAppUrl=**http://productionurl.com**
- Authentication\_\_Google\_\_ClientId=**GOOGLECLIENTID**
- Authentication\_\_Google\_\_ClientSecret=**GOOGLECLIENTSECRET**

### 2 - Running the containers

Once the Web API environment variables are properly configured, next step is start the services.
For that, using powershell in the root of the solution run the following command:

```bash
$ docker-compose up
```

Run the following command to make sure that both containers are running properly:

```bash
$ docker container ls
```

It should get a result like:

```bash
CONTAINER ID   IMAGE                     COMMAND                  CREATED         STATUS         PORTS                           NAMES
9eaf92ab0089   taskhive-task.hive.core   "dotnet TaskHive.Web…"   5 minutes ago   Up 5 minutes   443/tcp, 0.0.0.0:8080->80/tcp   taskhive-task.hive.core-1
b428e0001854   taskhive-task.hive.db     "/opt/mssql/bin/sqls…"   5 minutes ago   Up 5 minutes   0.0.0.0:1433->1433/tcp          taskhive-task.hive.db-1
```

Now, to send http requests to the API it's needed to send through the URL `https://localhost:8080/api/[DESIRED-ENDPOINT]`

### 3 - How to check the documentation

All endpoints of the solution are documented with swagger. Since you have started Task Hive Core you can check all endpoints, its purposes, how to send a request, data models, etc, by opening the URL `https://localhost:8080/swagger/index.html` in your browser.

# How to setup development environment

You are welcome to contribute for this project if you want to. To setup the development environment follow the steps below:

## Database

As database for the application you can use either your local SQL Server or the Docker container SQL Server.

### Local database option

- Go to `TaskHive.WebApi\appsettings.json`
- Define your connection strings according to your SQL Server configuration:

```bash
"ConnectionStrings": {
    "DefaultConnection": "Server={YOUR SERVER};Initial Catalog=TaskHive;Persist Security Info=False;User ID={YOUR USER};Password={YOUR USER PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
  },
```

### Container database option

- Make sure to have the `task.hive.db` container running.
- Go to `TaskHive.WebApi\appsettings.json`
- Define your connection strings according the same as the docker-compose.yml file (docker-compose.yml is for your reference, it will not be considered if you run the project outside the task.hive.core container):

##### `docker-compose.yml`

```bash
task.hive.core:
    ...
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=task.hive.db;Initial Catalog=taskhive;Persist Security Info=False;User ID=sa;Password=TaskHive@dm1n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;
    ...
```

##### `appsettings.json`

```bash
"ConnectionStrings": {
    "DefaultConnection": "Server=task.hive.db;Initial Catalog=taskhive;Persist Security Info=False;User ID=sa;Password=TaskHive@dm1n;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
  },
```

# Running the project

After configuring the connection to the database you can run the project using the command:

```bash
$ dotnet run --configuration debug
```

# Changes in database

This project uses code first approach. If you want to change database entities, it's necessary to properly configure it at `TaskHiveContext` class.

## Create migration

After you do your changes, create the migration by giving a short description and running the command:

```bash
$ dotnet ef migrations add "ShortDescription" --project TaskHive.Infrastructure --startup-project TaskHive.WebApi --output-dir Persistence\Migrations --verbose
```

## Update database

After create the migration, update your database using the command:

```bash
$ dotnet ef database update --project TaskHive.Infrastructure --startup-project TaskHive.WebApi --verbose
```

# Contributing

Pull requests are very welcome!
I would love to hear your feedback and suggestions in the issue tracker: **https://github.com/Vrossi28/TaskHive/issues**

## Commits

If you want to contribute by implementing code changes, after you do your changes please open the pull request and it will be reviewed :star:.

## Issues

If you found a bug or want to request a feature, please report it by creating a new issue.

## Contributors

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
    <tbody>
        <tr>
            <td align="center">
                <a href="https://www.linkedin.com/in/vinicius-rossi-br/?locale=en_US">
                    <img src="https://avatars.githubusercontent.com/u/57651321?s=400&u=8d5bd045263f2a42ad3e3a4a61dbd26270501ea3&v=4" width="100px;" alt="Vinicius Rossi"/>
                    <br />
                    <sub><b>Vinicius Rossi</b></sub>
                </a> 
            </td>
        </tr>
    </tbody>
</table>

## [License](LICENSE)
