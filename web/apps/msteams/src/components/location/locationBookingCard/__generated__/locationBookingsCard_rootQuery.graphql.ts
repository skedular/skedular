/**
 * @generated SignedSource<<e240660e919b828f683b874f2a6815b7>>
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
  peopleNameSearchText?: string | null | undefined;
  peopleSortingValues: ReadonlyArray<LocationMemberOrderInput>;
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
  "name": "to"
},
v6 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v7 = {
  "fields": [
    (v6/*: any*/),
    {
      "kind": "Variable",
      "name": "nameContains",
      "variableName": "peopleNameSearchText"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
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
  "name": "uniqueId",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v15 = [
  (v9/*: any*/),
  (v10/*: any*/),
  (v11/*: any*/),
  (v12/*: any*/),
  (v13/*: any*/),
  (v14/*: any*/)
],
v16 = [
  (v10/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/)
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
      (v3/*: any*/),
      (v4/*: any*/),
      (v2/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/),
      (v5/*: any*/)
    ],
    "kind": "Operation",
    "name": "locationBookingsCard_rootQuery",
    "selections": [
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
                "name": "orderBy",
                "variableName": "peopleSortingValues"
              },
              (v7/*: any*/)
            ],
            "concreteType": "LocationMemberDetails",
            "kind": "LinkedField",
            "name": "locationMembers",
            "plural": true,
            "selections": [
              (v8/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationCustomerDetails",
                "kind": "LinkedField",
                "name": "customer",
                "plural": false,
                "selections": (v15/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": [
              (v7/*: any*/)
            ],
            "concreteType": "CustomerDetails",
            "kind": "LinkedField",
            "name": "customersByDefaultLocation",
            "plural": true,
            "selections": [
              (v8/*: any*/),
              (v10/*: any*/),
              (v11/*: any*/),
              (v12/*: any*/),
              (v13/*: any*/),
              (v14/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": [
              (v6/*: any*/)
            ],
            "concreteType": "LocationBookingPermissions",
            "kind": "LinkedField",
            "name": "locationBookingPermissions",
            "plural": false,
            "selections": [
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
            "storageKey": null
          }
        ]
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v8/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerLocationDetails",
            "kind": "LinkedField",
            "name": "defaultLocations",
            "plural": true,
            "selections": [
              (v9/*: any*/)
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
          (v10/*: any*/),
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
              (v9/*: any*/),
              (v10/*: any*/)
            ],
            "storageKey": null
          },
          (v8/*: any*/)
        ],
        "storageKey": null
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
          (v8/*: any*/),
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
            "selections": (v15/*: any*/),
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
              (v10/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingLocationTagDetails",
                "kind": "LinkedField",
                "name": "locationTags",
                "plural": true,
                "selections": [
                  (v9/*: any*/),
                  (v10/*: any*/),
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
    "cacheID": "5a3e064d086b42409a47b1b1b867644d",
    "id": null,
    "metadata": {},
    "name": "locationBookingsCard_rootQuery",
    "operationKind": "query",
    "text": "query locationBookingsCard_rootQuery(\n  $peopleNameSearchText: String\n  $peopleSortingValues: [LocationMemberOrderInput!]!\n  $locationId: String!\n  $locationExists: Boolean!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...locationPeopleBookings_query\n}\n\nfragment locationPeopleBookings_query on Query {\n  locationMembers(where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $peopleSortingValues) @include(if: $locationExists) {\n    id\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n  }\n  customersByDefaultLocation(where: {locationId: $locationId, nameContains: $peopleNameSearchText}) @include(if: $locationExists) {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  locationBookingPermissions(locationId: $locationId) @include(if: $locationExists) {\n    canAddBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  me {\n    id\n    defaultLocations {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    name\n    deskCapacity\n    hasFutureBooking\n    canModify\n    canDelete\n    organization {\n      uniqueId\n      name\n    }\n    id\n  }\n  allBookings(where: {locationIds: [$locationId], fromGTE: $from, toLT: $to}) {\n    id\n    from\n    to\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    location {\n      name\n    }\n    team {\n      name\n    }\n    desks {\n      name\n      locationTags {\n        uniqueId\n        name\n        tagType\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c2f39aff8f6c8b33bc834d8253721876";

export default node;
