# Functional Specification

## 1. Objective
Provide households with a centralized platform to track income, expenses, budgets, bills, and savings goals while maintaining clear household-level access controls and usable financial summaries.

## 2. In-Scope Functional Domains
- Authentication and user identity.
- Household context and member visibility.
- Category management (system and household custom categories).
- Expense recording and maintenance.
- Income recording and maintenance.
- Budget planning by category and month.
- Bill tracking and payment status updates.
- Savings goal planning and contribution tracking.
- Dashboard summary for financial overview.

## 3. Out of Scope (Current Baseline)
- Multi-household membership for a single user account.
- Native mobile applications.
- Automated bank account aggregation.
- Advanced forecasting and predictive analytics.

## 4. Actors
- Household Member: Authenticated user with access to household-scoped financial data.
- System: API and persistence layer enforcing business rules and data integrity.

## 5. Functional Requirements

### FR-01 User Registration
- The system shall allow a new user to register with credentials and household context.
- The system shall create an associated household context during registration flow where required by backend logic.

### FR-02 User Login
- The system shall authenticate users by email and password.
- The system shall issue a JWT token after successful authentication.

### FR-03 Current User Retrieval
- The system shall provide a secure endpoint to return the current authenticated user profile.

### FR-04 Household Data Isolation
- The system shall scope all protected financial data access to the authenticated user household.

### FR-05 Category Management
- The system shall list available categories for the user context.
- The system shall allow create, update, and delete operations for eligible categories.

### FR-06 Expense Management
- The system shall support create, read, update, and delete operations for expenses.
- The system shall support filtered retrieval to support analysis views.

### FR-07 Income Management
- The system shall support create, read, update, and delete operations for income entries.

### FR-08 Budget Management
- The system shall support monthly budget planning by category.
- The system shall enforce one budget entry per household, category, and month.

### FR-09 Bill Management
- The system shall support create, read, update, and delete operations for bills.
- The system shall provide an upcoming bills view.
- The system shall support explicit mark-as-paid behavior.

### FR-10 Savings Goals
- The system shall support create, read, update, and delete operations for savings goals.

### FR-11 Goal Contributions
- The system shall support create, read, update, and delete operations for contributions tied to savings goals.

### FR-12 Dashboard Summary
- The system shall provide a summary endpoint including key metrics for a selected period.

## 6. Core User Journeys

### Journey A: New Household Onboarding
1. User registers account.
2. User logs in and receives token.
3. User accesses current profile and household context.
4. User starts adding financial records (income, expenses, budgets).

### Journey B: Monthly Budget Tracking
1. User defines category-level monthly budgets.
2. User records income and expenses.
3. User reviews dashboard summary and budget progress indicators.
4. User adjusts budgets and bill statuses as needed.

### Journey C: Savings Goal Management
1. User creates a savings goal.
2. User adds contributions over time.
3. User monitors progress from contributions and dashboard summaries.

## 7. Business Rules
- BR-01: All protected operations require authenticated access.
- BR-02: Household member access is restricted to own household data.
- BR-03: Email uniqueness is enforced for user identity.
- BR-04: Budget uniqueness is enforced per household/category/month.
- BR-05: Savings contributions must be linked to a valid goal.

## 8. Non-Functional Considerations (Functional Impact)
- Security: Token-based authorization and secure password storage.
- Data Integrity: Referential constraints and uniqueness controls.
- Usability: API-driven feature model intended for responsive web interfaces.
- Maintainability: Service-oriented backend supports modular feature evolution.

## 9. Acceptance Criteria Snapshot
- Users can register, log in, and retrieve profile context.
- Authenticated users can perform CRUD operations for in-scope financial modules.
- Dashboard summary returns period-based aggregate values.
- Unauthorized and cross-household access attempts are rejected.
- API contract is testable through Swagger in development mode.
