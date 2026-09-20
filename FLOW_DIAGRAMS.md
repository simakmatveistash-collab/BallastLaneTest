# Application Flow Diagrams

## User Journey Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    APPLICATION ENTRY POINT                       │
│                     http://localhost:5173                       │
└────────────────────────────┬────────────────────────────────────┘
							 │
					┌─────────▼─────────┐
					│  Check Auth Token │
					└─────────┬─────────┘
							  │
		┌─────────────────────┴─────────────────────┐
		│                                           │
	╔═══▼════════╗                            ┌─────▼──────┐
	║ Token Found ║                            │ No Token   │
	║ (Logged In) ║                            │ (Guest)    │
	╚═════╤══════╝                            └─────┬──────┘
		  │                                        │
		  │                                  ┌─────▼──────────┐
		  │                                  │  Auth Routes   │
		  │                                  │ /login /reg... │
		  │                                  └────┬───────────┘
		  │                                       │
	┌─────▼──────────────┐              ┌─────────▼─────────────┐
	│   Records Page     │              │  Login/Register Page  │
	│  /records          │              │  Form + Validation    │
	│  - List records    │              └─────────┬─────────────┘
	│  - Create record   │                        │
	│  - Edit record     │              ┌─────────▼──────────┐
	│  - Delete record   │              │ On Success Login   │
	└────┬───────────────┘              │ - Get JWT Token    │
		 │                              │ - Save to Storage  │
		 │◄────────────────────────────┴─ Redirect to /recs │
		 │                              └────────────────────┘
		 │
	┌────▼────────────────────┐
	│   Sign Out (Logout)     │
	│ - Clear JWT Token       │
	│ - Redirect to /login    │
	└────────────────────────┘
```

---

## Authentication Flow

```
USER REGISTRATION
═════════════════════════════════════════════════════════════════
┌──────────────┐       ┌────────────────┐       ┌──────────────┐
│   Frontend   │       │    Backend     │       │   Database   │
│RegisterPage  │       │  API Handler   │       │   (SQL)      │
└──────────────┘       └────────────────┘       └──────────────┘
	  │                       │                        │
	  │  Fill form data       │                        │
	  │  - Name              │                        │
	  │  - Email             │                        │
	  │  - Password          │                        │
	  │                      │                        │
	  │  POST /api/auth/register with data           │
	  ├─────────────────────►│                        │
	  │                      │                        │
	  │                      │  Hash password         │
	  │                      │  Validate input        │
	  │                      │  Check if email exists │
	  │                      │                        │
	  │                      │  INSERT user record   │
	  │                      ├───────────────────────►│
	  │                      │                        │
	  │                      │◄───────────────────────┤
	  │                      │  User ID returned      │
	  │                      │                        │
	  │  201 Created + UserDto                        │
	  │◄─────────────────────┤                        │
	  │                      │                        │
	  │  Show success message│                        │
	  │  Redirect to login   │                        │
	  │                      │                        │


USER LOGIN
═════════════════════════════════════════════════════════════════
┌──────────────┐       ┌────────────────┐       ┌──────────────┐
│   Frontend   │       │    Backend     │       │   Database   │
│  LoginPage   │       │  API Handler   │       │   (SQL)      │
└──────────────┘       └────────────────┘       └──────────────┘
	  │                       │                        │
	  │  Fill form data       │                        │
	  │  - Email              │                        │
	  │  - Password           │                        │
	  │                      │                        │
	  │  POST /api/auth/login with credentials        │
	  ├─────────────────────►│                        │
	  │                      │                        │
	  │                      │  SELECT user by email │
	  │                      ├───────────────────────►│
	  │                      │                        │
	  │                      │◄───────────────────────┤
	  │                      │  User record returned  │
	  │                      │  (with hashed pwd)    │
	  │                      │                        │
	  │                      │  Compare password      │
	  │                      │  hash with input       │
	  │                      │                        │
	  │                      │  Generate JWT token    │
	  │                      │  (signed with secret)  │
	  │                      │                        │
	  │  200 OK + Token      │                        │
	  │  + UserDto           │                        │
	  │◄─────────────────────┤                        │
	  │                      │                        │
	  │  Save token to:      │                        │
	  │  localStorage        │                        │
	  │                      │                        │
	  │  Store user in       │                        │
	  │  React Context       │                        │
	  │                      │                        │
	  │  Redirect to         │                        │
	  │  /records page       │                        │
	  │                      │                        │

