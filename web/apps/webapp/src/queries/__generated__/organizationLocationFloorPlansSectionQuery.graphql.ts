/**
 * @generated SignedSource<<d584fa12999d0e7ea20960d738e28b88>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type FloorPlanOrderField = "NAME" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type FloorPlanOrderInput = {
  direction: OrderDirection;
  field: FloorPlanOrderField;
};
export type organizationLocationFloorPlansSectionQuery$variables = {
  floorPlansSortingValues?: ReadonlyArray<FloorPlanOrderInput> | null | undefined;
  locationId: string;
};
export type organizationLocationFloorPlansSectionQuery$data = {
  readonly floorPlans: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
        readonly " $fragmentSpreads": FragmentRefs<"floorPlanCard_FloorPlanDetails">;
      };
    }>;
  };
};
export type organizationLocationFloorPlansSectionQuery = {
  response: organizationLocationFloorPlansSectionQuery$data;
  variables: organizationLocationFloorPlansSectionQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "floorPlansSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "floorPlansSortingValues"
  },
  {
    "fields": [
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
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = {
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
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocationFloorPlansSectionQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ConnectionOfFloorPlanEdge",
        "kind": "LinkedField",
        "name": "floorPlans",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "FloorPlanEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "FloorPlanDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v2/*: any*/),
                  (v3/*: any*/),
                  {
                    "args": null,
                    "kind": "FragmentSpread",
                    "name": "floorPlanCard_FloorPlanDetails"
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v4/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocationFloorPlansSectionQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ConnectionOfFloorPlanEdge",
        "kind": "LinkedField",
        "name": "floorPlans",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "FloorPlanEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "FloorPlanDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v2/*: any*/),
                  (v3/*: any*/),
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
                        "name": "thumbnail",
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
                    "kind": "ScalarField",
                    "name": "resourceCount",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v4/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "eb0f1e41a8c7af2d97baf3162de67e29",
    "id": null,
    "metadata": {},
    "name": "organizationLocationFloorPlansSectionQuery",
    "operationKind": "query",
    "text": "query organizationLocationFloorPlansSectionQuery(\n  $floorPlansSortingValues: [FloorPlanOrderInput!]\n  $locationId: String!\n) {\n  floorPlans(where: {locationId: $locationId}, orderBy: $floorPlansSortingValues) {\n    edges {\n      node {\n        id\n        name\n        ...floorPlanCard_FloorPlanDetails\n      }\n    }\n  }\n}\n\nfragment floorPlanCard_FloorPlanDetails on FloorPlanDetails {\n  id\n  name\n  image {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n  resourceCount\n}\n"
  }
};
})();

(node as any).hash = "f9144fe31c9a09fb8674afc0e87524d2";

export default node;
