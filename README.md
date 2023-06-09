# Microservice RPG

This a project I made following a (very long) YouTube tutorial/course to learn more about building microservice-based applications. The architecture of the services is described below:

![](docs/diagram.png)

**Link to Tutorial:** https://www.youtube.com/watch?v=CqCDOosvZIk

I provided all the notes I took following the videos further down this README. Judging by the size of it I'd say it was pretty productive.

## Features/Technologies
- Microservices made with .NET 7
- Each service has its own storage using MongoDB
- Services communicate asynchronously via RabbitMQ messages
- Frontend made in React.js
  - This is the only component I didn't make, since it was already provided for the course (the focus of the tutorials was on building the microservices)

## Post Tutorial Checklist

I've finished the course but there are some lingering ideas for these applications I still want to make.

- [ ] Build my own version of the Frontend (also in React)
- [ ] Dockerize all applications
- [ ] A Hollistic docker-compose to run all this on the flip of a switch
- [ ] Helm chart to run these services on a Kubernetes cluster
- [ ] Maybe a new *User* service made from scratch

## Screenshots
![](docs/main-page.png)
![](docs/catalog.png)
![](/docs/catalog-api.png)
![](docs/inventory.png)
![](docs/inventory-api.png)

---

## Tutorial Notes, Day 1

**Link to Tutorial:** https://www.youtube.com/watch?v=CqCDOosvZIk
- Note: the tutorial is made with .NET 5 in mind, but I'll use .NET 7 anyway because I'm not scared of some divergences from the course.

- Trust a development certificate: `dotnet dev-certs https --trust`
  - Didn't have to do this yet probably because all my projects so far used .NET 7
- Data Transfer Objects
  - Represent a contract between a microservice API and the client
  - Essentially they are the response for an API'S entity
- Record Types vs. Classes
  - Simpler to declare
  - Value-based equality
    - Two records are the same if they have the same values on all fields
  - Immutable by default
  - Built in `ToString()` override
- Controller classes are instantiated everytime someone calls one of their methods, so non-static properties will be recreated everytime the endpoints are called
- One more way to get elements in a List: `list.Where(o => o.Id == id)`
  - Don't forget about `Find`, `FirstOrDefault` and `SingleOrDefault`

---
## Tutorial Notes, Day 2
- Really weird syntax for changing a record:
```csharp
var updatedItem = existingItem with
{
    Name = updateItemDto.Name,
    Description = updateItemDto.Description,
    Price = updateItemDto.Price
};
```
- The tutorial guy is returning an `ActionResult` directly instead of an `IActionResult`, I'm doing it differently because using the interface is probably better
- Common errors that can occur and how to solve them
  - Searching for something that doesn't exist (404, not found)
    - Null checking
  - Trying to add something with wrong values (400, bad request)
    - Model validation (native to .NET)
- Repository pattern
  - Abstraction that sits between the data and business layers and decouples one from the other
  - If the app switches database, only the repository will be affected
- Did I mention I'll be using MongoDB for these apps?
  - NoSQL database
  - Stores JSON-like documents with a dynamic schema
  - Reasons to use it
    - Won't need relationships across the data
    - Won't need ACID (Atomicity, Consistency, Isolation, Durability) guarantees
    - No complex queries
    - Need low latency, high availability and high scalability
- mongodb quickstart:

    docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo

---

## Tutorial Notes, Day 3
- Application behaves weirdly if the database is not running, maybe handle this exception as well
- Dependency Injection
  - A big deal in .NET since almost every tutorial talks about it
  - If a class uses another class, put it in the class' constructor as a parameter and let some central code handle the instantiation and assignment of it
  - Dependency Inversion Principle (D from SOLID) => Classes should call abstractions of their dependencies instead of their concrete implementations (interfaces vs. concrete classes)
  - To do dependency injection, someone needs to instantiate the classe passing the right instances as parameters
    - This is the job of a Service Container (in .NET is the IServiceProvider class)
    - All dependencies need to be registered in it, and when it needs to it creates or fecthes an instance of the dependency class to put into the dependant
  - Configuration
    - At this point in the code, it is talking to MongoDB hardcoding a connection string in the code
    - Supported ASP.NET Core Configuration Sources
      - `appsettings.json`, `appsettings.Development.json`
      - Command line arguments
      - Environment variables
      - Local secrets
      - Cloud
  - General rule of thumb: avoid normal classes instantiating other classes and leave that job for the pros
- VSCode has its own "Extract Interface" feature that I just learned about now
- Consuming `appsettings.json` in the code
  - Create classes for each section so that they can be referred to programatically (see `Settings` directory)
- Expression body definition:
```csharp
public string ConnectionString => $"mongodb://{Host}:{Port}";
```

## Tutorial Notes, Day 4
- Code that can be repeated accross microservices: 
  - Repository
  - Settings
  - Service Broker
  - Instrumentation
  - Good practice: keep these implementations in a separate library and reference it with a nuGet package

## Tutorial Notes, Day 6
- Was worried that the implementation of Extensions for the IServiceCollection wouldn't work, but not only did it work, it also helped me understand what the builder class does in `Program.cs`. Still didn't get 100% how it works behind the scenes though.

