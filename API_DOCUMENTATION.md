# BallastLane API Documentation

## Base URL
```
https://localhost:5001/api
```

## Authentication

The API uses token-based authentication. After login, include the token in requests:

```
Headers:
Authorization: Bearer {token}
```

Or for development/testing, pass userId as query parameter:
```
?userId=1
```

## Endpoints

### Authentication Endpoints

#### Register User
Create a new user account.

**Endpoint**: `POST /auth/register`

**Request Body**:
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Response** (201 Created):
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "createdDate": "2025-01-19T10:30:00Z",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Status Codes**:
- `201 Created`: User successfully registered
- `400 Bad Request`: Validation error or user already exists
- `500 Internal Server Error`: Server error

**Validation Rules**:
- Name: Required, non-empty string
- Email: Required, valid email format, unique
- Password: Required, minimum 8 characters recommended

**Errors**:
```json
{
  "message": "User with email 'john@example.com' already exists"
}
```

---

#### Login User
Authenticate user with credentials.

**Endpoint**: `POST /auth/login`

**Request Body**:
```json
{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Response** (200 OK):
```json
{
  "userId": 1,
  "email": "john@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-20T10:30:00Z"
}
```

**Status Codes**:
- `200 OK`: Login successful
- `401 Unauthorized`: Invalid credentials or user not found
- `400 Bad Request`: Validation error
- `500 Internal Server Error`: Server error

**Errors**:
```json
{
  "message": "Invalid email or password"
}
```

---

### Records Endpoints

All record endpoints require authentication.

#### Get All User Records
Retrieve all records created by the authenticated user.

**Endpoint**: `GET /records`

**Headers**:
```
Authorization: Bearer {token}
```

**Query Parameters**:
- `userId`: User ID (for development/testing without token)

**Response** (200 OK):
```json
[
  {
	"id": 1,
	"title": "Complete Project Setup",
	"content": "Finish the initial project configuration",
	"userId": 1,
	"createdDate": "2025-01-19T09:00:00Z",
	"updatedDate": "2025-01-19T09:00:00Z"
  },
  {
	"id": 2,
	"title": "Write Documentation",
	"content": "Create comprehensive API documentation",
	"userId": 1,
	"createdDate": "2025-01-19T10:00:00Z",
	"updatedDate": "2025-01-19T10:00:00Z"
  }
]
```

**Status Codes**:
- `200 OK`: Records retrieved successfully
- `401 Unauthorized`: Missing or invalid authentication
- `500 Internal Server Error`: Server error

---

#### Get Record by ID
Retrieve a specific record by ID.

**Endpoint**: `GET /records/{id}`

**Path Parameters**:
- `id`: Record ID (integer)

**Headers**:
```
Authorization: Bearer {token}
```

**Response** (200 OK):
```json
{
  "id": 1,
  "title": "Complete Project Setup",
  "content": "Finish the initial project configuration",
  "userId": 1,
  "createdDate": "2025-01-19T09:00:00Z",
  "updatedDate": "2025-01-19T09:00:00Z"
}
```

**Status Codes**:
- `200 OK`: Record retrieved successfully
- `401 Unauthorized`: Missing or invalid authentication
- `403 Forbidden`: Record belongs to another user
- `404 Not Found`: Record not found
- `500 Internal Server Error`: Server error

**Errors**:
```json
{
  "statusCode": 403,
  "message": "You do not have permission to access this record"
}
```

---

#### Create Record
Create a new record for the authenticated user.

**Endpoint**: `POST /records`

**Headers**:
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body**:
```json
{
  "title": "Buy groceries",
  "content": "Milk, eggs, bread, and vegetables"
}
```

**Response** (201 Created):
```json
{
  "id": 3,
  "title": "Buy groceries",
  "content": "Milk, eggs, bread, and vegetables",
  "userId": 1,
  "createdDate": "2025-01-19T11:00:00Z",
  "updatedDate": "2025-01-19T11:00:00Z"
}
```

**Status Codes**:
- `201 Created`: Record created successfully
- `400 Bad Request`: Validation error
- `401 Unauthorized`: Missing or invalid authentication
- `500 Internal Server Error`: Server error

**Validation Rules**:
- Title: Required, max 500 characters
- Content: Required, text content

**Errors**:
```json
{
  "statusCode": 400,
  "message": "Title and content are required"
}
```

---

#### Update Record
Update an existing record. User must be the owner.

**Endpoint**: `PUT /records/{id}`

**Path Parameters**:
- `id`: Record ID (integer)

**Headers**:
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body**:
```json
{
  "title": "Shopping list - Updated",
  "content": "Milk, eggs, bread, vegetables, and fruits"
}
```

**Response** (200 OK):
```json
{
  "id": 3,
  "title": "Shopping list - Updated",
  "content": "Milk, eggs, bread, vegetables, and fruits",
  "userId": 1,
  "createdDate": "2025-01-19T11:00:00Z",
  "updatedDate": "2025-01-19T11:30:00Z"
}
```

**Status Codes**:
- `200 OK`: Record updated successfully
- `400 Bad Request`: Validation error
- `401 Unauthorized`: Missing or invalid authentication
- `403 Forbidden`: Record belongs to another user
- `404 Not Found`: Record not found
- `500 Internal Server Error`: Server error

**Errors**:
```json
{
  "statusCode": 404,
  "message": "Record not found"
}
```

---

#### Delete Record
Delete a record. User must be the owner.

**Endpoint**: `DELETE /records/{id}`

**Path Parameters**:
- `id`: Record ID (integer)

**Headers**:
```
Authorization: Bearer {token}
```

**Response** (204 No Content):
```
(Empty response)
```

**Status Codes**:
- `204 No Content`: Record deleted successfully
- `401 Unauthorized`: Missing or invalid authentication
- `403 Forbidden`: Record belongs to another user
- `404 Not Found`: Record not found
- `500 Internal Server Error`: Server error

---

## Error Responses

All error responses follow this format:

```json
{
  "statusCode": 400,
  "message": "Descriptive error message"
}
```

### Common HTTP Status Codes

| Code | Meaning | 
|------|---------|
| `200` | OK - Request succeeded |
| `201` | Created - Resource successfully created |
| `204` | No Content - Request succeeded with no response body |
| `400` | Bad Request - Invalid request format or validation error |
| `401` | Unauthorized - Missing or invalid authentication |
| `403` | Forbidden - User lacks permission |
| `404` | Not Found - Resource not found |
| `500` | Internal Server Error - Server-side error |

---

## Rate Limiting

Currently not implemented. To be added in future versions.

---

## Data Types

### User Object
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "createdDate": "2025-01-19T10:30:00Z",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Record Object
```json
{
  "id": 1,
  "title": "Task Title",
  "content": "Task Description",
  "userId": 1,
  "createdDate": "2025-01-19T09:00:00Z",
  "updatedDate": "2025-01-19T10:00:00Z"
}
```

### Login Response
```json
{
  "userId": 1,
  "email": "john@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-20T10:30:00Z"
}
```

---

## Example cURL Commands

### Register
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
	"name": "Alice",
	"email": "alice@example.com",
	"password": "Password123!"
  }'
```

### Login
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
	"email": "alice@example.com",
	"password": "Password123!"
  }'
```

### Create Record (with userId query param for testing)
```bash
curl -X POST https://localhost:5001/api/records?userId=1 \
  -H "Content-Type: application/json" \
  -d '{
	"title": "My First Record",
	"content": "This is the content"
  }'
```

### Get All Records
```bash
curl -X GET https://localhost:5001/api/records?userId=1
```

### Get Specific Record
```bash
curl -X GET https://localhost:5001/api/records/1?userId=1
```

### Update Record
```bash
curl -X PUT https://localhost:5001/api/records/1?userId=1 \
  -H "Content-Type: application/json" \
  -d '{
	"title": "Updated Title",
	"content": "Updated content"
  }'
```

### Delete Record
```bash
curl -X DELETE https://localhost:5001/api/records/1?userId=1
```

---

## Swagger/OpenAPI

Access interactive API documentation at:
```
https://localhost:5001/openapi/v1.json
```

The Swagger UI provides a user-friendly interface to explore and test all endpoints.

---

## Changelog

### v1.0.0
- Initial API release
- Authentication endpoints (register, login)
- Record CRUD operations
- User ownership verification
