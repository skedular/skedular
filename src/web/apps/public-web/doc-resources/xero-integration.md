# Xero Integration

## Overview

The Xero Integration allows organizations to connect their **Skedular Spaces** organization with a Xero account.

Once connected, Skedular can export invoices to Xero, support recurring invoice workflows, and synchronize payment information between the two systems.

This integration enables organizations to continue managing their accounting in Xero while using Skedular to manage bookings, products, subscriptions, and customers.

---

# Availability

The Xero Integration is available only in:

- **Skedular Spaces**

Only organization owners and administrators can configure the integration.

---

# Why Connect Xero?

Many coworking operators already use Xero to manage their accounting.

Connecting Skedular to Xero allows organizations to:

- Export invoices automatically.
- Use Xero's recurring invoice functionality.
- Keep accounting records in one place.
- Reduce manual data entry.
- Reconcile customer payments.
- Synchronize payment status back into Skedular.

This allows Skedular to manage the operational side of bookings while Xero remains the system of record for accounting.

---

# How the Integration Works

Once connected, Skedular securely communicates with Xero using OAuth.

The integration:

1. Connects your organization to a Xero tenant.
2. Stores the required authentication tokens securely.
3. Automatically refreshes authentication tokens when required.
4. Exports invoices according to your billing configuration.
5. Retrieves payment updates from Xero.
6. Synchronizes payment status back into Skedular.

This process is automatic once the integration has been configured.

---

# Connecting to Xero

To connect your organization:

1. Open your organization.
2. Navigate to **Admin**.
3. Open **Billing and Payments**.
4. Select **Xero**.
5. Choose **Connect Xero**.
6. Sign in with your Xero account.
7. Select the Xero organisation (tenant) you want to connect.
8. Save the configuration.

Once connected, Skedular can begin exchanging invoice and payment information with Xero.

---

# Billing Modes

The Xero integration supports different billing behaviours depending on your organization's requirements.

Depending on the selected mode, Skedular can:

- Keep invoices entirely within Skedular.
- Export invoices to Xero.
- Support recurring invoice workflows.
- Synchronize invoice information between both systems.

The selected billing mode determines how invoices are managed.

---

# Configuration Options

The Xero Integration includes several optional settings that allow exported invoices to match your accounting configuration.

These settings include:

- Default Sales Account Code
- Tracking Category 1
- Tracking Category 2
- Branding Theme ID
- Reference Prefix

These values are passed to Xero whenever invoices are created.

---

# Default Sales Account

The Default Sales Account Code determines which sales account is used when invoices are created in Xero.

This helps ensure revenue is posted to the correct account within your accounting system.

---

# Tracking Categories

Tracking Categories allow invoices to be categorised within Xero.

Organizations can use them for reporting purposes such as:

- Locations
- Business units
- Departments
- Revenue streams

Up to two tracking categories can be configured.

---

# Branding Theme

A Branding Theme ID allows invoices exported to Xero to use one of your existing Xero invoice templates.

This ensures invoices sent from Xero match your organisation's branding.

---

# Reference Prefix

A Reference Prefix is automatically added to invoice references before they are exported.

Examples include:

- SKED
- MKT
- HQ

This helps distinguish invoices created by Skedular from invoices created elsewhere.

---

# Authentication

Skedular uses Xero's secure OAuth authentication.

Authentication tokens are securely stored and automatically refreshed when required.

Administrators are not required to reconnect their Xero account every time the integration is used.

If authorization expires or is revoked, Skedular will notify administrators so the connection can be re-established.

---

# Payment Synchronisation

The integration is not limited to invoice creation.

When payments are recorded and reconciled in Xero, Skedular can synchronise the payment status back to the booking platform.

This allows bookings and subscriptions to reflect the latest payment information without requiring manual updates.

For example:

- A customer pays an invoice in Xero.
- The payment is reconciled.
- Skedular receives the updated payment status.
- The related booking or subscription is automatically marked as paid.

This keeps both operational and financial systems aligned.

---

# Disconnecting Xero

Organizations can disconnect the Xero integration at any time.

Once disconnected:

- New invoices are no longer exported.
- Payment synchronisation stops.
- Existing bookings and invoices remain unchanged.

The organization can reconnect later if required.

---

# Best Practices

For the best experience:

- Connect the correct Xero organization before creating products.
- Configure your billing mode before accepting customer bookings.
- Verify your sales account codes.
- Configure tracking categories for meaningful financial reporting.
- Keep your Xero connection active.
- Regularly review payment synchronisation.

---

# Things to Know

- Xero Integration is available only in **Skedular Spaces**.
- The integration is configured at the organization level.
- Secure OAuth authentication is used.
- Authentication tokens are refreshed automatically.
- Invoices can be exported directly into Xero.
- Recurring invoice workflows are supported.
- Payments reconciled in Xero can automatically update bookings and subscriptions in Skedular.
- Organizations can disconnect the integration at any time.

---

# Example

A coworking operator uses Xero to manage all accounting.

They connect their Skedular organization to Xero and configure:

- Billing Mode: Export invoices to Xero
- Default Sales Account: 200
- Tracking Category: Auckland
- Branding Theme: Standard Invoice
- Reference Prefix: SKED

A customer purchases a monthly desk membership.

Skedular creates the subscription and exports the invoice to Xero.

The customer pays the invoice through Xero.

After the payment is reconciled, Xero synchronises the payment status back to Skedular, and the related subscription and bookings are automatically marked as **Payment Confirmed**.

---

# Related Concepts

- Billing and Payouts
- Products
- Offers
- Subscriptions
- Payments
- Invoices
- Organizations
