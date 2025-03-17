/**
 * @generated SignedSource<<73bfe2e4e99e69d9446024e4e30c253a>>
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
export type pageOrganizationBooking_rootQuery$variables = {
  bookingId: string;
  customerExists: boolean;
  customerId: string;
  dateFromToGetAvailableResources: any;
  dateToGetAvailableDesks: any;
  dateToGetAvailableRooms: any;
  dateUntilToGetAvailableResources: any;
  deskIdsToIncludeToGetAvailableDesks: ReadonlyArray<string>;
  locationExists: boolean;
  locationId: string;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationId: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
  roomIdsToIncludeToGetAvailableRooms: ReadonlyArray<string>;
  teamsSortingValues?: ReadonlyArray<TeamOrderInput> | null | undefined;
};
export type pageOrganizationBooking_rootQuery$data = {
  readonly booking: {
    readonly from: any;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"editBooking_availableLocationDesks_query" | "editBooking_availableLocationRooms_query" | "editBooking_availableResources_query" | "editBooking_customerTeams_query" | "editBooking_organizationMembers_query" | "editBooking_query">;
};
export type pageOrganizationBooking_rootQuery = {
  response: pageOrganizationBooking_rootQuery$data;
  variables: pageOrganizationBooking_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingId"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customerExists"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customerId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateFromToGetAvailableResources"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateToGetAvailableDesks"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateToGetAvailableRooms"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateUntilToGetAvailableResources"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskIdsToIncludeToGetAvailableDesks"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationExists"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v11 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v12 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v13 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v14 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "roomIdsToIncludeToGetAvailableRooms"
},
v15 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamsSortingValues"
},
v16 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "bookingId"
  }
],
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v21 = [
  (v19/*: any*/),
  (v20/*: any*/),
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
v22 = [
  (v19/*: any*/),
  (v20/*: any*/)
],
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v24 = [
  (v19/*: any*/),
  (v20/*: any*/),
  (v23/*: any*/)
],
v25 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingOrganizationCustomTagDetails",
  "kind": "LinkedField",
  "name": "customTags",
  "plural": true,
  "selections": (v24/*: any*/),
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingOrganizationZoneDetails",
  "kind": "LinkedField",
  "name": "zones",
  "plural": true,
  "selections": (v24/*: any*/),
  "storageKey": null
},
v27 = [
  (v19/*: any*/),
  (v20/*: any*/),
  (v25/*: any*/),
  (v26/*: any*/)
],
v28 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v29 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v30 = [
  (v18/*: any*/),
  (v20/*: any*/)
],
v31 = {
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
v32 = [
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
      (v28/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v33 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
};
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
      (v12/*: any*/),
      (v13/*: any*/),
      (v14/*: any*/),
      (v15/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationBooking_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v16/*: any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v17/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editBooking_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editBooking_organizationMembers_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editBooking_customerTeams_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editBooking_availableLocationDesks_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editBooking_availableLocationRooms_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editBooking_availableResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v11/*: any*/),
      (v0/*: any*/),
      (v13/*: any*/),
      (v12/*: any*/),
      (v9/*: any*/),
      (v8/*: any*/),
      (v4/*: any*/),
      (v7/*: any*/),
      (v5/*: any*/),
      (v3/*: any*/),
      (v6/*: any*/),
      (v14/*: any*/),
      (v2/*: any*/),
      (v1/*: any*/),
      (v15/*: any*/),
      (v10/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationBooking_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v16/*: any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v17/*: any*/),
          (v18/*: any*/),
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
            "kind": "ScalarField",
            "name": "notes",
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
            "concreteType": "BookingCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": (v21/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": (v22/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingLocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": (v22/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingTeamDetails",
            "kind": "LinkedField",
            "name": "team",
            "plural": false,
            "selections": (v22/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDeskDetails",
            "kind": "LinkedField",
            "name": "desks",
            "plural": true,
            "selections": (v27/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingRoomDetails",
            "kind": "LinkedField",
            "name": "rooms",
            "plural": true,
            "selections": (v27/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingResourceDetails",
            "kind": "LinkedField",
            "name": "resources",
            "plural": true,
            "selections": [
              (v19/*: any*/),
              (v20/*: any*/),
              (v23/*: any*/),
              (v25/*: any*/),
              (v26/*: any*/)
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
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          {
            "fields": [
              (v28/*: any*/)
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
          (v29/*: any*/),
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
                "selections": (v30/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v31/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v32/*: any*/),
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "organizationMembers",
        "plural": false,
        "selections": [
          (v29/*: any*/),
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
                  (v18/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": (v21/*: any*/),
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
          (v31/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v32/*: any*/),
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
                  (v28/*: any*/)
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
              (v29/*: any*/),
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
                    "selections": (v30/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v31/*: any*/)
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
                  {
                    "kind": "Variable",
                    "name": "deskIdsToInclude",
                    "variableName": "deskIdsToIncludeToGetAvailableDesks"
                  },
                  (v33/*: any*/)
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "BookingDeskDetails",
            "kind": "LinkedField",
            "name": "availableDesks",
            "plural": true,
            "selections": (v27/*: any*/),
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
                  (v33/*: any*/),
                  {
                    "kind": "Variable",
                    "name": "roomIdsToInclude",
                    "variableName": "roomIdsToIncludeToGetAvailableRooms"
                  }
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "BookingRoomDetails",
            "kind": "LinkedField",
            "name": "availableRooms",
            "plural": true,
            "selections": (v27/*: any*/),
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
              (v33/*: any*/),
              (v28/*: any*/),
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
        "selections": (v27/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "2ba425c25197ad31aa953d6b024680b5",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationBooking_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationBooking_rootQuery(\n  $organizationId: String!\n  $bookingId: String!\n  $peopleNameSearchText: String\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $locationId: String!\n  $locationExists: Boolean!\n  $dateToGetAvailableDesks: DateTime!\n  $deskIdsToIncludeToGetAvailableDesks: [String!]!\n  $dateToGetAvailableRooms: DateTime!\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $roomIdsToIncludeToGetAvailableRooms: [String!]!\n  $customerId: String!\n  $customerExists: Boolean!\n  $teamsSortingValues: [TeamOrderInput!]\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  booking(id: $bookingId) {\n    from\n    id\n  }\n  ...editBooking_query\n  ...editBooking_organizationMembers_query\n  ...editBooking_customerTeams_query\n  ...editBooking_availableLocationDesks_query\n  ...editBooking_availableLocationRooms_query\n  ...editBooking_availableResources_query\n}\n\nfragment editBooking_availableLocationDesks_query on Query {\n  availableDesks(where: {locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks}) @include(if: $locationExists) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment editBooking_availableLocationRooms_query on Query {\n  availableRooms(where: {locationId: $locationId, date: $dateToGetAvailableRooms, roomIdsToInclude: $roomIdsToIncludeToGetAvailableRooms}) @include(if: $locationExists) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment editBooking_availableResources_query on Query {\n  availableResources(where: {organizationId: $organizationId, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment editBooking_customerTeams_query on Query {\n  customerTeams(where: {organizationId: $organizationId, customerId: $customerId}, orderBy: $teamsSortingValues) @include(if: $customerExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment editBooking_organizationMembers_query on Query {\n  organizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment editBooking_query on Query {\n  locations(where: {organizationId: $organizationId}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    type\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    organization {\n      uniqueId\n      name\n    }\n    location {\n      uniqueId\n      name\n    }\n    team {\n      uniqueId\n      name\n    }\n    desks {\n      uniqueId\n      name\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n    rooms {\n      uniqueId\n      name\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n    resources {\n      uniqueId\n      name\n      color\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e4cace90d52fdbe67d03581cb0053286";

export default node;
