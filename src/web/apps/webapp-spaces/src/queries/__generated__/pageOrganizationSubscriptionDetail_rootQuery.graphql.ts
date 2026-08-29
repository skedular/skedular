/**
 * @generated SignedSource<<f083a3a36c9ba7a53551e1883b51084d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type MarketplacePurchaseHistoryEventType = "CANCELLATION_COMPLETED" | "CANCELLATION_SCHEDULED" | "CREDITS_CONSUMED" | "ENTITLEMENT_CREATED" | "ENTITLEMENT_EXPIRED" | "PAYMENT_STATE_CHANGED" | "PURCHASE_CREATED" | "REFUND_STATE_CHANGED" | "SUBSCRIPTION_RENEWED" | "SUBSCRIPTION_STARTED" | "%future added value";
export type MarketplaceRefundEventType = "ACCOUNTING_PROJECTED" | "ACCOUNTING_PROJECTION_REQUIRED" | "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "SENT_TO_XERO" | "UNDER_REVIEW" | "%future added value";
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type pageOrganizationSubscriptionDetail_rootQuery$variables = {
  bookingAfter?: string | null | undefined;
  linkedBookingAfter?: string | null | undefined;
  organizationCustomDomain: string;
  subscriptionId: string;
};
export type pageOrganizationSubscriptionDetail_rootQuery$data = {
  readonly marketplaceBookingSubscription: {
    readonly autoRenew: boolean;
    readonly bookingInstances: {
      readonly edges: ReadonlyArray<{
        readonly cursor: string;
        readonly node: {
          readonly endDate: any | null | undefined;
          readonly id: string;
          readonly marketplaceBooking: {
            readonly id: string;
            readonly invoiceUrl: string | null | undefined;
            readonly paymentStatus: {
              readonly name: string;
              readonly type: PaymentStatus;
            };
          } | null | undefined;
          readonly startDate: any;
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
    readonly cancelAtPeriodEnd: boolean;
    readonly cancellationOverrideReason: string | null | undefined;
    readonly cancellationPolicyOverridden: boolean;
    readonly cancelledAt: any | null | undefined;
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
    readonly involvedCustomers: ReadonlyArray<{
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly id: string;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
      readonly photoUrl: string | null | undefined;
    }>;
    readonly linkedBookings: {
      readonly edges: ReadonlyArray<{
        readonly cursor: string;
        readonly node: {
          readonly bookingResources: ReadonlyArray<{
            readonly resource: {
              readonly id: string;
              readonly name: string;
            };
          }>;
          readonly from: any;
          readonly id: string;
          readonly involvedLocations: ReadonlyArray<{
            readonly name: string;
            readonly uniqueId: string;
          }>;
          readonly marketplaceBooking: {
            readonly paymentStatus: {
              readonly name: string;
              readonly type: PaymentStatus;
            };
          } | null | undefined;
          readonly until: any;
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
    readonly marketplaceBooking: {
      readonly invoiceUrl: string | null | undefined;
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
      readonly totalAmountToDisplay: string;
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
        readonly totalAmountToDisplay: string;
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
  } | null | undefined;
  readonly marketplaceBookingSubscriptionCancellationModes: ReadonlyArray<{
    readonly name: string;
    readonly type: MarketplaceBookingSubscriptionCancellationMode;
  }>;
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly organizationBookingPermissions: {
    readonly canModifyPaymentMethod: boolean;
    readonly canViewBookings: boolean;
  };
};
export type pageOrganizationSubscriptionDetail_rootQuery = {
  response: pageOrganizationSubscriptionDetail_rootQuery$data;
  variables: pageOrganizationSubscriptionDetail_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingAfter"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "linkedBookingAfter"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "subscriptionId"
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v6 = [
  (v4/*:: as any*/),
  (v5/*:: as any*/)
],
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingSubscriptionCancellationModes",
  "plural": true,
  "selections": (v6/*:: as any*/),
  "storageKey": null
},
v8 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v9 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "organizationCustomDomain",
      "variableName": "organizationCustomDomain"
    }
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
v10 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "subscriptionId"
  }
],
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "occurredAt",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v14 = {
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
            (v11/*:: as any*/),
            (v4/*:: as any*/),
            (v5/*:: as any*/),
            (v12/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "previousPaymentStatus",
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
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "creditQuantity",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "remainingCreditQuantity",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "amount",
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
            (v13/*:: as any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "storageKey": "history(first:100)"
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancellationPolicyOverridden",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancellationOverrideReason",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startedAt",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "nextRenewalAt",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancelledAt",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "autoRenew",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancelAtPeriodEnd",
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceRefundDetails",
  "kind": "LinkedField",
  "name": "refund",
  "plural": false,
  "selections": [
    (v11/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currency",
      "plural": false,
      "selections": (v6/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceRefundStatusDetails",
      "kind": "LinkedField",
      "name": "status",
      "plural": false,
      "selections": (v6/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "requestedAt",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "lastProcessedAt",
      "storageKey": null
    },
    (v22/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "refundPercentage",
      "storageKey": null
    },
    (v23/*:: as any*/),
    (v13/*:: as any*/),
    (v24/*:: as any*/),
    (v25/*:: as any*/),
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
        (v11/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceRefundEventTypeDetails",
          "kind": "LinkedField",
          "name": "eventType",
          "plural": false,
          "selections": (v6/*:: as any*/),
          "storageKey": null
        },
        (v12/*:: as any*/),
        (v22/*:: as any*/),
        (v23/*:: as any*/),
        (v13/*:: as any*/),
        (v24/*:: as any*/),
        (v25/*:: as any*/),
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
v27 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v6/*:: as any*/),
  "storageKey": null
},
v28 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "involvedCustomers",
  "plural": true,
  "selections": [
    (v11/*:: as any*/),
    (v5/*:: as any*/),
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
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "photoUrl",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v29 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v30 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalAmountToDisplay",
  "storageKey": null
},
v31 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v32 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v6/*:: as any*/),
  "storageKey": null
},
v33 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v6/*:: as any*/),
  "storageKey": null
},
v34 = {
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
v35 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startDate",
  "storageKey": null
},
v36 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "endDate",
  "storageKey": null
},
v37 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBookings",
  "plural": true,
  "selections": [
    (v11/*:: as any*/),
    (v35/*:: as any*/),
    (v36/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingDetails",
      "kind": "LinkedField",
      "name": "marketplaceBooking",
      "plural": false,
      "selections": [
        (v11/*:: as any*/),
        (v31/*:: as any*/),
        (v29/*:: as any*/),
        (v30/*:: as any*/),
        (v32/*:: as any*/),
        (v33/*:: as any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v38 = {
  "kind": "Literal",
  "name": "first",
  "value": 50
},
v39 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v40 = {
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
v41 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v42 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "after",
      "variableName": "bookingAfter"
    },
    (v38/*:: as any*/)
  ],
  "concreteType": "ConnectionOfMarketplaceBookingInstanceEdge",
  "kind": "LinkedField",
  "name": "bookingInstances",
  "plural": false,
  "selections": [
    (v39/*:: as any*/),
    (v40/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingInstanceEdge",
      "kind": "LinkedField",
      "name": "edges",
      "plural": true,
      "selections": [
        (v41/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "RecurringBookingDetails",
          "kind": "LinkedField",
          "name": "node",
          "plural": false,
          "selections": [
            (v11/*:: as any*/),
            (v35/*:: as any*/),
            (v36/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "MarketplaceBookingDetails",
              "kind": "LinkedField",
              "name": "marketplaceBooking",
              "plural": false,
              "selections": [
                (v11/*:: as any*/),
                (v29/*:: as any*/),
                (v32/*:: as any*/)
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
},
v43 = [
  {
    "kind": "Variable",
    "name": "after",
    "variableName": "linkedBookingAfter"
  },
  (v38/*:: as any*/)
],
v44 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v45 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v46 = {
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
        (v11/*:: as any*/),
        (v5/*:: as any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v47 = {
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
    (v5/*:: as any*/)
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v3/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationSubscriptionDetail_rootQuery",
    "selections": [
      (v7/*:: as any*/),
      {
        "alias": null,
        "args": (v8/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v5/*:: as any*/)
        ],
        "storageKey": null
      },
      (v9/*:: as any*/),
      {
        "alias": null,
        "args": (v10/*:: as any*/),
        "concreteType": "MarketplaceBookingSubscriptionDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscription",
        "plural": false,
        "selections": [
          (v11/*:: as any*/),
          (v14/*:: as any*/),
          (v15/*:: as any*/),
          (v16/*:: as any*/),
          (v17/*:: as any*/),
          (v18/*:: as any*/),
          (v19/*:: as any*/),
          (v20/*:: as any*/),
          (v21/*:: as any*/),
          (v26/*:: as any*/),
          (v27/*:: as any*/),
          (v28/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v29/*:: as any*/),
              (v30/*:: as any*/),
              (v31/*:: as any*/),
              (v32/*:: as any*/),
              (v33/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductVersionDetails",
                "kind": "LinkedField",
                "name": "productVersion",
                "plural": false,
                "selections": [
                  (v34/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v37/*:: as any*/),
          (v42/*:: as any*/),
          {
            "alias": null,
            "args": (v43/*:: as any*/),
            "concreteType": "ConnectionOfBookingEdge",
            "kind": "LinkedField",
            "name": "linkedBookings",
            "plural": false,
            "selections": [
              (v39/*:: as any*/),
              (v40/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  (v41/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v11/*:: as any*/),
                      (v44/*:: as any*/),
                      (v45/*:: as any*/),
                      (v46/*:: as any*/),
                      (v47/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "MarketplaceBookingDetails",
                        "kind": "LinkedField",
                        "name": "marketplaceBooking",
                        "plural": false,
                        "selections": [
                          (v32/*:: as any*/)
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
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v0/*:: as any*/),
      (v1/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationSubscriptionDetail_rootQuery",
    "selections": [
      (v7/*:: as any*/),
      {
        "alias": null,
        "args": (v8/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v5/*:: as any*/),
          (v11/*:: as any*/)
        ],
        "storageKey": null
      },
      (v9/*:: as any*/),
      {
        "alias": null,
        "args": (v10/*:: as any*/),
        "concreteType": "MarketplaceBookingSubscriptionDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscription",
        "plural": false,
        "selections": [
          (v11/*:: as any*/),
          (v14/*:: as any*/),
          (v15/*:: as any*/),
          (v16/*:: as any*/),
          (v17/*:: as any*/),
          (v18/*:: as any*/),
          (v19/*:: as any*/),
          (v20/*:: as any*/),
          (v21/*:: as any*/),
          (v26/*:: as any*/),
          (v27/*:: as any*/),
          (v28/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v29/*:: as any*/),
              (v30/*:: as any*/),
              (v31/*:: as any*/),
              (v32/*:: as any*/),
              (v33/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductVersionDetails",
                "kind": "LinkedField",
                "name": "productVersion",
                "plural": false,
                "selections": [
                  (v34/*:: as any*/),
                  (v11/*:: as any*/)
                ],
                "storageKey": null
              },
              (v11/*:: as any*/)
            ],
            "storageKey": null
          },
          (v37/*:: as any*/),
          (v42/*:: as any*/),
          {
            "alias": null,
            "args": (v43/*:: as any*/),
            "concreteType": "ConnectionOfBookingEdge",
            "kind": "LinkedField",
            "name": "linkedBookings",
            "plural": false,
            "selections": [
              (v39/*:: as any*/),
              (v40/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  (v41/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v11/*:: as any*/),
                      (v44/*:: as any*/),
                      (v45/*:: as any*/),
                      (v46/*:: as any*/),
                      (v47/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "MarketplaceBookingDetails",
                        "kind": "LinkedField",
                        "name": "marketplaceBooking",
                        "plural": false,
                        "selections": [
                          (v32/*:: as any*/),
                          (v11/*:: as any*/)
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
    ]
  },
  "params": {
    "cacheID": "6fed84b172041675279f273eebfe7a40",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptionDetail_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationSubscriptionDetail_rootQuery(\n  $organizationCustomDomain: String!\n  $subscriptionId: String!\n  $bookingAfter: String\n  $linkedBookingAfter: String\n) {\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    id\n  }\n  organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {\n    canViewBookings\n    canModifyPaymentMethod\n  }\n  marketplaceBookingSubscription(id: $subscriptionId) {\n    id\n    history(first: 100) {\n      edges {\n        node {\n          id\n          type\n          name\n          occurredAt\n          previousPaymentStatus\n          paymentStatus\n          previousRefundStatus\n          refundStatus\n          refundId\n          creditQuantity\n          remainingCreditQuantity\n          amount\n          currency\n          cancellationRequestedAt\n          cancellationEffectiveAt\n          reason\n        }\n      }\n    }\n    cancellationPolicyOverridden\n    cancellationOverrideReason\n    startedAt\n    nextRenewalAt\n    cancelledAt\n    autoRenew\n    cancelAtPeriodEnd\n    refund {\n      id\n      currency {\n        type\n        name\n      }\n      status {\n        type\n        name\n      }\n      requestedAt\n      lastProcessedAt\n      refundAmount\n      refundPercentage\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n      requestedByCustomerName\n      canProcessInXero\n      xeroProcessingBlockedReason\n      events {\n        id\n        eventType {\n          type\n          name\n        }\n        occurredAt\n        refundAmount\n        currencyToDisplay\n        reason\n        lastError\n        externalRefundNumber\n        actorName\n      }\n    }\n    status {\n      type\n      name\n    }\n    involvedCustomers {\n      id\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    marketplaceBooking {\n      invoiceUrl\n      totalAmountToDisplay\n      quantity\n      paymentStatus {\n        type\n        name\n      }\n      paymentMethod {\n        type\n        name\n      }\n      productVersion {\n        listingMetadata {\n          title\n        }\n        id\n      }\n      id\n    }\n    recurringBookings {\n      id\n      startDate\n      endDate\n      marketplaceBooking {\n        id\n        quantity\n        invoiceUrl\n        totalAmountToDisplay\n        paymentStatus {\n          type\n          name\n        }\n        paymentMethod {\n          type\n          name\n        }\n      }\n    }\n    bookingInstances(after: $bookingAfter, first: 50) {\n      totalCount\n      pageInfo {\n        hasNextPage\n        hasPreviousPage\n        startCursor\n        endCursor\n      }\n      edges {\n        cursor\n        node {\n          id\n          startDate\n          endDate\n          marketplaceBooking {\n            id\n            invoiceUrl\n            paymentStatus {\n              type\n              name\n            }\n          }\n        }\n      }\n    }\n    linkedBookings(after: $linkedBookingAfter, first: 50) {\n      totalCount\n      pageInfo {\n        hasNextPage\n        hasPreviousPage\n        startCursor\n        endCursor\n      }\n      edges {\n        cursor\n        node {\n          id\n          from\n          until\n          bookingResources {\n            resource {\n              id\n              name\n            }\n          }\n          involvedLocations {\n            uniqueId\n            name\n          }\n          marketplaceBooking {\n            paymentStatus {\n              type\n              name\n            }\n            id\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "625e7f0ccca636700521cc39bcc423a0";

export default node;
