<div class="top-banner">
	<span><strong>Household Budget Planner</strong> — Documentation</span>
	<span class="banner-links">
		<a href="https://github.com/JYOshiro/HouseholdBudgetPlanner" target="_blank" rel="noopener">View Repository</a>
		<a href="https://jyoshiro.github.io/HouseholdBudgetPlanner/" target="_blank" rel="noopener">Project Site</a>
	</span>
</div>

<section class="hero">
	<h1>Household Budget Planner</h1>
	<p class="hero-tagline">A shared household finance platform — track income, expenses, budgets, bills, and savings goals, all in one place.</p>
	<div class="hero-tags">
		<span class="tech-tag">ASP.NET Core 9</span>
		<span class="tech-tag">React 18 + TypeScript</span>
		<span class="tech-tag">PostgreSQL</span>
		<span class="tech-tag">JWT Auth</span>
		<span class="tech-tag">Vite + Tailwind</span>
	</div>
</section>

<div class="status-summary-grid">
	<article class="status-card status-done">
		<div class="status-label">Backend API</div>
		<div class="status-value">Implemented</div>
		<p>All core modules operational — auth, expenses, income, budgets, bills, savings goals, and dashboard.</p>
	</article>
	<article class="status-card status-progress">
		<div class="status-label">Frontend</div>
		<div class="status-value">In Progress</div>
		<p>Project foundation in place. Feature-level API integration is the current delivery track.</p>
	</article>
	<article class="status-card status-done">
		<div class="status-label">Database</div>
		<div class="status-value">Migrated</div>
		<p>PostgreSQL schema deployed. Migrations and startup category seeding are operational.</p>
	</article>
</div>

## What Is This?

Household Budget Planner is a web application for shared household finance management. Household members can record and review all financial activity — income, spending, recurring bills, monthly budgets, and savings goals — through a single authenticated interface.

All financial data is scoped per household, providing clear data isolation between separate households. The backend API is fully implemented. The frontend is in active development.

## Where to Start

<div class="audience-grid">
	<article class="audience-card">
		<h3>Stakeholders &amp; Assessors</h3>
		<p>Understand the product scope, delivery status, and what has been built.</p>
		<ul class="link-list">
			<li><a href="./business-overview.html">Business Overview</a></li>
			<li><a href="./roadmap.html">Roadmap</a></li>
			<li><a href="./faq.html">FAQ</a></li>
		</ul>
	</article>
	<article class="audience-card">
		<h3>Developers</h3>
		<p>Set up the project, explore the API, and understand the codebase.</p>
		<ul class="link-list">
			<li><a href="./getting-started.html">Getting Started</a></li>
			<li><a href="./api-reference.html">API Reference</a></li>
			<li><a href="./frontend-guide.html">Frontend Guide</a></li>
		</ul>
	</article>
	<article class="audience-card">
		<h3>Technical Reviewers</h3>
		<p>Review architecture decisions, security approach, and quality posture.</p>
		<ul class="link-list">
			<li><a href="./architecture.html">Architecture</a></li>
			<li><a href="./security-privacy.html">Security &amp; Privacy</a></li>
			<li><a href="./testing.html">Testing</a></li>
		</ul>
	</article>
</div>

## Documentation

<div class="doc-grid">
	<article class="doc-card">
		<h3><a href="./business-overview.html">Business Overview</a></h3>
		<p>Product goals, target users, in-scope features, and current delivery status.</p>
	</article>
	<article class="doc-card">
		<h3><a href="./getting-started.html">Getting Started</a></h3>
		<p>Set up the project locally — prerequisites, configuration, and first run.</p>
	</article>
	<article class="doc-card">
		<h3><a href="./architecture.html">Architecture</a></h3>
		<p>System structure, component roles, design decisions, and data model.</p>
	</article>
	<article class="doc-card">
		<h3><a href="./api-reference.html">API Reference</a></h3>
		<p>All implemented endpoints grouped by module, with auth and usage notes.</p>
	</article>
	<article class="doc-card">
		<h3><a href="./frontend-guide.html">Frontend Guide</a></h3>
		<p>Frontend structure, integration approach, token handling, and known gaps.</p>
	</article>
	<article class="doc-card">
		<h3><a href="./deployment.html">Deployment</a></h3>
		<p>Configuration, environment variables, deployment steps, and release checklist.</p>
	</article>
	<article class="doc-card">
		<h3><a href="./roadmap.html">Roadmap</a></h3>
		<p>What is implemented, what is in progress, and what is planned next.</p>
	</article>
	<article class="doc-card">
		<h3><a href="./security-privacy.html">Security &amp; Privacy</a></h3>
		<p>Authentication controls, authorization model, and data isolation approach.</p>
	</article>
	<article class="doc-card">
		<h3><a href="./testing.html">Testing</a></h3>
		<p>Current test posture and recommended quality gates.</p>
	</article>
</div>

## Quick Reference

| Item | Value |
|---|---|
| Frontend (dev) | `http://localhost:5173` |
| Backend API (dev) | `http://localhost:5000/api` |
| Swagger UI (dev) | `http://localhost:5000/swagger` |
| Database | PostgreSQL |
| Auth model | JWT Bearer token |
| Backend | ASP.NET Core 9.0 |
| Frontend | React 18 + TypeScript + Vite |

<div class="callout">
<strong>Current status:</strong> The backend API is fully implemented and operational across all financial modules. Frontend integration is in active development. See the <a href="./roadmap.html">Roadmap</a> for current priorities and progress.
</div>

---

*Documentation last updated: March 2026*
