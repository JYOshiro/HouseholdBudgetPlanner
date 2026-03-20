# Frontend Guide

<p class="page-intro">This page summarizes frontend delivery status, integration priorities, and implementation guidance for engineering teams.</p>

## Current State
The frontend foundation is present with routing, UI assets, and development tooling. Full API-backed feature parity is still in progress for several screens.

## Target Integration Approach
- Move features to module-based structure with clear ownership.
- Introduce feature-level API service wrappers.
- Align frontend models with backend DTO contracts.
- Ensure authenticated routes are protected consistently.

## Integration Priorities
1. Authentication bootstrap and token persistence.
2. Dashboard summary binding to backend endpoint.
3. CRUD workflows for expenses, income, budgets, bills, and savings.
4. Error, loading, and empty-state consistency across modules.

## Frontend Engineering Notes
- Keep API and UI concerns separated.
- Reuse shared formatters for date and currency rendering.
- Preserve household context assumptions across route transitions.
