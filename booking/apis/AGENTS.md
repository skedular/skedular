# Booking API Agent Notes

This file covers API-level entry points in `booking/apis/`.

## Manual Arrears Trigger

There is an API endpoint used to force the organization arrears workflow to run now:

- `PUT /v1/booking/organizations/{organizationId}/generate-arrears-invoices`

Relevant code:

- `Booking.Api/Controllers/BookingController.cs`
- `Booking.Api/Services/WorkaroundService.cs`

## Important Behavior

This endpoint does not simulate the next billing cycle.

It does:

- resolve the organization
- signal the Temporal workflow with `RunNow`
- process whatever earned uninvoiced arrears usage already exists in the current billing period

It does not:

- move time forward
- create future earned segments
- invoice already invoiced segments again

If a user says "I triggered it and nothing happened", that can be correct behavior.

## When Debugging This Path

Check:

1. whether the organization exists and is active
2. whether there are any earned uninvoiced segments in the current period
3. whether the workflow period for `RunNow` is the one you think it is
4. whether the booking graph loads enough owner/customer/currency data for segment generation

## Relationship To Shared Billing Logic

The API does not decide arrears amounts itself. It only triggers the shared workflow path.

If the bug is about:

- installment splitting
- GST/tax
- invoice template choice
- grouped invoice lines
- Stripe checkout

the real fix is usually under `booking/shared/Booking.Shared/`, not in the controller or workaround service.
