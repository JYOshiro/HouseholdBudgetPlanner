---
title: "[Update Title Here]"
---

> **Template:** Copy this file, rename it to match your update, fill in each section, and remove sections that do not apply. Add a corresponding row to [Improvements & Updates](./index.html).

---

## Purpose

Briefly describe what this update defines or resolves. One to three sentences.

**Example:** Define how the application should behave when [feature] reaches [condition], and confirm whether [related behavior] should apply after creation.

---

## Background

Describe the situation before this decision. Why was a decision needed? What was ambiguous, broken, or missing?

- what existed before
- what problem or gap was identified
- why the current behavior was insufficient

---

## Decision Summary

List the key decisions made. Use one sub-heading per distinct decision.

### Decision 1: [Short label]

Describe the decision clearly and without ambiguity.

### Decision 2: [Short label]

Describe the second decision if applicable.

---

## Rationale

Explain why these decisions were made over alternatives.

- why option A was chosen over option B
- what user or product outcomes this improves
- any constraints or tradeoffs accepted

---

## Affected Areas

| Area | What changes |
|---|---|
| Data model | [e.g. new field, new status values] |
| Backend API | [e.g. recalculation behavior, new response fields] |
| Frontend / UX | [e.g. different UI states, new actions, visual signals] |
| Documentation | [e.g. which pages are updated] |

---

## Functional Requirements

List concrete functional requirements that must be implemented.

- requirement one
- requirement two
- requirement three

Include any calculation rules, state transition logic, or invariants.

```text
Example rule:
if condition A then result B
if condition C then result D
```

---

## UX Requirements

Describe the expected user-facing behavior.

### State or scenario one

- what the user sees
- what actions are available
- what is displayed or hidden

### State or scenario two

- what the user sees
- what actions are available
- what is displayed or hidden

---

## Implementation Notes

### Backend

- what the backend must calculate or validate
- what fields to set, clear, or return
- any edge cases to handle

### Frontend

- how the frontend should detect or render states
- what actions should appear in each state
- any visual signals or messaging required

---

## Acceptance Criteria

Use Given/When/Then format for testable criteria.

### Scenario one

- Given [condition]
- When [action]
- Then [expected result]
- And [additional assertion]

### Scenario two

- Given [condition]
- When [action]
- Then [expected result]

---

## Implementation Priority

| Priority | Items |
|---|---|
| High | [list items required for initial implementation] |
| Medium | [list items that improve the feature but are not blocking] |
| Future | [list items deferred to a later phase] |

---

## Related Pages

- [Improvements & Updates](./index.html)
- [Business Overview](../business-overview.html)
- [Architecture](../architecture.html)
- [API Reference](../api-reference.html)
- [Frontend Guide](../frontend-guide.html)
- [Roadmap](../roadmap.html)
