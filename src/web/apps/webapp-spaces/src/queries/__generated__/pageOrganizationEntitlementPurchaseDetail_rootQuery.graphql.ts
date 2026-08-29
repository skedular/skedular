/**
 * @generated SignedSource<<0f73db488f18875989ae81df86c58af6>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type MarketplacePurchaseHistoryEventType = "CANCELLATION_COMPLETED" | "CANCELLATION_SCHEDULED" | "CREDITS_CONSUMED" | "ENTITLEMENT_CREATED" | "ENTITLEMENT_EXPIRED" | "PAYMENT_STATE_CHANGED" | "PURCHASE_CREATED" | "REFUND_STATE_CHANGED" | "SUBSCRIPTION_RENEWED" | "SUBSCRIPTION_STARTED" | "%future added value";
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type pageOrganizationEntitlementPurchaseDetail_rootQuery$variables = {
  linkedBookingsAfter?: string | null | undefined;
  purchaseId: string;
};
export type pageOrganizationEntitlementPurchaseDetail_rootQuery$data = {
  readonly entitlementPurchase: {
    readonly amount: any;
    readonly creditQuantity: number;
    readonly currency: string;
    readonly customerId: string;
    readonly customerName: string | null | undefined;
    readonly entitlement: {
      readonly autoRenew: boolean;
      readonly cancelAtPeriodEnd: boolean;
      readonly id: string;
      readonly nextRenewalAt: any | null | undefined;
      readonly renewalFailureReason: string | null | undefined;
      readonly status: EntitlementStatus;
    } | null | undefined;
    readonly entitlementId: string | null | undefined;
    readonly history: {
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly amount: any | null | undefined;
          readonly cancellationEffectiveAt: any | null | undefined;
          readonly cancellationRequestedAt: any | null | undefined;
          readonly creditQuantity: number | null | undefined;
          readonly currency: Currency | null | undefined;
          readonly id: string;
          readonly name: string;
          readonly occurredAt: any;
          readonly paymentStatus: PaymentStatus | null | undefined;
          readonly previousPaymentStatus: PaymentStatus | null | undefined;
          readonly previousRefundStatus: MarketplaceRefundStatus | null | undefined;
          readonly reason: string | null | undefined;
          readonly refundId: string | null | undefined;
          readonly refundStatus: MarketplaceRefundStatus | null | undefined;
          readonly remainingCreditQuantity: number | null | undefined;
          readonly type: MarketplacePurchaseHistoryEventType;
        };
      }>;
    };
    readonly id: string;
    readonly invoiceNumber: string | null | undefined;
    readonly invoiceUrl: string | null | undefined;
    readonly lifecycleState: string;
    readonly linkedBookings: {
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly bookingResources: ReadonlyArray<{
            readonly resource: {
              readonly id: string;
              readonly name: string;
            };
          }>;
          readonly from: any;
          readonly id: string;
          readonly involvedCustomers: ReadonlyArray<{
            readonly familyName: string | null | undefined;
            readonly givenName: string | null | undefined;
            readonly id: string;
            readonly middleName: string | null | undefined;
            readonly name: string | null | undefined;
          }>;
          readonly involvedLocations: ReadonlyArray<{
            readonly name: string;
            readonly uniqueId: string;
          }>;
          readonly until: any;
        };
      }>;
      readonly pageInfo: {
        readonly endCursor: string | null | undefined;
        readonly hasNextPage: boolean;
        readonly hasPreviousPage: boolean;
      };
      readonly totalCount: number;
    };
    readonly organizationId: string;
    readonly paymentMethod: string;
    readonly paymentStatus: string;
    readonly pricingId: string;
    readonly serviceStartAt: any;
    readonly validityDays: number;
  } | null | undefined;
};
export type pageOrganizationEntitlementPurchaseDetail_rootQuery = {
  response: pageOrganizationEntitlementPurchaseDetail_rootQuery$data;
  variables: pageOrganizationEntitlementPurchaseDetail_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "linkedBookingsAfter"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "purchaseId"
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentStatus",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "creditQuantity",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "amount",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currency",
  "storageKey": null
},
v8 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "purchaseId",
        "variableName": "purchaseId"
      }
    ],
    "concreteType": "EntitlementPurchaseDetails",
    "kind": "LinkedField",
    "name": "entitlementPurchase",
    "plural": false,
    "selections": [
      (v2/*:: as any*/),
      {
        "alias": null,
        "args": [
          {
            "kind": "Literal",
            "name": "first",
            "value": 100
          }
        ],
        "concreteType": "ConnectionOfMarketplacePurchaseHistoryEventEdge",
        "kind": "LinkedField",
        "name": "history",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplacePurchaseHistoryEventEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplacePurchaseHistoryEventDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v2/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "type",
                    "storageKey": null
                  },
                  (v3/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "occurredAt",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "previousPaymentStatus",
                    "storageKey": null
                  },
                  (v4/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "previousRefundStatus",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "refundStatus",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "refundId",
                    "storageKey": null
                  },
                  (v5/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "remainingCreditQuantity",
                    "storageKey": null
                  },
                  (v6/*:: as any*/),
                  (v7/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "cancellationRequestedAt",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "cancellationEffectiveAt",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "reason",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": "history(first:100)"
      },
      (v4/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "lifecycleState",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "paymentMethod",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "serviceStartAt",
        "storageKey": null
      },
      (v6/*:: as any*/),
      (v7/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "pricingId",
        "storageKey": null
      },
      (v5/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "validityDays",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "customerId",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "customerName",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "organizationId",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "entitlementId",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "EntitlementDetails",
        "kind": "LinkedField",
        "name": "entitlement",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "autoRenew",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cancelAtPeriodEnd",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "status",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "nextRenewalAt",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "renewalFailureReason",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "invoiceNumber",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "invoiceUrl",
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "after",
            "variableName": "linkedBookingsAfter"
          },
          {
            "kind": "Literal",
            "name": "first",
            "value": 10
          }
        ],
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "linkedBookings",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "totalCount",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "PageInfo",
            "kind": "LinkedField",
            "name": "pageInfo",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "hasNextPage",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "hasPreviousPage",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "endCursor",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v2/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "from",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "until",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CustomerDetails",
                    "kind": "LinkedField",
                    "name": "involvedCustomers",
                    "plural": true,
                    "selections": [
                      (v2/*:: as any*/),
                      (v3/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "givenName",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "middleName",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "familyName",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingResourceDetails",
                    "kind": "LinkedField",
                    "name": "bookingResources",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ResourceDetails",
                        "kind": "LinkedField",
                        "name": "resource",
                        "plural": false,
                        "selections": [
                          (v2/*:: as any*/),
                          (v3/*:: as any*/)
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Booking_LocationDetails",
                    "kind": "LinkedField",
                    "name": "involvedLocations",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "uniqueId",
                        "storageKey": null
                      },
                      (v3/*:: as any*/)
                    ],
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationEntitlementPurchaseDetail_rootQuery",
    "selections": (v8/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*:: as any*/),
      (v0/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationEntitlementPurchaseDetail_rootQuery",
    "selections": (v8/*:: as any*/)
  },
  "params": {
    "cacheID": "7a362627e734158352085425a8dab49d",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationEntitlementPurchaseDetail_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationEntitlementPurchaseDetail_rootQuery(\n  $purchaseId: String!\n  $linkedBookingsAfter: String\n) {\n  entitlementPurchase(purchaseId: $purchaseId) {\n    id\n    history(first: 100) {\n      edges {\n        node {\n          id\n          type\n          name\n          occurredAt\n          previousPaymentStatus\n          paymentStatus\n          previousRefundStatus\n          refundStatus\n          refundId\n          creditQuantity\n          remainingCreditQuantity\n          amount\n          currency\n          cancellationRequestedAt\n          cancellationEffectiveAt\n          reason\n        }\n      }\n    }\n    paymentStatus\n    lifecycleState\n    paymentMethod\n    serviceStartAt\n    amount\n    currency\n    pricingId\n    creditQuantity\n    validityDays\n    customerId\n    customerName\n    organizationId\n    entitlementId\n    entitlement {\n      id\n      autoRenew\n      cancelAtPeriodEnd\n      status\n      nextRenewalAt\n      renewalFailureReason\n    }\n    invoiceNumber\n    invoiceUrl\n    linkedBookings(after: $linkedBookingsAfter, first: 10) {\n      totalCount\n      pageInfo {\n        hasNextPage\n        hasPreviousPage\n        endCursor\n      }\n      edges {\n        node {\n          id\n          from\n          until\n          involvedCustomers {\n            id\n            name\n            givenName\n            middleName\n            familyName\n          }\n          bookingResources {\n            resource {\n              id\n              name\n            }\n          }\n          involvedLocations {\n            uniqueId\n            name\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "443979aa9bdff8248d9e0740b6e2393e";

export default node;
