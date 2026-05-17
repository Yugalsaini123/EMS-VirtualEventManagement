# API_DOCUMENTATION.md - RESTful API Reference

Live Demo: [EMS - Virtual Event Management System](http://ems-frontend-28551.centralindia.azurecontainer.io/)

## 🔐 Authentication

All protected endpoints require JWT token in Authorization header:

```
Authorization: Bearer <your_jwt_token>
```

### Authentication Endpoints

#### Register User
```
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe"
}

Response (201):
{
  "success": true,
  "message": "User registered successfully"
}
```

#### Login
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}

Response (200):
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "isAdmin": false
  }
}
```

#### Refresh Token
```
POST /api/auth/refresh
Content-Type: application/json

Response (200):
{
  "success": true,
  "token": "new_jwt_token..."
}
```

---

## 📅 Event Endpoints

### List All Events
```
GET /api/events
Authorization: Bearer <token>

Response (200):
{
  "success": true,
  "data": [
    {
      "id": 1,
      "title": "Virtual Conference",
      "description": "Annual tech conference",
      "startTime": "2026-06-01T09:00:00Z",
      "endTime": "2026-06-01T17:00:00Z",
      "capacity": 500,
      "attendees": 250,
      "createdBy": "admin@example.com",
      "status": "Upcoming"
    },
    ...
  ]
}
```

### Get Event by ID
```
GET /api/events/{id}
Authorization: Bearer <token>

Response (200):
{
  "success": true,
  "data": {
    "id": 1,
    "title": "Virtual Conference",
    "description": "Annual tech conference",
    "startTime": "2026-06-01T09:00:00Z",
    "endTime": "2026-06-01T17:00:00Z",
    "capacity": 500,
    "attendees": 250,
    "createdBy": "admin@example.com",
    "status": "Upcoming",
    "registrations": [
      {
        "userId": 2,
        "userName": "john@example.com",
        "registeredAt": "2026-05-15T10:30:00Z"
      }
    ]
  }
}
```

### Create Event (Admin Only)
```
POST /api/events
Authorization: Bearer <admin_token>
Content-Type: application/json

{
  "title": "New Webinar",
  "description": "Learn about new features",
  "startTime": "2026-06-15T14:00:00Z",
  "endTime": "2026-06-15T15:30:00Z",
  "capacity": 1000
}

Response (201):
{
  "success": true,
  "data": {
    "id": 2,
    "title": "New Webinar",
    ...
  }
}
```

### Update Event (Admin Only)
```
PUT /api/events/{id}
Authorization: Bearer <admin_token>
Content-Type: application/json

{
  "title": "Updated Webinar Title",
  "capacity": 1500
}

Response (200):
{
  "success": true,
  "data": {
    "id": 2,
    "title": "Updated Webinar Title",
    ...
  }
}
```

### Delete Event (Admin Only)
```
DELETE /api/events/{id}
Authorization: Bearer <admin_token>

Response (204): No Content
```

---

## 👥 User Endpoints

### Get User Profile
```
GET /api/users/{id}
Authorization: Bearer <token>

Response (200):
{
  "success": true,
  "data": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "isAdmin": false,
    "createdAt": "2026-01-15T08:30:00Z",
    "registeredEvents": [
      {
        "eventId": 1,
        "eventTitle": "Virtual Conference"
      }
    ]
  }
}
```

### Update User Profile
```
PUT /api/users/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith"
}

Response (200):
{
  "success": true,
  "data": {
    "id": 1,
    "email": "user@example.com",
    "firstName": "Jane",
    "lastName": "Smith"
  }
}
```

### Get All Users (Admin Only)
```
GET /api/users
Authorization: Bearer <admin_token>

Response (200):
{
  "success": true,
  "data": [
    {
      "id": 1,
      "email": "user1@example.com",
      "firstName": "John",
      "isAdmin": false
    },
    ...
  ],
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 20
}
```

---

## 🎫 Registration Endpoints

### Register for Event
```
POST /api/events/{eventId}/register
Authorization: Bearer <token>

Response (201):
{
  "success": true,
  "message": "Successfully registered for event",
  "data": {
    "registrationId": 123,
    "eventId": 1,
    "userId": 2,
    "registeredAt": "2026-05-17T10:00:00Z",
    "status": "Active"
  }
}
```

### Cancel Registration
```
DELETE /api/events/{eventId}/register
Authorization: Bearer <token>

Response (204): No Content
```

### Get Event Registrations
```
GET /api/events/{eventId}/registrations
Authorization: Bearer <admin_token>

Response (200):
{
  "success": true,
  "data": [
    {
      "registrationId": 123,
      "userId": 2,
      "userEmail": "user@example.com",
      "userName": "John Doe",
      "registeredAt": "2026-05-17T10:00:00Z",
      "status": "Active"
    },
    ...
  ]
}
```

---

## 🔍 Health Check

### Application Health
```
GET /api/health
No authentication required

Response (200):
{
  "status": "Healthy",
  "timestamp": "2026-05-17T14:30:00Z",
  "database": "Connected",
  "version": "1.0.0"
}
```

---

## 📊 Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "error": "Invalid email format",
  "statusCode": 400
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "error": "Missing or invalid authentication token",
  "statusCode": 401
}
```

### 403 Forbidden
```json
{
  "success": false,
  "error": "Only administrators can perform this action",
  "statusCode": 403
}
```

### 404 Not Found
```json
{
  "success": false,
  "error": "Event not found",
  "statusCode": 404
}
```

### 409 Conflict
```json
{
  "success": false,
  "error": "User already registered for this event",
  "statusCode": 409
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "error": "An unexpected error occurred",
  "statusCode": 500
}
```

---

## 📝 Query Parameters

### Pagination
```
GET /api/events?pageNumber=1&pageSize=20
GET /api/users?pageNumber=1&pageSize=10
```

### Filtering
```
GET /api/events?status=Upcoming
GET /api/users?role=admin
```

### Sorting
```
GET /api/events?sortBy=startTime&sortOrder=ascending
GET /api/events?sortBy=attendees&sortOrder=descending
```

---

## 🔐 Role-Based Access Control

### User Roles
- **User** - Can register for events, view own profile
- **Admin** - Can create/update/delete events, view all users

### Protected Endpoints
- **Public**: /api/auth/register, /api/auth/login, /api/health
- **User**: /api/events (list), /api/users/{id} (own profile)
- **Admin**: POST/PUT/DELETE events, GET all users

---

## 🧪 Testing with Swagger

Access API documentation and test endpoints:
```
http://localhost:5000/swagger
or
http://ems-api-28551.centralindia.azurecontainer.io:5000/swagger
```

### Try It Out
1. Login to get token
2. Click "Authorize" button
3. Paste token: `Bearer <your_token>`
4. Try endpoints directly in Swagger UI

---

## 📌 Rate Limiting (Optional)

Future implementation:
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1654020000
```

---

## 📖 Related Documentation

- **[README.md](./README.md)** - Project overview
- **[SETUP.md](./SETUP.md)** - Setup instructions
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - System design

---

**Status**: ✅ API Ready

**Last Updated**: May 17, 2026

**Swagger UI**: Available on deployed API
