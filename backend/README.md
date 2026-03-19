# Household Budget Planner API - Backend

ASP.NET Core Web API backend for managing household finances, expenses, budgets, bills, and savings goals with JWT authentication and Entity Framework Core with PostgreSQL.

## Project Status

Current implementation state:
- Backend foundations are in place and the financial controllers are wired to services.
- Authentication is implemented.
- Financial services and savings-goal workflows are implemented.
- Migrations are present, but automated tests have not been added yet.
- Frontend integration is still in progress.

## Project Structure

```
backend/
├── Controllers/          # API endpoints (10 controllers)
├── Services/            # Business logic (10 service interfaces + implementations)
├── Data/                # Entity Framework DbContext
├── Entities/            # Domain models (9 entities)
├── DTOs/                # Request/Response data transfer objects
├── Middleware/          # Custom middleware (exception handling, logging)
├── Helpers/             # Utilities (JWT, password hashing, claims)
├── Config/              # Configuration classes
├── Mappings/            # AutoMapper profiles
├── Properties/          # Launch settings
├── Program.cs           # Application startup and DI
├── appsettings.json     # Production config
└── appsettings.Development.json  # Development config
```

## Technologies Used

- **Framework:** ASP.NET Core 8.0
- **Database:** PostgreSQL with Entity Framework Core 9.0
- **Authentication:** JWT (JSON Web Tokens)
- **Password Hashing:** BCrypt.Net
- **Dependency Injection:** Built-in ASP.NET Core DI
- **Logging:** Built-in ILogger
- **API Documentation:** Swagger/OpenAPI
- **Mapping:** AutoMapper
- **Validation:** FluentValidation (ready to use)
- **CORS:** Configured for Vite frontend

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL 13+ installed and running
- A code editor (Visual Studio, VS Code, etc.)

## Setup Instructions

### 1. Configure Database Connection

Edit `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=household_budget_dev;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

### 2. Create PostgreSQL Database

```bash
createdb household_budget_dev
```

Or using psql:

```sql
CREATE DATABASE household_budget_dev;
```

### 3. Restore Dependencies

```bash
cd backend
dotnet restore
```

### 4. Apply EF Core Migrations

```bash
dotnet ef database update
```

This will create all tables in the database.

### 5. Run the Application

```bash
dotnet run
```

The API will start at `https://localhost:5001` (or check console output for exact URL).

### 6. Access Swagger Documentation

Navigate to: `https://localhost:5001/swagger`

Here you can test all endpoints directly in the browser.

## Current Architecture

### Entity Relationships

- **User** (Many-to-1) → Household
- **Household** (1-to-Many) → Users, Expenses, Income, Budgets, Bills, Categories, SavingsGoals
- **Category** (1-to-Many) → Expenses, Income, Budgets, Bills (can be system-default or custom)
- **Expense** (Many-to-1) → Household, Category, User (who paid)
- **Income** (Many-to-1) → Household, Category, User
- **Budget** (Many-to-1) → Household, Category (unique constraint: one per month per category)
- **Bill** (Many-to-1) → Household, Category (supports recurring and one-time)
- **SavingsGoal** (1-to-Many) → GoalContribution
- **GoalContribution** (Many-to-1) → SavingsGoal, User

### Key Features

✅ **JWT Authentication**
- Token generation with configurable expiration
- Claims-based user identification
- Household scoping in claims

✅ **Security**
- BCrypt password hashing with work factor 12
- CORS configured for localhost:5173 (Vite)
- Authorization checks on all protected endpoints
- Household isolation (users can only access their household data)

✅ **Exception Handling**
- Global exception middleware with consistent error responses
- Proper HTTP status codes
- Detailed logging

✅ **Database**
- Automatic migrations on startup
- Proper foreign key constraints
- Unique constraints (email, budget per month/category)
- Audit fields (CreatedAt, UpdatedAt) on all entities

## API Endpoints (Summary)

### Authentication (`/api/auth`)
- `POST /register` - Register new user and create household
- `POST /login` - Login with email/password
- `GET /me` - Get current user info (requires auth)

### Households (`/api/households`)
- `GET` - Get household details
- `GET /members` - Get household members

### Categories (`/api/categories`)
- `GET` - Get all categories (system + custom)
- `GET /{id}` - Get single category
- `POST` - Create custom category
- `PUT /{id}` - Update category
- `DELETE /{id}` - Delete category

### Expenses (`/api/expenses`)
- `GET` - Get expenses with optional filtering
- `GET /{id}` - Get single expense
- `POST` - Create expense
- `PUT /{id}` - Update expense
- `DELETE /{id}` - Delete expense

