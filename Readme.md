# Workshop - MediatR

## What is a MediatR?

MediatR as an “in-process” Mediator implementation. 

The Mediator pattern is a behavioral design pattern that defines an object that encapsulates how a set of objects interact 1. The pattern promotes loose coupling by keeping objects from referring to each other explicitly, and it lets you vary their interaction independently. Plays very well with Dependency injection.

Pros:
- Reduced coupling: The Mediator pattern reduces the dependencies between communicating objects, thereby reducing the complexity of communication between them
- Improved maintainability: Since the Mediator object encapsulates the communication between objects, it becomes easier to maintain and modify the program
- Improved reusability: The Mediator pattern makes it easier to reuse objects since they are not tightly coupled with each other
- Centralized control: The Mediator object provides centralized control over the communication between objects, making it easier to manage and debug the program
- Promotes feature/vertical slicing in the codebase

Cons:
- Increased complexity: The Mediator pattern can increase the program's complexity since it introduces an additional abstraction layer.
- Navigation in IDE is harder. You have to rely on conventions.

**Mediator is not CQRS**, but it can help implement it.

## Links 

MediatR [GitHub](https://github.com/jbogard/MediatR)

Article to look at (as a reference point on some topics):
[CQRS and MediatR in ASP.NET Core - Code Maze (code-maze.com)](https://code-maze.com/cqrs-mediatr-in-aspnet-core/)

## Background information
The code is a quiz solution based on .NET. You have a quiz client project and a quiz service project, both have corresponding test projects.

The solution uses a SQLite in-memory database with the following schema:
```
Quiz                           Question                        Answer
+-----------------+            +-----------------+            +-----------------+
| Id              |            | Id              |            | Id              |
| Title           |            | Text            |            | Text            |
|                 +------------+ QuizId          +------------+ QuestionId      |
|                 |            | CorrectAnswerId |            |                 |
|                 |            |                 |            |                 |
+-----------------+            +-----------------+            +-----------------+
```


## Developer Requirements:

1. **Install MediatR**: The first step is to install the MediatR library in your project. 

2. **Register Dependencies in DI**: Register MediatR and all needed dependencies within your Dependency Injection (DI) container following the installation.

3. **GET quiz by ID Endpoint Conversion**: Convert the existing GET quiz by ID endpoint to utilize a MediatR request. This endpoint should use the Request/Response feature of MediatR.

4. **POST Quiz Endpoint Conversion**: Similarly, convert the POST quiz endpoint to utilize a MediatR request.

5. **MediatR Notification Creation**: Create a new MediatR notification that will be dispatched when a new quiz is created. This notification can simulate sending an email or can be stored in the database for logging purposes.

6. **Explore Streaming Capabilities on GET Quizzes Endpoint**: Implement an example of streaming capabilities offered by MediatR on the GET quizzes endpoint.

7. **Logging Behavior Addition to All Requests**: Add a logging behavior that will apply to all requests processed by MediatR.

8. **Exception Handling Behavior Implementation**: Implement an exception handling behavior for requests processed by MediatR


## Edge Cases & Remarks:

- Ensure that all exceptions are thoroughly tested during the implementation of exception-handling behavior.
- Always remember that although using mediators can reduce coupling and improve maintainability, they also introduce an additional layer of abstraction which increases complexity; ensure this trade-off is well understood and mitigated effectively in your design decisions
