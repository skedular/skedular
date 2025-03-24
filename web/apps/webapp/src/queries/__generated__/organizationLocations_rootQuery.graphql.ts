/**
 * @generated SignedSource<<c1ebac8a2115d5daa2964b612a79fdb8>>
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
export type OrganizationTagOrderField = "Description" | "Name" | "Type" | "%future added value";
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
export type organizationLocations_rootQuery$variables = {
  customTagIds?: ReadonlyArray<string> | null | undefined;
  customTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  fromTodayDate: any;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationId: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  untilTodayDate: any;
  zoneIds?: ReadonlyArray<string> | null | undefined;
  zonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
};
export type organizationLocations_rootQuery$data = {
  readonly me: {
    readonly id: string;
    readonly preferredLocations: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
  } | null | undefined;
  readonly organization: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly organizationMembers: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly customer: {
          readonly familyName: string | null | undefined;
          readonly givenName: string | null | undefined;
          readonly middleName: string | null | undefined;
          readonly name: string | null | undefined;
          readonly photoUrl: string | null | undefined;
          readonly uniqueId: string;
        };
        readonly id: string;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"customTagSelector_allCustomTags_query" | "locationCard_query" | "organizationLocations_locations_availableOrganizationResources_query" | "zoneSelector_allZones_query">;
};
export type organizationLocations_rootQuery = {
  response: organizationLocations_rootQuery$data;
  variables: organizationLocations_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagIds"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagsSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "fromTodayDate"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "untilTodayDate"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneIds"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zonesSortingValues"
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
  "name": "uniqueId",
  "storageKey": null
},
v11 = [
  (v10/*: any*/)
],
v12 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v9/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerLocationDetails",
      "kind": "LinkedField",
      "name": "preferredLocations",
      "plural": true,
      "selections": (v11/*: any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v13 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v14 = {
  "fields": [
    (v13/*: any*/)
  ],
  "kind": "ObjectValue",
  "name": "where"
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
v17 = {
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
v18 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "orderBy",
      "variableName": "organizationMembersSortingValues"
    },
    (v14/*: any*/)
  ],
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
            (v9/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationCustomerDetails",
              "kind": "LinkedField",
              "name": "customer",
              "plural": false,
              "selections": [
                (v10/*: any*/),
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
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    (v17/*: any*/)
  ],
  "storageKey": null
},
v19 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationId"
  }
],
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v22 = [
  (v15/*: any*/),
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
          (v9/*: any*/),
          (v16/*: any*/),
          (v21/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  (v17/*: any*/)
],
v23 = {
  "kind": "Variable",
  "name": "customTagIds",
  "variableName": "customTagIds"
},
v24 = {
  "kind": "Variable",
  "name": "zoneIds",
  "variableName": "zoneIds"
},
v25 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationsSortingValues"
  },
  {
    "fields": [
      (v23/*: any*/),
      (v13/*: any*/),
      (v24/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v26 = [
  (v10/*: any*/),
  (v16/*: any*/),
  (v21/*: any*/)
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
    "name": "organizationLocations_rootQuery",
    "selections": [
      (v12/*: any*/),
      (v18/*: any*/),
      {
        "alias": null,
        "args": (v19/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v20/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationCard_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "customTagSelector_allCustomTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "zoneSelector_allZones_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationLocations_locations_availableOrganizationResources_query"
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
      (v8/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v6/*: any*/),
      (v5/*: any*/),
      (v7/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "organizationLocations_rootQuery",
    "selections": [
      (v12/*: any*/),
      (v18/*: any*/),
      {
        "alias": null,
        "args": (v19/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v20/*: any*/),
          (v9/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "customTagsSortingValues"
          },
          (v14/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "customTags",
        "plural": false,
        "selections": (v22/*: any*/),
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
          (v14/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v22/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v25/*: any*/),
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
                "selections": [
                  (v9/*: any*/),
                  (v16/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Location_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "customTags",
                    "plural": true,
                    "selections": (v26/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Location_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "zones",
                    "plural": true,
                    "selections": (v26/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ResourceDetails",
                    "kind": "LinkedField",
                    "name": "resources",
                    "plural": true,
                    "selections": [
                      (v9/*: any*/)
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
                  (v20/*: any*/),
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
                    "selections": (v11/*: any*/),
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
          (v17/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v25/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationLocations_locations",
        "kind": "LinkedHandle",
        "name": "locations"
      },
      {
        "alias": null,
        "args": [
          {
            "fields": [
              (v23/*: any*/),
              {
                "kind": "Variable",
                "name": "from",
                "variableName": "fromTodayDate"
              },
              (v13/*: any*/),
              {
                "kind": "Variable",
                "name": "until",
                "variableName": "untilTodayDate"
              },
              (v24/*: any*/)
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
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingLocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": (v11/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "d1d0613f085de6f5d596976df312d019",
    "id": null,
    "metadata": {},
    "name": "organizationLocations_rootQuery",
    "operationKind": "query",
    "text": "query organizationLocations_rootQuery(\n  $organizationId: String!\n  $locationsSortingValues: [LocationOrderInput!]\n  $zonesSortingValues: [OrganizationTagOrderInput!]\n  $customTagsSortingValues: [OrganizationTagOrderInput!]\n  $fromTodayDate: DateTime!\n  $untilTodayDate: DateTime!\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $zoneIds: [String!]\n  $customTagIds: [String!]\n) {\n  me {\n    id\n    preferredLocations {\n      uniqueId\n    }\n  }\n  organizationMembers(where: {organizationId: $organizationId}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n      }\n    }\n  }\n  organization(id: $organizationId) {\n    canModify\n    id\n  }\n  ...locationCard_query\n  ...customTagSelector_allCustomTags_query\n  ...zoneSelector_allZones_query\n  ...organizationLocations_locations_availableOrganizationResources_query\n}\n\nfragment customTagSelector_allCustomTags_query on Query {\n  customTags(where: {organizationId: $organizationId}, orderBy: $customTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n\nfragment locationCard_LocationDetails on LocationDetails {\n  id\n  name\n  customTags {\n    uniqueId\n    name\n    color\n  }\n  zones {\n    uniqueId\n    name\n    color\n  }\n  resources {\n    id\n  }\n  physicalAddress {\n    formattedAddress\n  }\n  hasFutureBooking\n  canModify\n  canDelete\n  organization {\n    uniqueId\n  }\n}\n\nfragment locationCard_query on Query {\n  me {\n    id\n    preferredLocations {\n      uniqueId\n    }\n  }\n}\n\nfragment organizationLocations_locations_availableOrganizationResources_query on Query {\n  locations(where: {organizationId: $organizationId, zoneIds: $zoneIds, customTagIds: $customTagIds}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        customTags {\n          uniqueId\n          name\n          color\n        }\n        zones {\n          uniqueId\n          name\n          color\n        }\n        resources {\n          id\n        }\n        physicalAddress {\n          formattedAddress\n        }\n        hasFutureBooking\n        canModify\n        canDelete\n        organization {\n          uniqueId\n        }\n        ...locationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  availableResources(where: {organizationId: $organizationId, from: $fromTodayDate, until: $untilTodayDate, zoneIds: $zoneIds, customTagIds: $customTagIds}) {\n    location {\n      uniqueId\n    }\n  }\n}\n\nfragment zoneSelector_allZones_query on Query {\n  zones(where: {organizationId: $organizationId}, orderBy: $zonesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2bac4973ad94e453432ef3b0aa7219f9";

export default node;
