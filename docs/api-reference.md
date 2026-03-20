# API Reference

<p class="page-intro">All implemented backend endpoints — grouped by module, with authentication details, descriptions, and common response codes. Use Swagger in development for full request/response schema inspection.</p>

## Base URL

| Environment | URL |
|---|---|
| Development API | `http://localhost:5000/api` |
| Swagger UI (dev) | `http://localhost:5000/swagger` |

## Authentication

All endpoints except `/api/auth/register` and `/api/auth/login` require a `Bearer` token in the `Authorization` header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**How to get a token:**

1. `POST /api/auth/register` — create an account (or use an existing one)
2. `POST /api/auth/login` — submit email + password, receive `token` in the response
3. Include the token in all subsequent requests as `Authorization: Bearer <token>`

The token contains `userId`, `email`, and `householdId` claims. **Household scoping is enforced server-side** — the API uses the `householdId` from the token, not from request bodies or URL parameters.

<div class="callout">
<strong>Testing in Swagger:</strong> Click the <strong>Authorize</strong> button in Swagger UI and enter <code>Bearer &lt;your-token&gt;</code>. All subsequent requests will be authenticated automatically.
</div>

## Common Status Codes

| Code | Meaning |
|---|---|
| `200 OK` | Request succeeded — body contains the result |
| `201 Created` | Resource successfully created |
| `204 No Content` | Request succeeded — no response body |
| `400 Bad Request` | Validation error or malformed request body |
| `401 Unauthorized` | Missing or invalid bearer token |
| `403 Forbidden` | Token valid but the operation is not permitted |
| `404 Not Found` | Resource does not exist, or does not belong to the current household |
| `500 Internal Server Error` | Unexpected server-side error |

## Endpoint Summary

| Module | Method | Route | Auth |
|---|---|---|---|
| Auth | <span class="method method-post">POST</span> | `/api/auth/register` | No |
| Auth | <span class="method method-post">POST</span> | `/api/auth/login` | No |
| Auth | <span class="method method-get">GET</span> | `/api/auth/me` | Yes |
| Households | <span class="method method-get">GET</span> | `/api/households` | Yes |
| Households | <span class="method method-get">GET</span> | `/api/households/members` | Yes |
| Categories | <span class="method method-get">GET</span> | `/api/categories` | Yes |
| Categories | <span class="method method-post">POST</span> | `/api/categories` | Yes |
| Categories | <span class="method method-get">GET</span> | `/api/categories/{id}` | Yes |
| Categories | <span class="method method-put">PUT</span> | `/api/categories/{id}` | Yes |
| Categories | <span class="method method-delete">DELETE</span> | `/api/categories/{id}` | Yes |
| Expenses | <span class="method method-get">GET</span> | `/api/expenses` | Yes |
| Expenses | <span class="method method-post">POST</span> | `/api/expenses` | Yes |
| Expenses | <span class="method method-get">GET</span> | `/api/expenses/{id}` | Yes |
| Expenses | <span class="method method-put">PUT</span> | `/api/expenses/{id}` | Yes |
| Expenses | <span class="method method-delete">DELETE</span> | `/api/expenses/{id}` | Yes |
| Income | <span class="method method-get">GET</span> | `/api/income` | Yes |
| Income | <span class="method method-post">POST</span> | `/api/income` | Yes |
| Income | <span class="method method-get">GET</span> | `/api/income/{id}` | Yes |
| Income | <span class="method method-put">PUT</span> | `/api/income/{id}` | Yes |
| Income | <span class="method method-delete">DELETE</span> | `/api/income/{id}` | Yes |
| Budgets | <span class="method method-get">GET</span> | `/api/budgets` | Yes |
| Budgets | <span class="method method-post">POST</span> | `/api/budgets` | Yes |
| Budgets | <span class="method method-get">GET</span> | `/api/budgets/{id}` | Yes |
| Budgets | <span class="method method-put">PUT</span> | `/api/budgets/{id}` | Yes |
| Budgets | <span class="method method-delete">DELETE</span> | `/api/budgets/{id}` | Yes |
| Bills | <span class="method method-get">GET</span> | `/api/bills` | Yes |
| Bills | <span class="method method-post">POST</span> | `/api/bills` | Yes |
| Bills | <span class="method method-get">GET</span> | `/api/bills/upcoming` | Yes |
| Bills | <span class="method method-get">GET</span> | `/api/bills/{id}` | Yes |
| Bills | <span class="method method-put">PUT</span> | `/api/bills/{id}` | Yes |
| Bills | <span class="method method-delete">DELETE</span> | `/api/bills/{id}` | Yes |
| Bills | <span class="method method-post">POST</span> | `/api/bills/{id}/pay` | Yes |
| Savings Goals | <span class="method method-get">GET</span> | `/api/savings-goals` | Yes |
| Savings Goals | <span class="method method-post">POST</span> | `/api/savings-goals` | Yes |
| Savings Goals | <span class="method method-get">GET</span> | `/api/savings-goals/{id}` | Yes |
| Savings Goals | <span class="method method-put">PUT</span> | `/api/savings-goals/{id}` | Yes |
| Savings Goals | <span class="method method-delete">DELETE</span> | `/api/savings-goals/{id}` | Yes |
| Goal Contributions | <span class="method method-get">GET</span> | `/api/goals/{goalId}/contributions` | Yes |
| Goal Contributions | <span class="method method-post">POST</span> | `/api/goals/{goalId}/contributions` | Yes |
| Goal Contributions | <span class="method method-get">GET</span> | `/api/goals/{goalId}/contributions/{id}` | Yes |
| Goal Contributions | <span class="method method-put">PUT</span> | `/api/goals/{goalId}/contributions/{id}` | Yes |
| Goal Contributions | <span class="method method-delete">DELETE</span> | `/api/goals/{goalId}/contributions/{id}` | Yes |
| Dashboard | <span class="method method-get">GET</span> | `/api/dashboard/summary` | Yes |

