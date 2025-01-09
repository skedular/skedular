/**
 * @generated SignedSource<<304b67d496aadbd82ead171e5bdced92>>
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
export type OrganizationTagOrderField = "Description" | "Name" | "TagType" | "%future added value";
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type locations_rootQuery$variables = {
  deskTypeIds?: ReadonlyArray<string> | null | undefined;
  deskTypesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationId: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  todayDate: any;
  zoneIds?: ReadonlyArray<string> | null | undefined;
  zonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
};
export type locations_rootQuery$data = {
  readonly organization: {
    readonly canModify: boolean;
  } | null | undefined;
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
  "name": "deskTypesSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "todayDate"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneIds"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zonesSortingValues"
},
v8 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationId"
  }
],
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
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
  "fields": [
    (v11/*: any*/)
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
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
  (v13/*: any*/),
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
        "selections": [
          (v10/*: any*/),
          (v14/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  (v15/*: any*/)
],
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v18 = {
  "kind": "Variable",
  "name": "deskTypeIds",
  "variableName": "deskTypeIds"
},
v19 = {
  "kind": "Variable",
  "name": "zoneIds",
  "variableName": "zoneIds"
},
v20 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationsSortingValues"
  },
  {
    "fields": [
      (v18/*: any*/),
      (v11/*: any*/),
      (v19/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v21 = [
  (v17/*: any*/),
  (v14/*: any*/)
],
v22 = [
  (v17/*: any*/)
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
    "name": "locations_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v8/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v9/*: any*/)
        ],
        "storageKey": null
      },
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
      (v3/*: any*/),
      (v2/*: any*/),
      (v7/*: any*/),
      (v1/*: any*/),
      (v5/*: any*/),
      (v4/*: any*/),
      (v6/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "locations_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v8/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v9/*: any*/),
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
            "variableName": "deskTypesSortingValues"
          },
          (v12/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "deskTypes",
        "plural": false,
        "selections": (v16/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "zonesSortingValues"
          },
          (v12/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v16/*: any*/),
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
          (v12/*: any*/)
        ],
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "organizationMembers",
        "plural": false,
        "selections": [
          (v13/*: any*/),
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
                      (v14/*: any*/),
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
          (v15/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v20/*: any*/),
        "concreteType": "LocationConnection",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v13/*: any*/),
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
                  (v10/*: any*/),
                  (v14/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "deskTypes",
                    "plural": true,
                    "selections": (v21/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "zones",
                    "plural": true,
                    "selections": (v21/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "DeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": [
                      (v10/*: any*/)
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
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "hasFutureBooking",
                    "storageKey": null
                  },
                  (v9/*: any*/),
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
                    "selections": (v22/*: any*/),
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
        "args": (v20/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "myLocations_locations",
        "kind": "LinkedHandle",
        "name": "locations"
      },
      {
        "alias": null,
        "args": [
          {
            "fields": [
              {
                "kind": "Literal",
                "name": "combineDeskTypesZones",
                "value": true
              },
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
              (v18/*: any*/),
              (v11/*: any*/),
              (v19/*: any*/)
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "BookingDeskDetails",
        "kind": "LinkedField",
        "name": "availableDesks",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingLocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": (v22/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "2f20c70bb889fc2c85235e435a2643c0",
    "id": null,
    "metadata": {},
    "name": "locations_rootQuery",
    "operationKind": "query",
    "text": "query locations_rootQuery(\n  $organizationId: String!\n  $locationsSortingValues: [LocationOrderInput!]\n  $zonesSortingValues: [OrganizationTagOrderInput!]\n  $deskTypesSortingValues: [OrganizationTagOrderInput!]\n  $todayDate: DateTime!\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $zoneIds: [String!]\n  $deskTypeIds: [String!]\n) {\n  organization(id: $organizationId) {\n    canModify\n    id\n  }\n  ...deskTypeSelector_allDeskTypes_query\n  ...zoneSelector_allZones_query\n  ...myLocations_query\n  ...myLocations_locations_availableOrganizationDesks_query\n}\n\nfragment deskTypeSelector_allDeskTypes_query on Query {\n  deskTypes(where: {organizationId: $organizationId}, orderBy: $deskTypesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment myLocationCard_LocationDetails on LocationDetails {\n  id\n  name\n  deskTypes {\n    uniqueId\n    name\n  }\n  zones {\n    uniqueId\n    name\n  }\n  desks {\n    id\n  }\n  physicalAddress {\n    formattedAddress\n  }\n  hasFutureBooking\n  canModify\n  canDelete\n  organization {\n    uniqueId\n  }\n}\n\nfragment myLocations_locations_availableOrganizationDesks_query on Query {\n  locations(where: {organizationId: $organizationId, zoneIds: $zoneIds, deskTypeIds: $deskTypeIds}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        deskTypes {\n          uniqueId\n          name\n        }\n        zones {\n          uniqueId\n          name\n        }\n        desks {\n          id\n        }\n        physicalAddress {\n          formattedAddress\n        }\n        hasFutureBooking\n        canModify\n        canDelete\n        organization {\n          uniqueId\n        }\n        ...myLocationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  availableDesks(where: {organizationId: $organizationId, date: $todayDate, deskIdsToInclude: [], zoneIds: $zoneIds, deskTypeIds: $deskTypeIds, combineDeskTypesZones: true}) {\n    location {\n      uniqueId\n    }\n  }\n}\n\nfragment myLocations_query on Query {\n  organizationMembers(where: {organizationId: $organizationId}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n      }\n    }\n  }\n}\n\nfragment zoneSelector_allZones_query on Query {\n  zones(where: {organizationId: $organizationId}, orderBy: $zonesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "31c4c271f5f77bdc7ac1befdbef19e0a";

export default node;
