/**
 * @generated SignedSource<<59456b7f5136aff4db32fe73cfcda1d1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ToggleContactedViaWhatsappInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type locationCard_toggleContactedViaWhatsappMutation$variables = {
  input: ToggleContactedViaWhatsappInput;
};
export type locationCard_toggleContactedViaWhatsappMutation$data = {
  readonly toggleContactedViaWhatsapp: {
    readonly location: {
      readonly contactedViaWhatsapp: boolean;
      readonly id: string;
    };
  };
};
export type locationCard_toggleContactedViaWhatsappMutation = {
  response: locationCard_toggleContactedViaWhatsappMutation$data;
  variables: locationCard_toggleContactedViaWhatsappMutation$variables;
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
    "name": "toggleContactedViaWhatsapp",
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
            "name": "contactedViaWhatsapp",
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
    "name": "locationCard_toggleContactedViaWhatsappMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationCard_toggleContactedViaWhatsappMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "5f02d66734ae27ac99ebb9be08135c31",
    "id": null,
    "metadata": {},
    "name": "locationCard_toggleContactedViaWhatsappMutation",
    "operationKind": "mutation",
    "text": "mutation locationCard_toggleContactedViaWhatsappMutation(\n  $input: ToggleContactedViaWhatsappInput!\n) {\n  toggleContactedViaWhatsapp(input: $input) {\n    location {\n      id\n      contactedViaWhatsapp\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c16a9067fc58a28bc5ccbfce0848c9d6";

export default node;
