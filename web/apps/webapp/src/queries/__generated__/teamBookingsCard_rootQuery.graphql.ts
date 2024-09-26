/**
 * @generated SignedSource<<4af850476ab3c28166325c04fc79d4cc>>
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
  field?: TeamMemberOrderField | null | undefined;
};
export type teamBookingsCard_rootQuery$variables = {
  fetchBookingPermission: boolean;
  from: any;
  organizationId: string;
  peopleNameSearchText: string;
  peopleSortingValues: ReadonlyArray<TeamMemberOrderInput>;
  teamId: string;
  to: any;
};
export type teamBookingsCard_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"teamPeopleBookingsMatrix_query">;
};
export type teamBookingsCard_rootQuery = {
  response: teamBookingsCard_rootQuery$data;
  variables: teamBookingsCard_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "fetchBookingPermission"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "from"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleSortingValues"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamId"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "to"
},
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
  "name": "uniqueId",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v10 = [
  (v8/*: any*/),
  (v9/*: any*/),
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
      (v6/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "teamBookingsCard_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "teamPeopleBookingsMatrix_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v3/*: any*/),
      (v4/*: any*/),
      (v0/*: any*/),
      (v2/*: any*/),
      (v5/*: any*/),
      (v1/*: any*/),
      (v6/*: any*/)
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
            "fields": [
              {
                "kind": "Variable",
                "name": "nameContains",
                "variableName": "peopleNameSearchText"
              },
              {
                "kind": "Variable",
                "name": "teamId",
                "variableName": "teamId"
              }
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "TeamMemberDetails",
        "kind": "LinkedField",
        "name": "teamMembers",
        "plural": true,
        "selections": [
          (v7/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": (v10/*: any*/),
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
          (v7/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerTeamDetails",
            "kind": "LinkedField",
            "name": "defaultTeams",
            "plural": true,
            "selections": [
              (v8/*: any*/)
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
          (v7/*: any*/)
        ],
        "storageKey": null
      },
      {
        "condition": "fetchBookingPermission",
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
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "canAddBookingOnBehalf",
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ]
      },
      {
        "alias": null,
        "args": [
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
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "allBookings",
        "plural": true,
        "selections": [
          (v7/*: any*/),
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
            "concreteType": "BookingCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": (v10/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingLocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": [
              (v9/*: any*/)
            ],
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
              (v9/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingLocationTagDetails",
                "kind": "LinkedField",
                "name": "locationTags",
                "plural": true,
                "selections": [
                  (v8/*: any*/),
                  (v9/*: any*/),
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
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "4c41e59ac5f64cb1f9a09092bcc6e1cb",
    "id": null,
    "metadata": {},
    "name": "teamBookingsCard_rootQuery",
    "operationKind": "query",
    "text": "query teamBookingsCard_rootQuery(\n  $peopleNameSearchText: String!\n  $peopleSortingValues: [TeamMemberOrderInput!]!\n  $fetchBookingPermission: Boolean!\n  $organizationId: String!\n  $teamId: String!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...teamPeopleBookingsMatrix_query\n}\n\nfragment teamPeopleBookingsMatrix_query on Query {\n  teamMembers(where: {teamId: $teamId, nameContains: $peopleNameSearchText}, orderBy: $peopleSortingValues) {\n    id\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n  }\n  me {\n    id\n    defaultTeams {\n      uniqueId\n    }\n  }\n  team(id: $teamId) {\n    hasFutureBooking\n    canModify\n    canDelete\n    id\n  }\n  organizationBookingPermissions(organizationId: $organizationId) @include(if: $fetchBookingPermission) {\n    canAddBookingOnBehalf\n  }\n  allBookings(where: {teamIds: [$teamId], fromGTE: $from, toLT: $to}) {\n    id\n    from\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    location {\n      name\n    }\n    desks {\n      name\n      locationTags {\n        uniqueId\n        name\n        tagType\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3e670ba7bb40dbfdd7c4c032d556cb23";

export default node;