---

## Auth

**Base route:** `/api/auth`

### <span class="method method-post">POST</span> `/register`

Register a new user account. A new Household is automatically created and linked to the user.

**Auth required:** No

**Request body:**
```json
{
  "email": "user@example.com",
  "password": "securePassword123",
  "name": "Jane Smith",
  "householdName": "Smith Household"
}
```

**Returns:** `201 Created` — user profile and JWT token.

---

### <span class="method method-post">POST</span> `/login`

Authenticate an existing user and return a JWT token.

**Auth required:** No

**Request body:**
```json
{
  "email": "user@example.com",
  "password": "securePassword123"
}
```

**Returns:** `200 OK` — includes `token`, `userId`, and `email`. Store the token for use in all subsequent requests.

---

### <span class="method method-get">GET</span> `/me`

Return the authenticated user's profile, including their `householdId`.

**Auth required:** Yes

**Returns:** `200 OK` — user details.

---

## Households

**Base route:** `/api/households`

The household is determined by the token — no `householdId` is needed in the URL.

| Method | Route | Description |
|---|---|---|
| <span class="method method-get">GET</span> | `/` | Returns the household name and details for the authenticated user |
| <span class="method method-get">GET</span> | `/members` | Returns all users who belong to the same household |

---

## Categories

**Base route:** `/api/categories`

Categories classify expenses and budget entries. System-default categories (null `HouseholdId`) are visible to all households. Custom categories are household-specific.

| Method | Route | Description |
|---|---|---|
| <span class="method method-get">GET</span> | `/` | List all categories available to the household (system defaults + custom) |
| <span class="method method-get">GET</span> | `/{id}` | Get a specific category by ID |
| <span class="method method-post">POST</span> | `/` | Create a custom category scoped to the current household |
| <span class="method method-put">PUT</span> | `/{id}` | Update a household-owned category |
| <span class="method method-delete">DELETE</span> | `/{id}` | Delete a household-owned category |

---

## Expenses

**Base route:** `/api/expenses`

Expense records are scoped to the authenticated user's household. The `householdId` is taken from the token — do not include it in the request body.

| Method | Route | Description |
|---|---|---|
| <span class="method method-get">GET</span> | `/` | List all expenses for the household |
| <span class="method method-get">GET</span> | `/{id}` | Get a specific expense by ID |
| <span class="method method-post">POST</span> | `/` | Record a new expense |
| <span class="method method-put">PUT</span> | `/{id}` | Update an existing expense |
| <span class="method method-delete">DELETE</span> | `/{id}` | Delete an expense |

