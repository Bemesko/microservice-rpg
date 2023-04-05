# Microservice RPG

Sample project for studying microservices in .NET. Consists of several backend services to handle a shop system in an RPG.

**Link to Tutorial:** https://www.youtube.com/watch?v=CqCDOosvZIk

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

## Look up Later
- DateTime vs. DateTimeOffset
  - Answer: `DateTime` by itself doesn't store time zone information, while `DateTimeOffset` does
- Order of precedence for configuration sources: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0#default-application-configuration-sources
  - I know `appsettings.Development.json` overrides `appsettings.json` but I want the full picture with other sources (important information for deployment)
- What the `this` is doing in this parameter list
```csharp
AddMongo(this IServiceCollection services)
```
  - Theory: the `this` makes so that this method can be called like `services.AddMongo` and the value for the service will be filled automatically
  - This is called an Extension Method. See: https://stackoverflow.com/questions/846766/use-of-this-keyword-in-formal-parameters-for-static-methods-in-c-sharp

---

Tutorial Checkpoint 4:00:20