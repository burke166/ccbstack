# CLAUDE.md

## Lake County MEG Website

This repository contains the website for the **Lake County Metropolitan
Enforcement Group (Lake County MEG)**.

The site is a static website built with **Hugo** and our intent is to
deploy AWS using GitLab CI/CD.

The public site is:

`https://www.lakecountymeg.org/`

This project replaces the organization's legacy website with a modern,
responsive, accessible, and maintainable static site while preserving the
useful content and general visual identity of the original.

---

## Project Goals

The primary goals of this project are:

1. Provide a professional public website for Lake County MEG.
2. Preserve and modernize useful content from the legacy website.
3. Keep the site simple enough that future maintenance does not require a
complex application stack.
4. Provide good mobile and desktop experiences.
5. Meet reasonable accessibility standards for a public-sector website.
6. Minimize hosting costs and operational maintenance.
7. Make deployments repeatable through GitLab CI/CD.
8. Preserve important legacy URLs through redirects where practical.

Favor simplicity over unnecessary functionality.

Do not turn the site into a web application when static HTML can solve the
problem.

---

## Technology Stack

The site uses:

* Hugo
* Markdown
* HTML5
* Bootstrap 5
* Custom CSS
* Minimal JavaScript
* Git
* GitLab CI/CD
* AWS S3
* Amazon CloudFront
* AWS Certificate Manager (ACM)

Dynamic functionality should use small serverless AWS services where
appropriate.

The public tips/contact form may use:

* AWS Lambda
* Amazon SES
* optionally API Gateway or a Lambda Function URL

Do not introduce Node.js, npm, TypeScript, React, Vue, Svelte, or another
frontend build system unless there is a compelling requirement that Hugo
and Bootstrap cannot reasonably satisfy.

---

## Design Philosophy

When making changes, prioritize:

1. Correctness
2. Accessibility
3. Simplicity
4. Maintainability
5. Security
6. Performance
7. Visual polish

The site should look professional and contemporary without looking like a
commercial SaaS product.

The visual character should remain appropriate for a law-enforcement/public-
sector organization.

Prefer restrained design over animation, visual effects, or unnecessary
interactivity.

---

## Repository as Source of Truth

Treat the repository as the authoritative description of the site.

Do not rely on undocumented assumptions from previous conversations or
development sessions.

Before making significant changes, inspect the relevant source files,
configuration, and implemented design documents.

Documentation should describe the repository as it actually exists, not
merely the intended future state.

When documentation and implementation disagree, identify the discrepancy rather
than silently assuming either is correct.

The legacy website is a migration source, not the architectural source of truth.

Once content has been migrated and reviewed in this repository, prefer the
repository version unless the task specifically involves comparing against the
legacy site.

---

## Legacy Website

The legacy website is available at:

`https://www.lakecountymeg.org/`

Use the legacy site as a source for:

* existing content
* organizational terminology
* navigation concepts
* images
* downloadable resources
* external links
* historical information
* general visual identity

Do **not** copy the legacy CSS or reproduce obsolete HTML structures.

Instead, reinterpret the existing site using modern semantic HTML, Bootstrap,
and the project's custom CSS.

When migrating content, correct obvious formatting and typographical problems,
but do not materially alter factual statements without a reason.

Do not invent organizational facts.

Because the legacy website may contain outdated information, do not assume
that content is current merely because it appears there.

---

## Site Structure

The expected primary sections are approximately:

```text
/
├── news/
├── fugitives/
├── drug-info/
│   ├── heroin/
│   ├── club-drugs/
│   └── prescription-drug-abuse/
├── training/
├── links/
└── tips/
```

Additional sections may be introduced when justified.

Prefer clean, lowercase, human-readable URLs.

For example:

```text
/fugitives/
/drug-info/
/drug-info/heroin/
```

Do not preserve malformed legacy URLs merely for compatibility.

Instead, redirect important legacy URLs to their canonical replacements.

---

## Hugo Structure

Follow conventional Hugo project organization.

```text
assets/
    css/
    js/

content/
    ...

data/
    ...

layouts/
    _default/
    partials/
    shortcodes/

static/
    images/
    favicon.ico
```

### `content/`

Markdown content belongs under `content/`.

Prefer Markdown over embedding large amounts of HTML in content files.

### `layouts/`

Site templates and presentation logic belong under `layouts/`.

Use partials for shared components such as:

* `<head>`
* navigation
* header
* footer
* forms
* reusable page components

Avoid duplicating markup between layouts.

### `static/`

Files that should be copied directly to the published site belong under `static/`.

Examples:

