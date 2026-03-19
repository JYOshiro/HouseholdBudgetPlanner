# Household Budget Planner - Complete Project Overview

## Current Implementation Snapshot

- Backend API foundations are complete and the financial controllers are wired to working services.
- Authentication, categories, expenses, income, budgets, bills, savings goals, contributions, household queries, and dashboard summary endpoints are implemented.
- The frontend app shell exists, but most feature screens are still mock-driven and not yet fully integrated with the backend.
- The backend project currently targets .NET 8 in the codebase.

## Phase 1: Architecture Analysis & Setup (COMPLETED ✅)

### Frontend Architecture Analysis

#### Problems Identified:
1. **Too shallow** - All code crammed into `app/` and `pages/` folders
2. **No API layer** - No centralized service for backend communication
3. **No feature isolation** - Features mixed together without clear boundaries
4. **Missing types structure** - No dedicated types/interfaces folder
5. **Scattered state management** - Only ThemeContext, no auth context
6. **No route guards** - Routes don't distinguish between public/private
7. **Mixed component types** - UI, shared, and feature components not organized

#### Recommended Frontend Structure:

```
src/
├── app/                          # Application root & setup
│   ├── App.tsx                  # Root component (wraps providers)
│   ├── main.tsx                 # Entry point
│   ├── router/                  # Routing layer
│   │   ├── index.ts             # Router instance
│   │   ├── PrivateRoute.tsx      # Auth guard wrapper
│   │   └── routes.ts            # Route definitions
│   ├── providers/               # Global providers
│   │   ├── AppProviders.tsx      # Root provider wrapper
│   │   ├── ThemeProvider.tsx     # Theme context
│   │   └── AuthProvider.tsx      # Auth context with JWT
│   └── layouts/                 # Layout components
│       ├── RootLayout.tsx        # Root layout (public)
│       └── AppLayout.tsx         # App layout (private)
│
├── features/                    # Feature modules (each has own folder)
│   ├── auth/                    # Authentication feature
│   │   ├── hooks/               # useAuth, useLogin, useRegister
│   │   ├── pages/               # LoginPage, RegisterPage
│   │   ├── services/            # authService.ts
│   │   ├── context/             # AuthContext.tsx
│   │   └── types/               # auth.types.ts
│   │
│   ├── dashboard/               # Dashboard feature
│   │   ├── pages/               # DashboardPage.tsx
│   │   ├── components/          # SummaryCard, BudgetProgress, etc
│   │   ├── hooks/               # useDashboardData.ts
│   │   ├── services/            # dashboardService.ts
│   │   └── types/               # dashboard.types.ts
│   │
│   ├── expenses/                # Expenses (transactions) feature
│   │   ├── pages/               # ExpensesPage.tsx (formerly Transactions)
│   │   ├── components/          # ExpenseList, ExpenseForm, ExpenseFilters
│   │   ├── services/            # expenseService.ts
│   │   ├── hooks/               # useExpenses.ts
│   │   └── types/               # expense.types.ts
│   │
│   ├── income/                  # Income feature
│   │   ├── services/            # incomeService.ts
│   │   └── types/               # income.types.ts
│   │
│   ├── budgets/                 # Budgets feature
│   │   ├── pages/               # BudgetPage.tsx
│   │   ├── components/          # BudgetList, BudgetForm
│   │   ├── services/            # budgetService.ts
│   │   └── types/               # budget.types.ts
│   │
│   ├── bills/                   # Bills feature
│   │   ├── pages/               # BillsPage.tsx
│   │   ├── components/          # BillsList, BillForm, UpcomingBills
│   │   ├── services/            # billService.ts
│   │   └── types/               # bill.types.ts
│   │
│   ├── savings/                 # Savings Goals feature
│   │   ├── pages/               # SavingsPage.tsx
│   │   ├── components/          # GoalsList, GoalForm, ContributionForm
│   │   ├── services/            # savingsService.ts
│   │   └── types/               # savings.types.ts
│   │
│   ├── households/              # Household management
│   │   ├── pages/               # HouseholdPage.tsx
│   │   ├── components/          # MembersList, AddMemberModal
│   │   ├── services/            # householdService.ts
│   │   └── types/               # household.types.ts
│   │
│   ├── categories/              # Categories (if needed as feature)
│   │   ├── services/            # categoryService.ts
│   │   └── types/               # category.types.ts
│   │
│   └── settings/                # Settings feature
│       ├── pages/               # SettingsPage.tsx
│       ├── components/          # ThemeSettings, AccountSettings
│       └── services/            # settingsService.ts
│
├── components/                  # Global components
│   ├── ui/                      # Shadcn/UI + Figma components
│   │   ├── button.tsx
│   │   ├── card.tsx
│   │   ├── input.tsx
│   │   ├── dialog.tsx
│   │   └── ... (all UI components from Figma Figma design)
│   │
│   └── shared/                  # Reusable cross-feature components
│       ├── Header.tsx           # App header
│       ├── Sidebar.tsx          # Navigation sidebar
│       ├── LoadingSpinner.tsx
│       ├── ErrorBoundary.tsx
│       └── EmptyState.tsx
│
├── services/                    # Global services (API client, etc)
│   ├── api/
│   │   ├── apiClient.ts         # Axios wrapper with auth
│   │   ├── interceptors.ts      # Request/response interceptors
│   │   └── config.ts            # Base URL from .env
│   └── index.ts                 # Export all services
│
├── hooks/                       # Global custom hooks
│   ├── useQuery.ts
│   ├── useMutation.ts
│   ├── useLocalStorage.ts
│   └── index.ts
│
├── types/                       # Global types/DTOs
│   ├── api.types.ts             # API request/response types
│   ├── common.types.ts          # Shared types
│   ├── models.types.ts          # Domain models
│   └── index.ts
│
├── utils/                       # Utility functions
│   ├── formatters.ts            # Number, date, currency formatting
│   ├── validators.ts            # Form validators
│   ├── calculations.ts          # Budget calculations, etc
│   └── index.ts
│
├── styles/
│   ├── index.css
│   ├── fonts.css
│   ├── theme.css
│   └── tailwind.css
│
└── App.tsx                      # Root component
```