## Tutorial Notes, Day 7
- `dotnet pack -o <output folder>` to compile a class library into a nuget package
- `dotnet nuget add source <path> -n <Play>` to add a local folder as a nuget directory
  - This path needs to be absolute
- `dotnet nuget list source` to show all nuget sources
- Docker compose seems like it would be very useful to learn as a middleground between Dockerfiles and Kubernetes specifications
- `docker-compose up -d` to run docker compose without sending output to console

## Tutorial Notes, Day 10
- Types of communication between microservices
  - synchronous: client waits for a response
    - Thread that calls other service may be blocking or non-blocking (callback)
    - Implementations
      - REST + HTTP is the traditional approach
      - gRPC can be used for inter-service communication
        - Most efficient than REST, not everything supports it
  - asynchronous: response is not sent immediately
- VSCode generates debugging assets per workspace, so having everything in a single workspace makes debugging multiple services a bit clunky

## Tutorial Notes, Day 11
- Problems that can happen when microservices communicate
  - Partial failures **will happen**, so handling them is not optional
    - Setting appropriate timeouts (fail fast)
    - Retries with exponential backoff (longer wait between each retry)
- Polly => Microsoft Package for HTTP client policies, such as setting timeouts and dealing with transient errors (network failures, http 5xx and 408 status)
- When using exponential retry times, it's a good idea to add a bit of randomness to the amount of time between retries, so multiple instances of the service don't end up causing peaks of requests all at once
- Resource exhaustion and the **circuit breaker pattern**
  - Have been hearing about this one for a while, excited to find out what it means
  - Resource exhaustion happens when an applications retries so many times it uses all available threads
  - Circuit breaker prevents the service from performing an operation that's likely to fail
    - All requests the service makes are sent through and monitored by the circuit breaker
    - After X amount of failed requests, the circuit breaker immediately fails all new requests from the service, for Y amount of time (opening the circuit)
    - After it waits a bit, it will let some requests pass and monitor their state, and if everything's in order it will close the circuit again and let everything pass
    - This helps the caller and callee services from being overwhelmed
  - Using Polly, this pattern can be implemeted as a transiend HTTP error policy

### Asynchronous Communication

- Problems with synchronous communication
  - If a service depends on many others, the full call request cycle starts taking too long
  - Failure in only 1 service results in a cascade failure for everything
  - For every service dependency, the actual SLA decreases a bit
- Asynchronous communication style
  - Client doesn't wait for a response
  - Generally involves a message broker with high availability
  - Messages can be received my:
    - a single receiver (asynchronous commands)
    - multiple receivers (publish/subscribe events)
- Asynchrnous communication enables microservice autonomy
  - All services are coupled only to the message broker instead of the other services
  - If one service fails, only it is affected, since everything is decoupled from it
  - As a result, a high SLA can be realistically maintained
- Asynchronous propagation of data
  - Scenario: Inventory service needs data from the Catalog service to return its response
  - Instead of requesting data each time, the Inventory service can keep its own storage of the data it needs from the Inventory service and subscribe to all messages from the message broker that are sent whenever a new item is registered into the Catalog service
  - Inventory service doesn't depend on the catalog service to return its requests, therefore making it more available and eliminating delays
- Implementing asynchronous communication
  - RabbitMQ
    - One implementation of a message broker that can be run locally
    - Messages are published to an exchange, after which RabbitMQ routes each message to one or more appropriate queues, which send the message to its consumers
  - MassTransit
    - Distributed app framework for .NET that supports RabbitMQ, among other message brokers
    - Separates between publishers and consumers
- `dotnet add reference <path>` Adds a reference to another project (csproj) into the project on the current directory
- Use `{ get; init; }` to make a class attribute read only, essentially

## Tutorial Notes, Day 12
- MassTransit apparently has some different configuration options from v8 (.NET 6) onwards, so no need to explicitly call `AddMassTransitHostedServices` in `Program.cs`
- `dotnet pack -p:PackageVersion=1.1.0 -o ..\..\..\packages\` to specify a package version with creating a nuget package

## Look up Later
- DateTime vs. DateTimeOffset
  - Answer: `DateTime` by itself doesn't store time zone information, while `DateTimeOffset` does
- Order of precedence for configuration sources: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0#default-application-configuration-sources
  - I know `appsettings.Development.json` overrides `appsettings.json` but I want the full picture with other sources (important information for deployment)
  - Order is as follows:
    1. Command line arguments
    2. Non prefixed environment variables
    3. User secrets (in Development environment)
    4. `appsettings.{Environment}.json`
    5. `appsettings.json`
    6. Default host configuration sources
       1. Command line arguments
       2. `DOTNET_` prefixed environment variables
       3. `ASPNETCORE_` prefixed environment variables
- What the `this` is doing in this parameter list
```csharp
AddMongo(this IServiceCollection services)
```
  - Theory: the `this` makes so that this method can be called like `services.AddMongo` and the value for the service will be filled automatically
  - This is called an Extension Method. See: https://stackoverflow.com/questions/846766/use-of-this-keyword-in-formal-parameters-for-static-methods-in-c-sharp

---

Tutorial Checkpoint: Finished!!