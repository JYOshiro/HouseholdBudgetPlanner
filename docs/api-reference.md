# API Reference

This page documents the current backend API in a format that is quick to scan and practical to use during development. For exact schemas, use Swagger alongside this page.

## Quick Links

- [Base URLs](#base-urls)
- [Authentication](#authentication)
- [Status Codes](#status-codes)
- [Endpoint Groups](#endpoint-groups)
- [Examples](#examples)
- [Developer Notes](#developer-notes)

## Base URLs

| Environment | URL |
|---|---|
| Local API | `http://localhost:5000/api` |
| Local Swagger UI | `http://localhost:5000/swagger` |

> The frontend HTTP client currently reads `VITE_API_URL` and falls back to `https://localhost:5001/api`. If you are developing locally against the documented backend URL, set `VITE_API_URL=http://localhost:5000/api` in the frontend environment.

## Authentication

All endpoints require a bearer token except:

- `POST /api/auth/register`
- `POST /api/auth/login`

Send the token in the request header:

```http
Authorization: Bearer <jwt-token>
```

Authentication uses JWT bearer tokens. Household scope is derived from JWT claims. Clients must not send `householdId` in request bodies.

### Auth flow

1. Register a user or log in.
2. Read the `token` field from the response.
3. Send that token on all protected requests.
4. Use `GET /api/auth/me` to restore the current session.

The token carries user and household context. The API derives household scope from JWT claims.

> Swagger tip: use the `Authorize` button and enter `Bearer <token>` once. Swagger will then send it on protected requests automatically.

## Status Codes

| Code | Meaning | Typical cause |
|---|---|---|
| `200 OK` | Request succeeded | Read, update, or action endpoints |
| `201 Created` | Resource created | Not commonly used in the current controllers |
| `204 No Content` | Delete or action succeeded with no body | Delete endpoints |
| `400 Bad Request` | Validation or rule failure | Missing fields, invalid month, duplicate budget |
| `401 Unauthorized` | Missing or invalid token | No token, expired token, bad credentials |
| `404 Not Found` | Resource not found in current scope | Wrong ID or cross-household lookup |
| `500 Internal Server Error` | Unexpected server failure | Unhandled server-side issue |

## Endpoint Groups

### Auth

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `POST` | `/api/auth/register` | No | Create a user and initial household |
| `POST` | `/api/auth/login` | No | Return a JWT token and current user |
| `GET` | `/api/auth/me` | Yes | Return the authenticated user |

### Household and Categories

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/households` | Yes | Get the current household |
| `GET` | `/api/households/members` | Yes | Get members of the current household |
| `GET` | `/api/categories` | Yes | List default and household-specific categories |
| `GET` | `/api/categories/{id}` | Yes | Get one category |
| `POST` | `/api/categories` | Yes | Create a household category |
| `PUT` | `/api/categories/{id}` | Yes | Update a household category |
| `DELETE` | `/api/categories/{id}` | Yes | Delete a household category |

### Transactions and Budgets

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/expenses` | Yes | List household expenses |
| `GET` | `/api/expenses/{id}` | Yes | Get one expense |
| `POST` | `/api/expenses` | Yes | Create an expense |
| `PUT` | `/api/expenses/{id}` | Yes | Update an expense |
| `DELETE` | `/api/expenses/{id}` | Yes | Delete an expense |
| `GET` | `/api/income` | Yes | List household income entries |
| `GET` | `/api/income/{id}` | Yes | Get one income entry |
| `POST` | `/api/income` | Yes | Create an income entry |
| `PUT` | `/api/income/{id}` | Yes | Update an income entry |
| `DELETE` | `/api/income/{id}` | Yes | Delete an income entry |
| `GET` | `/api/budgets` | Yes | List budgets |
| `GET` | `/api/budgets/{id}` | Yes | Get one budget |
| `POST` | `/api/budgets` | Yes | Create a monthly budget |
| `PUT` | `/api/budgets/{id}` | Yes | Update a budget |
| `DELETE` | `/api/budgets/{id}` | Yes | Delete a budget |

### Bills and Savings

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/bills` | Yes | List bills |
| `GET` | `/api/bills/upcoming` | Yes | List upcoming unpaid bills |
| `GET` | `/api/bills/{id}` | Yes | Get one bill |
| `POST` | `/api/bills` | Yes | Create a bill |
| `PUT` | `/api/bills/{id}` | Yes | Update a bill |
| `DELETE` | `/api/bills/{id}` | Yes | Delete a bill |
| `POST` | `/api/bills/{id}/pay` | Yes | Mark a bill as paid |
| `GET` | `/api/savings-goals` | Yes | List savings goals |
| `GET` | `/api/savings-goals/{id}` | Yes | Get one savings goal |
| `POST` | `/api/savings-goals` | Yes | Create a savings goal |
| `PUT` | `/api/savings-goals/{id}` | Yes | Update a savings goal |
| `DELETE` | `/api/savings-goals/{id}` | Yes | Delete a savings goal |
| `GET` | `/api/goals/{goalId}/contributions` | Yes | List contributions for a goal |
| `GET` | `/api/goals/{goalId}/contributions/{id}` | Yes | Get one contribution |
| `POST` | `/api/goals/{goalId}/contributions` | Yes | Create a contribution |
| `PUT` | `/api/goals/{goalId}/contributions/{id}` | Yes | Update a contribution |
| `DELETE` | `/api/goals/{goalId}/contributions/{id}` | Yes | Delete a contribution |

### Dashboard

| Method | Route | Auth | Purpose |
|---|---|---|---|
| `GET` | `/api/dashboard/summary` | Yes | Return month-based summary data |

Query parameters for `GET /api/dashboard/summary`:

| Parameter | Type | Required | Notes |
|---|---|---|---|
| `year` | integer | No | Defaults to current year |
| `month` | integer | No | Defaults to current month and must be between 1 and 12 |

## Examples

### Register a user

```http
POST /api/auth/register
Content-Type: application/json
```

```json
{
  "email": "jane@example.com",
  "password": "SecurePassword123!",
  "firstName": "Jane",
  "lastName": "Smith",
  "householdName": "Smith Household"
}
```

Example success response:

```json
{
  "token": "<jwt-token>",
  "expiresIn": 86400,
  "user": {
    "id": 1,
    "email": "jane@example.com",
    "firstName": "Jane",
    "lastName": "Smith",
    "householdId": 1
  }
}
```

### Create an expense

```http
POST /api/expenses
Authorization: Bearer <jwt-token>
Content-Type: application/json
```

```json
{
  "amount": 42.50,
  "description": "Groceries",
  "isShared": true,
  "date": "2026-03-20T18:30:00Z",
  "categoryId": 3
}
```

Example response shape:

```json
{
  "id": 14,
  "amount": 42.50,
  "description": "Groceries",
  "isShared": true,
  "date": "2026-03-20T18:30:00Z",
  "categoryId": 3,
  "categoryName": "Groceries",
  "paidByUserId": 1,
  "paidByUserName": "Jane Smith"
}
```

### Get dashboard summary

```http
GET /api/dashboard/summary?year=2026&month=3
Authorization: Bearer <jwt-token>
```

Example response shape:

```json
{
  "totalIncome": 4200.00,
  "totalExpenses": 1750.50,
  "netAmount": 2449.50,
  "budgetUsage": [],
  "upcomingBills": [],
  "recentTransactions": [],
  "savingsProgress": []
}
```

## Developer Notes

- Do not send `householdId` in request bodies. Household scope is derived from JWT claims.
- The register and login endpoints currently return `200 OK` in the controller implementation, even though many APIs use `201 Created` for registration.
- The fastest way to inspect full request and response contracts is still Swagger at `http://localhost:5000/swagger`.
- DTO source files live under `backend/DTOs/` and are the best code-level reference for response shapes.

## Related Pages

- [Getting Started](./getting-started.html)
- [Architecture](./architecture.html)
- [Frontend Guide](./frontend-guide.html)
- [Deployment](./deployment.html)
