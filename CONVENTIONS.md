# Medical Appointments MVP - Development Conventions

## Objective

This document defines the minimum development conventions for the project.

The goal is to keep the codebase simple, consistent and easy to evolve while building the MVP. Avoid unnecessary abstractions and overengineering.

---

# Project Architecture

The application is built as a **Modular Monolith** using:

* ASP.NET Core MVC
* Entity Framework Core
* SQL Server

The solution consists of a single application and a single database.

Do not introduce additional projects or services unless explicitly required.

---

# Design Principles

When implementing new functionality, always prioritize:

1. Simplicity
2. Readability
3. Low coupling
4. High cohesion
5. Incremental evolution

Prefer the simplest solution that satisfies the current business requirement.

Avoid designing for hypothetical future requirements.

---

# Project Structure

The application is organized by business modules.

```text
/src
    MedicalAppointments.Web

        Modules
            Administration
            Reservations
            Agenda

        Infrastructure

        Shared
```

Each module owns its own functionality.

Do not organize code by technical layers (global Controllers, Services, DTOs, etc.).

---

# Vertical Slice Organization

Every business use case should be implemented as a Vertical Slice.

Each slice contains everything required to execute a single business flow.

Example:

```text
Reservations

    BookAppointment

        BookAppointmentController.cs
        BookAppointmentRequest.cs
        BookAppointmentResponse.cs
        BookAppointmentService.cs
```

Keep all files related to the same use case together.

---

# Naming Conventions

Use descriptive names based on the business use case.

Examples:

* BookAppointmentController
* BookAppointmentService
* BookAppointmentRequest
* BookAppointmentResponse

Entity names should always be singular.

Examples:

* Doctor
* Patient
* Appointment
* Schedule
* Specialty

Avoid generic names such as:

* Manager
* Helper
* Utility
* CommonService

---

# Controllers

Controllers should only:

* Receive HTTP requests
* Validate the request model
* Invoke the corresponding Service
* Return the HTTP response

Do not place business rules inside Controllers.

---

# Services

Services implement the business use case.

Business rules belong here.

Each Service should have a single responsibility.

Prefer small and cohesive services.

---

# Data Access

Use Entity Framework Core directly through the shared AppDbContext.

Do not introduce:

* Repository Pattern
* Generic Repository
* Unit of Work

Unless there is a proven business need.

---

# Dependencies

The preferred dependency flow is:

```text
Controller
      ↓
Service
      ↓
AppDbContext
```

Keep dependencies between modules to a minimum.

Avoid unnecessary coupling.

---

# Business Rules

Business rules should always be implemented inside the corresponding Service.

Do not place business logic inside:

* Controllers
* Views
* Entity Framework configuration
* AppDbContext

---

# Code Style

Prefer:

* Small methods
* Clear names
* Simple logic
* Early returns when appropriate
* Readable code over clever code

Avoid unnecessary abstractions.

---

# New Features

When implementing a new feature:

1. Identify the business module.
2. Determine whether an existing Vertical Slice can be extended.
3. Create a new Vertical Slice only if the functionality represents a new business use case.
4. Keep related files together.
5. Implement business logic inside the Service.
6. Access the database through AppDbContext.

---

# MVP Philosophy

This project is an MVP.

When making implementation decisions:

* Prefer simplicity over flexibility.
* Prefer explicit code over generic abstractions.
* Prefer duplication over premature abstraction when the duplication is small.
* Introduce patterns only when there is a demonstrated need.

The objective is to validate the business, not to build the final architecture.

---

# What to Avoid

Unless explicitly requested, do not introduce:

* CQRS
* MediatR
* DDD
* Event Bus
* Microservices
* Repository Pattern
* Generic Repository
* Unit of Work
* Specification Pattern
* Generic CRUD base classes

These patterns may be introduced later if the project evolves and their benefits outweigh their complexity.