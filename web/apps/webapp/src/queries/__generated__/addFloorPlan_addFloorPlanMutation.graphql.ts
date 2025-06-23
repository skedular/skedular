/**
 * @generated SignedSource<<e438904fa194311ab764d22537a4d83b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddFloorPlanInput = {
  clientMutationId?: string | null | undefined;
  id?: string | null | undefined;
  image: CdnImageFileInput;
  locationId: string;
  name: string;
  resourcePositions?: ReadonlyArray<ResourcePositionInput> | null | undefined;
};
export type CdnImageFileInput = {
  original?: CdnFileInput | null | undefined;
  thumbnail?: CdnFileInput | null | undefined;
};
export type CdnFileInput = {
  height?: number | null | undefined;
  url: string;
  width?: number | null | undefined;
};
export type ResourcePositionInput = {
  resourceId: string;
  x: number;
  y: number;
};
export type addFloorPlan_addFloorPlanMutation$variables = {
  input: AddFloorPlanInput;
};
export type addFloorPlan_addFloorPlanMutation$data = {
  readonly addFloorPlan: {
    readonly floorPlan: {
      readonly id: string;
      readonly image: {
        readonly original: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
        readonly thumbnail: {
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
    };
  };
};
export type addFloorPlan_addFloorPlanMutation$rawResponse = {
  readonly addFloorPlan: {
    readonly floorPlan: {
      readonly id: string;
      readonly image: {
        readonly original: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
        readonly thumbnail: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
      };
      readonly name: string;
      readonly resourcePositions: ReadonlyArray<{
        readonly id: string;
        readonly resource: {
          readonly id: string;
        };
        readonly x: number;
        readonly y: number;
      }>;
    };
  };
};
export type addFloorPlan_addFloorPlanMutation = {
  rawResponse: addFloorPlan_addFloorPlanMutation$rawResponse;
  response: addFloorPlan_addFloorPlanMutation$data;
  variables: addFloorPlan_addFloorPlanMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
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
v4 = [
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
v5 = {
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
      "selections": (v4/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CdnFile",
      "kind": "LinkedField",
      "name": "thumbnail",
      "plural": false,
      "selections": (v4/*: any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "x",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "y",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "ResourceDetails",
  "kind": "LinkedField",
  "name": "resource",
  "plural": false,
  "selections": [
    (v2/*: any*/)
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addFloorPlan_addFloorPlanMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "FloorPlanPayload",
        "kind": "LinkedField",
        "name": "addFloorPlan",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "FloorPlanDetails",
            "kind": "LinkedField",
            "name": "floorPlan",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              (v3/*: any*/),
              (v5/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ResourcePositionDetails",
                "kind": "LinkedField",
                "name": "resourcePositions",
                "plural": true,
                "selections": [
                  (v6/*: any*/),
                  (v7/*: any*/),
                  (v8/*: any*/)
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
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addFloorPlan_addFloorPlanMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "FloorPlanPayload",
        "kind": "LinkedField",
        "name": "addFloorPlan",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "FloorPlanDetails",
            "kind": "LinkedField",
            "name": "floorPlan",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              (v3/*: any*/),
              (v5/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ResourcePositionDetails",
                "kind": "LinkedField",
                "name": "resourcePositions",
                "plural": true,
                "selections": [
                  (v6/*: any*/),
                  (v7/*: any*/),
                  (v8/*: any*/),
                  (v2/*: any*/)
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
  },
  "params": {
    "cacheID": "6f3ea2102f14a67080bef3177a127db7",
    "id": null,
    "metadata": {},
    "name": "addFloorPlan_addFloorPlanMutation",
    "operationKind": "mutation",
    "text": "mutation addFloorPlan_addFloorPlanMutation(\n  $input: AddFloorPlanInput!\n) {\n  addFloorPlan(input: $input) {\n    floorPlan {\n      id\n      name\n      image {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n      resourcePositions {\n        x\n        y\n        resource {\n          id\n        }\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3eec6b3b0ea1558583f4aea7a48ec3b1";

export default node;
