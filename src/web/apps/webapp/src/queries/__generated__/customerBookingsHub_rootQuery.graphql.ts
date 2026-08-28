/**
 * @generated SignedSource<<815b6f9a0873dd4f0a5787d377a57057>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type customerBookingsHub_rootQuery$variables = {
  organizationCustomDomain: string;
  today: any;
};
export type customerBookingsHub_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"customerBookingsHub_pastBookings_query" | "customerBookingsHub_upcomingBookings_query">;
};
export type customerBookingsHub_rootQuery = {
  response: customerBookingsHub_rootQuery$data;
  variables: customerBookingsHub_rootQuery$variables;
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
  "name": "today"
},
v2 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v3 = [
  (v2/*:: as any*/),
  {
    "kind": "Variable",
    "name": "today",
    "variableName": "today"
  }
],
v4 = {
  "kind": "Literal",
  "name": "first",
  "value": 25
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
      (v5/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
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
  "name": "name",
  "storageKey": null
},
v9 = [
  (v8/*:: as any*/)
],
v10 = [
  (v8/*:: as any*/),
  (v7/*:: as any*/)
],
v11 = [
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
          (v7/*:: as any*/),
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
            "concreteType": "OrganizationDetails",
            "kind": "LinkedField",
            "name": "involvedOrganizations",
            "plural": true,
            "selections": [
              (v8/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "customDomain",
                "storageKey": null
              },
              (v7/*:: as any*/)
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
            "selections": (v9/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamDetails",
            "kind": "LinkedField",
            "name": "involvedTeams",
            "plural": true,
            "selections": (v10/*:: as any*/),
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
                "selections": (v10/*:: as any*/),
                "storageKey": null
              }
            ],
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
              {
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
                  (v8/*:: as any*/)
                ],
                "storageKey": null
              },
              (v7/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "RecurringBookingDetails",
            "kind": "LinkedField",
            "name": "recurringBooking",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingFrequencyDetails",
                "kind": "LinkedField",
                "name": "frequency",
                "plural": false,
                "selections": (v9/*:: as any*/),
                "storageKey": null
              },
              (v7/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "__typename",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "cursor",
        "storageKey": null
      }
    ],
    "storageKey": null
  }
],
v12 = [
  "where",
  "orderBy"
],
v13 = [
  (v4/*:: as any*/),
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
      (v5/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
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
    "name": "customerBookingsHub_rootQuery",
    "selections": [
      {
        "args": (v3/*:: as any*/),
        "kind": "FragmentSpread",
        "name": "customerBookingsHub_upcomingBookings_query"
      },
      {
        "args": (v3/*:: as any*/),
        "kind": "FragmentSpread",
        "name": "customerBookingsHub_pastBookings_query"
      }
    ],
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
    "name": "customerBookingsHub_rootQuery",
    "selections": [
      {
        "alias": "upcomingBookings",
        "args": (v6/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v11/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": "upcomingBookings",
        "args": (v6/*:: as any*/),
        "filters": (v12/*:: as any*/),
        "handle": "connection",
        "key": "customerBookingsHub_upcomingBookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      },
      {
        "alias": "recentBookings",
        "args": (v13/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v11/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": "recentBookings",
        "args": (v13/*:: as any*/),
        "filters": (v12/*:: as any*/),
        "handle": "connection",
        "key": "customerBookingsHub_pastBookings__recentBookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "82606b7806a6238af4309e3f6d2d833c",
    "id": null,
    "metadata": {},
    "name": "customerBookingsHub_rootQuery",
    "operationKind": "query",
    "text": "query customerBookingsHub_rootQuery(\n  $today: DateTime!\n  $organizationCustomDomain: String!\n) {\n  ...customerBookingsHub_upcomingBookings_query_21poUn\n  ...customerBookingsHub_pastBookings_query_21poUn\n}\n\nfragment customerBookingsHub_pastBookings_query_21poUn on Query {\n  recentBookings: bookings(first: 25, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, fromLt: $today}, orderBy: [{field: FROM, direction: DESCENDING}]) {\n    totalCount\n    pageInfo {\n      hasNextPage\n      endCursor\n    }\n    edges {\n      node {\n        id\n        from\n        until\n        involvedOrganizations {\n          name\n          customDomain\n          id\n        }\n        involvedLocations {\n          name\n        }\n        involvedTeams {\n          name\n          id\n        }\n        bookingResources {\n          resource {\n            name\n            id\n          }\n        }\n        marketplaceBooking {\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n        recurringBooking {\n          frequency {\n            name\n          }\n          id\n        }\n        __typename\n      }\n      cursor\n    }\n  }\n}\n\nfragment customerBookingsHub_upcomingBookings_query_21poUn on Query {\n  upcomingBookings: bookings(first: 25, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, fromGte: $today}, orderBy: [{field: FROM, direction: ASCENDING}]) {\n    totalCount\n    pageInfo {\n      hasNextPage\n      endCursor\n    }\n    edges {\n      node {\n        id\n        from\n        until\n        involvedOrganizations {\n          name\n          customDomain\n          id\n        }\n        involvedLocations {\n          name\n        }\n        involvedTeams {\n          name\n          id\n        }\n        bookingResources {\n          resource {\n            name\n            id\n          }\n        }\n        marketplaceBooking {\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n        recurringBooking {\n          frequency {\n            name\n          }\n          id\n        }\n        __typename\n      }\n      cursor\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4e53d24d701275ac85f2a51e326ea354";

export default node;