### Backend Architecture Setup (COMPLETED ✅)

#### Core Foundation Created:
- **Program.cs** - Full DI and middleware setup
- **ApplicationDbContext** - EF Core configuration with all relationships
- **9 Domain Entities** - User, Household, Category, Expense, Income, Budget, Bill, SavingsGoal, GoalContribution
- **10 Controllers** - Auth, Households, Categories, Expenses, Income, Budgets, Bills, SavingsGoals, GoalContributions, Dashboard
- **10 Service Interfaces & Implementations** - All async, ready for business logic
- **DTOs** - All request/response DTOs for each module
- **JWT Configuration** - Token generation and validation ready
- **Password Hashing** - BCrypt with work factor 12
- **Exception Middleware** - Global error handling
- **CORS Configuration** - Allows Vite frontend on localhost:5173
- **Swagger** - Full API documentation

#### Database Setup:
- PostgreSQL with Entity Framework Core
- Migrations will run automatically on startup
- Unique constraints for data integrity
- Proper foreign key relationships

## Current File Structure (As Implemented)

### Backend (`/backend`)
```
✅ Program.cs                           (DI/middleware setup)
✅ appsettings.json                     (production config)
✅ appsettings.Development.json         (development config)
✅ HouseholdBudgetApi.csproj            (targets .NET 8)
✅ README.md                            (setup guide)
✅ Entities/                            (9 domain entities)
✅ Data/ApplicationDbContext.cs         (EF configuration)
✅ Controllers/                         (auth + financial endpoints implemented)
✅ Services/                            (financial service layer implemented)
✅ DTOs/                                (request/response DTOs)
✅ Middleware/ExceptionMiddleware.cs    (global exception handling)
✅ Helpers/                             (JWT, password, claims)
✅ Config/                              (JWT settings)
✅ Mappings/MappingProfile.cs           (AutoMapper)
```

### Frontend
```
App shell, providers, auth context, and API client exist.
Feature screens still need live backend integration and route protection.
```

## Next Steps

### STEP 7: Database Verification & Swagger Testing
**Goal:** Validate the implemented API against a local PostgreSQL instance.**

**Tasks:**
1. Start PostgreSQL locally.
2. Run `dotnet ef database update` inside `backend`.
3. Run `dotnet run` and verify auth, category, expense, income, budget, bill, savings, contribution, household, and dashboard endpoints in Swagger.

### STEP 8: Frontend Integration
**Goal:** Replace mock data and connect the React app to the implemented API.**

**Tasks:**
1. Add protected route handling for `/app` routes.
2. Replace mock dashboard data with `/api/dashboard/summary`.
3. Create feature-level API modules and hooks for financial workflows.
4. Align frontend feature naming with backend domain names.
5. Wire loading, empty, and error states for authenticated screens.

### STEP 9: Testing & Hardening
**Goal:** Add quality gates and tighten operational defaults.**

**Tasks:**
1. Add backend integration tests for auth and at least one financial CRUD flow.
2. Add frontend tests for auth bootstrap and protected routing.
3. Address dependency warnings and package vulnerabilities.
4. Review deployment-time migration behavior and production config.

## File Mapping (Current → Target)

The following current frontend files should move as follows:

