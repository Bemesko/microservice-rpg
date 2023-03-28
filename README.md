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
- Error handling
  - Common errors that can occur
    - Searching for something that doesn't exist
      - Null checking
    - Trying to add something with wrong values
      - Model validation (native to .NET)

### Look up Later
- DateTime vs. DateTimeOffset

---
