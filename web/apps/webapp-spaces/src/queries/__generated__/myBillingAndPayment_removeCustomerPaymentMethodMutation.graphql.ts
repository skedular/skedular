/**
 * @generated SignedSource<<e25a382c6b011581d42ddfbde7f7ebd3>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPaymentMethodInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type myBillingAndPayment_removeCustomerPaymentMethodMutation$variables = {
  input: RemoveCustomerPaymentMethodInput;
};
export type myBillingAndPayment_removeCustomerPaymentMethodMutation$data = {
  readonly removeCustomerPaymentMethod: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type myBillingAndPayment_removeCustomerPaymentMethodMutation = {
  response: myBillingAndPayment_removeCustomerPaymentMethodMutation$data;
  variables: myBillingAndPayment_removeCustomerPaymentMethodMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "myBillingAndPayment_removeCustomerPaymentMethodMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "myBillingAndPayment_removeCustomerPaymentMethodMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "49a9ed8a40801b00dbadb3bb809d86bd",
    "id": null,
    "metadata": {},
    "name": "myBillingAndPayment_removeCustomerPaymentMethodMutation",
    "operationKind": "mutation",
    "text": "mutation myBillingAndPayment_removeCustomerPaymentMethodMutation(\n  $input: RemoveCustomerPaymentMethodInput!\n) {\n  removeCustomerPaymentMethod(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "df670ca4e56f78624093d69e4bc3343e";

export default node;
