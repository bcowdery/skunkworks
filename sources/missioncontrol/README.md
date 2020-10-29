MissionControl
=============

Mission Control is the root-level admin app and monitoring dashboard.

 **👉 Endpoint URL:** http://localhost/admin/

&nbsp;

# Assemblies

| Assembly                       | Description                                         |
|--------------------------------|-----------------------------------------------------|
| `MissionControl.Web`           | Health check dashboard                              |

&nbsp;

# Tasks

Available tasks for this project:

| Task                      | Description                                   |
|---------------------------|-----------------------------------------------|
| `missioncontrol:logs`      | Display missioncontrol logs                    |
| `missioncontrol:rmi`       | Remove all local missioncontrol images         |
| `missioncontrol:sh`        | Connect to the missioncontrol-web shell        |

&nbsp;
# Configuration

## Kubernetes Serivice Discovery

HealthChecks UI supports automatic discovery of k8s services exposing pods that have health checks endpoints. This means, you can benefit from it and avoid registering all the endpoints you want to check and let the UI discover them using the k8s api.

```json
{
  "HealthChecksUI": {
    "KubernetesDiscoveryService": {
      "Enabled": true,
      "HealthPath": "health",
      "ServicesLabel": "HealthChecks",
      "ServicesPathAnnotation": "HealthChecksPath",
      "ServicesPortAnnotation": "HealthChecksPort",
      "ServicesSchemeAnnotation": "HealthChecksScheme"
    },
    "EvaluationTimeInSeconds": 10,
    "MinimumSecondsBetweenFailureNotifications": 60
  }
}
```

**resources:**
* https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/doc/k8s-ui-discovery.md