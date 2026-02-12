# task-tracker

A simple ASP.NET Core Web API that allows users to manage tasks and update task status while enforcing basic business rules.

## Time Spent

Approximate time spent (in minutes): 300

## Approach

The solution follows Clean Architecture with CQRS pattern using Cortex Mediator. Core features include JWT-based authentication, task CRUD operations with status workflow validation (Pending → InProgress → Completed), FluentValidation for input validation, and Mapster for object mapping. The API uses EF Core with an in-memory database for quick setup. Business rules prevent completed tasks from being updated and enforce valid status transitions. Comprehensive unit tests ensure reliability.

## Trade-offs

With more time, I would improve the solution by:

- Implementing a persistent database (SQL Server/PostgreSQL) instead of in-memory storage
- Adding comprehensive integration tests and end-to-end API tests
- Implementing proper logging and monitoring with Serilog/Application Insights
- Adding pagination and sorting for task listing endpoints
- Expanding business rules (due dates, task assignments, categories)
- Implementing refresh token mechanism for better security
- Adding API versioning and rate limiting
- Improving error responses with more detailed problem details
