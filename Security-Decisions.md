# Security Decisions

## Overview

Because this application handles financial and household data, security decisions are central to the architecture.

The main security choices include:

- JWT authentication
- password hashing
- household-scoped authorization
- DTO-based API responses
- backend-enforced access control
- CORS configuration
- validation and centralized error handling

## Why JWT authentication was chosen

JWT was selected because it works well in a separated frontend/backend architecture.

### Benefits

- stateless authentication
- simple integration with React
- works well for APIs
- suitable for future mobile or multi-client support

### How it fits this project

The backend issues a token after login, and the frontend uses that token for protected API calls.

This keeps authentication centralized and scalable.

## Why password hashing is required

Passwords must never be stored in plain text.

Password hashing protects users in case of:

- database leaks
- accidental exposure
- internal misuse

This is a standard and essential security practice for any application with accounts.

## Why household-scoped authorization is critical

This app supports multiple users and shared household data. Users must only access data that belongs to their own household.

This rule is critical because without it:

- one user could view another household’s expenses
- privacy would be compromised
- dashboard summaries could become incorrect

Authorization must be enforced on the backend for every protected operation.

## Why DTOs improve security

Returning DTOs instead of entities helps prevent accidental exposure of:

- password hashes
- internal fields
- unrelated navigation data

This keeps the API safer and more predictable.

## Why validation is important

Validation is needed to reject invalid input early, including invalid:

- amounts
- dates
- category references
- bill rules
- budget values

This helps protect the system from corrupted or inconsistent financial records.

## Why global exception handling was chosen

Centralized exception handling improves security and reliability by:

- preventing inconsistent error responses
- avoiding accidental exposure of internal stack details
- standardizing API error messages

## Why CORS configuration is necessary

During development, the frontend and backend run on different origins.

CORS is required so the browser can safely allow frontend requests to the backend.

It should be configured carefully to allow only trusted origins.

## Security principles followed

This project follows these practical principles:

- never trust the client for authorization
- enforce ownership rules on the server
- store credentials securely
- expose only necessary data
- validate inputs
- keep API responses predictable

## Summary

The security design was chosen to protect:

- user identity
- household privacy
- financial data
- API integrity

These decisions are necessary because the application handles sensitive personal and shared budget information.
