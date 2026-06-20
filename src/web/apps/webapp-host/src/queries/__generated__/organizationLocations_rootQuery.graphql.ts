/**
 * @generated SignedSource<<6c67ce762b702f9f787c19aab01bb023>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "NAME" | "TIMEZONE" | "TYPE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type organizationLocations_rootQuery$variables = {
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationCustomDomain: string;
};
export type organizationLocations_rootQuery$data = {
  readonly locations: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly " $fragmentSpreads": FragmentRefs<"locationCard_LocationDetails">;
      };
    }>;
  };
  readonly organization: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"locationCard_query" | "newLocationButton_query">;
};
export type organizationLocations_rootQuery = {
  response: organizationLocations_rootQuery$data;
  variables: organizationLocations_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v2 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
},
v4 = {
  "kind": "Variable",
  "name": "orderBy",
  "variableName": "locationsSortingValues"
},
v5 = {
  "fields": [
    {
      "kind": "Variable",
      "name": "organizationCustomDomain",
      "variableName": "organizationCustomDomain"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v9 = {
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
v10 = {
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
v11 = [
  (v6/*:: as any*/)
],
v12 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 100
  },
  (v4/*:: as any*/),
  (v5/*:: as any*/)
],
v13 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "url",
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocations_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v2/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newLocationButton_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationCard_query"
      },
      {
        "alias": "locations",
        "args": [
          (v4/*:: as any*/),
          (v5/*:: as any*/)
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "__organizationLocations_locations_connection",
        "plural": false,
        "selections": [
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
                  (v6/*:: as any*/),
                  {
                    "args": null,
                    "kind": "FragmentSpread",
                    "name": "locationCard_LocationDetails"
                  },
                  (v7/*:: as any*/)
                ],
                "storageKey": null
              },
              (v8/*:: as any*/)
            ],
            "storageKey": null
          },
          (v9/*:: as any*/),
          (v10/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*:: as any*/),
      (v0/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "organizationLocations_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v2/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*:: as any*/),
          (v6/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "type",
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
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "preferredLocations",
            "plural": true,
            "selections": (v11/*:: as any*/),
            "storageKey": null
          },
          (v6/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v12/*:: as any*/),
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
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
                  (v6/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "name",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ProductDetails",
                    "kind": "LinkedField",
                    "name": "products",
                    "plural": true,
                    "selections": (v11/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationPhysicalAddressDetails",
                    "kind": "LinkedField",
                    "name": "physicalAddress",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "multilinesFormattedAddress",
                        "storageKey": null
                      },
                      (v6/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CdnImageFile",
                    "kind": "LinkedField",
                    "name": "featureImages",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CdnFile",
                        "kind": "LinkedField",
                        "name": "original",
                        "plural": false,
                        "selections": (v13/*:: as any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CdnFile",
                        "kind": "LinkedField",
                        "name": "thumbnail",
                        "plural": false,
                        "selections": (v13/*:: as any*/),
                        "storageKey": null
                      }
                    ],
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
                    "concreteType": "OrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "customDomain",
                        "storageKey": null
                      },
                      (v6/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "uniqueClaimCode",
                    "storageKey": null
                  },
                  (v7/*:: as any*/)
                ],
                "storageKey": null
              },
              (v8/*:: as any*/)
            ],
            "storageKey": null
          },
          (v9/*:: as any*/),
          (v10/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v12/*:: as any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationLocations_locations",
        "kind": "LinkedHandle",
        "name": "locations"
      }
    ]
  },
  "params": {
    "cacheID": "c02b0729e930eba3ca68033fd0095b5d",
    "id": null,
    "metadata": {
      "connection": [
        {
          "count": null,
          "cursor": null,
          "direction": "forward",
          "path": [
            "locations"
          ]
        }
      ]
    },
    "name": "organizationLocations_rootQuery",
    "operationKind": "query",
    "text": "query organizationLocations_rootQuery(\n  $organizationCustomDomain: String!\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    canModify\n    id\n  }\n  ...newLocationButton_query\n  ...locationCard_query\n  locations(first: 100, where: {organizationCustomDomain: $organizationCustomDomain}, orderBy: $locationsSortingValues) {\n    edges {\n      node {\n        id\n        ...locationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationCard_LocationDetails on LocationDetails {\n  id\n  name\n  products {\n    id\n  }\n  physicalAddress {\n    multilinesFormattedAddress\n    id\n  }\n  featureImages {\n    original {\n      url\n    }\n    thumbnail {\n      url\n    }\n  }\n  canDelete\n  organization {\n    customDomain\n    id\n  }\n  uniqueClaimCode\n}\n\nfragment locationCard_query on Query {\n  me {\n    preferredLocations {\n      id\n    }\n    id\n  }\n}\n\nfragment newLocationButton_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    type {\n      type\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "a10d1375adf6c070ce6884dd3b73d75b";

export default node;
