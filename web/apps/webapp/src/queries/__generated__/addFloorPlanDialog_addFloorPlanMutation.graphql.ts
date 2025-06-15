/**
 * @generated SignedSource<<e02752bb6699fd481937e9fc1f6a4f83>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddFloorPlanInput = {
  clientMutationId?: string | null | undefined;
  floorLevel: number;
  floorName?: string | null | undefined;
  imageBase64: string;
  imageFileName: string;
  locationId: string;
  name: string;
};
export type addFloorPlanDialog_addFloorPlanMutation$variables = {
  input: AddFloorPlanInput;
};
export type addFloorPlanDialog_addFloorPlanMutation$data = {
  readonly addFloorPlan: {
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
export type addFloorPlanDialog_addFloorPlanMutation = {
  response: addFloorPlanDialog_addFloorPlanMutation$data;
  variables: addFloorPlanDialog_addFloorPlanMutation$variables;
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
    "name": "addFloorPlanDialog_addFloorPlanMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addFloorPlanDialog_addFloorPlanMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "ff7eacd5f474134a079c62c627612b6a",
    "id": null,
    "metadata": {},
    "name": "addFloorPlanDialog_addFloorPlanMutation",
    "operationKind": "mutation",
    "text": "mutation addFloorPlanDialog_addFloorPlanMutation(\n  $input: AddFloorPlanInput!\n) {\n  addFloorPlan(input: $input) {\n    floorPlan {\n      id\n      name\n      floorLevel\n      floorName\n      imagePath\n      thumbnailPath\n      width\n      height\n      isActive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "00633a9f4860440f6a12b73603c2f604";

export default node;
