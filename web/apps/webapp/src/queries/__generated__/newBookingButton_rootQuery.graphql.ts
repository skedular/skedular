/**
 * @generated SignedSource<<2c8d950ebfbe897705f107f6addd7a53>>
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
  dateUntilToGetAvailableResources: any;
  locationId: string;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationId: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
  teamsSortingValues?: ReadonlyArray<TeamOrderInput> | null | undefined;
};
export type newBookingButton_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"newBookingDialog_availableResources_query" | "newBookingDialog_customerTeams_query" | "newBookingDialog_organizationMembers_query" | "newBookingDialog_query">;
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
  "name": "dateUntilToGetAvailableResources"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamsSortingValues"
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v11 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v14 = [
  (v10/*: any*/),
  (v13/*: any*/)
],
v15 = {
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
v16 = [
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
      (v11/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v18 = [
  (v17/*: any*/),
  (v13/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
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
      (v9/*: any*/)
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
        "name": "newBookingDialog_availableResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v6/*: any*/),
      (v8/*: any*/),
      (v4/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v7/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/),
      (v9/*: any*/),
      (v5/*: any*/)
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
          (v10/*: any*/)
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
              (v11/*: any*/)
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
          (v12/*: any*/),
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
                "selections": (v14/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v15/*: any*/)
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
        "args": (v16/*: any*/),
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "organizationMembers",
        "plural": false,
        "selections": [
          (v12/*: any*/),
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
                  (v10/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": [
                      (v17/*: any*/),
                      (v13/*: any*/),
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
          (v15/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v16/*: any*/),
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
                  (v11/*: any*/)
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
              (v12/*: any*/),
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
                    "selections": (v14/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v15/*: any*/)
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
                "name": "from",
                "variableName": "dateFromToGetAvailableResources"
              },
              {
                "kind": "Variable",
                "name": "locationId",
                "variableName": "locationId"
              },
              (v11/*: any*/),
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
        "selections": [
          (v17/*: any*/),
          (v13/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingOrganizationCustomTagDetails",
            "kind": "LinkedField",
            "name": "customTags",
            "plural": true,
            "selections": (v18/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingOrganizationZoneDetails",
            "kind": "LinkedField",
            "name": "zones",
            "plural": true,
            "selections": (v18/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "e1711e89bdcfe3dfa24abd16d3e61843",
    "id": null,
    "metadata": {},
    "name": "newBookingButton_rootQuery",
    "operationKind": "query",
    "text": "query newBookingButton_rootQuery(\n  $organizationId: String!\n  $peopleNameSearchText: String\n  $locationId: String!\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $customerId: String!\n  $customerExists: Boolean!\n  $teamsSortingValues: [TeamOrderInput!]\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  ...newBookingDialog_query\n  ...newBookingDialog_organizationMembers_query\n  ...newBookingDialog_customerTeams_query\n  ...newBookingDialog_availableResources_query\n}\n\nfragment newBookingDialog_availableResources_query on Query {\n  availableResources(where: {organizationId: $organizationId, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment newBookingDialog_customerTeams_query on Query {\n  customerTeams(where: {organizationId: $organizationId, customerId: $customerId}, orderBy: $teamsSortingValues) @include(if: $customerExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment newBookingDialog_organizationMembers_query on Query {\n  organizationMembers(where: {organizationId: $organizationId, nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment newBookingDialog_query on Query {\n  me {\n    id\n  }\n  locations(where: {organizationId: $organizationId}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  openingHoursMinutesStep\n}\n"
  }
};
})();

(node as any).hash = "137b40e514ac3836277fbabf4aae459c";

export default node;
