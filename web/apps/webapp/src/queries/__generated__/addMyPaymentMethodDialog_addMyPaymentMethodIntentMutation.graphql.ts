/**
 * @generated SignedSource<<c2ba533f2cf6205422bbb980ab8204ea>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddMyPaymentMethodIntentInput = {
  clientMutationId?: string | null | undefined;
};
export type addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation$variables = {
  input: AddMyPaymentMethodIntentInput;
};
export type addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation$data = {
  readonly addMyPaymentMethodIntent: {
    readonly clientMutationId: string | null | undefined;
    readonly clientSecret: string;
    readonly publishedKeys: string;
  };
};
export type addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation = {
  response: addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation$data;
  variables: addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation$variables;
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
    "concreteType": "AddCustomerPaymentMethodIntentPayload",
    "kind": "LinkedField",
    "name": "addMyPaymentMethodIntent",
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
        "name": "publishedKeys",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "clientSecret",
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
    "name": "addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "99b977ae9d055a662f78f77131c9bf9e",
    "id": null,
    "metadata": {},
    "name": "addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation",
    "operationKind": "mutation",
    "text": "mutation addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation(\n  $input: AddMyPaymentMethodIntentInput!\n) {\n  addMyPaymentMethodIntent(input: $input) {\n    clientMutationId\n    publishedKeys\n    clientSecret\n  }\n}\n"
  }
};
})();

(node as any).hash = "01267ce184edbdaacfee8905532738cc";

export default node;
