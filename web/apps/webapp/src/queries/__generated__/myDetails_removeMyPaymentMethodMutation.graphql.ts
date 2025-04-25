/**
 * @generated SignedSource<<2d7274233a608dd73005bb5656c2e3b1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveMyPaymentMethodInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type myDetails_removeMyPaymentMethodMutation$variables = {
  input: RemoveMyPaymentMethodInput;
};
export type myDetails_removeMyPaymentMethodMutation$data = {
  readonly removeMyPaymentMethod: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type myDetails_removeMyPaymentMethodMutation = {
  response: myDetails_removeMyPaymentMethodMutation$data;
  variables: myDetails_removeMyPaymentMethodMutation$variables;
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
    "name": "removeMyPaymentMethod",
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
    "name": "myDetails_removeMyPaymentMethodMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myDetails_removeMyPaymentMethodMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "171ac0067a43c284c776f0a25a167550",
    "id": null,
    "metadata": {},
    "name": "myDetails_removeMyPaymentMethodMutation",
    "operationKind": "mutation",
    "text": "mutation myDetails_removeMyPaymentMethodMutation(\n  $input: RemoveMyPaymentMethodInput!\n) {\n  removeMyPaymentMethod(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "efe74fecf533b21f60f4b0972e507a6a";

export default node;
