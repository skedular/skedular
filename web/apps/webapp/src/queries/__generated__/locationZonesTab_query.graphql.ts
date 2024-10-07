/**
 * @generated SignedSource<<c2f4cca8badeb7014561b38b0b42a797>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationZonesTab_query$data = {
  readonly location: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly locationZonesTabPaginatedTags?: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly " $fragmentSpreads": FragmentRefs<"zoneCard_LocationTagDetails">;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"zoneCard_Query">;
  readonly " $fragmentType": "locationZonesTab_query";
};
export type locationZonesTab_query$key = {
  readonly " $data"?: locationZonesTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationZonesTab_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  "locationZonesTabPaginatedTags"
];
return {
  "argumentDefinitions": [
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
      "kind": "RootArgument",
      "name": "locationExists"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "zoneNameSearchText"
    },
    {
      "kind": "RootArgument",
      "name": "zoneSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "zoneTagType"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "connection": [
      {
        "count": "count",
        "cursor": "cursor",
        "direction": "forward",
        "path": (v0/*: any*/)
      }
    ],
    "refetch": {
      "connection": {
        "forward": {
          "count": "count",
          "cursor": "cursor"
        },
        "backward": null,
        "path": (v0/*: any*/)
      },
      "fragmentPathInResult": [],
      "operation": require('./zones_PaginationQuery.graphql')
    }
  },
  "name": "locationZonesTab_query",
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
          "alias": "locationZonesTabPaginatedTags",
          "args": [
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
          ],
          "concreteType": "LocationTagConnection",
          "kind": "LinkedField",
          "name": "__locationZonesTab_locationZonesTabPaginatedTags_connection",
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
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "id",
                      "storageKey": null
                    },
                    {
                      "args": null,
                      "kind": "FragmentSpread",
                      "name": "zoneCard_LocationTagDetails"
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
        }
      ]
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "zoneCard_Query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "c46717f85ec598cfd15e8f9d7860acda";

export default node;
