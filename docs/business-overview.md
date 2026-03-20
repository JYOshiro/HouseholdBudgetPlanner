# Business Overview

<p class="page-intro">Product goals, target users, problem context, in-scope features, and current delivery status — for stakeholders, assessors, and anyone new to the project.</p>

## The Problem

Households often track finances across spreadsheets, banking apps, and paper — with no single place to see the whole picture. This makes it hard to:

- Know where money actually went each month
- Set and stick to category-level spending budgets
- Track recurring bills before they become overdue
- Make progress toward shared savings goals

**Household Budget Planner** solves this by providing a single, shared platform for all household financial activity — accessible to everyone in the household, with a complete history and monthly summaries.

**At a glance:**
- One shared source of truth for household finances
- Clear monthly visibility for spending, income, and bills
- Structured progress tracking for savings goals

## Who Is It For?

<div class="feature-grid">
	<article class="feature-card">
		<h4>Household finance managers</h4>
		<p>People responsible for tracking the family budget — income, bills, spending limits, and savings targets. They need a clear monthly overview and control over category budgets.</p>
	</article>
	<article class="feature-card">
		<h4>Household members</h4>
		<p>Everyone who records transactions and wants visibility into the shared financial picture. They need a simple way to log expenses and see current budget status.</p>
	</article>
</div>

The platform is designed for small households — families, couples, or flatmates — who want shared visibility and straightforward financial governance.

## What It Does

| Feature | Description |
|---|---|
| **Authentication** | Secure JWT-based login — each session is tied to a specific household |
| **Expense tracking** | Record, categorize, and review spending transactions |
| **Income tracking** | Log all household income entries |
| **Monthly budgets** | Set per-category spending limits for each calendar month |
| **Bill management** | Track recurring bills, due dates, and payment status |
| **Savings goals** | Define savings targets, record contributions, and track progress |
| **Dashboard** | A period-based summary — income vs. expenses, budget utilisation, bills due, savings progress |

## Scope

**In scope for the current baseline:**
- User registration and authentication
- Household-scoped data access (users can only see their own household's data)
- Full CRUD for: categories, expenses, income, budgets, bills, savings goals, and goal contributions
- Dashboard summary endpoint with period filtering

**Out of scope:**
- Multi-household membership per user
- Bank or card data integrations
- Native mobile clients

## Current Delivery Status

| Area | Status | Notes |
|---|---|---|
| Backend API | <span class="badge badge-done">Implemented</span> | All core modules complete and operational |
| Database | <span class="badge badge-done">Implemented</span> | Migrations applied, startup seeding in place |
| Auth module | <span class="badge badge-done">Implemented</span> | Register, login, JWT token issuance |
| Expenses | <span class="badge badge-done">Implemented</span> | Full CRUD |
| Income | <span class="badge badge-done">Implemented</span> | Full CRUD |
| Budgets | <span class="badge badge-done">Implemented</span> | Full CRUD |
| Bills | <span class="badge badge-done">Implemented</span> | Full CRUD + payment tracking action |
| Savings Goals | <span class="badge badge-done">Implemented</span> | Full CRUD + contributions |
| Dashboard | <span class="badge badge-done">Implemented</span> | Period summary endpoint |
| Frontend | <span class="badge badge-progress">In Progress</span> | Foundation in place; API integration active |

## Success Indicators

This delivery is considered successful when:

1. All in-scope CRUD operations are functional through the frontend UI
2. Household data isolation is verified — users cannot access another household's data
3. The monthly dashboard provides accurate income, expense, and budget summary data
4. Authentication is secure and consistent — tokens expire, protected routes redirect unauthenticated users
5. The platform is reliably deployable from a clean repository checkout

## Why This Was Built

This project demonstrates a full-stack application with real business complexity: multi-entity data modeling, household-scoped authorization, domain-driven API design, and a production-ready security posture. It serves as both a practical household tool and a reference implementation of modern web application delivery.
