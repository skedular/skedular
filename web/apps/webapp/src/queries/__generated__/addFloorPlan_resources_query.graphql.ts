/**
 * @generated SignedSource<<1ca1965561be4df38d6038e4d185231f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type addFloorPlan_resources_query$data = {
  readonly location: {
    readonly resources: {
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly capacity: number;
          readonly color: string | null | undefined;
          readonly customTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly id: string;
          readonly inactive: boolean;
          readonly name: string;
          readonly productTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly resourceType: {
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
            readonly tagType: string;
          };
          readonly zones: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
        };
      }>;
    };
  } | null | undefined;
  readonly " $fragmentType": "addFloorPlan_resources_query";
};
export type addFloorPlan_resources_query$key = {
  readonly " $data"?: addFloorPlan_resources_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"addFloorPlan_resources_query">;
};

import addFloorPlan_resources_refetchableFragment_graphql from './addFloorPlan_resources_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = [
  "location",
  "resources"
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v4 = [
  (v1/*: any*/),
  (v2/*: any*/),
  (v3/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "defaultValue": null,
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
      "name": "floorPlanId"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "resourcesSortingValues"
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
      "operation": addFloorPlan_resources_refetchableFragment_graphql
    }
  },
  "name": "addFloorPlan_resources_query",
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
          "alias": "resources",
          "args": [
            {
              "kind": "Variable",
              "name": "orderBy",
              "variableName": "resourcesSortingValues"
            },
            {
              "fields": [
                {
                  "kind": "Variable",
                  "name": "floorPlanId",
                  "variableName": "floorPlanId"
                }
              ],
              "kind": "ObjectValue",
              "name": "where"
            }
          ],
          "concreteType": "ConnectionOfResourceEdge",
          "kind": "LinkedField",
          "name": "__addFloorPlanResourcesQuery_resources_connection",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "ResourceEdge",
              "kind": "LinkedField",
              "name": "edges",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "ResourceDetails",
                  "kind": "LinkedField",
                  "name": "node",
                  "plural": false,
                  "selections": [
                    (v1/*: any*/),
                    (v2/*: any*/),
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "inactive",
                      "storageKey": null
                    },
                    (v3/*: any*/),
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "capacity",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "OrganizationTagDetails",
                      "kind": "LinkedField",
                      "name": "customTags",
                      "plural": true,
                      "selections": (v4/*: any*/),
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "OrganizationTagDetails",
                      "kind": "LinkedField",
                      "name": "zones",
                      "plural": true,
                      "selections": (v4/*: any*/),
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "OrganizationTagDetails",
                      "kind": "LinkedField",
                      "name": "productTags",
                      "plural": true,
                      "selections": (v4/*: any*/),
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "OrganizationTagDetails",
                      "kind": "LinkedField",
                      "name": "resourceType",
                      "plural": false,
                      "selections": [
                        (v1/*: any*/),
                        (v2/*: any*/),
                        (v3/*: any*/),
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
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "2a5d7f5540acd80534fa766f797e76db";

export default node;
