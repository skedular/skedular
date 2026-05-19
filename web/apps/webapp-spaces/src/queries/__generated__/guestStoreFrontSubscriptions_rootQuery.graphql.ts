/**
 * @generated SignedSource<<d315e9d8bb4c06d9dd03b9cf2b524389>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type guestStoreFrontSubscriptions_rootQuery$variables = {
  organizationCustomDomain: string;
};
export type guestStoreFrontSubscriptions_rootQuery$data = {
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
        readonly marketplaceBooking: {
          readonly paymentMethod: {
            readonly name: string;
          };
          readonly paymentStatus: {
            readonly name: string;
            readonly type: PaymentStatus;
          };
          readonly productVersion: {
            readonly listingMetadata: {
              readonly subTitle: string | null | undefined;
              readonly title: string | null | undefined;
            };
          };
          readonly quantity: number;
        };
        readonly nextRenewalAt: any | null | undefined;
        readonly recurringBookings: ReadonlyArray<{
          readonly endDate: any | null | undefined;
          readonly id: string;
          readonly startDate: any;
        }>;
        readonly startedAt: any;
        readonly status: {
          readonly name: string;
          readonly type: MarketplaceBookingSubscriptionStatus;
        };
      };
    }>;
    readonly totalCount: number;
  };
  readonly organizationPublic: {
    readonly marketplaceListingMetadata: {
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly name: string;
  } | null | undefined;
};
export type guestStoreFrontSubscriptions_rootQuery = {
  response: guestStoreFrontSubscriptions_rootQuery$data;
  variables: guestStoreFrontSubscriptions_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
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
  (v1/*:: as any*/)
],
v3 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingSubscriptionCancellationModes",
  "plural": true,
  "selections": (v2/*:: as any*/),
  "storageKey": null
},
v4 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "title",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "subTitle",
    "storageKey": null
  }
],
v5 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "customDomain",
      "variableName": "organizationCustomDomain"
    }
  ],
  "concreteType": "OrganizationPublicDetails",
  "kind": "LinkedField",
  "name": "organizationPublic",
  "plural": false,
  "selections": [
    (v1/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "ListingMetadata",
      "kind": "LinkedField",
      "name": "marketplaceListingMetadata",
      "plural": false,
      "selections": (v4/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v6 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 24
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
      {
        "kind": "Literal",
        "name": "includeMineOnly",
        "value": true
      },
      {
        "kind": "Variable",
        "name": "organizationCustomDomain",
        "variableName": "organizationCustomDomain"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startedAt",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "nextRenewalAt",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "autoRenew",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancelAtPeriodEnd",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v2/*:: as any*/),
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v2/*:: as any*/),
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": [
    (v1/*:: as any*/)
  ],
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "listingMetadata",
  "plural": false,
  "selections": (v4/*:: as any*/),
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBookings",
  "plural": true,
  "selections": [
    (v8/*:: as any*/),
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
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "guestStoreFrontSubscriptions_rootQuery",
    "selections": [
      (v3/*:: as any*/),
      (v5/*:: as any*/),
      {
        "alias": null,
        "args": (v6/*:: as any*/),
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
          (v7/*:: as any*/),
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
                  (v8/*:: as any*/),
                  (v9/*:: as any*/),
                  (v10/*:: as any*/),
                  (v11/*:: as any*/),
                  (v12/*:: as any*/),
                  (v13/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v14/*:: as any*/),
                      (v15/*:: as any*/),
                      (v16/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v17/*:: as any*/)
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  (v18/*:: as any*/)
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "guestStoreFrontSubscriptions_rootQuery",
    "selections": [
      (v3/*:: as any*/),
      (v5/*:: as any*/),
      {
        "alias": null,
        "args": (v6/*:: as any*/),
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
          (v7/*:: as any*/),
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
                  (v8/*:: as any*/),
                  (v9/*:: as any*/),
                  (v10/*:: as any*/),
                  (v11/*:: as any*/),
                  (v12/*:: as any*/),
                  (v13/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v14/*:: as any*/),
                      (v15/*:: as any*/),
                      (v16/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v17/*:: as any*/),
                          (v8/*:: as any*/)
                        ],
                        "storageKey": null
                      },
                      (v8/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  (v18/*:: as any*/)
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
    "cacheID": "7c5ba93d0b23d17e6439632dde592802",
    "id": null,
    "metadata": {},
    "name": "guestStoreFrontSubscriptions_rootQuery",
    "operationKind": "query",
    "text": "query guestStoreFrontSubscriptions_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n    marketplaceListingMetadata {\n      title\n      subTitle\n    }\n  }\n  marketplaceBookingSubscriptions(first: 24, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain}, orderBy: [{field: NEXT_RENEWAL_AT, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        startedAt\n        nextRenewalAt\n        autoRenew\n        cancelAtPeriodEnd\n        status {\n          type\n          name\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          paymentMethod {\n            name\n          }\n          productVersion {\n            listingMetadata {\n              title\n              subTitle\n            }\n            id\n          }\n          id\n        }\n        recurringBookings {\n          id\n          startDate\n          endDate\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3a602b4267675a646c5e5afbdb4b87cb";

export default node;
