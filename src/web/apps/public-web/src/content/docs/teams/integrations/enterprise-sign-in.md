---
id: teams-sso
title: "Enterprise sign-in"
description: "Enable Single Sign-On for a Skedular Teams Organization and let members sign in with its enterprise identity provider."
product: teams
category: integrations
slug: enterprise-sign-in
articleKind: guide
publicationState: published
evidenceRefs:
  - spec:033-documentation-center
terminologyRefs:
  - docs-glossary:v1
relatedArticleIds: []
updatedAt: 2026-07-14
---

## Overview

Single Sign-On (SSO) is configured for one Skedular Teams Organization. When it is active, members of that Organization authenticate through the enterprise identity provider configured for the Organization instead of using the standard Skedular sign-in path.

## Before you enable SSO

SSO settings can be changed by an active Organization **Owner** or **Administrator**. You also need the identity provider's SAML details and access to the Organization administration area. The current setup requires all three values below:

- **Entity Id**: the identity provider's entity identifier.
- **Login Url**: the identity provider's SSO login endpoint.
- **App Federation Metadata Url**: the metadata URL Skedular uses to validate the provider metadata and signing certificate.

## Enable SSO for your Organization

1. Open your Organization in Skedular Teams and open **Admin**.
2. In the **Access** section, select **SSO**.
3. Enter the **Entity Id**, **Login Url**, and **App Federation Metadata Url** supplied by your identity administrator.
4. Turn on **Enable SSO across the organisation**.

Skedular validates the federation metadata and its signing certificate before an active configuration is saved. If validation fails, correct the identity-provider values and try again.

## How members sign in

When a member accesses an SSO-enabled Organization and needs to sign in, Skedular shows the Organization's single sign-on page. Select **Continue** to sign in through the Organization's configured identity provider. After successful authentication, the signed-in identity must match the member's Skedular identity before access is granted.

SSO is Organization-scoped. Turning it on changes authentication for that Organization's active members; it does not configure SSO for every Organization a User may belong to. Members must already be active members of the Organization to use its protected workspace. SSO does not automatically add new members; invitees must become active Organization members first.

## Manage or disable SSO

Owners and Administrators can return to **Admin → Access → SSO** to update the three configuration fields or switch **Enable SSO across the organisation** off. Turning the switch off deactivates the Organization's SSO requirement and allows the normal Skedular sign-in flow to be used again.

## Troubleshooting

- **The identity provider does not open:** check the **Login Url**. If the problem continues, ask your identity administrator to verify the SSO configuration.
- **The configuration cannot be enabled:** verify that the **App Federation Metadata Url** is reachable and contains valid provider metadata and a signing certificate.
- **Authentication succeeds but access is denied:** confirm that the signed-in identity matches the email identity of an active Organization member.
- **The SSO option is unavailable:** only active Organization Owners and Administrators can modify SSO settings.
