# Bank Accounts

## Overview

The **Bank Accounts** section allows organizations to accept payments by direct bank transfer.

Instead of processing payments through a payment gateway such as Stripe, organizations can provide their own bank account details and allow customers to transfer funds directly.

Skedular continues to manage bookings, invoices, subscriptions, and payment tracking, while payment verification is performed manually by the organization.

This option is ideal for organizations that prefer direct bank transfers or want to avoid payment gateway processing fees.

---

# Availability

Bank Account management is available only in:

- **Skedular Spaces**

Only organization owners and administrators can manage bank accounts.

---

# Why Use Bank Accounts?

Some organizations prefer to receive payments directly into their own bank account.

Using bank transfer allows organizations to:

- Avoid credit card processing fees.
- Accept direct bank transfers.
- Continue using Skedular's invoicing system.
- Manually verify incoming payments.
- Manage customer payments without requiring a payment gateway.

This provides complete flexibility while allowing organizations to control how payments are handled.

---

# How Bank Transfers Work

When bank transfer is enabled for a product offer:

1. The customer chooses **Bank Transfer** during checkout.
2. Skedular creates the booking or subscription.
3. An invoice is generated.
4. The invoice displays the organization's bank account details.
5. The customer transfers the funds directly to the organization's bank account.
6. The organization verifies the payment.
7. The payment status is updated within Skedular.

Unlike online card payments, bank transfers are not confirmed automatically.

---

# Managing Bank Accounts

Organizations can manage one or more bank accounts.

Administrators can:

- Add a new bank account.
- Edit existing bank accounts.
- Remove bank accounts.
- Select the default bank account.

The default bank account is normally used when generating invoices for customers paying by bank transfer.

---

# Default Bank Account

One bank account can be marked as the **Default** account.

The default account is used whenever bank account information needs to appear on customer invoices.

Changing the default account affects future invoices only.

---

# Invoice Information

When a customer selects **Bank Transfer**, Skedular generates an invoice containing the information required to complete the payment.

Depending on the organization's configuration, the invoice may include:

- Bank account name.
- Bank account number.
- Bank name.
- Reference information.
- GST or VAT details.
- Invoice number.
- Payment amount.

This provides customers with everything required to complete the transfer.

---

# Payment Verification

Because Skedular does not connect directly to the organization's bank account, payment verification is performed manually.

After reviewing the incoming payment, organization owners or administrators can update the payment status.

Typical actions include:

- Confirm Payment
- Reject Payment
- Payment Not Required

This allows the organization to control exactly when bookings become confirmed.

---

# Payment Status

Bank transfer payments typically move through several stages.

Examples include:

- Awaiting Payment
- Payment Confirmed
- Payment Rejected
- Payment Not Required

These statuses help administrators track outstanding customer payments.

---

# Product Configuration

Adding a bank account does not automatically enable bank transfer payments.

Each product offer determines which payment methods are available.

To accept bank transfers:

1. Configure a bank account.
2. Set it as the default if required.
3. Enable **Bank Transfer** as an accepted payment method on the product offer.

Customers will then be able to choose bank transfer during checkout.

---

# Bank Transfers vs Stripe

Skedular supports both manual bank transfers and automated Stripe payments.

| Bank Transfer                               | Stripe Connect                                     |
| ------------------------------------------- | -------------------------------------------------- |
| Payment made directly to your bank account. | Payment processed automatically through Stripe.    |
| No payment gateway processing fees.         | Stripe processing fees apply.                      |
| Payment confirmation is manual.             | Payment confirmation is automatic.                 |
| Invoice includes bank account details.      | Customer pays online using a credit or debit card. |
| Organization verifies payment.              | Stripe verifies payment automatically.             |

Organizations can enable either method or both, depending on how they wish to accept payments.

---

# Best Practices

For the best experience:

- Configure a default bank account before publishing products.
- Verify bank account details regularly.
- Confirm payments promptly after receiving funds.
- Keep GST or VAT information current.
- Review outstanding payments regularly.

---

# Things to Know

- Bank Accounts are available only in **Skedular Spaces**.
- Organizations can configure multiple bank accounts.
- One bank account can be selected as the default.
- Bank transfer payments are verified manually.
- Skedular generates invoices automatically.
- Invoices include the configured bank account information.
- Product offers must explicitly enable Bank Transfer before customers can use it.
- Bank transfers can be used alongside Stripe Connect.

---

# Example

A coworking operator prefers to receive payments directly into their business bank account.

The administrator opens **Admin → Bank Accounts**, adds the organization's ANZ account, and marks it as the default account.

The administrator then enables **Bank Transfer** as a payment method for a Premium Desk product.

A customer purchases the product and selects **Bank Transfer** during checkout.

Skedular immediately generates an invoice showing the bank account details and the applicable GST information.

The customer transfers the payment directly to the organization's bank account.

After verifying the payment has been received, the administrator opens the booking and selects **Confirm Payment**.

The booking is updated to **Payment Confirmed**, allowing the customer to continue using the purchased product.

---

# Related Concepts

- Billing and Payouts
- Products
- Offers
- Payments
- Invoices
- Stripe Connect
- Xero Integration
- Subscriptions
- Organizations
