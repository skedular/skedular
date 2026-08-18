# GraphQL Contract: Credit-Based Booking Entitlements

The generated Booking GraphQL schema must expose:

- pricing fulfillment type and token/renewal configuration;
- standalone token purchase creation, status/history, and payment action;
- Stripe checkout URL or manual bank-transfer invoice/instructions;
- entitlement cycle balance, validity, restrictions, renewal state, refund state, ledger, and linked bookings;
- customer booking create/update/cancel inputs that accept an eligible entitlement;
- authorized Spaces/Host operator actions on behalf of a customer with actor/customer audit fields;
- explicit choice/detail types for fulfillment, payment, renewal, entitlement, ledger, refund, and booking outcomes.

Purchase mutations must not return or create a booking. Existing booking mutations remain responsible for later token-funded bookings and must apply the same validation/resource/cancellation rules as reservation bookings.

## Payment and renewal

Stripe uses the existing automatic checkout/webhook pattern, with purchase/renewal identifiers and selected pricing metadata. Webhooks resolve the purchase or renewal directly, verify amount/currency/pricing, and grant exactly once.

Bank transfer returns invoice/instruction details and requires authorized manual confirmation before grant or renewal. Xero is an accounting projection; credit notes/manual settlement do not replace local payment state.

Renewal re-evaluates current active token pricing. A pending/failed renewal does not extend the ending cycle. No new cycle is returned until payment confirms. If no compatible token auto-renew pricing exists, renewal fails audibly without reservation fallback.

All service methods return domain models; database entities are mapped by injected mapper services. Generated GraphQL and Relay artifacts must be regenerated from source.
