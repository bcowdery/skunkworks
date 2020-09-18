Experimental Application Development Platform
==============================================

Experimental platform for building large multi-language, distributed applications. 

The primary goal is to the ease the barier to entry and provide developers with a quick and easy way to get up and running with the entire platform, with a minimum of installed dependencies and working knowledge. This project utilizes a task runner to script out complex workflows and provide a simple command-line interface. 

**Requirements:**
* [Docker](https://www.docker.com/products/docker-desktop)
* [Taskfile](https://taskfile.dev)

&nbsp;

# Getting Started

You can build, package and run the entire solution with `up`.

```shell
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

## Development Overrides

All services can be overridden by editing the `docker-compose.local.yml` file. This allows you to enable development & debugging utilities on a service-by-service basis. Developers are able to customize their environment to focus their development efforts on a small part of the platform 

Developer overrides are **local only** and are excluded from commits by the `.gitignore` file.


### Example

> 🚀 In this example, uncommenting the `shipyard_worker` section enables hot-reloading and Visual Studio debugging of the `Shipyard.Worker` dotnet core application. The solution source root is mapped to the image as a mounted volume so that the docker container can monitor the file system for changes. Developer user secrets are also mounted to provide safe storage of secrets via the `dotnet user-secrets` tool.

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

## Shipyard

Shipyard is a service designed to handle all external messaging traffic using external email and SMS providers. 


## Port Authority

Job and task management service. 
Intended for audit tracking and API driven orchestration of background tasks.
