# HOTEL BOOKING

This is an API for managing hotel reservations, guests, and rooms in a Hotel Booking System. It allows creating, updating, retrieving, and deleting hotel rooms, guests, and bookings. This API was developed using C# with .NET Framework 8.0 and a SQL Server database.

## Table of Contents
  - [Features](#features)
  - [Requirements](#requirements)
  - [Business Rules](#business-rules)
  - [Technologies](#technologies)
  - [Endpoints](#endpoints)
    - [Room Management](#room-management)
    - [Guest Management](#guest-management)
    - [Booking Management](#booking-management)
  - [Database Configuration](#database-configuration)
  - [Setup](#setup)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)

## Features
* Manage hotel rooms (CRUD operations)
* Manage guests (CRUD operations)
* Allow guests to create reservations for rooms
 
## Requirements
* Room Management Endpoints: Endpoints for creating, updating, retrieving by ID, deleting, and listing all rooms.
* Guest Management Endpoints: Endpoints for creating, updating, retrieving by ID, deleting, and listing all guests. Guests are identified by their unique ID.
* Booking Management Endpoints: Endpoints for creating, updating, retrieving by ID, deleting, and listing all room bookings.

**Business Rules:**
* A room cannot be reserved if it is already occupied.
* Allow 3 hours after the end of a reservation for cleaning.
* Some rooms may be unavailable for maintenance.

## Technologies

* Language: C# (.NET Framework 8.0)
* Database: SQL Server
* ORM: Entity Framework Core
* Documentation: Swagger/OpenAPI for interactive API documentation

Endpoints
---
### Room Management
**Retrieve Room by ID**
* **GET** /api/Room/{id}

**Update Room**
* **PUT** /api/Room/{id}

**Delete Room**
* **DELETE** /api/Room/{id}

**List All Rooms**
* **GET** /api/Room

**Create Room**
* **POST** /api/Room
Body Example: 
```gherkin=
{
    "name": "Room 3",
    "level": 3,
    "inMaintenance": false,
    "priceValue": 300,
    "currencyCode": 0
}
```

### Guest Management
**Retrieve Guest by ID**
* **GET** /guests/{guestId}

**Update Guest**
* **PUT** /guests/{guestId}

**Delete Guest**
* **DELETE** /guests/{guestId}

**List All Guests**
* **GET** /guests

**Create Guest**
* **POST** /guests
Body Example:

```gherkin=
{
    "name": "teste",
    "surname": "string",
    "email": "string@email.com",
    "idNumber": "12345",
    "idTypeCode": 2
}
```

### Booking Management
**Retrieve Booking by ID**
* **GET** /api/Booking/{id}

**Update Booking**
* **PUT** /api/Booking/{id}

**Delete Booking**
* **DELETE** /api/Booking/{bookingId}

**List All Bookings**
* **GET** /api/Booking

**Create Booking**
* **POST** /api/Booking
Body Example:

```gherkin=
{
    "placedAt": "2024-11-11T18:21:13.569",
    "start": "2024-11-12T00:00:00",
    "end": "2024-11-13T00:00:00",
    "roomId": 2,
    "guestId": 1003,
    "status": 0
}
```


Database Configuration
---
The API uses Entity Framework Core with a SQL Server database. The connection string is specified in appsettings.json under ConnectionStrings.

Setup
---
### Prerequisites
* .NET SDK 8.0
* SQL Server

### Installation
1. Clone the repository.
2. Configure database settings in appsettings.json.
3. Run migrations using dotnet ef database update.
4. Start the application using dotnet run.
5. Access Swagger documentation at http://localhost:5000/swagger.

---