---

## Income

**Base route:** `/api/income`

| Method | Route | Description |
|---|---|---|
| <span class="method method-get">GET</span> | `/` | List all income records for the household |
| <span class="method method-get">GET</span> | `/{id}` | Get a specific income record |
| <span class="method method-post">POST</span> | `/` | Record a new income entry |
| <span class="method method-put">PUT</span> | `/{id}` | Update an income record |
| <span class="method method-delete">DELETE</span> | `/{id}` | Delete an income record |

---

## Budgets

**Base route:** `/api/budgets`

A budget defines a spending limit for a specific category in a specific calendar month. **One budget per household + category + month** — duplicate attempts will return `400`.

| Method | Route | Description |
|---|---|---|
| <span class="method method-get">GET</span> | `/` | List all budgets for the household |
| <span class="method method-get">GET</span> | `/{id}` | Get a specific budget |
| <span class="method method-post">POST</span> | `/` | Create a monthly budget for a category |
| <span class="method method-put">PUT</span> | `/{id}` | Update a budget's amount or period |
| <span class="method method-delete">DELETE</span> | `/{id}` | Delete a budget |

---

## Bills

**Base route:** `/api/bills`

Bills track recurring payments with due dates and paid/unpaid status. The `/upcoming` route is useful for dashboard and notification features.

| Method | Route | Description |
|---|---|---|
| <span class="method method-get">GET</span> | `/` | List all bills for the household |
| <span class="method method-get">GET</span> | `/upcoming` | List unpaid bills with upcoming due dates |
| <span class="method method-get">GET</span> | `/{id}` | Get a specific bill |
| <span class="method method-post">POST</span> | `/` | Create a new recurring bill |
| <span class="method method-put">PUT</span> | `/{id}` | Update bill details |
| <span class="method method-delete">DELETE</span> | `/{id}` | Delete a bill |
| <span class="method method-post">POST</span> | `/{id}/pay` | Mark a bill as paid for the current period |

---

## Savings Goals

**Base route:** `/api/savings-goals`

Savings goals have a target amount and an optional deadline. Total contributed amount is tracked via GoalContributions.

| Method | Route | Description |
|---|---|---|
| <span class="method method-get">GET</span> | `/` | List all savings goals for the household |
| <span class="method method-get">GET</span> | `/{id}` | Get a specific savings goal |
| <span class="method method-post">POST</span> | `/` | Create a savings goal |
| <span class="method method-put">PUT</span> | `/{id}` | Update a savings goal |
| <span class="method method-delete">DELETE</span> | `/{id}` | Delete a savings goal |

---

## Goal Contributions

**Base route:** `/api/goals/{goalId}/contributions`

Contributions are deposits toward a savings goal. They are nested under the goal they belong to — always include the correct `goalId` in the path.

| Method | Route | Description |
|---|---|---|
| <span class="method method-get">GET</span> | `/` | List all contributions for a savings goal |
| <span class="method method-get">GET</span> | `/{id}` | Get a specific contribution |
| <span class="method method-post">POST</span> | `/` | Record a new contribution toward the goal |
| <span class="method method-put">PUT</span> | `/{id}` | Update a contribution record |
| <span class="method method-delete">DELETE</span> | `/{id}` | Delete a contribution |

---

## Dashboard

**Base route:** `/api/dashboard`

### <span class="method method-get">GET</span> `/summary`

Returns a period-based aggregate summary of the household's financial position.

**Auth required:** Yes

**Query parameters:**

| Parameter | Type | Description | Example |
|---|---|---|---|
| `year` | int | Calendar year | `2026` |
| `month` | int | Calendar month (1–12) | `3` |

**Returns:** `200 OK` — totals for income, expenses, budget utilisation, upcoming bills, and savings goal progress for the specified period.

---

<div class="callout">
<strong>Source of truth for schemas:</strong> Run the backend locally and open <code>http://localhost:5000/swagger</code> to browse full request/response schemas interactively. DTO source files are in <code>backend/DTOs/</code>.
</div>
