/**
 * @generated SignedSource<<27c0880296e4b752cd060c12792d9b38>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "About" | "Name" | "Timezone" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationMemberOrderField = "FamilyName" | "GivenName" | "MiddleName" | "Name" | "PhoneNumber" | "Role" | "Status" | "%future added value";
export type TeamOrderField = "About" | "Name" | "%future added value";
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type TeamOrderInput = {
  direction: OrderDirection;
  field: TeamOrderField;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type newBookingButton_rootQuery$variables = {
  customerExists: boolean;
  customerId: string;
  dateFromToGetAvailableResources: any;
  dateToGetAvailableDesks: any;
  dateToGetAvailableRooms: any;
  dateUntilToGetAvailableResources: any;
  locationExists: boolean;
  locationId: string;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationId: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
  teamsSortingValues?: ReadonlyArray<TeamOrderInput> | null | undefined;
};
export type newBookingButton_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"newBookingDialog_availableLocationDesks_query" | "newBookingDialog_availableLocationRooms_query" | "newBookingDialog_availableResources_query" | "newBookingDialog_customerTeams_query" | "newBookingDialog_organizationMembers_query" | "newBookingDialog_query">;
};
export type newBookingButton_rootQuery = {
  response: newBookingButton_rootQuery$data;
  variables: newBookingButton_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customerExists"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customerId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateFromToGetAvailableResources"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateToGetAvailableDesks"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateToGetAvailableRooms"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateUntilToGetAvailableResources"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationExists"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v11 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v12 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamsSortingValues"
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v14 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v17 = [
  (v13/*: any*/),
  (v16/*: any*/)
],
v18 = {
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
},
v19 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 20
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationMembersSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "peopleNameSearchText"
      },
      (v14/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v21 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v22 = [
  (v20/*: any*/),
  (v16/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
],
v23 = [
  (v20/*: any*/),
  (v16/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingOrganizationCustomTagDetails",
    "kind": "LinkedField",
    "name": "customTags",
    "plural": true,
    "selections": (v22/*: any*/),
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingOrganizationZoneDetails",
    "kind": "LinkedField",
    "name": "zones",
    "plural": true,
    "selections": (v22/*: any*/),
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
      (v6/*: any*/),
      (v7/*: any*/),
      (v8/*: any*/),
      (v9/*: any*/),
      (v10/*: any*/),
      (v11/*: any*/),
      (v12/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "newBookingButton_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newBookingDialog_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newBookingDialog_organizationMembers_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newBookingDialog_customerTeams_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newBookingDialog_availableLocationDesks_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newBookingDialog_availableLocationRooms_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newBookingDialog_availableResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v9/*: any*/),
      (v11/*: any*/),
      (v7/*: any*/),
      (v6/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v2/*: any*/),
      (v5/*: any*/),
      (v10/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/),
      (v12/*: any*/),
      (v8/*: any*/)
    ],
    "kind": "Operation",
    "name": "newBookingButton_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v13/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          {
            "fields": [
              (v14/*: any*/)
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "LocationConnection",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v15/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": (v17/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v18/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "openingHoursMinutesStep",
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*: any*/),
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "organizationMembers",
        "plural": false,
        "selections": [
          (v15/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationMemberEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationMemberDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v13/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": [
                      (v20/*: any*/),
                      (v16/*: any*/),
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
          (v18/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "bookingDetailsSelectorQuery_organizationMembers",
        "kind": "LinkedHandle",
        "name": "organizationMembers"
      },
      {
        "condition": "customerExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "orderBy",
                "variableName": "teamsSortingValues"
              },
              {
                "fields": [
                  {
                    "kind": "Variable",
                    "name": "customerId",
                    "variableName": "customerId"
                  },
                  (v14/*: any*/)
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "TeamConnection",
            "kind": "LinkedField",
            "name": "customerTeams",
            "plural": false,
            "selections": [
              (v15/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": (v17/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v18/*: any*/)
            ],
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
                "fields": [
                  {
                    "kind": "Variable",
                    "name": "date",
                    "variableName": "dateToGetAvailableDesks"
                  },
                  (v21/*: any*/),
                  (v14/*: any*/)
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "BookingDeskDetails",
            "kind": "LinkedField",
            "name": "availableDesks",
            "plural": true,
            "selections": (v23/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": [
              {
                "fields": [
                  {
                    "kind": "Variable",
                    "name": "date",
                    "variableName": "dateToGetAvailableRooms"
                  },
                  (v21/*: any*/),
                  (v14/*: any*/)
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "BookingRoomDetails",
            "kind": "LinkedField",
            "name": "availableRooms",
            "plural": true,
            "selections": (v23/*: any*/),
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
                "name": "from",
                "variableName": "dateFromToGetAvailableResources"
              },
              (v21/*: any*/),
              (v14/*: any*/),
              {
                "kind": "Variable",
                "name": "until",
                "variableName": "dateUntilToGetAvailableResources"
              }
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "BookingResourceDetails",
        "kind": "LinkedField",
        "name": "availableResources",
        "plural": true,
        "selections": (v23/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "3845d0039c60c50c59f125291cbc8d04",
    "id": null,
    "metadata": {},
    "name": "newBookingButton_rootQuery",
    "operationKind": "query",
    "text": "query newBookingButton_rootQuery(\n  $organizationId: String!\n  $peopleNameSearchText: String\n  $locationId: String!\n  $locationExists: Boolean!\n  $dateToGetAvailableDesks: DateTime!\n  $dateToGetAvailableRooms: DateTime!\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $customerId: String!\n  $customerExists: Boolean!\n  $teamsSortingValues: [TeamOrderInput!]\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  ...newBookingDialog_query\n  ...newBookingDialog_organizationMembers_query\n  ...newBookingDialog_customerTeams_query\n  ...newBookingDialog_availableLocationDesks_query\n  ...newBookingDialog_availableLocationRooms_query\n  ...newBookingDialog_availableResources_query\n}\n\nfragment newBookingDialog_availableLocationDesks_query on Query {\n  availableDesks(where: {organizationId: $organizationId, locationId: $locationId, date: $dateToGetAvailableDesks}) @include(if: $locationExists) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment newBookingDialog_availableLocationRooms_query on Query {\n  availableRooms(where: {organizationId: $organizationId, locationId: $locationId, date: $dateToGetAvailableRooms}) @include(if: $locationExists) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment newBookingDialog_availableResources_query on Query {\n  availableResources(where: {organizationId: $organizationId, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment newBookingDialog_customerTeams_query on Query {\n  customerTeams(where: {organizationId: $organizationId, customerId: $customerId}, orderBy: $teamsSortingValues) @include(if: $customerExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment newBookingDialog_organizationMembers_query on Query {\n  organizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment newBookingDialog_query on Query {\n  me {\n    id\n  }\n  locations(where: {organizationId: $organizationId}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  openingHoursMinutesStep\n}\n"
  }
};
})();

(node as any).hash = "ea413d921bf603a3298142ffa273673d";

export default node;
