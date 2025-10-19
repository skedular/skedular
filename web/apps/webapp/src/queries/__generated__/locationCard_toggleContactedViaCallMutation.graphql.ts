/**
 * @generated SignedSource<<360f3a01fcc29f4f118bd0a7cc457453>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ToggleContactedViaCallInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type locationCard_toggleContactedViaCallMutation$variables = {
  input: ToggleContactedViaCallInput;
};
export type locationCard_toggleContactedViaCallMutation$data = {
  readonly toggleContactedViaCall: {
    readonly location: {
      readonly contactedViaCall: boolean;
      readonly id: string;
    };
  };
};
export type locationCard_toggleContactedViaCallMutation = {
  response: locationCard_toggleContactedViaCallMutation$data;
  variables: locationCard_toggleContactedViaCallMutation$variables;
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
    "concreteType": "LocationPayload",
    "kind": "LinkedField",
    "name": "toggleContactedViaCall",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
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
            "name": "contactedViaCall",
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
    "name": "locationCard_toggleContactedViaCallMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationCard_toggleContactedViaCallMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "e3127d1f067953cc1f2cc53246d2d85b",
    "id": null,
    "metadata": {},
    "name": "locationCard_toggleContactedViaCallMutation",
    "operationKind": "mutation",
    "text": "mutation locationCard_toggleContactedViaCallMutation(\n  $input: ToggleContactedViaCallInput!\n) {\n  toggleContactedViaCall(input: $input) {\n    location {\n      id\n      contactedViaCall\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2624a17732c2b7277926155009d9c57b";

export default node;