AUTHORIZED REQUEST
═════════════════════════════════════════════════════════════════
┌──────────────┐       ┌────────────────┐       ┌──────────────┐
│   Frontend   │       │    Backend     │       │   Database   │
│RecordsPage   │       │  API Handler   │       │   (SQL)      │
└──────────────┘       └────────────────┘       └──────────────┘
	  │                       │                        │
	  │  Get token from       │                        │
	  │  localStorage         │                        │
	  │                      │                        │
	  │  Add header:         │                        │
	  │  Authorization:      │                        │
	  │  Bearer <token>      │                        │
	  │                      │                        │
	  │  GET /api/records    │                        │
	  ├─────────────────────►│                        │
	  │                      │                        │
	  │                      │  Verify token         │
	  │                      │  (check signature)     │
	  │                      │  Extract user ID      │
	  │                      │                        │
	  │                      │  SELECT records       │
	  │                      │  WHERE userId = XX    │
	  │                      ├───────────────────────►│
	  │                      │                        │
	  │                      │◄───────────────────────┤
	  │                      │  Records list returned │
	  │                      │                        │
	  │  200 OK + Records    │                        │
	  │◄─────────────────────┤                        │
	  │                      │                        │
	  │  Render records      │                        │
	  │  in grid layout      │                        │
	  │                      │                        │
```

---

## CRUD Operation Flow

```
CREATE RECORD
═════════════════════════════════════════════════════════════════
User clicks "+ New Record"
		 │
		 ▼
┌─────────────────────────┐
│ RecordForm Modal Opens  │
│ - Title input field    │
│ - Content textarea      │
│ - Validation enabled    │
└────────┬────────────────┘
		 │
	User fills form
		 │
		 ▼
┌──────────────────────┐
│ Validate inputs      │
│ - Title required     │
│ - Content required   │
└────────┬─────────────┘
		 │
	 Valid? ─────NO─────► Show error message ───┐
		 │                                        │
		YES                                       │
		 │                                        │
		 ▼                                        │
┌──────────────────────────────┐                │
│ POST /api/records            │                │
│ Body: { title, content }     │                │
│ Header: Auth token           │                │
└────────┬─────────────────────┘                │
		 │                                        │
	Backend:                                      │
  - Validate inputs                              │
  - Create record                                │
  - Save to DB                                   │
  - Return 201 + record                          │
		 │                                        │
	Response received                             │
		 │                                        │
		 ▼                                        │
  Backend success?                               │
		 │                                        │
	   YES                                        │
		 │                                        │
		 ▼                                        │
┌──────────────────────────────┐                │
│ - Add record to state        │                │
│ - Close modal                │                │
│ - Show success notification  │                │
│ - Re-render records list     │                │
└──────────────────────────────┘                │
												 │
		 ┌─────────────────────────────────────┘
		 │
		 ▼ (User can retry)
   Error state cleared


READ RECORDS
═════════════════════════════════════════════════════════════════
Page loads (useEffect)
		 │
		 ▼
┌──────────────────────┐
│ SET isLoading = true │
│ Clear error          │
└────────┬─────────────┘
		 │
		 ▼
┌──────────────────────────────┐
│ GET /api/records             │
│ Header: Auth token           │
└────────┬─────────────────────┘
		 │
		 ▼
┌────────────────────────────┐
│ Backend returns array of   │
│ RecordDto objects          │
└────────┬───────────────────┘
		 │
		 ▼
┌──────────────────────────────┐
│ - Store records in state     │
│ - SET isLoading = false      │
│ - Render RecordsList         │
└──────────────────────────────┘
		 │
		 ▼
┌──────────────────────────────┐
│ RecordsList component        │
│ Maps over array              │
│ Renders card for each record │
└──────────────────────────────┘


UPDATE RECORD
═════════════════════════════════════════════════════════════════
User clicks Edit (✎) on card
		 │
		 ▼
┌──────────────────────────────┐
│ RecordForm modal opens       │
│ Populate with existing data: │
│ - title = record.title       │
│ - content = record.content   │
└────────┬─────────────────────┘
		 │
   User edits form
		 │
		 ▼
