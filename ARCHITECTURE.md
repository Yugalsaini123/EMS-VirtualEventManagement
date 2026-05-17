# ARCHITECTURE.md - System Design & Technical Architecture

Live Demo: [EMS - Virtual Event Management System](http://ems-frontend-28551.centralindia.azurecontainer.io/)

## 📊 System Overview

```
┌─────────────────────────────────────────────────────────┐
│                    End Users/Clients                    │
└────────────────────────┬────────────────────────────────┘
                         │ HTTP/HTTPS
                         ▼
┌─────────────────────────────────────────────────────────┐
│          Frontend Application (Angular)                 │
│  • Running in Docker Container (Port 80)               │
│  • Nginx reverse proxy                                  │
│  • Responsive UI                                        │
└────────────────────────┬────────────────────────────────┘
                         │ REST API Calls
                         ▼ (http://api:5000)
┌─────────────────────────────────────────────────────────┐
│         Backend API (ASP.NET Core)                      │
│  • Running in Docker Container (Port 5000)             │
│  • REST endpoints                                       │
│  • JWT authentication                                   │
│  • Business logic layer                                 │
└────────────────────────┬────────────────────────────────┘
                         │ EF Core
                         ▼
┌─────────────────────────────────────────────────────────┐
│          Database (SQL Server 2022)                     │
│  • Tables for Users, Events, Registrations             │
│  • Indexes for performance                             │
│  • Migrations for version control                      │
└─────────────────────────────────────────────────────────┘
```

---

## 🏗️ Architectural Layers

### 1. Presentation Layer (Frontend)
**Technology**: Angular 17+, TypeScript, HTML/CSS

**Components**:
- User Interface (Components, Templates)
- State Management (RxJS Observables)
- HTTP Client (API Communication)
- Routing (SPA Navigation)

**Responsibilities**:
- Display user interface
- Handle user interactions
- Validate user input (client-side)
- Format data for display
- Call API endpoints

### 2. API Layer (Backend)
**Technology**: ASP.NET Core 8.0, C#

**Components**:
- Controllers (HTTP endpoints)
- Middleware (Authentication, CORS, Logging)
- Services (Business logic)
- Models (Data transfer objects)
- Exceptions handlers

**Responsibilities**:
- Expose REST endpoints
- Validate requests
- Execute business logic
- Call data layer
- Return responses

### 3. Business Logic Layer (Services)
**Technology**: C# Classes

**Components**:
- AuthService (Authentication & Authorization)
- EventService (Event operations)
- UserService (User management)
- RegistrationService (Event registration)

**Responsibilities**:
- Core business rules
- Data validation
- Calculations
- Service orchestration
- Error handling

### 4. Data Access Layer (DAL)
**Technology**: Entity Framework Core

**Components**:
- DbContext (Database context)
- Entities (Domain models)
- Migrations (Schema versioning)
- Repositories (Data access)

**Responsibilities**:
- Database operations
- CRUD operations
- Query building
- Lazy loading
- Change tracking

### 5. Database Layer
**Technology**: SQL Server 2022

**Components**:
- Tables (Data storage)
- Relationships (Foreign keys)
- Indexes (Performance)
- Constraints (Data integrity)

**Responsibilities**:
- Persistent data storage
- Data validation
- Referential integrity
- Query execution

---

## 🗄️ Database Schema

### Tables

#### Users Table
```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    IsAdmin BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME DEFAULT GETUTCDATE()
);
```

#### Events Table
```sql
CREATE TABLE Events (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    Capacity INT,
    CreatedBy INT FOREIGN KEY REFERENCES Users(Id),
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME DEFAULT GETUTCDATE()
);
```

#### Registrations Table
```sql
CREATE TABLE Registrations (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    EventId INT FOREIGN KEY REFERENCES Events(Id),
    RegisteredAt DATETIME DEFAULT GETUTCDATE(),
    Status NVARCHAR(50) DEFAULT 'Active'
);
```

---

## 🔐 Security Architecture

### Authentication Flow
```
User Login
    ↓
Credentials → API /auth/login
    ↓
Verify password (bcrypt)
    ↓
Generate JWT Token
    ↓
Return token to Frontend
    ↓
Store in localStorage
    ↓
Include in Authorization header for subsequent requests
```

### JWT Token Structure
```
Header:
{
  "alg": "HS256",
  "typ": "JWT"
}

Payload:
{
  "sub": "user_id",
  "email": "user@example.com",
  "iat": 1234567890,
  "exp": 1234571490
}

Signature:
HMACSHA256(
  base64UrlEncode(header) + "." +
  base64UrlEncode(payload),
  secret
)
```

### Security Measures
- ✅ Password hashing with bcrypt
- ✅ JWT token expiration
- ✅ CORS restrictions
- ✅ SQL injection prevention (parameterized queries)
- ✅ Cross-site scripting (XSS) protection
- ✅ HTTPS in production
- ✅ Non-root Docker containers

---

## 📁 Project Structure

### EMS.API
```
EMS.API/
├── Controllers/           # HTTP endpoints
│   ├── AuthController.cs
│   ├── EventsController.cs
│   └── UsersController.cs
├── Models/               # DTOs and request models
│   ├── LoginRequest.cs
│   ├── EventDto.cs
│   └── UserDto.cs
├── Services/            # Business logic
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── IEventService.cs
│   └── EventService.cs
├── Middleware/          # Custom middleware
│   └── ErrorHandlingMiddleware.cs
├── Program.cs           # Application startup
├── appsettings.json     # Configuration
└── Dockerfile           # Container definition
```

### EMS.DAL
```
EMS.DAL/
├── Entities/           # Domain models
│   ├── User.cs
│   ├── Event.cs
│   └── Registration.cs
├── DbContext/          # Entity Framework
│   └── EMSDbContext.cs
├── Migrations/         # Database migrations
│   ├── Initial.cs
│   └── AddEventTable.cs
└── Repositories/       # Data access (optional)
```

### EMS.Services
```
EMS.Services/
├── AuthService.cs       # Authentication
├── EventService.cs      # Event operations
├── UserService.cs       # User management
└── IServices/           # Service interfaces
```

### EMS.Frontend
```
EMS.Frontend/
├── src/
│   ├── app/
│   │   ├── components/     # Angular components
│   │   ├── services/       # HTTP services
│   │   ├── guards/         # Route guards
│   │   ├── models/         # TypeScript models
│   │   ├── app.component.ts
│   │   └── app-routing.module.ts
│   ├── assets/            # Static files
│   ├── environments/       # Environment configs
│   │   ├── environment.ts
│   │   └── environment.prod.ts
│   └── main.ts
├── nginx.conf           # Nginx configuration
├── Dockerfile           # Container definition
├── angular.json         # Angular config
├── package.json         # Dependencies
└── tsconfig.json        # TypeScript config
```

---

## 🐳 Docker Architecture

### Docker Compose Network
```
┌──────────────────────────────────────────────┐
│         ems-network (Bridge Network)         │
│                                              │
│  ┌──────────────┐        ┌──────────────┐  │
│  │  frontend    │        │     api      │  │
│  │  container   │───────→│  container   │  │
│  │  (Port 80)   │        │ (Port 5000)  │  │
│  └──────────────┘        └────┬─────────┘  │
│                                │            │
└────────────────────────────────┼────────────┘
                                 │
                    host.docker.internal:1433
                                 │
                        ┌────────▼────────┐
                        │  SQL Server     │
                        │  (Host Machine) │
                        └─────────────────┘
```

### Container Configuration

#### API Container (ems-api)
- Base Image: mcr.microsoft.com/dotnet/aspnet:8.0
- Port: 5000
- Environment: Production
- Health Check: GET /health every 30s

#### Frontend Container (ems-frontend)
- Base Image: nginx:latest (multi-stage: node:latest)
- Port: 80
- Environment: Production
- Health Check: GET / every 30s

---

## 🔄 API Request/Response Flow

### Example: User Login

**Request**:
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Processing**:
1. Controller receives request
2. AuthService validates credentials
3. Check user exists in database
4. Verify password hash
5. Generate JWT token
6. Return token and user info

**Response**:
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

---

## 🚀 Deployment Architecture

### Local Development
```
Docker Compose
├── API Container (localhost:5000)
├── Frontend Container (localhost:80)
└── Local SQL Server (localhost:1433)
```

### Azure Production
```
Azure Container Registry
├── ems-api:latest image
└── ems-frontend:latest image
        ↓
Azure Container Instances
├── API Instance (Public IP)
├── Frontend Instance (Public IP)
└── Azure SQL Database OR Local SQL via VPN
```

---

## 📈 Scalability Considerations

### Horizontal Scaling
- Stateless API design (scale API containers)
- Shared database (single point)
- Load balancer in front (Azure Load Balancer)
- Session management (JWT - stateless)

### Vertical Scaling
- Increase container CPU/RAM
- Database optimization (indexing)
- Connection pooling
- Query optimization

### Optimization Techniques
- Caching (Redis)
- CDN for static content
- Database replication
- Connection pooling
- Async operations

---

## 🔧 Technology Decisions

| Component | Choice | Rationale |
|-----------|--------|-----------|
| Backend | ASP.NET Core 8.0 | Type-safe, high performance, .NET ecosystem |
| Frontend | Angular 17+ | Full framework, TypeScript, enterprise ready |
| Database | SQL Server 2022 | Reliable, T-SQL, Azure integration |
| Containerization | Docker | Industry standard, portable, scalable |
| Cloud Platform | Azure | Enterprise support, integrated services |
| Authentication | JWT | Stateless, scalable, REST-friendly |
| Password Hashing | bcrypt | Industry standard, slow by design |
| ORM | Entity Framework | Type-safe, LINQ support, migrations |

---

## 📝 Database Migrations

### Running Migrations

```bash
# Create new migration
dotnet ef migrations add AddNewTable

# Apply to database
dotnet ef database update

# Revert last migration
dotnet ef migrations remove
```

### Migration History
1. Initial - Create Users, Events, Registrations tables
2. AddIndexes - Add performance indexes
3. AddConstraints - Add referential integrity

---

## 🔄 Deployment Process

```
Code Commit
    ↓
CI/CD Pipeline (optional)
    ↓
Build Docker Image
    ↓
Push to Azure Container Registry
    ↓
Deploy to Azure Container Instances
    ↓
Health Checks
    ↓
Live ✅
```

---

## 📖 Related Documentation

- **[README.md](./README.md)** - Project overview
- **[SETUP.md](./SETUP.md)** - Local setup
- **[DEPLOYMENT.md](./DEPLOYMENT.md)** - Deployment guide
- **[API_DOCUMENTATION.md](./API_DOCUMENTATION.md)** - API reference

---

**Status**: ✅ Production Architecture

**Last Updated**: May 17, 2026
