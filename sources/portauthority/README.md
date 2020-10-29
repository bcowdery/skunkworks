PortAuthority
=============

Port Authority is a job management and task tracking system.

This system provides a global task orchestration mechanism for long running distributed tasks. It provides visibility on the state of long running jobs, tracking progress 

 **👉 Endpoint URL:** http://localhost/jobs/

&nbsp;

# Assemblies

| Assembly                       | Description                                         |
|--------------------------------|-----------------------------------------------------|
| `PortAuthority`                | Main service assembly, contains all business logic. | 
| `PortAuthority.Contracts`      | Service bus messaging contracts                     | 
| `PortAuthority.Data`           | Data access layer                                   |
| `PortAuthority.Data.Migration` | Executable EF migration host for updating databases |
| `PortAuthority.Web`            | REST API                                            |
| `PortAuthority.Worker`         | Background service worker                           |

&nbsp;

# Tasks

Available tasks for this project:

| Task                      | Description                                   |
|---------------------------|-----------------------------------------------|
| `portauthority:logs`      | Display portauthority logs                    |
| `portauthority:rmi`       | Remove all local portauthority images         |
| `portauthority:sh:web`    | Connect to the portauthority-web shell        |
| `portauthority:sh:worker` | Connect to the portauthority-worker shell     |

&nbsp;

# Environment Variables

| Var                              | Description                                   |
|----------------------------------|-----------------------------------------------|
| `APPINSIGHTS_INSTRUMENTATIONKEY` | Application insights instrumentation key      | 
| `DD_AGENT_HOST `                 | DataDog DogStatsD metrics host url            |
| `DD_DOGSTATSD_PORT`              | Optional port for the DogStatsD host          |
| `DD_ENTITY_ID `                  | Value to be injected as a global `dd.internal.entity_id tag`. |         

**Resources:**
* https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core#enable-application-insights-server-side-telemetry-no-visual-studio
* https://github.com/DataDog/dogstatsd-csharp-client               