┌──────────────────────────────┐
│ User clicks Update            │
│ - Validate inputs             │
└────────┬─────────────────────┘
		 │
	 Valid?
		 │
		YES
		 │
		 ▼
┌──────────────────────────────┐
│ PUT /api/records/{id}        │
│ Body: { title, content }     │
│ Header: Auth token           │
└────────┬─────────────────────┘
		 │
	Backend:
  - Verify ownership
  - Update record
  - Save to DB
  - Return 200 + updated
		 │
	Response received
		 │
		 ▼
┌──────────────────────────────┐
│ - Update record in state     │
│ - Close modal                │
│ - Show success notification  │
│ - Re-render card             │
└──────────────────────────────┘


DELETE RECORD
═════════════════════════════════════════════════════════════════
User clicks Delete (🗑) on card
		 │
		 ▼
┌──────────────────────────────┐
│ Confirm dialog               │
│ "Are you sure you want to... │
│  delete this record?"        │
└────────┬─────────────────────┘
		 │
	User confirms
		 │
		 ▼
┌──────────────────────────────┐
│ DELETE /api/records/{id}     │
│ Header: Auth token           │
└────────┬─────────────────────┘
		 │
	Backend:
  - Verify ownership
  - Delete record
  - Return 204 No Content
		 │
	Response received
		 │
		 ▼
┌──────────────────────────────┐
│ - Remove from state array    │
│ - Re-render list             │
│ - Show success notification  │
└──────────────────────────────┘
```

---

## Component Hierarchy

```
┌─────────────────────────────────────────┐
│ BrowserRouter                           │
│ ┌───────────────────────────────────┐   │
│ │ AuthProvider                      │   │
│ │ ┌─────────────────────────────────┴─┐ │
│ │ │ App Component                   │ │
│ │ │ ┌─────────────────────────────┐ │ │
│ │ │ │ Conditional Header          │ │ │
│ │ │ │ (if authenticated)          │ │ │
│ │ │ │ ┌──────────────────────────┐│ │ │
│ │ │ │ │ Routes                   ││ │ │
│ │ │ │ │ ┌────────────────────────┤│ │ │
│ │ │ │ │ │ Route: /login          ││ │ │
│ │ │ │ │ │ ┌──────────────────────┤│ │ │
│ │ │ │ │ │ │ LoginPage            ││ │ │
│ │ │ │ │ │ ├──────────────────────┤│ │ │
│ │ │ │ │ │ │ - Email input        ││ │ │
│ │ │ │ │ │ │ - Password input     ││ │ │
│ │ │ │ │ │ │ - Form validation    ││ │ │
│ │ │ │ │ │ │ - Submit handler     ││ │ │
│ │ │ │ │ │ └──────────────────────┤│ │ │
│ │ │ │ │ ├────────────────────────┤│ │ │
│ │ │ │ │ │ Route: /register       ││ │ │
│ │ │ │ │ │ - RegisterPage         ││ │ │
│ │ │ │ │ ├────────────────────────┤│ │ │
│ │ │ │ │ │ Route: /records        ││ │ │
│ │ │ │ │ │ ├──────────────────────┤│ │ │
│ │ │ │ │ │ │ ProtectedRoute       ││ │ │
│ │ │ │ │ │ │ └──────────────────┐ ││ │ │
│ │ │ │ │ │ │   RecordsPage     │ ││ │ │
│ │ │ │ │ │ │   ┌─────────────┐ │ ││ │ │
│ │ │ │ │ │ │   │RecordsList  │ │ ││ │ │
│ │ │ │ │ │ │   │ - Grid      │ │ ││ │ │
│ │ │ │ │ │ │   │ - Cards     │ │ ││ │ │
│ │ │ │ │ │ │   │ - Actions   │ │ ││ │ │
│ │ │ │ │ │ │   └─────────────┘ │ ││ │ │
│ │ │ │ │ │ │                   │ ││ │ │
│ │ │ │ │ │ │   ┌─────────────┐ │ ││ │ │
│ │ │ │ │ │ │   │RecordForm   │ │ ││ │ │
│ │ │ │ │ │ │   │ - Modal     │ │ ││ │ │
│ │ │ │ │ │ │   │ - Form      │ │ ││ │ │
│ │ │ │ │ │ │   │ - Logic     │ │ ││ │ │
│ │ │ │ │ │ │   └─────────────┘ │ ││ │ │
│ │ │ │ │ │ │                   │ ││ │ │
│ │ │ │ │ │ │ (Edit/Create mode)│ ││ │ │
│ │ │ │ │ │ └──────────────────┘ ││ │ │
│ │ │ │ │ └────────────────────┘│ │ │
│ │ │ │ └────────────────────────┘ │ │
│ │ │ ├─────────────────────────────┘ │ │
│ │ │ │ Service Layer               │ │
│ │ │ │ ├─────────────────────────┐ │ │
│ │ │ │ │ api.ts                  │ │ │
│ │ │ │ │ - HTTP client           │ │ │
│ │ │ │ │ - Auth headers          │ │ │
│ │ │ │ │ - Error handling        │ │ │
│ │ │ │ ├─────────────────────────┤ │ │
│ │ │ │ │ authService.ts          │ │ │
│ │ │ │ │ - Login/Register        │ │ │
│ │ │ │ │ - Token management      │ │ │
│ │ │ │ ├─────────────────────────┤ │ │
│ │ │ │ │ recordService.ts        │ │ │
│ │ │ │ │ - CRUD operations       │ │ │
│ │ │ │ └─────────────────────────┘ │ │
│ │ │ └─────────────────────────────┘ │ │
│ │ └─────────────────────────────────┘ │
│ └───────────────────────────────────────┘
└─────────────────────────────────────────┘
```

---

## State Flow

```
Global Auth State
═════════════════════════════════════════════════════════════════
┌──────────────────────────┐
│   AuthContext            │
│  ┌────────────────────┐  │
│  │ user: UserDto|null │  │
│  │ ├─ id              │  │
│  │ ├─ name            │  │
│  │ └─ email           │  │
│  │                    │  │
│  │ isAuthenticated    │  │
│  │ ├─ true (user?)    │  │
│  │ └─ false (no user) │  │
│  │                    │  │
│  │ isLoading: boolean │  │
│  │ ├─ true (pending)  │  │
│  │ └─ false (done)    │  │
│  │                    │  │
│  │ error: string|null │  │
│  │ ├─ "Invalid pwd"   │  │
│  │ └─ "User exists"   │  │
│  │                    │  │
│  │ Functions:         │  │
│  │ ├─ login()         │  │
│  │ ├─ register()      │  │
│  │ ├─ logout()        │  │
│  │ └─ clearError()    │  │
│  └────────────────────┘  │
│           ▲              │
│           │              │
│      Used by all         │
│      authenticated       │
│      components          │
└──────────────────────────┘

