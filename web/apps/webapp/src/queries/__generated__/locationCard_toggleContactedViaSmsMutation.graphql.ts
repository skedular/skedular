/**
 * @generated SignedSource<<51e5f2283957866b1ac38733ae920a69>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ToggleContactedViaSmsInput = {
  clientMutationId?: string | null | undefined;
  locationId: string;
};
export type locationCard_toggleContactedViaSmsMutation$variables = {
  input: ToggleContactedViaSmsInput;
};
export type locationCard_toggleContactedViaSmsMutation$data = {
  readonly toggleContactedViaSms: {
    readonly location: {
      readonly contactedViaSms: boolean;
      readonly id: string;
    };
  };
};
export type locationCard_toggleContactedViaSmsMutation = {
  response: locationCard_toggleContactedViaSmsMutation$data;
  variables: locationCard_toggleContactedViaSmsMutation$variables;
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
    "name": "toggleContactedViaSms",
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
            "name": "contactedViaSms",
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
    "name": "locationCard_toggleContactedViaSmsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationCard_toggleContactedViaSmsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "0a8edb40623b8292580ab02350d05b29",
    "id": null,
    "metadata": {},
    "name": "locationCard_toggleContactedViaSmsMutation",
    "operationKind": "mutation",
    "text": "mutation locationCard_toggleContactedViaSmsMutation(\n  $input: ToggleContactedViaSmsInput!\n) {\n  toggleContactedViaSms(input: $input) {\n    location {\n      id\n      contactedViaSms\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c4c6e7937e7bc168281fe84d400b9e46";

export default node;
