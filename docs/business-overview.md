---
title: Business Overview
---

Household Budget Planner is a shared budgeting and finance tracking platform for small households. This page explains the product problem, who it serves, what is in scope, and where delivery stands today.

## Quick Links

- [Problem Statement](#problem-statement)
- [Who It Is For](#who-it-is-for)
- [Product Value](#product-value)
- [Scope Summary](#scope-summary)
- [Current Status](#current-status)
- [User Journeys](#user-journeys)
- [Success Criteria](#success-criteria)

## Problem Statement

Many households manage money across separate tools such as spreadsheets, banking apps, reminders, and chat messages. That makes even simple questions harder than they should be:

- How much have we spent this month?
- Are we still inside budget for groceries, utilities, or transport?
- Which bills are due soon?
- Are we making progress toward savings goals?

Household Budget Planner brings those answers into one shared system with household-level access control, monthly visibility, and a clearer record of financial activity.

## Who It Is For

The product is designed for small households that want practical budgeting support without enterprise-level complexity.

| Audience | Needs | How the product helps |
|---|---|---|
| Couples and families | Shared visibility into spending, bills, and savings | Centralizes household finance activity in one place |
| Flatmates or shared households | A simple way to record transactions and review shared spending | Supports day-to-day transaction tracking and monthly summaries |
| Household finance managers | Better control over budgets and recurring costs | Adds category budgets, bill tracking, and dashboard summaries |
| Reviewers and assessors | A clear example of a complete budgeting platform | Demonstrates product scope, domain modelling, and delivery progress |

## What the Product Does

At a functional level, the app supports the following capabilities.

| Capability | What it supports |
|---|---|
| Authentication | Register, log in, and work inside a household-specific account |
| Expense tracking | Record and review spending by category |
| Income tracking | Capture income entries and include them in monthly reporting |
| Monthly budgets | Set category limits and compare planned vs actual spending |
| Bill management | Track recurring bills, due dates, and payment status |
| Savings goals | Define goals, track contributions, and monitor progress |
| Dashboard | View month-based summary information across income, expenses, budgets, bills, and savings |

## Product Value

These are the main outcomes the product is built to deliver.

| Value area | Why it matters |
|---|---|
| Shared visibility | Everyone in the household sees the same financial picture |
| Better monthly control | Budgets and bill tracking make overspending easier to detect |
| Practical accountability | Shared records reduce guesswork and missing information |
| Progress tracking | Savings goals turn long-term plans into visible milestones |
| Technical completeness | The project demonstrates a realistic full-stack product with meaningful domain complexity |

## Scope Summary

Scope is intentionally focused to keep delivery practical and testable.

### In scope for the current baseline

- household-scoped authentication
- categories, expenses, income, budgets, bills, savings goals, and goal contributions
- dashboard summary by period
- backend API with PostgreSQL persistence
- frontend application shell with auth and protected routes

### Out of scope for the current baseline

- bank account or card integrations
- multi-household membership for a single user
- native mobile applications
- advanced forecasting, exports, or financial advisor features

## Current Status

> Current status: Backend API and database are complete. Frontend feature integration is in progress. Production hardening is planned.

| Area | Status | Notes |
|---|---|---|
| Backend API | Complete | Core finance modules and dashboard endpoints are implemented |
| Data model and database | Complete | PostgreSQL schema, migrations, and seed data are in place |
| Security baseline | Complete | JWT auth, password hashing, and household-scoped access are implemented |
| Frontend authentication | Complete | Login, registration, token persistence, and protected routes exist |
| Frontend feature wiring | In progress | Feature pages exist, but API integration and refinement are still needed |
| Production hardening | Planned | Expanded tests, release automation, and operational checks remain |

In short: core backend capability is complete, while frontend feature integration and release hardening are the active delivery focus.

## User Journeys

### Journey 1: Starting a household budget

1. A user registers and creates a household.
2. The household starts with seeded categories.
3. The user adds budgets, expenses, and bills.
4. The dashboard begins showing monthly financial position.

### Journey 2: Tracking day-to-day finances

1. A household member logs income and expenses.
2. The app groups entries by category and time period.
3. The household reviews spending against budget.
4. Upcoming bills and savings progress remain visible.

### Journey 3: Reviewing household health

1. A user opens the dashboard for a selected month.
2. The app shows income, expenses, net amount, bills, and savings progress.
3. The household can decide where to adjust spending next.

## Success Criteria

This project is successful when:

1. the core finance workflows are usable through the frontend, not just the API
2. household isolation is consistently enforced across all protected data
3. dashboard values match the underlying financial records for the selected period
4. the project can be set up and deployed from a clean checkout with clear documentation
5. stakeholders can quickly understand the product purpose and current delivery state from the docs alone

## Related Pages

- [Architecture](./architecture.html)
- [API Reference](./api-reference.html)
- [Frontend Guide](./frontend-guide.html)
- [Roadmap](./roadmap.html)
