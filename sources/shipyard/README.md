Shipyard
========

Shipyard is communication system for SMS & Email messages.

It provides message scheduling, delivery status tracking & reply handling. 

&nbsp;

# Assemblies

| Assembly                  | Description                                         |
|---------------------------|-----------------------------------------------------|
| `Shipyard`                | Main service assembly, contains all business logic. | 
| `Shipyard.Contracts`      | Service bus messaging contracts                     | 
| `Shipyard.Data`           | Data access layer                                   |
| `Shipyard.Data.Migration` | Executable EF migration host for updating databases |
| `Shipyard.Web`            | REST API                                            |
| `Shipyard.Worker`         | Background service worker                           |

&nbsp;


# Tasks

Available tasks for this project:

| Task                 | Description                              |
|----------------------|------------------------------------------|
| `shipyard:logs`      | Display shipyard logs                    |
| `shipyard:rmi`       | Remove all local shipyard images         |
| `shipyard:sh:web`    | Connect to the shipyard-web shell        |
| `shipyard:sh:worker` | Connect to the shipyard-worker shell     |

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

&nbsp;

# Workflow

1. POST /v1/api/email/send
2. Apply personalizations and generate unique message identifiers => EmailMessage[]
3. Send ScheduleEmail message { email = {..}, schedule = {..} }
4. Return message IDs

5. IConsume<ScheduleEmail>
   1. Calculate time of day bucket within send window
   2. 
   





# Interfaces

## `ISubtask`

Job details are optional.
- If job id provided, the send operation added as a sub-task.
- If job id is null, a new job will be created with the send operation as a sub-task.

If both a job ID and a sub task ID are provided, do nothing and simply queue up the send operation. Assume that the API caller has orchestrated the tasks and Shipyard is carrying out a known operation as part of a larger pre-determiend workflow.

The sending sub-task will be updated with the status of the operation upon completion or failure.

```csharp
public interface ISubtask
{
    public Guid? Id { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? CorrelationId { get; set; }
}
```
```json
{
    "job": {
        "id": "049f4d1b-c0c2-4f39-b396-fe5e179f2d92",     
        "task_id": null,
        "correlation_id": null
    }    
}
```


## `IScheduled`

Scheduling of messages for a later date. 

If provided, `schedule_date` serves as an absolute schedule time. If the date is outside of the load balancers send window an exception will be thrown.

The `time_zone` value is used for automatic scheduling. The system will find an appropriate slot within the send window for the day. If neither `schedule_date` or `time_zone` is provided, the configured default timezone is used instead.

```csharp
public interface IScheduled
{
    public DateTimeZone? TimeZone { get; set; }
    public OffsetDateTime? ScheduleDate { get; set; }
}
```
```json
{
    "schedule": {
        /* Automatic scheduling using the time zone */
        "time_zone": "America/Edmonton",
    
        /* Explicit scheduling based on absolute time, allow UTC or +/- offsets */
        "schedule_date": "2020-09-17T00:08:30:00.000Z"
    }    
}
```

&nbsp;

# API Docs

> 🚀 Hey! This is just a rough design of the API and is subject to change.

&nbsp;

**POST `/v1/config`**
```json
{    
    "providers": {
        "email": "sendgrid",
        "sms": "twilio"
    },
    "scheduling": {
        "max_age": "5D", 
        "min_send": 10000,
        "granularity": "15M",
        "window_start": "08:00:00",
        "window_end": "18:00:00",
        "default_time_zone": "America/Edmonton",
    },    
    "features": {
        "load_balancing": true,        
    }
}
```
returns `200 OK`

<hr/>&nbsp;

**PUT `/v1/providers/sendgrid`**

Update configuration for a sendgrid provider

```json
{
    "api_key": "SG.XXXXXXXXXX.XXXXXXXXXXXXXXXXXXXXXXX",
    "bcc_address": null,
    "from_address": { "name": "Foo", "address":"foo@test.com" },
    "callback_url": "http://localhost/shipyard/v1/email/callback/sendgrid"
}
```
returns `200 OK`

<hr/>&nbsp;

**PUT `/v1/providers/twilio`**

Update configuration for a twilio provider

```json
{
    "sid": "AC9473e9de34a4413ab49caec8b91ace02",
    "sid_token": "a2e88d638c864278932634b0d326135d",
    "phone_numbers": [ "+4033101010" ],
    "callback_url": "http://localhost/shipyard/v1/email/callback/twilio"
}
```
returns `200 OK`

<hr/>&nbsp;

**POST `/v1/email/send`**

Send an email two one or more recipients (supports `IScheduled` and `ISubtask`)

```json
{       
    "to": [ 
         { "name": "Foo", "address": "foo@test.com" }
    ],
    "cc": null,
    "bcc": null,

    "subject": "Subject line",    
    "content": "Email content body",
    "content_type": "text/html",

    "personalizations": [
        {
            "to":"",  
            "cc":"",
            "bcc":"",
            "subject":"",
            "substitutions": {
                "%first_name%": "First Name",
                "%last_name%": "Last Name"
            },
            "metadata": {
                "release_id": 123,
                "customer_id": 456   
            }
        }
    ]    
}
```
returns
```json
200 OK
{
    "job": {
        "id": "049f4d1b-c0c2-4f39-b396-fe5e179f2d92",
        "task_id": "f232df48-8360-4168-8ce7-9a97c81b7f1c"
    },
    "messages": [
        "AX7asd)gaXASD*12KGAJS#"
    ]         
}
```

<hr/>&nbsp;

**POST `/v1/sms/send`**

Send an sms message two one or more recipients

```json
{    
    "to": [ 
         { "number": "+14033101010" }
    ],
        
    "content": "Email content body",
    
    "personalizations": [
        {
            "to":"",
            "substitutions": {
                "%first_name%": "First Name",
                "%last_name%": "Last Name"
            },
            "metadata": {
                "release_id": 123,
                "customer_id": 456   
            }
        }
    ]  
}
```
returns
```json
200 OK
{
    "schedule": {
        "id": "11bf8a05-cbec-47ef-9417-04e57d45481a",
        "schedule_date": "2020-09-17T00:08:30:00.000Z"
    }, 
    "job": {
        "id": "049f4d1b-c0c2-4f39-b396-fe5e179f2d92",
        "task_id": "f232df48-8360-4168-8ce7-9a97c81b7f1c"
    }
}
```

<hr/>&nbsp;

**POST `/v1/email/callback/{provider}`**

Callback endpoint that recieves provider specific HTTP callbacks 

```json
{ /* provider specific body */ }
```

<hr/>&nbsp;
