/**
 * @generated SignedSource<<678426d7ccac1cc383eacf32800743d5>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type TeamMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type TeamMemberOrderInput = {
  direction: OrderDirection;
  field: TeamMemberOrderField;
};
export type teamBookingsCard_rootQuery$variables = {
  from: any;
  locationExists: boolean;
  locationId: string;
  organizationExists: boolean;
  organizationId: string;
  peopleSortingValues: ReadonlyArray<TeamMemberOrderInput>;
  teamExists: boolean;
  teamId: string;
  to: any;
};
export type teamBookingsCard_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"teamMembersBookings_query">;
};
export type teamBookingsCard_rootQuery = {
  response: teamBookingsCard_rootQuery$data;
  variables: teamBookingsCard_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "from"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationExists"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleSortingValues"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamExists"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamId"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "to"
},
v9 = [
  {
    "kind": "Variable",
    "name": "teamId",
    "variableName": "teamId"
  }
],
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v13 = [
  (v11/*: any*/),
  (v12/*: any*/),
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
v14 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "canAddBookingOnBehalf",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "canDeleteBookingOnBehalf",
    "storageKey": null
  }
],
v15 = [
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "fromGTE",
        "variableName": "from"
      },
      {
        "items": [
          {
            "kind": "Variable",
            "name": "locationIds.0",
            "variableName": "locationId"
          }
        ],
        "kind": "ListValue",
        "name": "locationIds"
      },
      {
        "items": [
          {
            "kind": "Variable",
            "name": "organizationIds.0",
            "variableName": "organizationId"
          }
        ],
        "kind": "ListValue",
        "name": "organizationIds"
      },
      {
        "items": [
          {
            "kind": "Variable",
            "name": "teamIds.0",
            "variableName": "teamId"
          }
        ],
        "kind": "ListValue",
        "name": "teamIds"
      },
      {
        "kind": "Variable",
        "name": "toLT",
        "variableName": "to"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v16 = [
  (v12/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v7/*: any*/),
      (v8/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "teamBookingsCard_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "teamMembersBookings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v5/*: any*/),
      (v4/*: any*/),
      (v3/*: any*/),
      (v2/*: any*/),
      (v1/*: any*/),
      (v7/*: any*/),
      (v6/*: any*/),
      (v0/*: any*/),
      (v8/*: any*/)
    ],
    "kind": "Operation",
    "name": "teamBookingsCard_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "peopleSortingValues"
          },
          {
            "fields": (v9/*: any*/),
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "TeamMemberDetails",
        "kind": "LinkedField",
        "name": "teamMembers",
        "plural": true,
        "selections": [
          (v10/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": (v13/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v10/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerTeamDetails",
            "kind": "LinkedField",
            "name": "defaultTeams",
            "plural": true,
            "selections": [
              (v11/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "teamId"
          }
        ],
        "concreteType": "TeamDetails",
        "kind": "LinkedField",
        "name": "team",
        "plural": false,
        "selections": [
          (v12/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "hasFutureBooking",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canModify",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canDelete",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              (v11/*: any*/),
              (v12/*: any*/)
            ],
            "storageKey": null
          },
          (v10/*: any*/)
        ],
        "storageKey": null
      },
      {
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "organizationId",
                "variableName": "organizationId"
              }
            ],
            "concreteType": "OrganizationBookingPermissions",
            "kind": "LinkedField",
            "name": "organizationBookingPermissions",
            "plural": false,
            "selections": (v14/*: any*/),
            "storageKey": null
          }
        ]
      },
      {
        "condition": "locationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "locationId",
                "variableName": "locationId"
              }
            ],
            "concreteType": "LocationBookingPermissions",
            "kind": "LinkedField",
            "name": "locationBookingPermissions",
            "plural": false,
            "selections": (v14/*: any*/),
            "storageKey": null
          }
        ]
      },
      {
        "condition": "teamExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v9/*: any*/),
            "concreteType": "TeamBookingPermissions",
            "kind": "LinkedField",
            "name": "teamBookingPermissions",
            "plural": false,
            "selections": (v14/*: any*/),
            "storageKey": null
          }
        ]
      },
      {
        "alias": null,
        "args": (v15/*: any*/),
        "concreteType": "BookingConnection",
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
                  (v10/*: any*/),
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
                    "concreteType": "BookingCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": (v13/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v16/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v16/*: any*/),
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
                      (v12/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "BookingLocationTagDetails",
                        "kind": "LinkedField",
                        "name": "locationTags",
                        "plural": true,
                        "selections": [
                          (v11/*: any*/),
                          (v12/*: any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "tagType",
                            "storageKey": null
                          }
                        ],
                        "storageKey": null
                      }
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
        "args": (v15/*: any*/),
        "filters": [
          "where"
        ],
        "handle": "connection",
        "key": "bookingsWeekGrid_bookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "6bac03b04ee4f622257b966b4c8a7ecd",
    "id": null,
    "metadata": {},
    "name": "teamBookingsCard_rootQuery",
    "operationKind": "query",
    "text": "query teamBookingsCard_rootQuery(\n  $peopleSortingValues: [TeamMemberOrderInput!]!\n  $organizationId: String!\n  $organizationExists: Boolean!\n  $locationId: String!\n  $locationExists: Boolean!\n  $teamId: String!\n  $teamExists: Boolean!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...teamMembersBookings_query\n}\n\nfragment bookingsWeekGrid_allBookings_query on Query {\n  bookings(where: {organizationIds: [$organizationId], locationIds: [$locationId], teamIds: [$teamId], fromGTE: $from, toLT: $to}) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        to\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        location {\n          name\n        }\n        team {\n          name\n        }\n        desks {\n          name\n          locationTags {\n            uniqueId\n            name\n            tagType\n          }\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment bookingsWeekGrid_query on Query {\n  me {\n    id\n  }\n  organizationBookingPermissions(organizationId: $organizationId) @include(if: $organizationExists) {\n    canAddBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  locationBookingPermissions(locationId: $locationId) @include(if: $locationExists) {\n    canAddBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  teamBookingPermissions(teamId: $teamId) @include(if: $teamExists) {\n    canAddBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n}\n\nfragment teamMembersBookings_query on Query {\n  teamMembers(where: {teamId: $teamId}, orderBy: $peopleSortingValues) {\n    id\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n  }\n  me {\n    id\n    defaultTeams {\n      uniqueId\n    }\n  }\n  team(id: $teamId) {\n    name\n    hasFutureBooking\n    canModify\n    canDelete\n    organization {\n      uniqueId\n      name\n    }\n    id\n  }\n  ...bookingsWeekGrid_query\n  ...bookingsWeekGrid_allBookings_query\n}\n"
  }
};
})();

(node as any).hash = "3ba025e60dc0971857166c448b3aed92";

export default node;
