/**
 * @generated SignedSource<<a624f339dcbf7177d038b8c4d1bb6543>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type Currency = "NZD" | "USD" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type MarketplacePurchaseHistoryOrderField = "ACTIVITY_AT" | "BOOKING_FROM" | "BOOKING_UNTIL" | "PURCHASED_AT" | "%future added value";
export type MarketplacePurchaseLifecycleState = "ACTIVE" | "CANCELLED" | "DELETED" | "EXPIRED" | "PAYMENT_FAILED" | "PENDING" | "%future added value";
export type MarketplacePurchaseRenewalState = "DOES_NOT_RENEW" | "NOT_APPLICABLE" | "RENEWS" | "%future added value";
export type MarketplacePurchaseSourceType = "BOOKING" | "SUBSCRIPTION" | "%future added value";
export type MarketplaceRefundEventType = "ACCOUNTING_PROJECTED" | "ACCOUNTING_PROJECTION_REQUIRED" | "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "SENT_TO_XERO" | "UNDER_REVIEW" | "%future added value";
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type MarketplacePurchaseHistoryOrderInput = {
  direction: OrderDirection;
  field: MarketplacePurchaseHistoryOrderField;
};
export type pageOrganizationSubscriptions_rootQuery$variables = {
  organizationCustomDomain: string;
  paymentStatuses?: ReadonlyArray<PaymentStatus> | null | undefined;
  purchaseActivityFrom?: any | null | undefined;
  purchaseActivityUntil?: any | null | undefined;
  purchaseAfter?: string | null | undefined;
  purchaseFirst?: number | null | undefined;
  purchaseLifecycleStates?: ReadonlyArray<MarketplacePurchaseLifecycleState> | null | undefined;
  purchaseOrderBy?: ReadonlyArray<MarketplacePurchaseHistoryOrderInput> | null | undefined;
  purchasePaymentStatuses?: ReadonlyArray<PaymentStatus> | null | undefined;
  purchaseSourceTypes?: ReadonlyArray<MarketplacePurchaseSourceType> | null | undefined;
  statuses?: ReadonlyArray<MarketplaceBookingSubscriptionStatus> | null | undefined;
};
export type pageOrganizationSubscriptions_rootQuery$data = {
  readonly marketplaceBookingSubscriptionCancellationModes: ReadonlyArray<{
    readonly name: string;
    readonly type: MarketplaceBookingSubscriptionCancellationMode;
  }>;
  readonly marketplaceBookingSubscriptions: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly autoRenew: boolean;
        readonly cancelAtPeriodEnd: boolean;
        readonly cancellationOverrideReason: string | null | undefined;
        readonly cancellationPolicyOverridden: boolean;
        readonly id: string;
        readonly involvedCustomers: ReadonlyArray<{
          readonly familyName: string | null | undefined;
          readonly givenName: string | null | undefined;
          readonly id: string;
          readonly middleName: string | null | undefined;
          readonly name: string | null | undefined;
        }>;
        readonly marketplaceBooking: {
          readonly paymentMethod: {
            readonly name: string;
            readonly type: PaymentMethod;
          };
          readonly paymentStatus: {
            readonly name: string;
            readonly type: PaymentStatus;
          };
          readonly productVersion: {
            readonly listingMetadata: {
              readonly title: string | null | undefined;
            };
          };
          readonly quantity: number;
        };
        readonly nextRenewalAt: any | null | undefined;
        readonly recurringBookings: ReadonlyArray<{
          readonly endDate: any | null | undefined;
          readonly id: string;
          readonly marketplaceBooking: {
            readonly id: string;
            readonly invoiceUrl: string | null | undefined;
            readonly paymentMethod: {
              readonly name: string;
              readonly type: PaymentMethod;
            };
            readonly paymentStatus: {
              readonly name: string;
              readonly type: PaymentStatus;
            };
            readonly quantity: number;
          } | null | undefined;
          readonly startDate: any;
        }>;
        readonly refund: {
          readonly canProcessInXero: boolean;
          readonly currency: {
            readonly name: string;
            readonly type: Currency;
          } | null | undefined;
          readonly currencyToDisplay: string;
          readonly events: ReadonlyArray<{
            readonly actorName: string | null | undefined;
            readonly currencyToDisplay: string;
            readonly eventType: {
              readonly name: string;
              readonly type: MarketplaceRefundEventType;
            };
            readonly externalRefundNumber: string | null | undefined;
            readonly id: string;
            readonly lastError: string | null | undefined;
            readonly occurredAt: any;
            readonly reason: string | null | undefined;
            readonly refundAmount: any | null | undefined;
          }>;
          readonly externalRefundNumber: string | null | undefined;
          readonly id: string;
          readonly lastError: string | null | undefined;
          readonly lastProcessedAt: any | null | undefined;
          readonly reason: string | null | undefined;
          readonly refundAmount: any | null | undefined;
          readonly refundPercentage: number;
          readonly requestedAt: any;
          readonly requestedByCustomerName: string | null | undefined;
          readonly status: {
            readonly name: string;
            readonly type: MarketplaceRefundStatus;
          };
          readonly xeroProcessingBlockedReason: string | null | undefined;
        } | null | undefined;
        readonly startedAt: any;
        readonly status: {
          readonly name: string;
          readonly type: MarketplaceBookingSubscriptionStatus;
        };
      };
    }>;
    readonly totalCount: number;
  };
  readonly marketplacePurchases: {
    readonly edges: ReadonlyArray<{
      readonly cursor: string;
      readonly node: {
        readonly activityAt: any;
        readonly bookingFrom: any | null | undefined;
        readonly bookingId: string | null | undefined;
        readonly bookingUntil: any | null | undefined;
        readonly cancellationReason: string | null | undefined;
        readonly currency: Currency | null | undefined;
        readonly customerId: string | null | undefined;
        readonly deletedByCustomerId: string | null | undefined;
        readonly id: string;
        readonly isDeleted: boolean;
        readonly lifecycleState: MarketplacePurchaseLifecycleState;
        readonly lifecycleStateName: string;
        readonly paymentStatus: PaymentStatus;
        readonly productTitle: string | null | undefined;
        readonly productVersionId: string | null | undefined;
        readonly purchasedAt: any;
        readonly refund: {
          readonly events: ReadonlyArray<{
            readonly eventType: {
              readonly name: string;
            };
            readonly id: string;
            readonly occurredAt: any;
          }>;
          readonly id: string;
          readonly lastProcessedAt: any | null | undefined;
          readonly refundAmount: any | null | undefined;
          readonly requestedAt: any;
          readonly status: {
            readonly name: string;
          };
        } | null | undefined;
        readonly refundId: string | null | undefined;
        readonly renewalState: MarketplacePurchaseRenewalState;
        readonly renewalStateName: string;
        readonly sourceId: string;
        readonly sourceType: MarketplacePurchaseSourceType;
        readonly sourceTypeName: string;
        readonly totalAmount: any | null | undefined;
      };
    }>;
    readonly pageInfo: {
      readonly endCursor: string | null | undefined;
      readonly hasNextPage: boolean;
      readonly hasPreviousPage: boolean;
      readonly startCursor: string | null | undefined;
    };
    readonly totalCount: number;
  };
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly organizationBookingPermissions: {
    readonly canModifyPaymentMethod: boolean;
    readonly canViewBookings: boolean;
  };
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesMarketplaceBookingPaymentStatuses_query" | "multipleChoicesMarketplaceBookingSubscriptionStatuses_query">;
};
export type pageOrganizationSubscriptions_rootQuery = {
  response: pageOrganizationSubscriptions_rootQuery$data;
  variables: pageOrganizationSubscriptions_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "paymentStatuses"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "purchaseActivityFrom"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "purchaseActivityUntil"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "purchaseAfter"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "purchaseFirst"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "purchaseLifecycleStates"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "purchaseOrderBy"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "purchasePaymentStatuses"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "purchaseSourceTypes"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "statuses"
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v12 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v11/*:: as any*/)
],
v13 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingSubscriptionCancellationModes",
  "plural": true,
  "selections": (v12/*:: as any*/),
  "storageKey": null
},
v14 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v15 = [
  (v11/*:: as any*/)
],
v16 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v17 = {
  "alias": null,
  "args": [
    (v16/*:: as any*/)
  ],
  "concreteType": "OrganizationBookingPermissions",
  "kind": "LinkedField",
  "name": "organizationBookingPermissions",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canViewBookings",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canModifyPaymentMethod",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v18 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
  },
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "ASCENDING",
        "field": "NEXT_RENEWAL_AT"
      }
    ]
  },
  {
    "fields": [
      (v16/*:: as any*/),
      {
        "kind": "Variable",
        "name": "paymentStatuses",
        "variableName": "paymentStatuses"
      },
      {
        "kind": "Variable",
        "name": "statuses",
        "variableName": "statuses"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancellationPolicyOverridden",
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancellationOverrideReason",
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startedAt",
  "storageKey": null
},
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "nextRenewalAt",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "autoRenew",
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancelAtPeriodEnd",
  "storageKey": null
},
v27 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "requestedAt",
  "storageKey": null
},
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastProcessedAt",
  "storageKey": null
},
v29 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v30 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v31 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v32 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v33 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v34 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "occurredAt",
  "storageKey": null
},
v35 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceRefundDetails",
  "kind": "LinkedField",
  "name": "refund",
  "plural": false,
  "selections": [
    (v20/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currency",
      "plural": false,
      "selections": (v12/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceRefundStatusDetails",
      "kind": "LinkedField",
      "name": "status",
      "plural": false,
      "selections": (v12/*:: as any*/),
      "storageKey": null
    },
    (v27/*:: as any*/),
    (v28/*:: as any*/),
    (v29/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "refundPercentage",
      "storageKey": null
    },
    (v30/*:: as any*/),
    (v31/*:: as any*/),
    (v32/*:: as any*/),
    (v33/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "requestedByCustomerName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canProcessInXero",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "xeroProcessingBlockedReason",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceRefundEventDetails",
      "kind": "LinkedField",
      "name": "events",
      "plural": true,
      "selections": [
        (v20/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceRefundEventTypeDetails",
          "kind": "LinkedField",
          "name": "eventType",
          "plural": false,
          "selections": (v12/*:: as any*/),
          "storageKey": null
        },
        (v34/*:: as any*/),
        (v29/*:: as any*/),
        (v30/*:: as any*/),
        (v31/*:: as any*/),
        (v32/*:: as any*/),
        (v33/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "actorName",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v36 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v12/*:: as any*/),
  "storageKey": null
},
v37 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "involvedCustomers",
  "plural": true,
  "selections": [
    (v20/*:: as any*/),
    (v11/*:: as any*/),
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
v38 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v39 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v12/*:: as any*/),
  "storageKey": null
},
v40 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v12/*:: as any*/),
  "storageKey": null
},
v41 = {
  "alias": null,
  "args": null,
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "listingMetadata",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "title",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v42 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBookings",
  "plural": true,
  "selections": [
    (v20/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "startDate",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "endDate",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingDetails",
      "kind": "LinkedField",
      "name": "marketplaceBooking",
      "plural": false,
      "selections": [
        (v20/*:: as any*/),
        (v38/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "invoiceUrl",
          "storageKey": null
        },
        (v39/*:: as any*/),
        (v40/*:: as any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v43 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "activityFrom",
      "variableName": "purchaseActivityFrom"
    },
    {
      "kind": "Variable",
      "name": "activityUntil",
      "variableName": "purchaseActivityUntil"
    },
    {
      "kind": "Variable",
      "name": "after",
      "variableName": "purchaseAfter"
    },
    {
      "kind": "Variable",
      "name": "first",
      "variableName": "purchaseFirst"
    },
    {
      "kind": "Variable",
      "name": "lifecycleStates",
      "variableName": "purchaseLifecycleStates"
    },
    {
      "kind": "Variable",
      "name": "orderBy",
      "variableName": "purchaseOrderBy"
    },
    (v16/*:: as any*/),
    {
      "kind": "Variable",
      "name": "paymentStatuses",
      "variableName": "purchasePaymentStatuses"
    },
    {
      "kind": "Variable",
      "name": "sourceTypes",
      "variableName": "purchaseSourceTypes"
    }
  ],
  "concreteType": "ConnectionOfMarketplacePurchaseHistoryEdge",
  "kind": "LinkedField",
  "name": "marketplacePurchases",
  "plural": false,
  "selections": [
    (v19/*:: as any*/),
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
          "name": "startCursor",
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
      "concreteType": "MarketplacePurchaseHistoryEdge",
      "kind": "LinkedField",
      "name": "edges",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "cursor",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplacePurchaseHistoryDetails",
          "kind": "LinkedField",
          "name": "node",
          "plural": false,
          "selections": [
            (v20/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "sourceId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "sourceType",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "sourceTypeName",
              "storageKey": null
            },
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
              "name": "lifecycleStateName",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "renewalState",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "renewalStateName",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "purchasedAt",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "activityAt",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "bookingFrom",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "bookingUntil",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "paymentStatus",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "productVersionId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "productTitle",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "totalAmount",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "currency",
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
              "name": "deletedByCustomerId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "cancellationReason",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "refundId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "bookingId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "MarketplaceRefundDetails",
              "kind": "LinkedField",
              "name": "refund",
              "plural": false,
              "selections": [
                (v20/*:: as any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "MarketplaceRefundStatusDetails",
                  "kind": "LinkedField",
                  "name": "status",
                  "plural": false,
                  "selections": (v15/*:: as any*/),
                  "storageKey": null
                },
                (v27/*:: as any*/),
                (v28/*:: as any*/),
                (v29/*:: as any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "MarketplaceRefundEventDetails",
                  "kind": "LinkedField",
                  "name": "events",
                  "plural": true,
                  "selections": [
                    (v20/*:: as any*/),
                    (v34/*:: as any*/),
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "MarketplaceRefundEventTypeDetails",
                      "kind": "LinkedField",
                      "name": "eventType",
                      "plural": false,
                      "selections": (v15/*:: as any*/),
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "isDeleted",
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
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      (v6/*:: as any*/),
      (v7/*:: as any*/),
      (v8/*:: as any*/),
      (v9/*:: as any*/),
      (v10/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationSubscriptions_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesMarketplaceBookingSubscriptionStatuses_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesMarketplaceBookingPaymentStatuses_query"
      },
      (v13/*:: as any*/),
      {
        "alias": null,
        "args": (v14/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v15/*:: as any*/),
        "storageKey": null
      },
      (v17/*:: as any*/),
      {
        "alias": null,
        "args": (v18/*:: as any*/),
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
          (v19/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingSubscriptionEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingSubscriptionDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v20/*:: as any*/),
                  (v21/*:: as any*/),
                  (v22/*:: as any*/),
                  (v23/*:: as any*/),
                  (v24/*:: as any*/),
                  (v25/*:: as any*/),
                  (v26/*:: as any*/),
                  (v35/*:: as any*/),
                  (v36/*:: as any*/),
                  (v37/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v38/*:: as any*/),
                      (v39/*:: as any*/),
                      (v40/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v41/*:: as any*/)
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  (v42/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      (v43/*:: as any*/)
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v10/*:: as any*/),
      (v1/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      (v9/*:: as any*/),
      (v6/*:: as any*/),
      (v8/*:: as any*/),
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v7/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationSubscriptions_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptionStatuses",
        "plural": true,
        "selections": (v12/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceBookingPaymentStatusDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingPaymentStatuses",
        "plural": true,
        "selections": (v12/*:: as any*/),
        "storageKey": null
      },
      (v13/*:: as any*/),
      {
        "alias": null,
        "args": (v14/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v11/*:: as any*/),
          (v20/*:: as any*/)
        ],
        "storageKey": null
      },
      (v17/*:: as any*/),
      {
        "alias": null,
        "args": (v18/*:: as any*/),
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
          (v19/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingSubscriptionEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingSubscriptionDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v20/*:: as any*/),
                  (v21/*:: as any*/),
                  (v22/*:: as any*/),
                  (v23/*:: as any*/),
                  (v24/*:: as any*/),
                  (v25/*:: as any*/),
                  (v26/*:: as any*/),
                  (v35/*:: as any*/),
                  (v36/*:: as any*/),
                  (v37/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v38/*:: as any*/),
                      (v39/*:: as any*/),
                      (v40/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v41/*:: as any*/),
                          (v20/*:: as any*/)
                        ],
                        "storageKey": null
                      },
                      (v20/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  (v42/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      (v43/*:: as any*/)
    ]
  },
  "params": {
    "cacheID": "9d8e251c54779adcaf13d4a59927acc8",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptions_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationSubscriptions_rootQuery(\n  $organizationCustomDomain: String!\n  $statuses: [MarketplaceBookingSubscriptionStatus!]\n  $paymentStatuses: [PaymentStatus!]\n  $purchaseAfter: String\n  $purchaseFirst: Int\n  $purchaseSourceTypes: [MarketplacePurchaseSourceType!]\n  $purchaseLifecycleStates: [MarketplacePurchaseLifecycleState!]\n  $purchasePaymentStatuses: [PaymentStatus!]\n  $purchaseActivityFrom: DateTime\n  $purchaseActivityUntil: DateTime\n  $purchaseOrderBy: [MarketplacePurchaseHistoryOrderInput!]\n) {\n  ...multipleChoicesMarketplaceBookingSubscriptionStatuses_query\n  ...multipleChoicesMarketplaceBookingPaymentStatuses_query\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    id\n  }\n  organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {\n    canViewBookings\n    canModifyPaymentMethod\n  }\n  marketplaceBookingSubscriptions(first: 50, where: {organizationCustomDomain: $organizationCustomDomain, statuses: $statuses, paymentStatuses: $paymentStatuses}, orderBy: [{field: NEXT_RENEWAL_AT, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        cancellationPolicyOverridden\n        cancellationOverrideReason\n        startedAt\n        nextRenewalAt\n        autoRenew\n        cancelAtPeriodEnd\n        refund {\n          id\n          currency {\n            type\n            name\n          }\n          status {\n            type\n            name\n          }\n          requestedAt\n          lastProcessedAt\n          refundAmount\n          refundPercentage\n          currencyToDisplay\n          reason\n          lastError\n          externalRefundNumber\n          requestedByCustomerName\n          canProcessInXero\n          xeroProcessingBlockedReason\n          events {\n            id\n            eventType {\n              type\n              name\n            }\n            occurredAt\n            refundAmount\n            currencyToDisplay\n            reason\n            lastError\n            externalRefundNumber\n            actorName\n          }\n        }\n        status {\n          type\n          name\n        }\n        involvedCustomers {\n          id\n          name\n          givenName\n          middleName\n          familyName\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          paymentMethod {\n            type\n            name\n          }\n          productVersion {\n            listingMetadata {\n              title\n            }\n            id\n          }\n          id\n        }\n        recurringBookings {\n          id\n          startDate\n          endDate\n          marketplaceBooking {\n            id\n            quantity\n            invoiceUrl\n            paymentStatus {\n              type\n              name\n            }\n            paymentMethod {\n              type\n              name\n            }\n          }\n        }\n      }\n    }\n  }\n  marketplacePurchases(after: $purchaseAfter, first: $purchaseFirst, organizationCustomDomain: $organizationCustomDomain, sourceTypes: $purchaseSourceTypes, lifecycleStates: $purchaseLifecycleStates, paymentStatuses: $purchasePaymentStatuses, activityFrom: $purchaseActivityFrom, activityUntil: $purchaseActivityUntil, orderBy: $purchaseOrderBy) {\n    totalCount\n    pageInfo {\n      hasNextPage\n      hasPreviousPage\n      startCursor\n      endCursor\n    }\n    edges {\n      cursor\n      node {\n        id\n        sourceId\n        sourceType\n        sourceTypeName\n        lifecycleState\n        lifecycleStateName\n        renewalState\n        renewalStateName\n        purchasedAt\n        activityAt\n        bookingFrom\n        bookingUntil\n        paymentStatus\n        productVersionId\n        productTitle\n        totalAmount\n        currency\n        customerId\n        deletedByCustomerId\n        cancellationReason\n        refundId\n        bookingId\n        refund {\n          id\n          status {\n            name\n          }\n          requestedAt\n          lastProcessedAt\n          refundAmount\n          events {\n            id\n            occurredAt\n            eventType {\n              name\n            }\n          }\n        }\n        isDeleted\n      }\n    }\n  }\n}\n\nfragment multipleChoicesMarketplaceBookingPaymentStatuses_query on Query {\n  marketplaceBookingPaymentStatuses {\n    type\n    name\n  }\n}\n\nfragment multipleChoicesMarketplaceBookingSubscriptionStatuses_query on Query {\n  marketplaceBookingSubscriptionStatuses {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "f0a6300fb1c98058286e0184e3947982";

export default node;
