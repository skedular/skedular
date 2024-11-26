/**
 * @generated SignedSource<<3f21d672aa7e275aed0788e0a5eea5ed>>
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
export type OrganizationMemberOrderField = "FamilyName" | "GivenName" | "MembershipType" | "MiddleName" | "Name" | "%future added value";
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type locations_rootQuery$variables = {
  deskTypeIds: ReadonlyArray<string>;
  locationsSortingValues: ReadonlyArray<LocationOrderInput>;
  organizationId: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  todayDate: any;
  zoneIds: ReadonlyArray<string>;
};
export type locations_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"deskTypeSelector_allDeskTypes_query" | "myLocations_locations_availableOrganizationDesks_query" | "myLocations_query" | "zoneSelector_allZones_query">;
};
export type locations_rootQuery = {
  response: locations_rootQuery$data;
  variables: locations_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskTypeIds"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "todayDate"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneIds"
},
v6 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v7 = {
  "fields": [
    (v6/*: any*/)
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v11 = [
  (v9/*: any*/),
  (v10/*: any*/)
],
v12 = {
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
v13 = {
  "kind": "Variable",
  "name": "orderBy",
  "variableName": "locationsSortingValues"
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
};
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
    "name": "locations_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "deskTypeSelector_allDeskTypes_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "zoneSelector_allZones_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "myLocations_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "myLocations_locations_availableOrganizationDesks_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v1/*: any*/),
      (v4/*: any*/),
      (v3/*: any*/),
      (v5/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "locations_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          (v7/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "deskTypes",
        "plural": false,
        "selections": [
          (v8/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": (v11/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v12/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": "zonesLocations",
        "args": [
          (v13/*: any*/),
          (v7/*: any*/)
        ],
        "concreteType": "LocationConnection",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v8/*: any*/),
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
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationTagDetails",
                    "kind": "LinkedField",
                    "name": "locationTags",
                    "plural": true,
                    "selections": (v11/*: any*/),
                    "storageKey": null
                  },
                  (v9/*: any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v12/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "organizationMembersSortingValues"
          },
          (v7/*: any*/)
        ],
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "organizationMembers",
        "plural": false,
        "selections": [
          (v8/*: any*/),
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
                  (v9/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": [
                      (v14/*: any*/),
                      (v10/*: any*/),
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
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v12/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          (v13/*: any*/),
          {
            "fields": [
              {
                "kind": "Variable",
                "name": "deskTypeIds",
                "variableName": "deskTypeIds"
              },
              (v6/*: any*/),
              {
                "kind": "Variable",
                "name": "zoneIds",
                "variableName": "zoneIds"
              }
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
          (v8/*: any*/),
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
                "selections": [
                  (v9/*: any*/),
                  (v10/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "DeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": [
                      (v9/*: any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationTagDetails",
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
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationAddressDetails",
                    "kind": "LinkedField",
                    "name": "physicalAddress",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "formattedAddress",
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
          },
          (v12/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "date",
            "variableName": "todayDate"
          },
          {
            "kind": "Literal",
            "name": "deskIdsToInclude",
            "value": []
          },
          (v6/*: any*/)
        ],
        "concreteType": "BookingDeskDetails",
        "kind": "LinkedField",
        "name": "availableOrganizationDesks",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingLocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": [
              (v14/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "ae6b894d7cdb1e47e07b1323721e2412",
    "id": null,
    "metadata": {},
    "name": "locations_rootQuery",
    "operationKind": "query",
    "text": "query locations_rootQuery(\n  $organizationId: String!\n  $locationsSortingValues: [LocationOrderInput!]!\n  $todayDate: DateTime!\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $zoneIds: [String!]!\n  $deskTypeIds: [String!]!\n) {\n  ...deskTypeSelector_allDeskTypes_query\n  ...zoneSelector_allZones_query\n  ...myLocations_query\n  ...myLocations_locations_availableOrganizationDesks_query\n}\n\nfragment deskTypeSelector_allDeskTypes_query on Query {\n  deskTypes(where: {organizationId: $organizationId}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment myLocations_locations_availableOrganizationDesks_query on Query {\n  locations(where: {organizationId: $organizationId, zoneIds: $zoneIds, deskTypeIds: $deskTypeIds}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        desks {\n          id\n        }\n        locationTags {\n          id\n          name\n          tagType\n        }\n        physicalAddress {\n          formattedAddress\n        }\n      }\n    }\n  }\n  availableOrganizationDesks(organizationId: $organizationId, date: $todayDate, deskIdsToInclude: []) {\n    location {\n      uniqueId\n    }\n  }\n}\n\nfragment myLocations_query on Query {\n  organizationMembers(where: {organizationId: $organizationId}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n      }\n    }\n  }\n}\n\nfragment zoneSelector_allZones_query on Query {\n  zonesLocations: locations(where: {organizationId: $organizationId}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        locationTags {\n          id\n          name\n        }\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5c54138c7357e49eac09cd5d08038726";

export default node;
