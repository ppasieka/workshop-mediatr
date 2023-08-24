# Workshop - MediatR

TODO: write short intro

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

## Rules

TODO: specify rules

## Challenge tasks

That's it - best of luck!
