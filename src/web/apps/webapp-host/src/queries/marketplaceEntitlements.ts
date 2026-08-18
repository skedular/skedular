import { graphql } from 'react-relay';

export const marketplaceEntitlementsQuery = graphql`
  query marketplaceEntitlementsQuery($customerId: String!) {
    entitlementsByCustomer(customerId: $customerId) {
      id
      status
      availableQuantity
      grantedQuantity
      expiresAt
      refund {
        id
        amount
        unusedCreditQuantity
        status
        paymentRefundStatus
      }
      ledger {
        id
        bookingId
        quantity
        transactionType
        referenceKey
        createdAt
      }
    }
  }
`;

export const consumeEntitlementCreditMutation = graphql`
  mutation marketplaceEntitlementsConsumeMutation($input: ConsumeEntitlementCreditInput!) {
    consumeEntitlementCredit(input: $input) {
      clientMutationId
      error
      ledgerEntry {
        id
        bookingId
        quantity
        transactionType
        referenceKey
        createdAt
      }
    }
  }
`;