Local Component State
═════════════════════════════════════════════════════════════════
LoginPage State:
  - formData: { email, password }
  - validationError: string

RegisterPage State:
  - formData: { name, email, password, confirmPassword }
  - validationError: string

RecordsPage State:
  - records: RecordDto[]
  - isLoading: boolean
  - error: string|null
  - isFormOpen: boolean
  - editingRecord: RecordDto|null
  - successMessage: string|null

RecordForm State:
  - formData: { title, content }
  - isSubmitting: boolean
  - validationError: string
```

---

## Data Flow Diagram

```
User Input
	├─ Form submission
	├─ Button click
	└─ Route navigation
		  │
		  ▼
	Component Handler
	(onSubmit, onClick, etc)
		  │
		  ▼
	Service Method Call
	(authService, recordService)
		  │
		  ▼
	API Client (apiClient)
	├─ Prepare request
	├─ Add auth token
	├─ Send HTTP request
	└─ Handle response
		  │
		  ▼
	Backend API (.NET)
	├─ Authenticate
	├─ Validate
	├─ Process
	├─ Database
	└─ Return response
		  │
		  ▼
	Response received
	├─ Parse response
	├─ Handle errors
	└─ Update state
		  │
		  ▼
	Update React State
	├─ Context (auth)
	├─ Component state
	└─ UI refresh
		  │
		  ▼
	Re-render Component
	├─ Show data
	├─ Hide loading
	├─ Show success/error
	└─ User sees result
```

---

These diagrams provide visual representations of:
1. **User Journey**: How users navigate through the application
2. **Authentication Flow**: Login and registration processes
3. **CRUD Operations**: Create, Read, Update, and Delete flows
4. **Component Hierarchy**: Component structure and relationships
5. **State Flow**: State management within the application
6. **Data Flow**: How data moves through the system

All flows are production-ready and tested.
