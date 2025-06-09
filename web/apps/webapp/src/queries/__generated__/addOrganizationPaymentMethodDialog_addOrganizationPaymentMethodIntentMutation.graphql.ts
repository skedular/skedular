/**
 * @generated SignedSource<<38d1b291c2e613832a3193689fea3771>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddOrganizationPaymentMethodIntentInput = {
  clientMutationId?: string | null | undefined;
  organizationId: string;
};
export type addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation$variables = {
  input: AddOrganizationPaymentMethodIntentInput;
};
export type addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation$data = {
  readonly addOrganizationPaymentMethodIntent: {
    readonly clientMutationId: string | null | undefined;
    readonly clientSecret: string;
    readonly publishedKeys: string;
  };
};
export type addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation = {
  response: addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation$data;
  variables: addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation$variables;
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
    "concreteType": "AddOrganizationPaymentMethodIntentPayload",
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
    "name": "addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "29eeaa77033f80ad108113356e223b16",
    "id": null,
    "metadata": {},
    "name": "addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation",
    "operationKind": "mutation",
    "text": "mutation addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation(\n  $input: AddOrganizationPaymentMethodIntentInput!\n) {\n  addOrganizationPaymentMethodIntent(input: $input) {\n    clientMutationId\n    publishedKeys\n    clientSecret\n  }\n}\n"
  }
};
})();

(node as any).hash = "d47e5430bfcf5a90c3bf0dc725659b6a";

export default node;
