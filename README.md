# Microservice RPG

Sample project for studying microservices in .NET. Consists of several backend services to handle a shop system in an RPG.

**Link to Tutorial:** https://www.youtube.com/watch?v=CqCDOosvZIk

## Tutorial Notes

**Link to Tutorial:** https://www.youtube.com/watch?v=CqCDOosvZIk
- Note: the tutorial is made with .NET 5 in mind, but I'll use .NET 7 anyway because I'm not scared of some divergences from the course.

- Trust a development certificate: `dotnet dev-certs https --trust`
  - Didn't have to do this yet probably because all my projects so far used .NET 7
- Data Transfer Objects
  - Represent a contract between a microservice API and the client
  - Essentially they are the response for an API'S entity