# Public Documentation Contract

Public help content for Skedular Spaces and Host must explain:

- reservation pricing selects a date/resource during purchase, while token pricing purchases future usage without creating a booking;
- Stripe card payment is automatic and bank transfer requires manual confirmation;
- tokens are granted only after confirmed payment;
- token quantity, validity, weekday/resource scope, cancellation, refund, and auto-renew settings;
- auto-renew uses the current active token pricing; failed renewal does not extend the current cycle or grant unconfirmed tokens;
- customers can later create, modify date/time/resource, and cancel token-funded bookings;
- authorized Spaces/Host owners and administrators can perform those actions on behalf of customers;
- cancellation restores or forfeits tokens according to the configured policy;
- expiry, refund, manual settlement, ledger, and payment states;
- existing reservation-based and recurring behavior remains unchanged.

Update the public-web source map and generated llms content through the existing documentation pipeline.
