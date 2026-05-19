/**
 * @generated SignedSource<<bbf888b9be54d6d806a7e4e1596315eb>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type guestStoreFrontBookings_rootQuery$variables = {
  organizationCustomDomain: string;
  today: any;
};
export type guestStoreFrontBookings_rootQuery$data = {
  readonly organizationPublic: {
    readonly marketplaceListingMetadata: {
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly name: string;
  } | null | undefined;
  readonly recentBookings: {
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
        readonly until: any;
      };
    }>;
    readonly totalCount: number;
  };
};
export type guestStoreFrontBookings_rootQuery = {
  response: guestStoreFrontBookings_rootQuery$data;
  variables: guestStoreFrontBookings_rootQuery$variables;
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
v2 = {
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
    }
  ],
  "storageKey": null
},
v3 = {
  "kind": "Literal",
  "name": "first",
  "value": 24
},
v4 = {
  "kind": "Literal",
  "name": "channel",
  "value": "MARKETPLACE"
},
v5 = {
  "kind": "Literal",
  "name": "includeMineOnly",
  "value": true
},
v6 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v7 = [
  (v3/*:: as any*/),
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
      (v4/*:: as any*/),
      {
        "kind": "Variable",
        "name": "fromGte",
        "variableName": "today"
      },
      (v5/*:: as any*/),
      (v6/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_LocationDetails",
  "kind": "LinkedField",
  "name": "involvedLocations",
  "plural": true,
  "selections": [
    (v1/*:: as any*/)
  ],
  "storageKey": null
},
v13 = {
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
        (v9/*:: as any*/),
        (v1/*:: as any*/)
      ],
      "storageKey": null
    }
  ],
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
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "type",
      "storageKey": null
    },
    (v1/*:: as any*/)
  ],
  "storageKey": null
},
v16 = [
  (v8/*:: as any*/),
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
              (v15/*:: as any*/)
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
v17 = [
  (v3/*:: as any*/),
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
      (v4/*:: as any*/),
      {
        "kind": "Variable",
        "name": "fromLt",
        "variableName": "today"
      },
      (v5/*:: as any*/),
      (v6/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v18 = [
  (v8/*:: as any*/),
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
              (v9/*:: as any*/)
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "guestStoreFrontBookings_rootQuery",
    "selections": [
      (v2/*:: as any*/),
      {
        "alias": "upcomingBookings",
        "args": (v7/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v16/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": "recentBookings",
        "args": (v17/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v16/*:: as any*/),
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
    "name": "guestStoreFrontBookings_rootQuery",
    "selections": [
      (v2/*:: as any*/),
      {
        "alias": "upcomingBookings",
        "args": (v7/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v18/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": "recentBookings",
        "args": (v17/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v18/*:: as any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "00a0ee22ce21555d13f52e4caa5b3e71",
    "id": null,
    "metadata": {},
    "name": "guestStoreFrontBookings_rootQuery",
    "operationKind": "query",
    "text": "query guestStoreFrontBookings_rootQuery(\n  $organizationCustomDomain: String!\n  $today: DateTime!\n) {\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n    marketplaceListingMetadata {\n      title\n      subTitle\n    }\n  }\n  upcomingBookings: bookings(first: 24, where: {organizationCustomDomain: $organizationCustomDomain, includeMineOnly: true, channel: MARKETPLACE, fromGte: $today}, orderBy: [{field: FROM, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        until\n        involvedLocations {\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n          }\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n      }\n    }\n  }\n  recentBookings: bookings(first: 24, where: {organizationCustomDomain: $organizationCustomDomain, includeMineOnly: true, channel: MARKETPLACE, fromLt: $today}, orderBy: [{field: FROM, direction: DESCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        until\n        involvedLocations {\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n          }\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9fe642029dedb641818649d7d3e74d60";

export default node;
