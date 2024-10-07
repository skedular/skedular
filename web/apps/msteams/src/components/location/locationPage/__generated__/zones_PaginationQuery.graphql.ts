/**
 * @generated SignedSource<<a6c2ca5d275b649b31ed2dcfd1ba5601>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationTagOrderField = "description" | "name" | "tagType" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type LocationTagOrderInput = {
  direction: OrderDirection;
  field: LocationTagOrderField;
};
export type zones_PaginationQuery$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  locationExists: boolean;
  locationId: string;
  zoneNameSearchText?: string | null | undefined;
  zoneSortingValues?: ReadonlyArray<LocationTagOrderInput> | null | undefined;
  zoneTagType?: string | null | undefined;
};
export type zones_PaginationQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationZonesTab_query">;
};
export type zones_PaginationQuery = {
  response: zones_PaginationQuery$data;
  variables: zones_PaginationQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": 50,
    "kind": "LocalArgument",
    "name": "count"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "cursor"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationExists"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "zoneNameSearchText"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "zoneSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "zoneTagType"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
  {
    "kind": "Variable",
    "name": "after",
    "variableName": "cursor"
  },
  {
    "kind": "Variable",
    "name": "first",
    "variableName": "count"
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "zones_PaginationQuery",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "count",
            "variableName": "count"
          },
          {
            "kind": "Variable",
            "name": "cursor",
            "variableName": "cursor"
          }
        ],
        "kind": "FragmentSpread",
        "name": "locationZonesTab_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "zones_PaginationQuery",
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
          (v1/*: any*/)
        ],
        "storageKey": null
      },
      {
        "condition": "locationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": "locationZonesTabPaginatedTags",
            "args": (v2/*: any*/),
            "concreteType": "LocationTagConnection",
            "kind": "LinkedField",
            "name": "paginatedLocationTags",
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
                      (v1/*: any*/),
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
            "alias": "locationZonesTabPaginatedTags",
            "args": (v2/*: any*/),
            "filters": [
              "where",
              "orderBy"
            ],
            "handle": "connection",
            "key": "locationZonesTab_locationZonesTabPaginatedTags",
            "kind": "LinkedHandle",
            "name": "paginatedLocationTags"
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
          (v1/*: any*/),
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
      }
    ]
  },
  "params": {
    "cacheID": "da4702d3fcee88a49ee19a74838e98c3",
    "id": null,
    "metadata": {},
    "name": "zones_PaginationQuery",
    "operationKind": "query",
    "text": "query zones_PaginationQuery(\n  $count: Int = 50\n  $cursor: String\n  $locationExists: Boolean!\n  $locationId: String!\n  $zoneNameSearchText: String\n  $zoneSortingValues: [LocationTagOrderInput!]\n  $zoneTagType: String\n) {\n  ...locationZonesTab_query_1G22uz\n}\n\nfragment locationZonesTab_query_1G22uz on Query {\n  location(id: $locationId) {\n    canModify\n    id\n  }\n  locationZonesTabPaginatedTags: paginatedLocationTags(first: $count, after: $cursor, where: {locationId: $locationId, tagType: $zoneTagType, nameContains: $zoneNameSearchText}, orderBy: $zoneSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...zoneCard_LocationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...zoneCard_Query\n}\n\nfragment zoneCard_LocationTagDetails on LocationTagDetails {\n  id\n  name\n}\n\nfragment zoneCard_Query on Query {\n  me {\n    id\n    preferredZones {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    canModify\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "c46717f85ec598cfd15e8f9d7860acda";

export default node;