```
Current                          → New Location
─────────────────────────────────────────────────────
pages/Landing.tsx                → pages/Landing.tsx (keep as is)
pages/Dashboard.tsx              → features/dashboard/pages/DashboardPage.tsx
pages/Transactions.tsx           → features/expenses/pages/ExpensesPage.tsx
pages/Budget.tsx                 → features/budgets/pages/BudgetPage.tsx
pages/Bills.tsx                  → features/bills/pages/BillsPage.tsx
pages/Savings.tsx                → features/savings/pages/SavingsPage.tsx
pages/Household.tsx              → features/households/pages/HouseholdPage.tsx
pages/Settings.tsx               → features/settings/pages/SettingsPage.tsx

components/layout/Layout.tsx     → app/layouts/AppLayout.tsx
components/figma/*               → components/ui/ (already there)
context/ThemeContext.tsx         → app/providers/ThemeProvider.tsx

New files to create:
app/router/routes.ts
app/router/PrivateRoute.tsx
app/providers/AppProviders.tsx
app/providers/AuthProvider.tsx
types/api.types.ts
services/api/apiClient.ts
services/api/config.ts
... and feature-specific files
```

## Environment Setup

### Backend Environment Variables
```bash
# .env or appsettings.Development.json
DATABASE_URL="Host=localhost;Port=5432;Database=household_budget_dev;..."
JWT_SECRET="development-secret-key-minimum-32-characters"
JWT_EXPIRATION_MINUTES=1440
CORS_ORIGINS="http://localhost:5173"
```

### Frontend Environment Variables
```bash
# .env.development
VITE_API_BASE_URL=http://localhost:5001/api
VITE_APP_NAME=Household Budget Planner
```

## Key Design Decisions

### 1. Service Layer
- All business logic in services, not controllers (keeps controllers thin)
- Services handle validation errors as exceptions
- Global exception middleware catches and formats responses

### 2. DTOs Over Entities
- API never exposes entities directly
- All responses use specific DTOs
- Prevents accidental data leaks (e.g., password hashes)

### 3. Household Isolation
- Every database entity belongs to a household
- All queries filter by householdId
- User's householdId stored in JWT claim
- Cannot access data from another household

### 4. Async/Await
- All database operations are async
- Controllers are all async
- Improves app scalability

### 5. AutoMapper
- Maps entities to DTOs automatically
- Reduces boilerplate in services
- Configuration in MappingProfile.cs

## Testing the API

### Using Swagger (Recommended)
1. Start backend: `dotnet run`
2. Navigate to `https://localhost:5001/swagger`
3. Test endpoints directly

### Using Postman/Thunderclient
1. Import endpoints manually or auto-discover via Swagger
2. Set `Authorization: Bearer <token>` header after login

### Using Frontend (Once integrated)
1. Register new user
2. Frontend receives JWT token
3. All subsequent requests include token automatically

## Performance Considerations

- Entity Framework queries use `.AsNoTracking()` for read-only operations
- Pagination on list endpoints (expenses, income)
- Indexes on frequently queried columns (email, householdId, dates)
- Database indexes on foreign keys automatically

## Security Checklist

✅ JWT authentication enabled
✅ Password hashing with BCrypt
✅ CORS configured for specific origins
✅ Household data isolation
✅ HttpOnly cookie option ready
✅ Global exception handling (no stack traces leaked)
✅ Validation on input
- [ ] Rate limiting (TODO - consider for production)
- [ ] HTTPS only in production (TODO - configure load balancer)
- [ ] API key for third-party integrations (TODO - if needed)

## Common Patterns

### Creating a New Module (e.g., new feature)

1. **Create Entities in `/Entities`**
2. **Add DbSet to ApplicationDbContext**
3. **Create DTOs in `/DTOs/FeatureName`**
4. **Create IServiceInterface in `/Services`**
5. **Create ServiceImplementation in `/Services`**
6. **Add AutoMapper mappings in `/Mappings/MappingProfile.cs`**
7. **Register service in `Program.cs`**
8. **Create Controller in `/Controllers`**

## Deployment Considerations

### Development
- Vite dev server + Express middleware for backend
- Hot reload enabled
- Debug logging

### Production
- Build Vite app to static files
- Serve from ASP.NET Core static files
- Use PostgreSQL managed service (Azure Database, AWS RDS)
- Configure HTTPS/SSL
- Set secure JWT secret
- Enable rate limiting
- Configure logging to centralized service
- Set proper CORS origins

## File Statistics

- **Backend Files Created:** 80+ files
- **Entities:** 9
- **Controllers:** 10
- **Services:** 10 (interface + implementation)
- **DTOs:** 30+
- **Lines of Code:** ~5,000+ (backend only)

## Next: STEP 5 - Authentication Implementation

Ready to implement? Follow these steps:

1. **Implement RegisterAsync** in AuthService
   - Validate input
   - Hash password
   - Create household
   - Generate JWT token

2. **Implement LoginAsync** in AuthService
   - Find user
   - Verify password
   - Generate JWT token

3. **Test via Swagger**

4. **Then proceed to STEP 6** - Financial modules

---

**Status:** Phase 1 ✅ Complete  
**Date:** March 19, 2026  
**Next:** Phase 2 - Authentication & Financial Modules