```text
static/favicon.ico
static/images/logo.png
static/images/fugitives/
```

A file stored as:

```text
static/images/logo.png
```

is available publicly as:

```text
/images/logo.png
```

### `assets/`

Resources processed by Hugo belong under `assets/`.

The primary custom stylesheet should normally be:

```text
assets/css/site.css
```

Use Hugo Pipes to process it.

Example:

```html
{{ $css := resources.Get "css/site.css" | minify | fingerprint }}
<link rel="stylesheet" href="{{ $css.RelPermalink }}">
```

Do not reference files under `assets/` directly as static URLs.

---

## Bootstrap

Bootstrap provides the basic responsive layout and common UI components.

Prefer Bootstrap utilities and components before writing custom CSS.

Custom CSS should primarily provide:

* Lake County MEG branding
* typography
* colors
* spacing adjustments
* specialized components
* presentation not reasonably handled by Bootstrap

Do not recreate Bootstrap functionality in custom CSS.

When using Bootstrap from a CDN, ensure that any Subresource Integrity
(`integrity`) value exactly matches the referenced Bootstrap version.

Never change a CDN version without also verifying its integrity hashes.

---

## CSS

Keep custom CSS relatively small.

Prefer semantic classes over page-specific selectors.

Use CSS custom properties for major site colors.

For example:

```css
:root {
    --lcmeg-navy: #1f3a5f;
    --lcmeg-navy-dark: #152943;
    --lcmeg-gold: #c8a96b;
    --lcmeg-background: #f5f7fa;
    --lcmeg-text: #1f2933;
}
```

Avoid:

* inline styles
* `!important` unless genuinely necessary
* excessively specific selectors
* duplicated Bootstrap functionality
* CSS frameworks in addition to Bootstrap

---

## JavaScript

Use as little JavaScript as practical.

JavaScript is appropriate for lightweight enhancements such as:

* filtering fugitives
* form submission
* Bootstrap interactive components
* small usability improvements

The site's core content and navigation should work without custom JavaScript
whenever practical.

Do not introduce a JavaScript framework for functionality that can be
implemented with HTML, CSS, Bootstrap, or a few lines of vanilla JavaScript.

---

## Images

General static images belong under:

```text
static/images/
```

Organize images by purpose when useful.

For example:

```text
static/images/
├── branding/
├── fugitives/
├── news/
└── general/
```

Use descriptive filenames.

Prefer:

```text
adrian-xavier-sanchez.jpg
```

over:

```text
IMG00437.jpg
```

Provide meaningful `alt` text for informative images.

Decorative images should use empty alt text:

```html
alt=""
```

Do not invent descriptions of people shown in photographs.

---

## Fugitives

The fugitives section is public.

Fugitive information should be structured separately from presentation.

Prefer Hugo data files such as:

```text
data/fugitives/
```

Each fugitive may have a YAML record containing fields such as:

```yaml
name:
aka:
charges:
sex:
race:
height:
weight:
hair:
eyes:
dob:
last_known_address:
notes:
image:
```

Templates should render this structured data.

Do not duplicate fugitive information directly into HTML templates.

Because fugitive information may change, do not assume that data copied from
the legacy site is still current. Treat migrated records as content requiring
verification before production publication.

---

## Tips Form

The site may provide a public form for submitting tips.

The frontend is static, but the form backend may use AWS Lambda and Amazon SES.

Treat all submitted information as untrusted input.

Server-side processing must:

* validate input
* enforce reasonable field lengths
* reject malformed requests
* rate-limit or otherwise mitigate abuse
* avoid exposing internal email addresses unnecessarily
* avoid including secrets in frontend JavaScript
* log failures without unnecessarily logging sensitive submission contents

Do not rely exclusively on client-side validation.

If CAPTCHA or another anti-abuse mechanism is introduced, keep it isolated from
the rest of the site architecture.

---

## Accessibility

Accessibility is a core requirement.

Use semantic HTML wherever possible.

Pages should have:

* one logical primary `<h1>`
* properly nested headings
* keyboard-accessible navigation
* visible focus indicators
* descriptive link text
* appropriate form labels
* meaningful image alt text
* sufficient color contrast
* responsive layouts
* reasonable text sizes

Avoid links such as:

```text
Click here
```

Prefer:

```text
View Lake County MEG training information
```

Do not use color as the only way to communicate meaning.

---

## Responsive Design

The site must work well on:

* phones
* tablets
* laptops
* desktop monitors

Use Bootstrap's responsive grid and utilities.

Do not design exclusively for desktop and attempt to repair mobile layouts
afterward.

