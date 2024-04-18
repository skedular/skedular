/**
 * @generated SignedSource<<46741450073ad52f45530d99b41bf583>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type UpdateLocationTagInput = {
  clientMutationId?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
  tagType: string;
};
export type zoneCard_updateZoneMutation$variables = {
  input: UpdateLocationTagInput;
};
export type zoneCard_updateZoneMutation$data = {
  readonly updateLocationTag: {
    readonly locationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type zoneCard_updateZoneMutation = {
  response: zoneCard_updateZoneMutation$data;
  variables: zoneCard_updateZoneMutation$variables;
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
    "name": "zoneCard_updateZoneMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "zoneCard_updateZoneMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "9d7156e13ce55b7f13e125bc8f2e5d0e",
    "id": null,
    "metadata": {},
    "name": "zoneCard_updateZoneMutation",
    "operationKind": "mutation",
    "text": "mutation zoneCard_updateZoneMutation(\n  $input: UpdateLocationTagInput!\n) {\n  updateLocationTag(input: $input) {\n    locationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3f6301413ceacb5a718fdfa279b6d3f8";

export default node;
