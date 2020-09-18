PortAuthority
=============

Port Authority is a job management and task tracking system.

This system provides a global task orchestration mechanism for long running distributed tasks. It provides visibility on the state of long running jobs, tracking progress 


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



# API Docs

> 🚀 Hey! This is just a rough design of the API and is subject to change.

&nbsp;

**POST `/v1/jobs`**
```json    
{
    "id": "049f4d1b-c0c2-4f39-b396-fe5e179f2d92",         
    "correlation_id": null,

    "type": "send",
    "namespace": "com.shipyard.email",
    
    "meta": {
        "foo": "bar",
        "release_id": 123,
        "created_by": "foo@test.com"
    }
}
```
returns
```json
200 OK
{
    "job": {
        "id": "049f4d1b-c0c2-4f39-b396-fe5e179f2d92",         
        "correlation_id": null,

        "type": "send",
        "namespace": "com.shipyard.email",

        "status": "(pending|inprogress|failed|completed)",
        "start_time": "2020-09-18T00:09:27:30.132-06:00",
        "end_time": null,

        "tasks": {
            "total": 0,
            "pending": 0,
            "in_progress": 0,
            "failed": 0,
            "completed": 0            
        },

        "meta": {
            "foo": "bar",
            "release_id": 123,
            "created_by": "foo@test.com"
        }
    }    
}

```

<hr/> &nbsp;

**GET `/v1/jobs/{id}`**

Get job by ID

```json
200 OK
{
    "job": {
        "id": "049f4d1b-c0c2-4f39-b396-fe5e179f2d92",         
        "correlation_id": null,

        "type": "send",
        "namespace": "com.shipyard.email",

        "status": "(pending|inprogress|failed|completed)",
        "start_time": "2020-09-18T00:09:27:30.132-06:00",
        "end_time": null,

        "tasks": {
            "total": 0,
            "pending": 0,
            "in_progress": 0,
            "failed": 0,
            "completed": 0            
        },

        "meta": {
            ...
        }
    }    
}
```

<hr/> &nbsp;

**GET `/v1/jobs/?namespace={namespace}&type={type}&correllation_id={corellation}`**

List all jobs

Supports paging operators `&page={page}&size={size}&sort={sort}&order={order}`

Can search by any combination of
- `namespace`
- `type`
- `corellation_id`

```json
200 OK
{
    "page": 1,
    "page_size": 25,
    "total_pages": 7,
    "total_items": 176,
    "sort": "start_time",
    "order": "asc",

    "jobs": [
        {
            "id": "049f4d1b-c0c2-4f39-b396-fe5e179f2d92",         
            "correlation_id": null,

            "type": "send",
            "namespace": "com.shipyard.email",

            "status": "(pending|inprogress|failed|completed)",
            "start_time": "2020-09-18T00:09:27:30.132-06:00",
            "end_time": null,

            "tasks": {
                "total": 0,
                "pending": 0,
                "in_progress": 0,
                "failed": 0,
                "completed": 0            
            },

            "meta": {
                ...
            }
        }    
    ]
}
```

<hr/> &nbsp;

**POST `/v1/jobs/{id}/tasks`**

Add a task to a job

```json
{
    "id": "049f4d1b-c0c2-4f39-b396-fe5e179f2d92",         

    "type": "send",
    "namespace": "com.shipyard.email",
    
    "meta": {
        "foo": "bar",
        "release_id": 123,
        "created_by": "foo@test.com"
    }
}
```
returns `200 OK`

<hr/> &nbsp;

**GET `/v1/jobs/{id}/tasks/{id}`**

Get task by ID

```json
200 OK
{
    "task": {
        "id": "049f4d1b-c0c2-4f39-b396-fe5e179f2d92",         
        
        "type": "send",
        "namespace": "com.shipyard.email",

        "status": "(pending|inprogress|failed|completed)",
        "start_time": "2020-09-18T00:09:27:30.132-06:00",
        "end_time": null,

        "meta": {
            ...
        }
    }    
}
```

<hr/> &nbsp;

**GET `/v1/jobs/{id}/tasks?namespace={namespace}&type={type}`**

List all job subtasks

Supports paging operators `&page={page}&size={size}&sort={sort}&order={order}`

Can search by any combination of
- `namespace`
- `type`

```json
200 OK
{
    "page": 1,
    "page_size": 25,
    "total_pages": 7,
    "total_items": 176,
    "sort": "start_time",
    "order": "asc",

    "tasks": [
        {
            "id": "049f4d1b-c0c2-4f39-b396-fe5e179f2d92",         

            "type": "send",
            "namespace": "com.shipyard.email",

            "status": "(pending|inprogress|failed|completed)",
            "start_time": "2020-09-18T00:09:27:30.132-06:00",
            "end_time": null,

            "meta": {
                ...
            }
        }    
    ]
}
```

<hr/> &nbsp;





<span style="color: #66d9ef;">TODO: Spec out additional API endpoints</span>

- update status of job (should this be explicit or happen automatically when a task is started?)
- update status of task 
- Add history event to job
- Add history event to task
