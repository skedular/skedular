/**
 * @generated SignedSource<<db07dcd456599595c596a1eac308726c>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveOrganizationPaymentMethodInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationSettingsBillingPaymentSection_removeOrganizationPaymentMethodMutation$variables = {
  input: RemoveOrganizationPaymentMethodInput;
};
export type organizationSettingsBillingPaymentSection_removeOrganizationPaymentMethodMutation$data = {
  readonly removeOrganizationPaymentMethod: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type organizationSettingsBillingPaymentSection_removeOrganizationPaymentMethodMutation = {
  response: organizationSettingsBillingPaymentSection_removeOrganizationPaymentMethodMutation$data;
  variables: organizationSettingsBillingPaymentSection_removeOrganizationPaymentMethodMutation$variables;
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
    "concreteType": "RemoveOrganizationPaymentMethodPayload",
    "kind": "LinkedField",
    "name": "removeOrganizationPaymentMethod",
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
    "name": "organizationSettingsBillingPaymentSection_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsBillingPaymentSection_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "e3c69ef401c64a5f0d4d7d44a2ec08e9",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsBillingPaymentSection_removeOrganizationPaymentMethodMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsBillingPaymentSection_removeOrganizationPaymentMethodMutation(\n  $input: RemoveOrganizationPaymentMethodInput!\n) {\n  removeOrganizationPaymentMethod(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "e5c3b914cc35c25775306dbaaeb37881";

export default node;
