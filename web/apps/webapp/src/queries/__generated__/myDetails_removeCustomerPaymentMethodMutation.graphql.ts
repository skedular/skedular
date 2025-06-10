/**
 * @generated SignedSource<<7049c20378e84e493aa1cc413b252037>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPaymentMethodInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type myDetails_removeCustomerPaymentMethodMutation$variables = {
  input: RemoveCustomerPaymentMethodInput;
};
export type myDetails_removeCustomerPaymentMethodMutation$data = {
  readonly removeCustomerPaymentMethod: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type myDetails_removeCustomerPaymentMethodMutation = {
  response: myDetails_removeCustomerPaymentMethodMutation$data;
  variables: myDetails_removeCustomerPaymentMethodMutation$variables;
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
    "concreteType": "RemoveCustomerPaymentMethodPayload",
    "kind": "LinkedField",
    "name": "removeCustomerPaymentMethod",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "clientMutationId",
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
    "name": "myDetails_removeCustomerPaymentMethodMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myDetails_removeCustomerPaymentMethodMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "51046ca53b99abc50b3b0f6a1838589e",
    "id": null,
    "metadata": {},
    "name": "myDetails_removeCustomerPaymentMethodMutation",
    "operationKind": "mutation",
    "text": "mutation myDetails_removeCustomerPaymentMethodMutation(\n  $input: RemoveCustomerPaymentMethodInput!\n) {\n  removeCustomerPaymentMethod(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "d821c3afe48f46ef22c28e9087e3ce98";

export default node;
