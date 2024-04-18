/**
 * @generated SignedSource<<89e03e7e4327492b7c2ba8c884f536cd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type AddOrganizationPaymentMethodIntentInput = {
  clientMutationId?: string | null | undefined;
  organizationId: string;
};
export type organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation$variables = {
  input: AddOrganizationPaymentMethodIntentInput;
};
export type organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation$data = {
  readonly addOrganizationPaymentMethodIntent: {
    readonly clientMutationId: string | null | undefined;
    readonly clientSecret: string;
    readonly publishedKeys: string;
  } | null | undefined;
};
export type organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation = {
  response: organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation$data;
  variables: organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation$variables;
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
    "concreteType": "AddOrganizationPaymentMethodIntentResponse",
    "kind": "LinkedField",
    "name": "addOrganizationPaymentMethodIntent",
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
    "name": "organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "74d0ed1411fc7bb86c0eb27f856270f4",
    "id": null,
    "metadata": {},
    "name": "organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation",
    "operationKind": "mutation",
    "text": "mutation organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation(\n  $input: AddOrganizationPaymentMethodIntentInput!\n) {\n  addOrganizationPaymentMethodIntent(input: $input) {\n    clientMutationId\n    publishedKeys\n    clientSecret\n  }\n}\n"
  }
};
})();

(node as any).hash = "2071732625a53b01b3b1ad138fdbf14b";

export default node;
