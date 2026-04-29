/**
 * @generated SignedSource<<ba51b0596c97d800e8583f89d3a73e53>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type Currency = "NZD" | "USD" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type pageOrganizationSubscriptions_rootQuery$variables = {
  organizationCustomDomain: string;
  paymentStatuses?: ReadonlyArray<PaymentStatus> | null | undefined;
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
      };
    }>;
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
  "name": "statuses"
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v3/*: any*/)
],
v5 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingSubscriptionCancellationModes",
  "plural": true,
  "selections": (v4/*: any*/),
  "storageKey": null
},
v6 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v7 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v8 = {
  "alias": null,
  "args": [
    (v7/*: any*/)
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
v9 = [
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
      (v7/*: any*/),
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
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
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
  "name": "startedAt",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "nextRenewalAt",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "autoRenew",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancelAtPeriodEnd",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceRefundDetails",
  "kind": "LinkedField",
  "name": "refund",
  "plural": false,
  "selections": [
    (v11/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currency",
      "plural": false,
      "selections": (v4/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceRefundStatusDetails",
      "kind": "LinkedField",
      "name": "status",
      "plural": false,
      "selections": (v4/*: any*/),
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
    (v16/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "refundPercentage",
      "storageKey": null
    },
    (v17/*: any*/),
    (v18/*: any*/),
    (v19/*: any*/),
    (v20/*: any*/),
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
        (v11/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceRefundEventTypeDetails",
          "kind": "LinkedField",
          "name": "eventType",
          "plural": false,
          "selections": (v4/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "occurredAt",
          "storageKey": null
        },
        (v16/*: any*/),
        (v17/*: any*/),
        (v18/*: any*/),
        (v19/*: any*/),
        (v20/*: any*/),
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
v22 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v4/*: any*/),
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "involvedCustomers",
  "plural": true,
  "selections": [
    (v11/*: any*/),
    (v3/*: any*/),
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
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v4/*: any*/),
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v4/*: any*/),
  "storageKey": null
},
v27 = {
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
v28 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBookings",
  "plural": true,
  "selections": [
    (v11/*: any*/),
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
        (v11/*: any*/),
        (v24/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "invoiceUrl",
          "storageKey": null
        },
        (v25/*: any*/),
        (v26/*: any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
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
      (v5/*: any*/),
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*: any*/)
        ],
        "storageKey": null
      },
      (v8/*: any*/),
      {
        "alias": null,
        "args": (v9/*: any*/),
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
          (v10/*: any*/),
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
                  (v11/*: any*/),
                  (v12/*: any*/),
                  (v13/*: any*/),
                  (v14/*: any*/),
                  (v15/*: any*/),
                  (v21/*: any*/),
                  (v22/*: any*/),
                  (v23/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v24/*: any*/),
                      (v25/*: any*/),
                      (v26/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v27/*: any*/)
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  (v28/*: any*/)
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
      (v0/*: any*/),
      (v2/*: any*/),
      (v1/*: any*/)
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
        "selections": (v4/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceBookingPaymentStatusDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingPaymentStatuses",
        "plural": true,
        "selections": (v4/*: any*/),
        "storageKey": null
      },
      (v5/*: any*/),
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          (v11/*: any*/)
        ],
        "storageKey": null
      },
      (v8/*: any*/),
      {
        "alias": null,
        "args": (v9/*: any*/),
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
          (v10/*: any*/),
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
                  (v11/*: any*/),
                  (v12/*: any*/),
                  (v13/*: any*/),
                  (v14/*: any*/),
                  (v15/*: any*/),
                  (v21/*: any*/),
                  (v22/*: any*/),
                  (v23/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v24/*: any*/),
                      (v25/*: any*/),
                      (v26/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v27/*: any*/),
                          (v11/*: any*/)
                        ],
                        "storageKey": null
                      },
                      (v11/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v28/*: any*/)
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
    "cacheID": "99efc0575344de1192eec98e49abdbde",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptions_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationSubscriptions_rootQuery(\n  $organizationCustomDomain: String!\n  $statuses: [MarketplaceBookingSubscriptionStatus!]\n  $paymentStatuses: [PaymentStatus!]\n) {\n  ...multipleChoicesMarketplaceBookingSubscriptionStatuses_query\n  ...multipleChoicesMarketplaceBookingPaymentStatuses_query\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    id\n  }\n  organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {\n    canViewBookings\n    canModifyPaymentMethod\n  }\n  marketplaceBookingSubscriptions(first: 50, where: {organizationCustomDomain: $organizationCustomDomain, statuses: $statuses, paymentStatuses: $paymentStatuses}, orderBy: [{field: NEXT_RENEWAL_AT, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        startedAt\n        nextRenewalAt\n        autoRenew\n        cancelAtPeriodEnd\n        refund {\n          id\n          currency {\n            type\n            name\n          }\n          status {\n            type\n            name\n          }\n          requestedAt\n          lastProcessedAt\n          refundAmount\n          refundPercentage\n          currencyToDisplay\n          reason\n          lastError\n          externalRefundNumber\n          requestedByCustomerName\n          canProcessInXero\n          xeroProcessingBlockedReason\n          events {\n            id\n            eventType {\n              type\n              name\n            }\n            occurredAt\n            refundAmount\n            currencyToDisplay\n            reason\n            lastError\n            externalRefundNumber\n            actorName\n          }\n        }\n        status {\n          type\n          name\n        }\n        involvedCustomers {\n          id\n          name\n          givenName\n          middleName\n          familyName\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          paymentMethod {\n            type\n            name\n          }\n          productVersion {\n            listingMetadata {\n              title\n            }\n            id\n          }\n          id\n        }\n        recurringBookings {\n          id\n          startDate\n          endDate\n          marketplaceBooking {\n            id\n            quantity\n            invoiceUrl\n            paymentStatus {\n              type\n              name\n            }\n            paymentMethod {\n              type\n              name\n            }\n          }\n        }\n      }\n    }\n  }\n}\n\nfragment multipleChoicesMarketplaceBookingPaymentStatuses_query on Query {\n  marketplaceBookingPaymentStatuses {\n    type\n    name\n  }\n}\n\nfragment multipleChoicesMarketplaceBookingSubscriptionStatuses_query on Query {\n  marketplaceBookingSubscriptionStatuses {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "94b99addc17ddb78656e52d57a41676c";

export default node;