Avoid fixed-width content containers that create horizontal scrolling.

---

## Markdown

Prefer standard Markdown syntax in content files.

Links:

```md
[Lake County MEG](https://www.lakecountymeg.org/)
```

Images:

```md
![Description](/images/example.jpg)
```

Blockquotes:

```md
> Quoted or emphasized information.
```

Use raw HTML inside Markdown only when Markdown cannot reasonably express the
required structure.

---

## External Links

Verify external URLs when modifying or migrating them.

Do not knowingly preserve dead links simply because they appeared on the legacy
site.

External links should use descriptive text.

Do not unnecessarily force links to open in new browser tabs.

---

## Redirects

Clean URLs are preferred over preserving malformed legacy URLs.

Important legacy URLs should redirect to their new canonical locations.

Examples include mappings such as:

```text
/Fugitiveshtm -> /fugitives/
/News         -> /news/
/Training     -> /training/
/Tips         -> /tips/
```

The exact redirect mechanism depends on the production CloudFront/S3
configuration.

Maintain redirect definitions in a centralized, documented location rather than
scattering redirect behavior throughout templates.

---

## SEO and Metadata

Each significant page should have:

* a useful title
* a useful description
* a canonical URL where appropriate

Hugo should generate:

* `sitemap.xml`
* `robots.txt`

Do not use keyword stuffing or other obsolete SEO techniques.

Page titles and descriptions should primarily help humans understand the content.

---

## Security

This is a public-sector website and should be treated accordingly.

Never commit:

* AWS access keys
* SMTP credentials
* API secrets
* private keys
* passwords
* Lambda secrets
* SES credentials

Use GitLab CI/CD variables or AWS IAM roles for deployment credentials.

Grant deployment credentials only the permissions required to deploy this site.

Do not expose administrative systems or internal services through the static
website.

---

## AWS Hosting

Production hosting is expected to use:

```text
GitLab
   │
   │ CI/CD
   ▼
Hugo Build
   │
   ▼
Amazon S3
   │
   ▼
Amazon CloudFront
   │
   ▼
www.lakecountymeg.org
```

TLS certificates should use AWS Certificate Manager.

For CloudFront, ACM certificates must be provisioned in the AWS region required
by CloudFront.

The S3 origin should not be made unnecessarily public when CloudFront can access
it through the appropriate AWS origin access mechanism.

---

## CI/CD

GitLab is the source repository and CI/CD platform.

The expected production pipeline is approximately:

```text
commit
   ↓
GitLab
   ↓
Hugo build
   ↓
validation
   ↓
S3 sync
   ↓
CloudFront invalidation
```

Only the production branch should deploy to the production site.

A failed Hugo build must prevent deployment.

Prefer:

```bash
hugo --minify
```

for production builds.

Do not manually edit generated files under:

```text
public/
```

The `public/` directory is build output and should be reproducible from source.

---

## Git

Keep commits focused and understandable.

Do not mix unrelated formatting changes with functional changes.

Avoid mass file rewrites and preserve file history whenever practical.

Do not commit generated Hugo output unless the repository explicitly adopts that convention.

Normally ignore:

```text
public/
resources/_gen/
```

Also ignore local development artifacts such as:

```text
.venv/
```

Do not perform destructive Git operations unless explicitly requested.

Do not create commits, push branches, merge branches, or rewrite Git history
unless explicitly requested.

Before significant structural changes, inspect the repository and understand
the existing content/layout relationships.

---

## Development Workflow

For routine changes:

1. Inspect the existing implementation.
2. Identify the smallest reasonable change.
3. Make the change.
4. Run the appropriate build and validation.
5. Verify the affected pages or functionality.
6. Report what changed and what was verified.

Use:

```text
hugo server -D
```

for local development.

Use:

```text
hugo --minify
```

to verify a production build.

Do not manually modify generated files as a substitute for changing source
files.

---

## Verification

After implementation:

1. Run `hugo --minify`.
2. Resolve all build errors and unexpected warnings.
3. Verify affected internal links and referenced assets.
4. Inspect affected generated pages for malformed HTML or missing content.
5. When presentation changes, verify representative desktop and mobile
layouts.
6. Report what was verified and identify anything that still requires human
review.

Do not claim that a page was visually verified unless it was actually rendered
and inspected.

Do not claim that functionality was tested unless the relevant test or manual
verification was actually performed.

---

## Content Changes

Be conservative when changing factual content.

The website contains law-enforcement and public-information material.

Do not:

- invent facts
- invent charges
- infer fugitive status
- infer identities from photographs
- change dates without evidence
- rewrite legal terminology merely for stylistic reasons

