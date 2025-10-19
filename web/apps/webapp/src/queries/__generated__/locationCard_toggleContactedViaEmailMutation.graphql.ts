/**
 * @generated SignedSource<<1f028c2e8c6d06d67d069f9ae302515e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ToggleContactedViaEmailInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type locationCard_toggleContactedViaEmailMutation$variables = {
  input: ToggleContactedViaEmailInput;
};
export type locationCard_toggleContactedViaEmailMutation$data = {
  readonly toggleContactedViaEmail: {
    readonly location: {
      readonly contactedViaEmail: boolean;
      readonly id: string;
    };
  };
};
export type locationCard_toggleContactedViaEmailMutation = {
  response: locationCard_toggleContactedViaEmailMutation$data;
  variables: locationCard_toggleContactedViaEmailMutation$variables;
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
    "name": "toggleContactedViaEmail",
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
            "name": "contactedViaEmail",
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
    "name": "locationCard_toggleContactedViaEmailMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationCard_toggleContactedViaEmailMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "b2c989ebc976cbf2cbaeb57c02b3c4bb",
    "id": null,
    "metadata": {},
    "name": "locationCard_toggleContactedViaEmailMutation",
    "operationKind": "mutation",
    "text": "mutation locationCard_toggleContactedViaEmailMutation(\n  $input: ToggleContactedViaEmailInput!\n) {\n  toggleContactedViaEmail(input: $input) {\n    location {\n      id\n      contactedViaEmail\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cbceac076e18258be2bc652325c45d68";

export default node;
