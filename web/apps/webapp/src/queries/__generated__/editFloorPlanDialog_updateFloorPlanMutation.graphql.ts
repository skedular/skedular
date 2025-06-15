/**
 * @generated SignedSource<<8d01dd860cd9277c64690789f6f74453>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateFloorPlanInput = {
  clientMutationId?: string | null | undefined;
  floorLevel: number;
  floorName?: string | null | undefined;
  id: string;
  isActive: boolean;
  name: string;
};
export type editFloorPlanDialog_updateFloorPlanMutation$variables = {
  input: UpdateFloorPlanInput;
};
export type editFloorPlanDialog_updateFloorPlanMutation$data = {
  readonly updateFloorPlan: {
    readonly floorPlan: {
      readonly floorLevel: number;
      readonly floorName: string | null | undefined;
      readonly height: number;
      readonly id: string;
      readonly imagePath: string;
      readonly isActive: boolean;
      readonly name: string;
      readonly thumbnailPath: string | null | undefined;
      readonly width: number;
    };
  };
};
export type editFloorPlanDialog_updateFloorPlanMutation = {
  response: editFloorPlanDialog_updateFloorPlanMutation$data;
  variables: editFloorPlanDialog_updateFloorPlanMutation$variables;
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
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "FloorPlanPayload",
    "kind": "LinkedField",
    "name": "updateFloorPlan",
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
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
            "name": "floorLevel",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "floorName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "imagePath",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "thumbnailPath",
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
            "name": "isActive",
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
    "name": "editFloorPlanDialog_updateFloorPlanMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editFloorPlanDialog_updateFloorPlanMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "c4251bc0be4dc477e03a8a099059a77c",
    "id": null,
    "metadata": {},
    "name": "editFloorPlanDialog_updateFloorPlanMutation",
    "operationKind": "mutation",
    "text": "mutation editFloorPlanDialog_updateFloorPlanMutation(\n  $input: UpdateFloorPlanInput!\n) {\n  updateFloorPlan(input: $input) {\n    floorPlan {\n      id\n      name\n      floorLevel\n      floorName\n      imagePath\n      thumbnailPath\n      width\n      height\n      isActive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ebe61034bd829c02618ff178c1721fd8";

export default node;
