/**
 * @generated SignedSource<<0e7043d38427b24dcd98517180901d9c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type LocationMemberOrderInput = {
  direction: OrderDirection;
  field: LocationMemberOrderField;
};
export type locationBookingsCard_rootQuery$variables = {
  from: any;
  locationExists: boolean;
  locationId: string;
  organizationId: string;
  peopleSortingValues: ReadonlyArray<LocationMemberOrderInput>;
  teamExists: boolean;
  teamId: string;
  to: any;
};
export type locationBookingsCard_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationPeopleBookings_query">;
};
export type locationBookingsCard_rootQuery = {
  response: locationBookingsCard_rootQuery$data;
  variables: locationBookingsCard_rootQuery$variables;
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
  "name": "organizationId"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleSortingValues"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamExists"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamId"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "to"
},
v8 = [
  {
    "kind": "Variable",
    "name": "locationId",
    "variableName": "locationId"
  }
],
v9 = {
  "fields": (v8/*: any*/),
  "kind": "ObjectValue",
  "name": "where"
},
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
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v17 = [
  (v11/*: any*/),
  (v12/*: any*/),
  (v13/*: any*/),
  (v14/*: any*/),
  (v15/*: any*/),
  (v16/*: any*/)
],
v18 = [
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
v19 = [
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
      (v7/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "locationBookingsCard_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationPeopleBookings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v4/*: any*/),
      (v3/*: any*/),
      (v2/*: any*/),
      (v1/*: any*/),
      (v6/*: any*/),
      (v5/*: any*/),
      (v0/*: any*/),
      (v7/*: any*/)
    ],
    "kind": "Operation",
    "name": "locationBookingsCard_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "peopleSortingValues"
          },
          (v9/*: any*/)
        ],
        "concreteType": "LocationMemberDetails",
        "kind": "LinkedField",
        "name": "locationMembers",
        "plural": true,
        "selections": [
          (v10/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": (v17/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          (v9/*: any*/)
        ],
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "customersByDefaultLocation",
        "plural": true,
        "selections": [
          (v10/*: any*/),
          (v12/*: any*/),
          (v13/*: any*/),
          (v14/*: any*/),
          (v15/*: any*/),
          (v16/*: any*/)
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
            "concreteType": "CustomerLocationDetails",
            "kind": "LinkedField",
            "name": "defaultLocations",
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
            "variableName": "locationId"
          }
        ],
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v12/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "deskCapacity",
            "storageKey": null
          },
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
            "concreteType": "LocationOrganizationDetails",
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
        "selections": (v18/*: any*/),
        "storageKey": null
      },
      {
        "condition": "locationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v8/*: any*/),
            "concreteType": "LocationBookingPermissions",
            "kind": "LinkedField",
            "name": "locationBookingPermissions",
            "plural": false,
            "selections": (v18/*: any*/),
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
            "args": [
              {
                "kind": "Variable",
                "name": "teamId",
                "variableName": "teamId"
              }
            ],
            "concreteType": "TeamBookingPermissions",
            "kind": "LinkedField",
            "name": "teamBookingPermissions",
            "plural": false,
            "selections": (v18/*: any*/),
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
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "allBookings",
        "plural": true,
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
            "selections": (v17/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingLocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": (v19/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingTeamDetails",
            "kind": "LinkedField",
            "name": "team",
            "plural": false,
            "selections": (v19/*: any*/),
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
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "18ac5e47d84c9e339527d1fa36d71f0c",
    "id": null,
    "metadata": {},
    "name": "locationBookingsCard_rootQuery",
    "operationKind": "query",
    "text": "query locationBookingsCard_rootQuery(\n  $peopleSortingValues: [LocationMemberOrderInput!]!\n  $organizationId: String!\n  $locationId: String!\n  $locationExists: Boolean!\n  $teamId: String!\n  $teamExists: Boolean!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...locationPeopleBookings_query\n}\n\nfragment bookingsWeekGrid_allBookings_query on Query {\n  allBookings(where: {organizationIds: [$organizationId], locationIds: [$locationId], teamIds: [$teamId], fromGTE: $from, toLT: $to}) {\n    id\n    from\n    to\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    location {\n      name\n    }\n    team {\n      name\n    }\n    desks {\n      name\n      locationTags {\n        uniqueId\n        name\n        tagType\n      }\n    }\n  }\n}\n\nfragment bookingsWeekGrid_query on Query {\n  me {\n    id\n  }\n  organizationBookingPermissions(organizationId: $organizationId) {\n    canAddBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  locationBookingPermissions(locationId: $locationId) @include(if: $locationExists) {\n    canAddBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  teamBookingPermissions(teamId: $teamId) @include(if: $teamExists) {\n    canAddBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n}\n\nfragment locationPeopleBookings_query on Query {\n  locationMembers(where: {locationId: $locationId}, orderBy: $peopleSortingValues) {\n    id\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n  }\n  customersByDefaultLocation(where: {locationId: $locationId}) {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  me {\n    id\n    defaultLocations {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    name\n    deskCapacity\n    hasFutureBooking\n    canModify\n    canDelete\n    organization {\n      uniqueId\n      name\n    }\n    id\n  }\n  ...bookingsWeekGrid_query\n  ...bookingsWeekGrid_allBookings_query\n}\n"
  }
};
})();

(node as any).hash = "f938805cb717ca549db3d79c05fa96c2";

export default node;
