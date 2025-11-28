# Vehicle managment repository

The goal of the assignment was to build a minimalistic application using ASP.NET MVC and Entity Framework, following clean architecture principles and implementing CRUD functionality with sorting, filtering, and paging.
The application manages two model entities:
- VehicleMake (Id, Name, Abrv) — e.g., BMW, Ford, Volkswagen
- VehicleModel (Id, MakeId, Name, Abrv) — e.g., 128, 325, X5 (BMW)

It consists of:

- Project.Service  — business logic, EF models, and service layer
- Project.MVC — MVC application with administration views for Makes and Models

Technologies used:
ASP.NET MVC 5
Entity Framework 6 (Code First)
AutoMapper 
Ninject & Ninject.MVC5
.NET Framework 4.8.1

**This project was created as part of a technical assessment.
The focus was on following best practices such as clean abstraction, proper DI, async programming, and clean separation of layers.**