### Income (`/api/income`)
- `GET` - Get all income entries
- `GET /{id}` - Get single income
- `POST` - Create income
- `PUT /{id}` - Update income
- `DELETE /{id}` - Delete income

### Budgets (`/api/budgets`)
- `GET ?year=2024&month=1` - Get budgets for month
- `GET /{id}` - Get single budget
- `POST` - Create budget
- `PUT /{id}` - Update budget
- `DELETE /{id}` - Delete budget

### Bills (`/api/bills`)
- `GET` - Get all bills
- `GET /upcoming` - Get upcoming bills
- `GET /{id}` - Get single bill
- `POST` - Create bill
- `PUT /{id}` - Update bill
- `DELETE /{id}` - Delete bill
- `POST /{id}/pay` - Mark bill as paid

### Savings Goals (`/api/savings-goals`)
- `GET` - Get all goals
- `GET /{id}` - Get single goal
- `POST` - Create goal
- `PUT /{id}` - Update goal
- `DELETE /{id}` - Delete goal

### Goal Contributions (`/api/goals/{goalId}/contributions`)
- `GET` - Get contributions to a goal
- `GET /{id}` - Get single contribution
- `POST` - Create contribution
- `PUT /{id}` - Update contribution
- `DELETE /{id}` - Delete contribution

### Dashboard (`/api/dashboard`)
- `GET /summary ?year=2024&month=1` - Get dashboard summary with income, expenses, budgets, upcoming bills, recent transactions, savings progress

## Environment Configuration

### Development (`appsettings.Development.json`)
- Debug logging enabled
- Extended JWT expiration (1440 minutes / 24 hours)
- CORS allows localhost:5173 and localhost:3000

### Production (`appsettings.json`)
- Information level logging
- Standard JWT expiration (60 minutes)
- Configure actual JWT secret (minimum 32 characters)
- Update database connection string

## JWT Configuration

Edit `appsettings.json`:

```json
"JwtSettings": {
  "Secret": "your-very-long-secret-key-minimum-32-characters",
  "Issuer": "HouseholdBudgetApi",
  "Audience": "HouseholdBudgetApp",
  "ExpirationMinutes": 60
}
```

**Important:** For production, use a strong secret key with at least 32 characters.

## Next Steps

### STEP 7: Database Verification
- Start PostgreSQL locally
- Run `dotnet ef database update`
- Exercise the implemented endpoints through Swagger

### STEP 8: Frontend Integration
- Add protected route handling
- Replace mock dashboard data with live API data
- Build feature-level API hooks and pages for expenses, income, budgets, bills, and savings

### STEP 9: Testing & Hardening
- Add backend integration tests
- Add frontend component and routing tests
- Review package vulnerabilities and deployment settings

## Testing Endpoints with Swagger

1. Navigate to `https://localhost:5001/swagger`
2. All endpoints will show 501 (Not Implemented) until STEP 5-6 are completed
3. Click the lock icon to authenticate with JWT token once registration/login are implemented

## Troubleshooting

### Database Connection Failed
- Verify PostgreSQL is running: `psql --version`
- Check connection string in `appsettings.Development.json`
- Ensure database exists: `createdb household_budget_dev`

### Migration Errors
- Delete existing migrations and start fresh:
  ```bash
  dotnet ef database drop -f
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```

### Port Already in Use
- Change launch settings in `Properties/launchSettings.json`
- Or find process on port: `netstat -ano | findstr :5001`

## Architecture Principles

- **Clean Architecture:** Separation of concerns (Controllers → Services → Data)
- **SOLID Principles:** Each service has single responsibility
- **DDD Concepts:** Domain models with clear relationships
- **Async/Await:** All I/O operations are asynchronous
- **DTOs:** Clear separation between entities and API contracts
- **Security First:** Authorization checks, password hashing, JWT tokens
- **Household Isolation:** All queries filtered by household ID to ensure data privacy

## Notes for Frontend Integration

### API Base URL
Frontend should be configured to call `http://localhost:5001/api` in development.

### Authentication Flow
1. User registers → `POST /api/auth/register`
2. API returns JWT token
3. Store token in httpOnly cookie or secure storage
4. Include token in `Authorization: Bearer <token>` header for all requests
5. Handle 401 Unauthorized by redirecting to login

### CORS Configuration
Already configured to allow requests from `localhost:5173` (Vite default).

## Database Backup & Restore

### Backup
```bash
pg_dump -U postgres household_budget_dev > backup.sql
```

### Restore
```bash
psql -U postgres household_budget_dev < backup.sql
```

---

**Created:** March 19, 2026  
**Framework:** ASP.NET Core 9.0  
**Status:** Ready for authentication & business logic implementation
