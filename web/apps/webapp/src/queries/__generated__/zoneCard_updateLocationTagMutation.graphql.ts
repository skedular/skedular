/**
 * @generated SignedSource<<2729facb1843d0de2fcae3a8d9e6760a>>
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
export type zoneCard_updateLocationTagMutation$variables = {
  input: UpdateLocationTagInput;
};
export type zoneCard_updateLocationTagMutation$data = {
  readonly updateLocationTag: {
    readonly locationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type zoneCard_updateLocationTagMutation = {
  response: zoneCard_updateLocationTagMutation$data;
  variables: zoneCard_updateLocationTagMutation$variables;
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
    "name": "zoneCard_updateLocationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "zoneCard_updateLocationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "21a3c138b8c198e145b8e8948076ca5a",
    "id": null,
    "metadata": {},
    "name": "zoneCard_updateLocationTagMutation",
    "operationKind": "mutation",
    "text": "mutation zoneCard_updateLocationTagMutation(\n  $input: UpdateLocationTagInput!\n) {\n  updateLocationTag(input: $input) {\n    locationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "726e5875176c426ae08a44528d7f4521";

export default node;
