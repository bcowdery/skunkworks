Experimental Application Development Platform
==============================================

Experimental platform for building large multi-language, distributed applications. 

The primary goal is to the ease the barier to entry and provide developers with a quick and easy way to get up and running with the entire platform, with a minimum of installed dependencies and working knowledge. This project utilizes a task runner to script out complex workflows and provide a simple command-line interface. 

**Requirements:**
* [Docker](https://www.docker.com/products/docker-desktop)
* [Taskfile](https://taskfile.dev)

&nbsp;

> 💡 Run `.\bootstrap` on windows to bootstrap the Docker & Taskfile installation.

&nbsp;

# Goals

| Goal                                 | How                     | Success |
|--------------------------------------|-------------------------|---------|
| **Low barrier to entry**<br/> Developers should be able to get up and running without needing to install and configure services for each aspect of the application stack (e.g., SQL Server, Reddis, Azure emulators, dotnet core versions etc.). Out of the box, developers should be able to get up and running with a single command. | Docker Compose + Task Runners | ✅ 🎉 |
| **Dotnet core live-reload**<br/> Provide the option to run dotnet core applications using the `watch` command for live reloading of changes. Changes to source code should be refleted in the running container allowing fast turaround time and testing of changes. This needs to be an opt-in feature allowing developers to enable a "development mode" for just the service they're working on. | Implemented in `Dockerfile.develop` image files and docker compose development overrides  | ✅ |
| **Consumer testing**<br/> MassTransit async message consumers need to be unit-testable! The bulk of our testing should be done as unit tests, with a few integration tests to ensure that messages can be delivered and processed when "sent" over the bus.  | NUnit `ConsumerFixture` using the MassTransit in-memory test harness. | ✅ | 
| **Send/Publish endpoint testing**<br/> Need a way to assert that messages have been sent to a publish or send endpoint when verifing unit test. Much of this behaviour lives in MassTransit extension methods and message content is set as an anonymous type - making it difficult to unit test services that are sending messages. Goal is to have something that can be passed into the unit, with asserts to verify the sent message content. | Mock send endpoints and custom extension methods created to verify send and publish operations. | ✅ | 
| **EF DbContext testing**<br/> EntityFramework `DbContext` classes cannot be mocked out - using InMemory databases also has some drawbacks as it does not expose issues in generated SQL & possible client-side evaluation errors. Need to support testing data repositories against a relational database, and an easy way to construct a DbContext & provide setup data for unit tested services. | Use [Bogus](https://github.com/bchavez/Bogus) data generation and custom extension methods setting up EF.<br/> `DbContextFactory` allows easy creation of a context for unit testing. | ✅ |
| **Load Testing**<br/> Developers should be able to load test services easily with a single command. Requires no local dependencies and can be easily scripted. Load tests should be ran against the running application stack and not as one-off builds or unit tests. _The goal of these tests is to proove that the service can stand up to expected load._ | [k6.io](http://k6.io) load tests embedded in an ad-hoc container. | ✅ 🚀 |
| **Health checks**<br/> All services should report health checks. API endpoints should expose a `/health` endpoint that can be called to inspect the status of the service and it's background workers. | Built on top of dotnet core healthchecks, can publish to application insights and datadog for reporting. | ✅ |

&nbsp;

# Getting Started

You can build, package and run the entire solution with `up`.

```shell
$ task update-local-packages # if not using remote nuget repo
$ task up
```

&nbsp;

## Tasks

Available tasks for this project:

| Task      | Description                                             |
|-----------|---------------------------------------------------------|
| `up`      | Bring up the solution                                   |
| `down`    | Bring down the solution                                 |
| `restart` | Restart the solution                                    |
| `status`  | Display the status of all containers in the solution    |
| `logs`    | Display logs from all containers in the solution        |
| `build`   | Builds all container images                             |
| `pull`    | Pulls the most recent container images                  |
| `push`    | Pushes container images to remote repositories          |
| `prune`   | Prune obsolete remote git branches and local containers |
| `nuke`    | Nuke all data files and reset the solution              |

&nbsp;

# Docker Compose

## Development Overrides

All services can be overridden by editing the `docker-compose.local.yml` file. This allows you to enable development & debugging utilities on a service-by-service basis. Developers are able to customize their environment to focus their development efforts on a small part of the platform 

> 🚀 Developer overrides are **local only** and are excluded from commits by the `.gitignore` file.


### Example

In this example, uncommenting the `shipyard_worker` section enables hot-reloading and Visual Studio debugging of the `Shipyard.Worker` dotnet core application. The solution source root is mapped to the image as a mounted volume so that the docker container can monitor the file system for changes. Developer user secrets are also mounted to provide safe storage of secrets via the `dotnet user-secrets` tool.

**docker-compose.local.yml:**
```yml
version: '3'

services:
  shipyard_worker:
    image: shipyard-worker:dev
    build: 
      dockerfile: src/Shipyard.Worker/Dockerfile.develop
    volumes:
      - ./sources/shipyard/src/:/app/src/        
      - $APPDATA/Microsoft/UserSecrets/:/root/.microsoft/usersecrets/
      - $HOME/.microsoft/usersecrets/:/root/.microsoft/usersecrets/
```

&nbsp;

# Services

| Service          | Role                                 | Local Port | Private Port | URL                     |
|------------------|--------------------------------------|------------|--------------|-------------------------|
| Nginx            | Reverse proxy for API containers     | 80         | 80           | https://localhost       |
| SQL Server       | Database server                      | 11433      | 1433         |                         |
| RabbitMQ         | Message broker for pub/sub messaging | 15672      | 5672         | https://localhost:15672 |
| Azureite         | Azure storage emulator               | 10000      | 10000        |                         |


&nbsp;

> ✨ Port numbers, usernames and passwords are all defined in the environment `.env` file.

&nbsp;

## Mission Control

Mission Control is a top level admin app and system monitoring dashboard. 

**Url:** http://localhost/admin/

## Shipyard

Shipyard is a service designed to handle all external messaging traffic using external email and SMS providers. 

**API Endpoint:** http://localhost/jobs/

## Port Authority

Job and task management service. 
Intended for audit tracking and API driven orchestration of background tasks.

**API Endpoint:** http://localhost/messaging/
