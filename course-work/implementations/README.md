# Hunting Permit & Trip Management System

## Student Information

- **Name:** Taylan Takev
- **Faculty Number:** 2401321045

---

## Project Description

Hunting Permit & Trip Management System is a distributed web application developed with ASP.NET Core.

The system allows users to manage hunters, hunting permits, hunting locations, hunting trips, harvest records, and statistical reports. It consists of a RESTful Web API backend, an ASP.NET Core MVC frontend client, and a SQL Server database.

The application supports role-based access control with two main roles: **Admin** and **Hunter**.

---

## User Roles

### Admin

The Admin role has full access to the system.

Admin users can:

- View all users
- Create, edit, and delete users
- View, create, edit, and delete hunting permits
- View, create, edit, and delete hunting locations
- View, create, edit, and delete hunting trips
- View, create, edit, and delete harvest records
- View global system statistics

### Hunter

The Hunter role has limited access.

Hunter users can:

- View only their own personal information
- View only their own permits
- View and manage only their own hunting trips
- View and manage only harvest records related to their own trips
- View statistics based only on their own data
- View hunting locations without managing them

---

## Main Features

- User management
- Hunting permit management
- Hunting location management
- Hunting trip management
- Harvest record tracking
- Statistics dashboard
- JWT authentication
- Protected API endpoints
- Filtering, pagination, and sorting
- Global exception handling
- Swagger/OpenAPI documentation

---

## Technologies Used

### Backend

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger / OpenAPI

### Frontend

- ASP.NET Core MVC
- Bootstrap
- HttpClient

### Database

- Microsoft SQL Server
- SQL Server Management Studio

---

## Project Structure


HuntingPermitTripManagement
│
├── HuntingPermitTripManagement.Api
│   ├── Controllers
│   ├── Data
│   ├── Middleware
│   ├── Models
│   ├── Migrations
│   ├── appsettings.json
│   └── Program.cs
│
├── HuntingPermitTripManagement.Web
│   ├── Controllers
│   ├── Models
│   ├── Views
│   ├── appsettings.json
│   └── Program.cs
│
└── HuntingPermitTripManagement.sln

------------------------------------


### Database

The project uses SQL Server.

Default database name:

HuntingPermitTripManagementDb

Main tables:

Users
Permits
Locations
HuntingTrips
HarvestRecords

### How to Run the Project
#### Clone the Repository
git clone https://github.com/taylyan/distributed-applications-se.git
#### Open the Project

Open the solution file:

HuntingPermitTripManagement.sln

in Visual Studio.

The solution is located in:

course-work/implementations/HTPM/

#### Configure the Database Connection

In the API project, open:

HuntingPermitTripManagement.Api/appsettings.json

Update the connection string:

"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=HuntingPermitTripManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}

Example:

"ConnectionStrings": {
  "DefaultConnection": "Server=DESKTOP-IN026J8;Database=HuntingPermitTripManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
#### Apply Database Migrations

Open terminal in the API project folder:

HuntingPermitTripManagement.Api

Run:

dotnet ef database update

This will create the SQL Server database and all required tables.

#### Start the API Project

Open terminal in:

HuntingPermitTripManagement.Api

Run:

dotnet run

API Swagger URL:

http://localhost:5116/swagger

#### Start the MVC Frontend Project

Open another terminal in:

HuntingPermitTripManagement.Web

Run:

dotnet run

Frontend URL:

http://localhost:5122
Test Login

A test user can be created through Swagger using the Users endpoint.

Example user:

{
  "firstName": "Taylan",
  "lastName": "Takev",
  "email": "taylan@test.com",
  "passwordHash": "123456",
  "role": "Hunter"
}

Login credentials:

Email: taylan@test.com
Password: 123456
API Endpoints
Authentication


------------------------------------

### POST /api/Auth/login
#### Users
GET    /api/Users
GET    /api/Users/{id}
POST   /api/Users
PUT    /api/Users/{id}
DELETE /api/Users/{id}
#### Permits
GET    /api/Permits
GET    /api/Permits/{id}
POST   /api/Permits
PUT    /api/Permits/{id}
DELETE /api/Permits/{id}
#### Locations
GET    /api/Locations
GET    /api/Locations/{id}
POST   /api/Locations
PUT    /api/Locations/{id}
DELETE /api/Locations/{id}
#### Hunting Trips
GET    /api/HuntingTrips
GET    /api/HuntingTrips/{id}
POST   /api/HuntingTrips
PUT    /api/HuntingTrips/{id}
DELETE /api/HuntingTrips/{id}
#### Harvest Records
GET    /api/HarvestRecords
GET    /api/HarvestRecords/{id}
POST   /api/HarvestRecords
PUT    /api/HarvestRecords/{id}
DELETE /api/HarvestRecords/{id}
#### Statistics
GET /api/Statistics/harvest
GET /api/Statistics/trips
GET /api/Statistics/permits

------------------------------------


### Filtering, Pagination and Sorting

The API supports filtering, pagination, and sorting for list endpoints.

Example:

GET /api/Users?firstName=Taylan&pageNumber=1&pageSize=5&sortBy=firstName&sortDirection=asc
Security

The backend uses JWT authentication.

The user logs in through:

POST /api/Auth/login

After successful login, the API returns a JWT token. Protected endpoints require the token to be sent in the Authorization header.

Authorization: Bearer <token>
Global Exception Handling

The API includes centralized exception handling middleware. Instead of returning raw server errors directly to the client, the system returns structured JSON error responses.

Example:

{
  "status": 500,
  "message": "Internal Server Error",
  "detail": "Error details"
}
### Statistics

The system provides statistical reports for:

Total harvested animals
Total harvest weight
Most hunted animal
Total hunting trips
Most visited location
Total permits
Active permits
Expired permits

### Architecture
ASP.NET MVC Frontend
        ↓
HTTP Requests with HttpClient
        ↓
ASP.NET Core REST API
        ↓
Entity Framework Core
        ↓
SQL Server Database

------------------------------------

Notes:

Both backend and frontend are separate applications. The frontend communicates with the backend through HTTP requests, which makes the system follow a distributed application structure.

------------------------------------

Author:

Taylan Takev
Faculty Number: 2401321045