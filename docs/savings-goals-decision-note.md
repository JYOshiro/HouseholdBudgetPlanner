---
title: Savings Goals Decision Note
---

## Product Requirement / UX Decision Note

### Title

Savings Goals: completed-state behavior and goal editability

### Purpose

Define how the application should behave when a savings goal reaches its target amount, and confirm whether savings goals should remain editable after creation.

### Background

The current Savings Goals screen shows a goal at 100% completion while still presenting it similarly to an active goal, including an Add Contribution action. This creates ambiguity in the user experience and makes it harder to distinguish between goals that are still in progress and goals that have already been achieved.

In addition, savings goals need to support real-world changes such as updated deadlines, revised target amounts, or corrected naming, which means goals should be editable.

---

## Decision Summary

### Decision 1: Completed goals must have a different state

When a savings goal reaches or exceeds its target amount, it should no longer behave exactly like an active goal.

The system should mark the goal as Completed and the interface should reflect that status clearly.

### Decision 2: Savings goals should be editable

Users must be able to edit savings goals after creation. This includes updates to the goal name, target amount, target date, priority, and other descriptive details.

Edits must trigger recalculation of progress and status.

---

## Product Rationale

### Why completed goals need a distinct state

A completed goal represents a different stage in the savings journey. Keeping it visually and functionally identical to an active goal causes confusion, especially when the main action still suggests adding more money.

A distinct completed state improves:

- clarity
- feedback
- goal tracking accuracy
- user confidence
- overall UX polish

### Why goals should be editable

Savings goals often change over time. A user may:

- increase the target amount
- extend the deadline
- rename the goal
- correct a mistake
- change the priority
- reopen the goal after editing

Allowing edits supports realistic financial planning and prevents users from needing to delete and recreate goals for simple changes.

---

## Functional Requirements

### Goal status

Each savings goal must have a status field.

Recommended statuses:

- Active
- Completed
- Archived

Optional future status:

- Paused

### Automatic completion rule

A goal must be marked as Completed when:

```text
currentSaved >= targetAmount
```

### Remaining amount rule

Remaining amount must never display as negative.

Rule:

- if `currentSaved < targetAmount`, remaining = `targetAmount - currentSaved`
- if `currentSaved >= targetAmount`, remaining = `0.00`

### Editability

Users must be able to edit:

- goal name
- target amount
- target date
- priority
- description or notes if supported

When a goal is edited:

- progress must be recalculated
- remaining amount must be recalculated
- goal status must be recalculated

Example:

- if a completed goal's target amount is increased above the current saved amount, the goal should return to Active
- if an active goal's target amount is reduced below the current saved amount, the goal should become Completed

---

## UX Requirements

### Active goals

Active goals should display:

- goal title
- priority
- amount saved
- target amount
- remaining amount
- progress bar
- target date
- primary action: Add Contribution
- secondary action: Edit Goal

Optional:

- delete
- archive
- overflow menu

### Completed goals

Completed goals should display:

- goal title
- completed badge or status label
- amount saved
- target amount
- completion date if available
- remaining amount shown as `$0.00`

Primary actions for completed goals should not be the same as for active goals.

Recommended actions:

- Edit Goal
- View Details
- Archive Goal
- optional Reopen Goal

The Add Contribution action should either:

- be removed for completed goals, or
- be replaced with a more intentional label such as Add More or Continue Saving only if overfunding is an intentional supported behavior

### Visual distinction

Completed goals should have a visibly different UI state from active goals.

Possible signals:

- completed badge
- success accent color
- Goal achieved label
- completion date
- separate section heading

The design should remain clean and professional, not overly celebratory.

---

## Recommended Page Structure

### Preferred structure

Split the page into two sections:

- Active Goals
- Completed Goals

This keeps users focused on goals that still need attention while still preserving a clear record of achieved goals.

### Alternative

If separate sections are not implemented, completed goals must still be visually distinct and sorted below active goals.

---

## Business Rules

### Completion logic

- A goal becomes Completed automatically when saved amount reaches or exceeds target
- Completed goals remain visible unless archived
- Archived goals should be hidden from the default main view

### Contribution logic

Default recommendation:

- completed goals should not show the normal Add Contribution primary action

Optional supported behavior:

- if the product allows overfunding, users may continue contributing after completion
- if this is allowed, the UI must clearly indicate that the goal is already complete

### Edit logic

Editing a goal must:

- preserve contribution history
- update status and progress calculations
- update any dashboard or savings summary metrics

---

## Data Model Impact

### SavingsGoal

Recommended fields:

- `id`
- `householdId`
- `name`
- `targetAmount`
- `currentSaved`
- `priority`
- `targetDate`
- `status`
- `completedAt` nullable
- `createdAt`
- `updatedAt`

### GoalContribution

Recommended fields:

- `id`
- `goalId`
- `amount`
- `date`
- `createdByUserId`
- `note` optional

### Status field

Add or confirm support for:

```text
Active | Completed | Archived
```

### Completion date

When a goal becomes completed, store `completedAt`.

This allows the UI to display:

- Completed on Jul 1, 2026

---

## API / Backend Considerations

The backend should:

- calculate or validate status updates when contributions are added
- recalculate status when a goal is edited
- return status and completion date in savings goal responses
- prevent inconsistent remaining values
- optionally block new contributions for archived goals

Suggested backend responsibilities:

- mark goal as completed automatically
- set `completedAt` when completion first occurs
- clear `completedAt` if a goal returns to active after editing target amount

---

## Frontend Considerations

The frontend should:

- render different goal states based on `status`
- update button actions depending on status
- show completed-state messaging
- display remaining as `$0.00` when complete
- separate or sort completed goals appropriately
- support an Edit Goal flow through modal or page form

---

## Acceptance Criteria

### Completed goal state

- Given a goal where `currentSaved >= targetAmount`
- When the goal is displayed
- Then the goal status must be shown as Completed
- And remaining amount must show as `$0.00`
- And the goal must not display the same primary action as an active goal

### Goal editing

- Given an existing savings goal
- When the user edits the goal name, amount, date, or priority
- Then the system must save the changes
- And recalculate progress, remaining amount, and status

### Status recalculation after edit

- Given a completed goal
- When the target amount is increased above the current saved amount
- Then the goal status must change back to Active

### UI separation

- Given multiple savings goals with mixed statuses
- When the page loads
- Then active goals should be clearly distinguishable from completed goals
- And completed goals should not visually compete with active goals

---

## Recommended Implementation Priority

### High priority

- add/edit goal support
- goal status field
- automatic completion logic
- completed-state UI
- remove or replace Add Contribution for completed goals

### Medium priority

- completion date
- separate active/completed sections
- archive support

### Future enhancement

- reopen goal flow
- paused goals
- overfunding support with dedicated UX
- celebration micro-interactions

---

## Final UX Decision

Savings goals should be treated as lifecycle-based objects, not static cards.

This means:

- goals are editable
- goals move through meaningful statuses
- completed goals should be clearly distinguished from active goals
- the interface should guide users differently once a goal is achieved

This approach creates a more realistic, maintainable, and user-friendly savings experience for a household budgeting application.
