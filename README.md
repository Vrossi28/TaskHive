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
  - [Running the database](#1---running-the-database)
  - [Running the Web API](#2---running-the-web-api)
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
- Docker

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

### 1 - Running the database
Using powershell inside the root directory of the solution, run the following command:

```bash
$ docker-compose up -d task.hive.db
```
It will start the docker container for SQL server, where Task Hive database is located. After the container is successfully started, run the following command to identify the container id:
```bash
$ docker container ls
```

You should get a result like:
```bash
CONTAINER ID   IMAGE                      COMMAND                  CREATED          STATUS         PORTS                    NAMES
3672b4bea7e3   taskhivecore-task.hive.db   "/opt/mssql/bin/perm…"   7 minutes ago   Up 6 minutes   0.0.0.0:1433->1433/tcp   taskhivecore-task.hive.db-1
```

With the container ID, it is necessary to run the following command, in the field `[CONTAINER-ID]` use e.g. `3672b4bea7e3`:
```bash
$ docker exec -it [CONTAINER-ID] /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P TaskHive@dm1n -i /docker-entrypoint-initdb.d/create-database.sql
```

This command will create the `TaskHive` database inside the SQL Server container.

### 2 - Running the Web API
Once the SQL Server container is properly configured, next step is start the core of Task Hive, for that. First make sure to properly configure the following environment variables at `docker-compose.yml` to allow you to properly upload files to your Amazon S3 bucket and also to receive email notifications:
* AwsConfiguration__AwsAccessKey=**AWSACCESSKEY**
* AwsConfiguration__AwsSecretKey=**AWSSECRETKEY**
* AwsConfiguration__AwsBucketName=**AWSBUCKETNAME**
* EmailService__SenderEmail=**senderemail@outlook.com**
* EmailService__SenderPassword=**senderpassword**
* EmailService__WebAppUrl=**http://productionurl.com**

Next step is, again in the root of the solution, using powershell run the following command:
```bash
$ docker-compose up -d task.hive.core
```

Run the following command to make sure that both containers are running properly:
```bash
$ docker container ls
```

It should get a result like:
```bash
CONTAINER ID   IMAGE                        COMMAND                  CREATED             STATUS          PORTS                    NAMES
cdd9073ef488   taskhivecore-task.hive.core   "dotnet TaskHive.Web…"   7 minutes ago       Up 6 minutes    0.0.0.0:8080->80/tcp     taskhivecore-task.hive.core-1
3672b4bea7e3   taskhivecore-task.hive.db     "/opt/mssql/bin/perm…"   10 minutes ago   Up 9 minutes   0.0.0.0:1433->1433/tcp   taskhivecore-task.hive.db-1
```

Now, to send http requests to the API it's needed to send through the URL `http://localhost:8080/api/[DESIRED-ENDPOINT]`

# How to setup development environment

You are welcome to contribute for this project if you want to. To setup the development environment follow the steps below:

## Database
As database for the application you can use either your local SQL Server or the Docker container SQL Server.

### Local database option
* Go to `TaskHive.WebApi\appsettings.json`
* Define your connection strings according to your SQL Server configuration:
```bash
"ConnectionStrings": {
    "DefaultConnection": "Server={YOUR SERVER};Initial Catalog=TaskHive;Persist Security Info=False;User ID={YOUR USER};Password={YOUR USER PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
  },
```

### Container database option

* Make sure to have the `task.hive.db` container running.
* Go to `TaskHive.WebApi\appsettings.json`
* Define your connection strings according the same as the docker-compose.yml file (docker-compose.yml is for your reference, it will not be considered if you run the project outside the task.hive.core container):

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
