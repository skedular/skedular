/**
 * @generated SignedSource<<c153ad75b63c12b82912aade90d0315d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingChannel = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type customerBookingsHub_rootQuery$variables = {
  today: any;
};
export type customerBookingsHub_rootQuery$data = {
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
        readonly involvedOrganizations: ReadonlyArray<{
          readonly customDomain: string | null | undefined;
          readonly id: string;
          readonly name: string;
        }>;
        readonly involvedTeams: ReadonlyArray<{
          readonly id: string;
          readonly name: string;
        }>;
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
  readonly recentBookings: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly bookingResources: ReadonlyArray<{
          readonly resource: {
            readonly id: string;
            readonly name: string;
          };
        }>;
        readonly channel: {
          readonly channel: BookingChannel;
          readonly name: string;
        };
        readonly from: any;
        readonly id: string;
        readonly involvedLocations: ReadonlyArray<{
          readonly name: string;
        }>;
        readonly involvedOrganizations: ReadonlyArray<{
          readonly customDomain: string | null | undefined;
          readonly id: string;
          readonly name: string;
        }>;
        readonly involvedTeams: ReadonlyArray<{
          readonly id: string;
          readonly name: string;
        }>;
        readonly marketplaceBooking: {
          readonly paymentStatus: {
            readonly name: string;
            readonly type: PaymentStatus;
          };
          readonly quantity: number;
        } | null | undefined;
        readonly recurringBooking: {
          readonly frequency: {
            readonly name: string;
          };
          readonly id: string;
          readonly marketplaceBooking: {
            readonly id: string;
          } | null | undefined;
        } | null | undefined;
        readonly until: any;
      };
    }>;
    readonly totalCount: number;
  };
  readonly upcomingBookings: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly bookingResources: ReadonlyArray<{
          readonly resource: {
            readonly id: string;
            readonly name: string;
          };
        }>;
        readonly channel: {
          readonly channel: BookingChannel;
          readonly name: string;
        };
        readonly from: any;
        readonly id: string;
        readonly involvedLocations: ReadonlyArray<{
          readonly name: string;
        }>;
        readonly involvedOrganizations: ReadonlyArray<{
          readonly customDomain: string | null | undefined;
          readonly id: string;
          readonly name: string;
        }>;
        readonly involvedTeams: ReadonlyArray<{
          readonly id: string;
          readonly name: string;
        }>;
        readonly marketplaceBooking: {
          readonly paymentStatus: {
            readonly name: string;
            readonly type: PaymentStatus;
          };
          readonly quantity: number;
        } | null | undefined;
        readonly recurringBooking: {
          readonly frequency: {
            readonly name: string;
          };
          readonly id: string;
          readonly marketplaceBooking: {
            readonly id: string;
          } | null | undefined;
        } | null | undefined;
        readonly until: any;
      };
    }>;
    readonly totalCount: number;
  };
};
export type customerBookingsHub_rootQuery = {
  response: customerBookingsHub_rootQuery$data;
  variables: customerBookingsHub_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "today"
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
v4 = {
  "kind": "Literal",
  "name": "first",
  "value": 48
},
v5 = {
  "kind": "Literal",
  "name": "includeMineOnly",
  "value": true
},
v6 = [
  (v4/*:: as any*/),
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "ASCENDING",
        "field": "FROM"
      }
    ]
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "fromGte",
        "variableName": "today"
      },
      (v5/*:: as any*/)
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
  "name": "from",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingChannelDetails",
  "kind": "LinkedField",
  "name": "channel",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "channel",
      "storageKey": null
    },
    (v1/*:: as any*/)
  ],
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "involvedOrganizations",
  "plural": true,
  "selections": [
    (v8/*:: as any*/),
    (v1/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "customDomain",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v13 = [
  (v1/*:: as any*/)
],
v14 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_LocationDetails",
  "kind": "LinkedField",
  "name": "involvedLocations",
  "plural": true,
  "selections": (v13/*:: as any*/),
  "storageKey": null
},
v15 = [
  (v8/*:: as any*/),
  (v1/*:: as any*/)
],
v16 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamDetails",
  "kind": "LinkedField",
  "name": "involvedTeams",
  "plural": true,
  "selections": (v15/*:: as any*/),
  "storageKey": null
},
v17 = {
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
      "selections": (v15/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v2/*:: as any*/),
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBooking",
  "plural": false,
  "selections": [
    (v8/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingFrequencyDetails",
      "kind": "LinkedField",
      "name": "frequency",
      "plural": false,
      "selections": (v13/*:: as any*/),
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
        (v8/*:: as any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v21 = [
  (v7/*:: as any*/),
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
          (v8/*:: as any*/),
          (v9/*:: as any*/),
          (v10/*:: as any*/),
          (v11/*:: as any*/),
          (v12/*:: as any*/),
          (v14/*:: as any*/),
          (v16/*:: as any*/),
          (v17/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v18/*:: as any*/),
              (v19/*:: as any*/)
            ],
            "storageKey": null
          },
          (v20/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
],
v22 = [
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
        "direction": "DESCENDING",
        "field": "FROM"
      }
    ]
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "fromLt",
        "variableName": "today"
      },
      (v5/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v23 = [
  (v4/*:: as any*/),
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
    "kind": "Literal",
    "name": "where",
    "value": {
      "includeMineOnly": true
    }
  }
],
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startedAt",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "nextRenewalAt",
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "autoRenew",
  "storageKey": null
},
v27 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancelAtPeriodEnd",
  "storageKey": null
},
v28 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v2/*:: as any*/),
  "storageKey": null
},
v29 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v13/*:: as any*/),
  "storageKey": null
},
v30 = {
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
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "subTitle",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v31 = {
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
},
v32 = [
  (v7/*:: as any*/),
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
          (v8/*:: as any*/),
          (v9/*:: as any*/),
          (v10/*:: as any*/),
          (v11/*:: as any*/),
          (v12/*:: as any*/),
          (v14/*:: as any*/),
          (v16/*:: as any*/),
          (v17/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v18/*:: as any*/),
              (v19/*:: as any*/),
              (v8/*:: as any*/)
            ],
            "storageKey": null
          },
          (v20/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "customerBookingsHub_rootQuery",
    "selections": [
      (v3/*:: as any*/),
      {
        "alias": "upcomingBookings",
        "args": (v6/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v21/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": "recentBookings",
        "args": (v22/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v21/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v23/*:: as any*/),
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
                  (v24/*:: as any*/),
                  (v25/*:: as any*/),
                  (v26/*:: as any*/),
                  (v27/*:: as any*/),
                  (v28/*:: as any*/),
                  (v12/*:: as any*/),
                  (v16/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v18/*:: as any*/),
                      (v19/*:: as any*/),
                      (v29/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v30/*:: as any*/)
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  (v31/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": "marketplaceBookingSubscriptions(first:48,orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"NEXT_RENEWAL_AT\"}],where:{\"includeMineOnly\":true})"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "customerBookingsHub_rootQuery",
    "selections": [
      (v3/*:: as any*/),
      {
        "alias": "upcomingBookings",
        "args": (v6/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v32/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": "recentBookings",
        "args": (v22/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v32/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v23/*:: as any*/),
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
                  (v24/*:: as any*/),
                  (v25/*:: as any*/),
                  (v26/*:: as any*/),
                  (v27/*:: as any*/),
                  (v28/*:: as any*/),
                  (v12/*:: as any*/),
                  (v16/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v18/*:: as any*/),
                      (v19/*:: as any*/),
                      (v29/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v30/*:: as any*/),
                          (v8/*:: as any*/)
                        ],
                        "storageKey": null
                      },
                      (v8/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  (v31/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": "marketplaceBookingSubscriptions(first:48,orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"NEXT_RENEWAL_AT\"}],where:{\"includeMineOnly\":true})"
      }
    ]
  },
  "params": {
    "cacheID": "4a1a4375fcf4e3f9dee4e3d4abd1b67c",
    "id": null,
    "metadata": {},
    "name": "customerBookingsHub_rootQuery",
    "operationKind": "query",
    "text": "query customerBookingsHub_rootQuery(\n  $today: DateTime!\n) {\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  upcomingBookings: bookings(first: 48, where: {includeMineOnly: true, fromGte: $today}, orderBy: [{field: FROM, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        until\n        channel {\n          channel\n          name\n        }\n        involvedOrganizations {\n          id\n          name\n          customDomain\n        }\n        involvedLocations {\n          name\n        }\n        involvedTeams {\n          id\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n          }\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n        recurringBooking {\n          id\n          frequency {\n            name\n          }\n          marketplaceBooking {\n            id\n          }\n        }\n      }\n    }\n  }\n  recentBookings: bookings(first: 24, where: {includeMineOnly: true, fromLt: $today}, orderBy: [{field: FROM, direction: DESCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        until\n        channel {\n          channel\n          name\n        }\n        involvedOrganizations {\n          id\n          name\n          customDomain\n        }\n        involvedLocations {\n          name\n        }\n        involvedTeams {\n          id\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n          }\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n        recurringBooking {\n          id\n          frequency {\n            name\n          }\n          marketplaceBooking {\n            id\n          }\n        }\n      }\n    }\n  }\n  marketplaceBookingSubscriptions(first: 48, where: {includeMineOnly: true}, orderBy: [{field: NEXT_RENEWAL_AT, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        startedAt\n        nextRenewalAt\n        autoRenew\n        cancelAtPeriodEnd\n        status {\n          type\n          name\n        }\n        involvedOrganizations {\n          id\n          name\n          customDomain\n        }\n        involvedTeams {\n          id\n          name\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          paymentMethod {\n            name\n          }\n          productVersion {\n            listingMetadata {\n              title\n              subTitle\n            }\n            id\n          }\n          id\n        }\n        recurringBookings {\n          id\n          startDate\n          endDate\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cb7211bd8bac2b2cf484392419cd3fcc";

export default node;
