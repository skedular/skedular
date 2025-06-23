/**
 * @generated SignedSource<<aedde787b563d8a41e2a4387067dcfa6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type floorPlans_floorPlan_query$data = {
  readonly floorPlan?: {
    readonly id: string;
    readonly image: {
      readonly original: {
        readonly height: number | null | undefined;
        readonly url: string;
        readonly width: number | null | undefined;
      } | null | undefined;
    };
    readonly name: string;
    readonly resourcePositions: ReadonlyArray<{
      readonly resource: {
        readonly id: string;
      };
      readonly x: number;
      readonly y: number;
    }>;
  } | null | undefined;
  readonly resources?: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
        readonly resourceType: {
          readonly tagType: string | null | undefined;
        };
        readonly " $fragmentSpreads": FragmentRefs<"resourceCard_ResourceDetails">;
      };
    }>;
  };
  readonly " $fragmentType": "floorPlans_floorPlan_query";
};
export type floorPlans_floorPlan_query$key = {
  readonly " $data"?: floorPlans_floorPlan_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"floorPlans_floorPlan_query">;
};

import floorPlans_floorPlan_refetchableFragment_graphql from './floorPlans_floorPlan_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "floorPlanExists"
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
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": floorPlans_floorPlan_refetchableFragment_graphql
    }
  },
  "name": "floorPlans_floorPlan_query",
  "selections": [
    {
      "condition": "floorPlanExists",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": null,
          "args": [
            {
              "kind": "Variable",
              "name": "id",
              "variableName": "floorPlanId"
            }
          ],
          "concreteType": "FloorPlanDetails",
          "kind": "LinkedField",
          "name": "floorPlan",
          "plural": false,
          "selections": [
            (v0/*: any*/),
            (v1/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "CdnImageFile",
              "kind": "LinkedField",
              "name": "image",
              "plural": false,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "CdnFile",
                  "kind": "LinkedField",
                  "name": "original",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "url",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "height",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "width",
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
              "concreteType": "ResourcePositionDetails",
              "kind": "LinkedField",
              "name": "resourcePositions",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "x",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "y",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "ResourceDetails",
                  "kind": "LinkedField",
                  "name": "resource",
                  "plural": false,
                  "selections": [
                    (v0/*: any*/)
                  ],
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
          "concreteType": "ResourceConnection",
          "kind": "LinkedField",
          "name": "resources",
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
                    (v0/*: any*/),
                    (v1/*: any*/),
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "Location_OrganizationTagDetails",
                      "kind": "LinkedField",
                      "name": "resourceType",
                      "plural": false,
                      "selections": [
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
                      "args": null,
                      "kind": "FragmentSpread",
                      "name": "resourceCard_ResourceDetails"
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
      ]
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "f012ebe5fecfa93d07f94b2c21ada196";

export default node;
