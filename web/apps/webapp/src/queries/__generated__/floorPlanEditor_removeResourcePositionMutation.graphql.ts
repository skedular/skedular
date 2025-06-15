/**
 * @generated SignedSource<<6366c2b60fca2ccef4b0cd951675f3a1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveResourcePositionInput = {
  clientMutationId?: string | null | undefined;
  resourceId: string;
};
export type floorPlanEditor_removeResourcePositionMutation$variables = {
  input: RemoveResourcePositionInput;
};
export type floorPlanEditor_removeResourcePositionMutation$data = {
  readonly removeResourcePosition: {
    readonly clientMutationId: string | null | undefined;
    readonly success: boolean;
  };
};
export type floorPlanEditor_removeResourcePositionMutation = {
  response: floorPlanEditor_removeResourcePositionMutation$data;
  variables: floorPlanEditor_removeResourcePositionMutation$variables;
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
    "concreteType": "RemoveResourcePositionPayload",
    "kind": "LinkedField",
    "name": "removeResourcePosition",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "clientMutationId",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "success",
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
    "name": "floorPlanEditor_removeResourcePositionMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "floorPlanEditor_removeResourcePositionMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "d30fda2e93b147688d7cd86e2aac8758",
    "id": null,
    "metadata": {},
    "name": "floorPlanEditor_removeResourcePositionMutation",
    "operationKind": "mutation",
    "text": "mutation floorPlanEditor_removeResourcePositionMutation(\n  $input: RemoveResourcePositionInput!\n) {\n  removeResourcePosition(input: $input) {\n    clientMutationId\n    success\n  }\n}\n"
  }
};
})();

(node as any).hash = "810b877ae424bad3b59a5495933b19f5";

export default node;
