/**
 * @generated SignedSource<<64fb6d15073a66eab298c3242631346d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateLocationTagInput = {
  clientMutationId?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
  tagType: string;
};
export type oldZoneCard_updateLocationTagMutation$variables = {
  input: UpdateLocationTagInput;
};
export type oldZoneCard_updateLocationTagMutation$data = {
  readonly updateLocationTag: {
    readonly locationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type oldZoneCard_updateLocationTagMutation = {
  response: oldZoneCard_updateLocationTagMutation$data;
  variables: oldZoneCard_updateLocationTagMutation$variables;
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
    "concreteType": "LocationTagPayload",
    "kind": "LinkedField",
    "name": "updateLocationTag",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationTagDetails",
        "kind": "LinkedField",
        "name": "locationTag",
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
    "name": "oldZoneCard_updateLocationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "oldZoneCard_updateLocationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "650385acb1d3a3920f3125a644517498",
    "id": null,
    "metadata": {},
    "name": "oldZoneCard_updateLocationTagMutation",
    "operationKind": "mutation",
    "text": "mutation oldZoneCard_updateLocationTagMutation(\n  $input: UpdateLocationTagInput!\n) {\n  updateLocationTag(input: $input) {\n    locationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "46183307b306ac92c93627a8a67e27a2";

export default node;
