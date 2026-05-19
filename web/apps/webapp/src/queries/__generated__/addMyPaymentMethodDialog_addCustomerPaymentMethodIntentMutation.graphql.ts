/**
 * @generated SignedSource<<c3e2b8d51589f5a3f3ce7744e1650d3e>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerPaymentMethodIntentInput = {
  clientMutationId?: string | null | undefined;
};
export type addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation$variables = {
  input: AddCustomerPaymentMethodIntentInput;
};
export type addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation$data = {
  readonly addCustomerPaymentMethodIntent: {
    readonly clientMutationId: string | null | undefined;
    readonly clientSecret: string;
    readonly publishedKeys: string;
  };
};
export type addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation = {
  response: addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation$data;
  variables: addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation$variables;
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
    "name": "addCustomerPaymentMethodIntent",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "2c5727842e52dca26d5518174e47c3f8",
    "id": null,
    "metadata": {},
    "name": "addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation",
    "operationKind": "mutation",
    "text": "mutation addMyPaymentMethodDialog_addCustomerPaymentMethodIntentMutation(\n  $input: AddCustomerPaymentMethodIntentInput!\n) {\n  addCustomerPaymentMethodIntent(input: $input) {\n    clientMutationId\n    publishedKeys\n    clientSecret\n  }\n}\n"
  }
};
})();

(node as any).hash = "671199b08f35575e012cc7e8cbf7762c";

export default node;
