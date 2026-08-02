# Cancellation GraphQL Contract

The public GraphQL surface must distinguish customer cancellation from an authorized operator cancellation without allowing clients to self-grant authority.

## Mutation behavior

The booking and subscription cancellation mutations should accept an explicit cancellation request shape containing:

- target identifier
- existing cancellation mode where supported
- optional client mutation id
- operator reason, required when the authenticated caller is using an administrative policy override

The authenticated request context determines whether the caller is a customer or an authorized owner/admin. The service must reject attempts to use an override reason without the required product-owning organization permission.

## Response behavior

Successful responses should expose the resulting booking/subscription state and, where the existing schema exposes refund details, the refund's current provider-specific state.

Denied responses must distinguish:

- customer cancellation policy restriction
- insufficient owner/admin permission
- missing operator reason
- invalid or already terminal cancellation state

## Provider refund behavior

- Stripe: eligible refund proceeds automatically.
- Bank transfer: refund remains awaiting owner/admin approval and transfer confirmation.
- Xero: refund remains awaiting owner/admin approval or Xero processing/reconciliation as applicable.

If the GraphQL schema or generated Relay operations change, regenerate the backend GraphQL schema through `scripts/generate-graphql.sh` and regenerate affected Relay artifacts through the existing web generation workflow. Generated outputs must not be hand-edited.
