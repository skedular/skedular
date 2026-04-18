/**
 * @generated SignedSource<<78d4a2528fd438c68c57e0b549c7a101>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type pageOrganizationSubscriptionDetail_rootQuery$variables = {
  organizationCustomDomain: string;
  subscriptionId: string;
};
export type pageOrganizationSubscriptionDetail_rootQuery$data = {
  readonly marketplaceBookingSubscription: {
    readonly autoRenew: boolean;
    readonly cancelAtPeriodEnd: boolean;
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
          readonly type: string;
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
        readonly type: string;
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
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "subscriptionId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v1/*: any*/)
],
v3 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingSubscriptionCancellationModes",
  "plural": true,
  "selections": (v2/*: any*/),
  "storageKey": null
},
v4 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v5 = {
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
v6 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "subscriptionId"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startedAt",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "nextRenewalAt",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "autoRenew",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancelAtPeriodEnd",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceRefundDetails",
  "kind": "LinkedField",
  "name": "refund",
  "plural": false,
  "selections": [
    (v7/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currency",
      "plural": false,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceRefundStatusDetails",
      "kind": "LinkedField",
      "name": "status",
      "plural": false,
      "selections": (v2/*: any*/),
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
    (v12/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "refundPercentage",
      "storageKey": null
    },
    (v13/*: any*/),
    (v14/*: any*/),
    (v15/*: any*/),
    (v16/*: any*/),
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
        (v7/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceRefundEventTypeDetails",
          "kind": "LinkedField",
          "name": "eventType",
          "plural": false,
          "selections": (v2/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "occurredAt",
          "storageKey": null
        },
        (v12/*: any*/),
        (v13/*: any*/),
        (v14/*: any*/),
        (v15/*: any*/),
        (v16/*: any*/),
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
v18 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v2/*: any*/),
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "involvedCustomers",
  "plural": true,
  "selections": [
    (v7/*: any*/),
    (v1/*: any*/),
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
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v2/*: any*/),
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v2/*: any*/),
  "storageKey": null
},
v23 = {
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
v24 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBookings",
  "plural": true,
  "selections": [
    (v7/*: any*/),
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
        (v7/*: any*/),
        (v20/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "invoiceUrl",
          "storageKey": null
        },
        (v21/*: any*/),
        (v22/*: any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationSubscriptionDetail_rootQuery",
    "selections": [
      (v3/*: any*/),
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v1/*: any*/)
        ],
        "storageKey": null
      },
      (v5/*: any*/),
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "MarketplaceBookingSubscriptionDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscription",
        "plural": false,
        "selections": [
          (v7/*: any*/),
          (v8/*: any*/),
          (v9/*: any*/),
          (v10/*: any*/),
          (v11/*: any*/),
          (v17/*: any*/),
          (v18/*: any*/),
          (v19/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v20/*: any*/),
              (v21/*: any*/),
              (v22/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductVersionDetails",
                "kind": "LinkedField",
                "name": "productVersion",
                "plural": false,
                "selections": [
                  (v23/*: any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v24/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageOrganizationSubscriptionDetail_rootQuery",
    "selections": [
      (v3/*: any*/),
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          (v7/*: any*/)
        ],
        "storageKey": null
      },
      (v5/*: any*/),
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "MarketplaceBookingSubscriptionDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscription",
        "plural": false,
        "selections": [
          (v7/*: any*/),
          (v8/*: any*/),
          (v9/*: any*/),
          (v10/*: any*/),
          (v11/*: any*/),
          (v17/*: any*/),
          (v18/*: any*/),
          (v19/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v20/*: any*/),
              (v21/*: any*/),
              (v22/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductVersionDetails",
                "kind": "LinkedField",
                "name": "productVersion",
                "plural": false,
                "selections": [
                  (v23/*: any*/),
                  (v7/*: any*/)
                ],
                "storageKey": null
              },
              (v7/*: any*/)
            ],
            "storageKey": null
          },
          (v24/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "d15fce96131a3eac3e0c2d4757235e3b",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptionDetail_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationSubscriptionDetail_rootQuery(\n  $organizationCustomDomain: String!\n  $subscriptionId: String!\n) {\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    id\n  }\n  organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {\n    canViewBookings\n    canModifyPaymentMethod\n  }\n  marketplaceBookingSubscription(id: $subscriptionId) {\n    id\n    startedAt\n    nextRenewalAt\n    autoRenew\n    cancelAtPeriodEnd\n    refund {\n      id\n      currency {\n        type\n        name\n      }\n      status {\n        type\n        name\n      }\n      requestedAt\n      lastProcessedAt\n      refundAmount\n      refundPercentage\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n      requestedByCustomerName\n      canProcessInXero\n      xeroProcessingBlockedReason\n      events {\n        id\n        eventType {\n          type\n          name\n        }\n        occurredAt\n        refundAmount\n        currencyToDisplay\n        reason\n        lastError\n        externalRefundNumber\n        actorName\n      }\n    }\n    status {\n      type\n      name\n    }\n    involvedCustomers {\n      id\n      name\n      givenName\n      middleName\n      familyName\n    }\n    marketplaceBooking {\n      quantity\n      paymentStatus {\n        type\n        name\n      }\n      paymentMethod {\n        type\n        name\n      }\n      productVersion {\n        listingMetadata {\n          title\n        }\n        id\n      }\n      id\n    }\n    recurringBookings {\n      id\n      startDate\n      endDate\n      marketplaceBooking {\n        id\n        quantity\n        invoiceUrl\n        paymentStatus {\n          type\n          name\n        }\n        paymentMethod {\n          type\n          name\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "56464980d7395572469ba50bb12d0d8f";

export default node;
