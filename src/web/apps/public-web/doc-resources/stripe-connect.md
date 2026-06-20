# Stripe Connect

## Overview

Stripe Connect allows organizations to securely accept credit and debit card payments through **Skedular Spaces**.

By connecting a Stripe Connect account to your organization, customers can pay for products and subscriptions using supported payment cards.

Payments are processed securely by Stripe, and funds are deposited directly into your connected Stripe account.

Skedular does not process or store card information.

---

# Availability

Stripe Connect is available only in:

- **Skedular Spaces**

Only organization owners and administrators can configure Stripe Connect.

---

# Why Use Stripe Connect?

Stripe Connect provides a secure and reliable way to accept online payments.

By connecting your Stripe account, you can:

- Accept credit and debit card payments.
- Receive payments directly into your Stripe account.
- Support immediate payment confirmation.
- Reduce manual payment processing.
- Provide a faster checkout experience for customers.

---

# How Stripe Connect Works

Skedular integrates directly with Stripe Connect.

When a customer chooses to pay by card:

1. The payment is securely processed by Stripe.
2. Stripe deducts its processing fees.
3. The remaining funds are deposited into your connected Stripe account.
4. Skedular receives confirmation of the successful payment.
5. The related booking or subscription is automatically confirmed.

At no point does Skedular handle or store the customer's payment card details.

---

# Connecting a Stripe Account

To connect your Stripe account:

1. Open your organization.
2. Navigate to **Admin**.
3. Open **Stripe Connect**.
4. Select one of the following options:
   - **Add New Account** to create a new Stripe account.
   - **Add Existing Account** to connect an existing Stripe account.
5. Complete Stripe's onboarding process.
6. Return to Skedular.

Once onboarding is complete, your organization is ready to accept card payments.

---

# Stripe Onboarding

Stripe manages the onboarding process directly.

Depending on your country and business type, Stripe may ask you to provide:

- Business information.
- Contact details.
- Identity verification.
- Banking information.
- Tax information.

These requirements are determined entirely by Stripe.

---

# Payment Processing

Once connected, supported product offers can accept payments using credit and debit cards.

When a customer completes payment:

- Stripe authorises the payment.
- Funds are transferred to your connected Stripe account.
- Skedular updates the booking or subscription status.
- Customers can continue with their booking immediately.

---

# Stripe Fees

Stripe charges payment processing fees according to the pricing applicable in your country.

These fees are determined entirely by Stripe.

Skedular does **not** add any additional payment processing fees to Stripe transactions.

Because Stripe deducts its fees before transferring funds, organizations should consider these fees when setting product prices.

For example, if you want to receive **$100** after payment processing fees, you should configure your product pricing accordingly.

Current Stripe pricing and fees can be found in your Stripe account or on the Stripe website.

---

# Managing Stripe Accounts

Organization owners and administrators can:

- Connect a new Stripe account.
- Connect an existing Stripe account.
- Review connected accounts.
- View onboarding status.
- Configure the default payout account.
- Disconnect Stripe when required.

Organizations can manage one or more connected Stripe accounts depending on their business requirements.

---

# Booking Confirmation

Stripe integrates directly with the booking workflow.

When payment is successfully completed:

- The booking is confirmed.
- The payment status is updated.
- Customers can immediately access their booking.

Failed or incomplete payments do not automatically confirm bookings.

---

# Security

Stripe Connect uses secure OAuth authentication.

Sensitive payment information is handled entirely by Stripe.

Skedular does not:

- Store payment card numbers.
- Store security codes.
- Process raw card information.

This helps organizations meet modern payment security standards.

---

# Best Practices

For the best experience:

- Complete Stripe onboarding before publishing products.
- Verify your bank account within Stripe.
- Review Stripe's processing fees before setting product prices.
- Keep your Stripe account active and in good standing.
- Monitor payment activity regularly through both Stripe and Skedular.

---

# Things to Know

- Stripe Connect is available only in **Skedular Spaces**.
- Credit and debit card payments currently require a connected Stripe account.
- Stripe processes all card payments securely.
- Funds are paid directly into your connected Stripe account.
- Stripe deducts its own processing fees.
- Skedular does not charge additional fees on Stripe transactions.
- Product pricing should take Stripe's processing fees into account.
- Successful card payments automatically confirm bookings and subscriptions.

---

# Example

A coworking operator wants to accept credit card payments.

The administrator opens **Admin → Stripe Connect** and connects an existing Stripe account.

After completing Stripe's onboarding process, the operator enables card payments for selected product offers.

A customer purchases a Premium Desk Day Pass using a credit card.

Stripe securely processes the payment, deducts its processing fee, and deposits the remaining funds into the operator's Stripe account.

Skedular receives payment confirmation immediately and automatically marks the booking as **Payment Confirmed**, allowing the customer to access their booking without any manual intervention.

---

# Related Concepts

- Billing and Payouts
- Products
- Offers
- Payments
- Subscriptions
- Xero Integration
- Organizations
