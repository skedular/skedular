/**
 * @generated SignedSource<<4d8267a82d69e7bdbafa50217336f27c>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type customerBookingsHub_pastBookingsPaginationQuery$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  organizationCustomDomain: string;
  today: any;
};
export type customerBookingsHub_pastBookingsPaginationQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"customerBookingsHub_pastBookings_query">;
};
export type customerBookingsHub_pastBookingsPaginationQuery = {
  response: customerBookingsHub_pastBookingsPaginationQuery$data;
  variables: customerBookingsHub_pastBookingsPaginationQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": 25,
    "kind": "LocalArgument",
    "name": "count"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "cursor"
  },
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
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v2 = [
  {
    "kind": "Variable",
    "name": "after",
    "variableName": "cursor"
  },
  {
    "kind": "Variable",
    "name": "first",
    "variableName": "count"
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
      {
        "kind": "Literal",
        "name": "includeMineOnly",
        "value": true
      },
      (v1/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
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
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = [
  (v4/*:: as any*/)
],
v6 = [
  (v4/*:: as any*/),
  (v3/*:: as any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "customerBookingsHub_pastBookingsPaginationQuery",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "count",
            "variableName": "count"
          },
          {
            "kind": "Variable",
            "name": "cursor",
            "variableName": "cursor"
          },
          (v1/*:: as any*/),
          {
            "kind": "Variable",
            "name": "today",
            "variableName": "today"
          }
        ],
        "kind": "FragmentSpread",
        "name": "customerBookingsHub_pastBookings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "customerBookingsHub_pastBookingsPaginationQuery",
    "selections": [
      {
        "alias": "recentBookings",
        "args": (v2/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
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
                  (v3/*:: as any*/),
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
                      (v4/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "customDomain",
                        "storageKey": null
                      },
                      (v3/*:: as any*/)
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
                    "selections": (v5/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamDetails",
                    "kind": "LinkedField",
                    "name": "involvedTeams",
                    "plural": true,
                    "selections": (v6/*:: as any*/),
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
                        "selections": (v6/*:: as any*/),
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
                          (v4/*:: as any*/)
                        ],
                        "storageKey": null
                      },
                      (v3/*:: as any*/)
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
                        "selections": (v5/*:: as any*/),
                        "storageKey": null
                      },
                      (v3/*:: as any*/)
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
        "storageKey": null
      },
      {
        "alias": "recentBookings",
        "args": (v2/*:: as any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "customerBookingsHub_pastBookings__recentBookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "1e76951548564d5eabd78157e393cde1",
    "id": null,
    "metadata": {},
    "name": "customerBookingsHub_pastBookingsPaginationQuery",
    "operationKind": "query",
    "text": "query customerBookingsHub_pastBookingsPaginationQuery(\n  $count: Int = 25\n  $cursor: String\n  $organizationCustomDomain: String!\n  $today: DateTime!\n) {\n  ...customerBookingsHub_pastBookings_query_1p3vo2\n}\n\nfragment customerBookingsHub_pastBookings_query_1p3vo2 on Query {\n  recentBookings: bookings(first: $count, after: $cursor, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, fromLt: $today}, orderBy: [{field: FROM, direction: DESCENDING}]) {\n    totalCount\n    pageInfo {\n      hasNextPage\n      endCursor\n    }\n    edges {\n      node {\n        id\n        from\n        until\n        involvedOrganizations {\n          name\n          customDomain\n          id\n        }\n        involvedLocations {\n          name\n        }\n        involvedTeams {\n          name\n          id\n        }\n        bookingResources {\n          resource {\n            name\n            id\n          }\n        }\n        marketplaceBooking {\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n        recurringBooking {\n          frequency {\n            name\n          }\n          id\n        }\n        __typename\n      }\n      cursor\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0693fdf55401cf5966836bb438fb8554";

export default node;
