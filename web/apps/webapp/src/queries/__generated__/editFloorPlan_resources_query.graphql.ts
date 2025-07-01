/**
 * @generated SignedSource<<815e671b26ecf867f3733ce05452e71f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editFloorPlan_resources_query$data = {
  readonly resources: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly capacity: number;
        readonly color: string | null | undefined;
        readonly customTags: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly id: string;
        readonly inactive: boolean;
        readonly name: string;
        readonly productTags: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly resourceType: {
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly tagType: string | null | undefined;
          readonly uniqueId: string;
        };
        readonly zones: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
      };
    }>;
  };
  readonly " $fragmentType": "editFloorPlan_resources_query";
};
export type editFloorPlan_resources_query$key = {
  readonly " $data"?: editFloorPlan_resources_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editFloorPlan_resources_query">;
};

import editFloorPlan_resources_refetchableFragment_graphql from './editFloorPlan_resources_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = [
  "resources"
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v4 = [
  (v3/*: any*/),
  (v1/*: any*/),
  (v2/*: any*/)
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
      "operation": editFloorPlan_resources_refetchableFragment_graphql
    }
  },
  "name": "editFloorPlan_resources_query",
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
            },
            {
              "kind": "Variable",
              "name": "locationId",
              "variableName": "locationId"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ConnectionOfResourceEdge",
      "kind": "LinkedField",
      "name": "__editFloorPlanResourcesQuery_resources_connection",
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
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "id",
                  "storageKey": null
                },
                (v1/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "inactive",
                  "storageKey": null
                },
                (v2/*: any*/),
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
                  "concreteType": "Location_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "customTags",
                  "plural": true,
                  "selections": (v4/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Location_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "zones",
                  "plural": true,
                  "selections": (v4/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Location_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "productTags",
                  "plural": true,
                  "selections": (v4/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Location_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "resourceType",
                  "plural": false,
                  "selections": [
                    (v3/*: any*/),
                    (v1/*: any*/),
                    (v2/*: any*/),
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
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "b5d1f002d859dda9a0b3cb84a99f2176";

export default node;
