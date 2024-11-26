/**
 * @generated SignedSource<<e39018c8462a168d906f4321a5ad3354>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationTagOrderField = "Description" | "Name" | "TagType" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type LocationTagOrderInput = {
  direction: OrderDirection;
  field: LocationTagOrderField;
};
export type locationZonesTab_rootQuery$variables = {
  locationExists: boolean;
  locationId: string;
  zoneNameSearchText?: string | null | undefined;
  zoneSortingValues: ReadonlyArray<LocationTagOrderInput>;
  zoneTagType: string;
};
export type locationZonesTab_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationZonesTab_locationTags_query" | "locationZonesTab_query">;
};
export type locationZonesTab_rootQuery = {
  response: locationZonesTab_rootQuery$data;
  variables: locationZonesTab_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationExists"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneNameSearchText"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneTagType"
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v6 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "zoneSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "locationId",
        "variableName": "locationId"
      },
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "zoneNameSearchText"
      },
      {
        "kind": "Variable",
        "name": "tagType",
        "variableName": "zoneTagType"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "locationZonesTab_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationZonesTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationZonesTab_locationTags_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/),
      (v4/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Operation",
    "name": "locationZonesTab_rootQuery",
    "selections": [
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canModify",
            "storageKey": null
          },
          (v5/*: any*/)
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
          (v5/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerLocationTagDetails",
            "kind": "LinkedField",
            "name": "preferredZones",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "condition": "locationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v6/*: any*/),
            "concreteType": "LocationTagConnection",
            "kind": "LinkedField",
            "name": "locationTags",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "totalCount",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationTagEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationTagDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v5/*: any*/),
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
              {
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
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v6/*: any*/),
            "filters": [
              "where",
              "orderBy"
            ],
            "handle": "connection",
            "key": "locationZonesTab_locationTags",
            "kind": "LinkedHandle",
            "name": "locationTags"
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "06152d228d8a6710949b68936cacbb47",
    "id": null,
    "metadata": {},
    "name": "locationZonesTab_rootQuery",
    "operationKind": "query",
    "text": "query locationZonesTab_rootQuery(\n  $locationId: String!\n  $locationExists: Boolean!\n  $zoneTagType: String!\n  $zoneNameSearchText: String\n  $zoneSortingValues: [LocationTagOrderInput!]!\n) {\n  ...locationZonesTab_query\n  ...locationZonesTab_locationTags_query\n}\n\nfragment locationZonesTab_locationTags_query on Query {\n  locationTags(first: 50, where: {locationId: $locationId, tagType: $zoneTagType, nameContains: $zoneNameSearchText}, orderBy: $zoneSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...oldZoneCard_LocationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationZonesTab_query on Query {\n  location(id: $locationId) {\n    canModify\n    id\n  }\n  ...oldZoneCard_Query\n}\n\nfragment oldZoneCard_LocationTagDetails on LocationTagDetails {\n  id\n  name\n}\n\nfragment oldZoneCard_Query on Query {\n  me {\n    id\n    preferredZones {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    canModify\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "43189d532dca506c66bd6b7d7a1ba098";

export default node;
