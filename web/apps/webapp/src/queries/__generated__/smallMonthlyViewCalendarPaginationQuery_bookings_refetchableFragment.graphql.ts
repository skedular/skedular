/**
 * @generated SignedSource<<c2f261c8d4cec90319525761e3ef0439>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type BookingOrderField = "BookingType" | "FamilyName" | "From" | "GivenName" | "LocationName" | "MiddleName" | "Name" | "Notes" | "OrganizationName" | "TeamName" | "To" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type BookingOrderInput = {
  direction: OrderDirection;
  field: BookingOrderField;
};
export type smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  monthlyCalendarDateFrom?: any | null | undefined;
  monthlyCalendarDateTo?: any | null | undefined;
  smallMonthlyViewCalendarBookingsSortingValues?: ReadonlyArray<BookingOrderInput> | null | undefined;
};
export type smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"smallMonthlyViewCalendar_bookings_query">;
};
export type smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment = {
  response: smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment$data;
  variables: smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": 1000,
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
    "name": "monthlyCalendarDateFrom"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "monthlyCalendarDateTo"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "smallMonthlyViewCalendarBookingsSortingValues"
  }
],
v1 = [
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
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "smallMonthlyViewCalendarBookingsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "fromGTE",
        "variableName": "monthlyCalendarDateFrom"
      },
      {
        "kind": "Literal",
        "name": "includeMineOnly",
        "value": true
      },
      {
        "kind": "Variable",
        "name": "toLT",
        "variableName": "monthlyCalendarDateTo"
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
  "name": "uniqueId",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = [
  (v2/*: any*/),
  (v3/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment",
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
          }
        ],
        "kind": "FragmentSpread",
        "name": "smallMonthlyViewCalendar_bookings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingConnection",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
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
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "id",
                    "storageKey": null
                  },
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
                    "name": "to",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "notes",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": [
                      (v2/*: any*/),
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
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v4/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v4/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v4/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": [
                      (v2/*: any*/),
                      (v3/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "BookingOrganizationDeskTypeDetails",
                        "kind": "LinkedField",
                        "name": "deskTypes",
                        "plural": true,
                        "selections": (v4/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "BookingOrganizationZoneDetails",
                        "kind": "LinkedField",
                        "name": "zones",
                        "plural": true,
                        "selections": (v4/*: any*/),
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "type",
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
                "name": "endCursor",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "hasNextPage",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "kind": "ClientExtension",
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "__id",
                "storageKey": null
              }
            ]
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v1/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "SmallMonthlyViewCalendar_bookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "97e54f2247323721fa482aba2f4e0a05",
    "id": null,
    "metadata": {},
    "name": "smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment",
    "operationKind": "query",
    "text": "query smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment(\n  $count: Int = 1000\n  $cursor: String\n  $monthlyCalendarDateFrom: DateTime\n  $monthlyCalendarDateTo: DateTime\n  $smallMonthlyViewCalendarBookingsSortingValues: [BookingOrderInput!]\n) {\n  ...smallMonthlyViewCalendar_bookings_query_1G22uz\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  to\n  notes\n  type\n  customer {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    uniqueId\n    name\n  }\n  location {\n    uniqueId\n    name\n  }\n  team {\n    uniqueId\n    name\n  }\n  desks {\n    uniqueId\n    name\n    deskTypes {\n      uniqueId\n      name\n    }\n    zones {\n      uniqueId\n      name\n    }\n  }\n}\n\nfragment smallMonthlyViewCalendar_bookings_query_1G22uz on Query {\n  bookings(first: $count, after: $cursor, where: {fromGTE: $monthlyCalendarDateFrom, toLT: $monthlyCalendarDateTo, includeMineOnly: true}, orderBy: $smallMonthlyViewCalendarBookingsSortingValues) {\n    edges {\n      node {\n        id\n        from\n        to\n        notes\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        organization {\n          uniqueId\n          name\n        }\n        location {\n          uniqueId\n          name\n        }\n        team {\n          uniqueId\n          name\n        }\n        desks {\n          uniqueId\n          name\n          deskTypes {\n            uniqueId\n            name\n          }\n          zones {\n            uniqueId\n            name\n          }\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f169a5e55e8145fb63ab330268257a28";

export default node;