Formatting, grammar, and clarity may be improved while preserving meaning.

Flag questionable factual content for human review rather than guessing.

---

## Generated Content

AI-generated public-facing prose should be treated as a draft unless
explicitly approved.

When migrating existing material:

- preserve factual meaning
- improve structure
- remove obsolete presentational markup
- use Markdown where possible
- identify obviously stale material
- do not silently fabricate missing information

---

## Maintainability

Assume this website may remain in service for ten years.

A future maintainer should be able to understand the project without knowing
how it was originally created.

Favor:

- conventional Hugo structure
- obvious filenames
- small partials
- Markdown content
- structured YAML data
- Bootstrap conventions
- minimal dependencies

Avoid clever abstractions that save a few lines of code but make the project
harder to understand.

When choosing between a sophisticated solution and a straightforward one that
adequately solves the problem, choose the straightforward solution.

---

## Documentation

Significant architectural changes should begin with
a document under:

```text
docs/design/
```

Use the `docs/design/` directory to document proposed designs and significant
architectural decisions.

Design documents should use one of these statuses:

- **Proposed** — initial design awaiting human review.
- **Accepted** — reviewed and approved for implementation.
- **Implemented** — implementation is complete and the document reflects the
resulting system.

The normal workflow for significant changes is:

1. Inspect the existing implementation.
2. Create or update a design document with status `Proposed`.
3. Stop and request review.
4. Incorporate requested changes into the design.
5. Change the status to `Accepted` only after the design has been approved.
6. Implement the accepted design.
7. Verify the implementation.
8. Update the design document to reflect implementation decisions that differ
from the original proposal.
9. Change the status to `Implemented`.

Do not implement a `Proposed` design unless explicitly instructed to proceed
with implementation.

Do not mark a design `Accepted` merely because it has been written.

Do not mark a design `Implemented` until the corresponding implementation and
verification are complete.

Documents with status `Implemented` should collectively describe the current
architecture and significant design decisions of the site.

Keep implemented design documents accurate.

If later work materially changes a documented design decision, update the
existing document or create a superseding design document as appropriate.

---

## When a Design Document Is Required

A design docuiment is normally appropriate for changes involving:

- site architecture or major navigation changes.
- new AWS services or infrastructure.
- CI/CD architecture.
- forms or other server-side functionality.
- new content or data models.
- significant Hugo template or layout restructuring.
- security- or privacy-sensitive functionality.
- redirects or URL-structure changes with substantial compatibility impact.
- introduction of a new dependency, framework, or build tool.
- changes the affect multiple, major areas of the site.
- decisions that a future maintainer would reasonably need to understand.

Routine changes normally do **not** require a design document.

Examples include:

- content edits
- typo fixes
- small CSS adjustments
- small template fixes
- correcting broken links
- replacing and image
- straightforward dependency version updates
- maintenance that does not change an established design decision

When uncertain whether a change requires esign work, prefer a short design
proposal over making an undocumented architectural decision.

---

## Design Document Structure

Design documents should normally contain:

```markdown
# Title

Status: Proposed

## Problem

## Goals

## Non-Goals

## Current State

## Proposed Design

## Alternatives Considered

## Security and Privacy Considerations

## Implementation Plan

## Validation
```

Not every section requires extensive content.

Omit sections that genuinely do not apply rather than filling them with
boilerplate.

Design documents should explain **why** a decision is being made, the should
not merely list the files that will be changed.

The `Current State` section should be based on inspection of the repository
rather than assumptions.

The `Alternatives Considered` section should record meaningful alternatives and
why they were rejected.

The `Implementation Plan` should be specific enough that implementation can
proceed from the accepted design without rediscovering major architectural
decisions.

The `Validation` section should describe how the completed implementation will be
verified.

---

## Design and Implementation

When asked to design a significant change, focus on understanding:

- the problem
- existing implementation
- constraints
- goals and non-goals
- reasonable alternatives
- security and privacy implications
- the proposed implementation
- validation requirements

Do not begin modifying production code unless implementation is also explicitly
requested.

When asked to implement an accepted design, treat the design document as the
primary specification, but verify its assumptions against the current
repository before making changes.

If the accepted design conflicts with the current repository, stop and explain
the conflict rather than silently changing the design or implementation.

If implementation reveals that the accepted design requires a significant
change, update the design and request review before proceeding with that
deviation.

Small implementation details that do not materially change the accepted design
may be resolved during implementation and documented afterward without
requiring another review cycle.

The purpose of the design process is to make significant decisions explicit and
reviewable, not to add unnecessary ceremony to routine maintenance.