/**
 * @generated SignedSource<<afcc9684b860f364e927767a4fc50625>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type marketplaceProductSubscriptionDetails_relatedBookingsQuery$variables = {
  organizationCustomDomain: string;
  recurringBookingIds?: ReadonlyArray<string> | null | undefined;
  relatedBookingsFirst: number;
  today: any;
};
export type marketplaceProductSubscriptionDetails_relatedBookingsQuery$data = {
  readonly bookings: {
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
        readonly involvedLocations: ReadonlyArray<{
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
          readonly id: string;
        } | null | undefined;
        readonly until: any;
      };
    }>;
    readonly totalCount: number;
  };
};
export type marketplaceProductSubscriptionDetails_relatedBookingsQuery = {
  response: marketplaceProductSubscriptionDetails_relatedBookingsQuery$data;
  variables: marketplaceProductSubscriptionDetails_relatedBookingsQuery$variables;
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
    "name": "recurringBookingIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "relatedBookingsFirst"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "today"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "first",
    "variableName": "relatedBookingsFirst"
  },
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
        "kind": "Literal",
        "name": "channel",
        "value": "MARKETPLACE"
      },
      {
        "kind": "Variable",
        "name": "fromGte",
        "variableName": "today"
      },
      {
        "kind": "Literal",
        "name": "includeMineOnly",
        "value": true
      },
      {
        "kind": "Variable",
        "name": "organizationCustomDomain",
        "variableName": "organizationCustomDomain"
      },
      {
        "kind": "Variable",
        "name": "recurringBookingIds",
        "variableName": "recurringBookingIds"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBooking",
  "plural": false,
  "selections": [
    (v3/*:: as any*/)
  ],
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_LocationDetails",
  "kind": "LinkedField",
  "name": "involvedLocations",
  "plural": true,
  "selections": [
    (v7/*:: as any*/)
  ],
  "storageKey": null
},
v9 = {
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
        (v3/*:: as any*/),
        (v7/*:: as any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "type",
      "storageKey": null
    },
    (v7/*:: as any*/)
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceProductSubscriptionDetails_relatedBookingsQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
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
                  (v3/*:: as any*/),
                  (v4/*:: as any*/),
                  (v5/*:: as any*/),
                  (v6/*:: as any*/),
                  (v8/*:: as any*/),
                  (v9/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v10/*:: as any*/),
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
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductSubscriptionDetails_relatedBookingsQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
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
                  (v3/*:: as any*/),
                  (v4/*:: as any*/),
                  (v5/*:: as any*/),
                  (v6/*:: as any*/),
                  (v8/*:: as any*/),
                  (v9/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v10/*:: as any*/),
                      (v11/*:: as any*/),
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
    ]
  },
  "params": {
    "cacheID": "258846ee0b3f2a714185ca23a88af8de",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductSubscriptionDetails_relatedBookingsQuery",
    "operationKind": "query",
    "text": "query marketplaceProductSubscriptionDetails_relatedBookingsQuery(\n  $organizationCustomDomain: String!\n  $recurringBookingIds: [String!]\n  $relatedBookingsFirst: Int!\n  $today: DateTime!\n) {\n  bookings(first: $relatedBookingsFirst, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, channel: MARKETPLACE, recurringBookingIds: $recurringBookingIds, fromGte: $today}, orderBy: [{field: FROM, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        recurringBooking {\n          id\n        }\n        from\n        until\n        involvedLocations {\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n          }\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "eb23147e358b3020db76e63ff420cd1c";

export default node;
