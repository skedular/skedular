/**
 * @generated SignedSource<<59439254847c7b40c51dd001668db4a7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateResourcePositionsInput = {
  clientMutationId?: string | null | undefined;
  floorPlanId: string;
  positions: ReadonlyArray<ResourcePositionInput>;
};
export type ResourcePositionInput = {
  height: number;
  id?: string | null | undefined;
  metadata?: string | null | undefined;
  resourceId: string;
  shape?: string | null | undefined;
  width: number;
  x: number;
  y: number;
};
export type floorPlanEditor_updateResourcePositionsMutation$variables = {
  input: UpdateResourcePositionsInput;
};
export type floorPlanEditor_updateResourcePositionsMutation$data = {
  readonly updateResourcePositions: {
    readonly resourcePositions: ReadonlyArray<{
      readonly height: number;
      readonly id: string;
      readonly metadata: string | null | undefined;
      readonly resource: {
        readonly id: string;
        readonly name: string;
      };
      readonly shape: string | null | undefined;
      readonly width: number;
      readonly x: number;
      readonly y: number;
    }>;
  };
};
export type floorPlanEditor_updateResourcePositionsMutation = {
  response: floorPlanEditor_updateResourcePositionsMutation$data;
  variables: floorPlanEditor_updateResourcePositionsMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
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
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "UpdateResourcePositionsPayload",
    "kind": "LinkedField",
    "name": "updateResourcePositions",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "ResourcePosition",
        "kind": "LinkedField",
        "name": "resourcePositions",
        "plural": true,
        "selections": [
          (v1/*: any*/),
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
            "kind": "ScalarField",
            "name": "width",
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
            "name": "shape",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "metadata",
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
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "name",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "floorPlanEditor_updateResourcePositionsMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "floorPlanEditor_updateResourcePositionsMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "4e89bf9bbe2f76ecf7fccd6c0edce833",
    "id": null,
    "metadata": {},
    "name": "floorPlanEditor_updateResourcePositionsMutation",
    "operationKind": "mutation",
    "text": "mutation floorPlanEditor_updateResourcePositionsMutation(\n  $input: UpdateResourcePositionsInput!\n) {\n  updateResourcePositions(input: $input) {\n    resourcePositions {\n      id\n      x\n      y\n      width\n      height\n      shape\n      metadata\n      resource {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "41506aaa5c88f55953323f40bff2daec";

export default node;
