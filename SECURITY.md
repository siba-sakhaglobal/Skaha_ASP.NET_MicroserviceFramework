# Security Policy

## Reporting a Vulnerability

If you believe you've found a security vulnerability in SakhaERP, please report it privately
rather than opening a public issue:

- Use [GitHub's private vulnerability reporting](../../security/advisories/new) for this
  repository, or
- Email the maintainers (see repository contact details).

Please include:
- A description of the vulnerability and its potential impact
- Steps to reproduce it
- Any affected version(s)

We aim to acknowledge reports within 5 business days.

## Supported Versions

This project is pre-1.0 and evolving quickly. Security fixes are made against `main`; there are
no long-term-support branches yet.

## Scope Notes

SakhaERP is a *framework* — the shared multi-tenancy, security, and logging libraries
(`SakhaERP.Shared.*`) are in scope for reports. The reference `inventory-service` is a sample
implementation for demonstrating framework usage, not a hardened production service; treat
sample-service findings as documentation gaps unless they trace back to a framework-level issue.
