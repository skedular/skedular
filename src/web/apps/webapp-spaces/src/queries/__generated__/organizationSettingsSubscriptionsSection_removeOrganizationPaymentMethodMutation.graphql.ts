/**
 * @generated SignedSource<<698647328d525da20a5cffa1ae4269a2>>
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
export type organizationSettingsSubscriptionsSection_removeOrganizationPaymentMethodMutation$variables = {
  input: RemoveOrganizationPaymentMethodInput;
};
export type organizationSettingsSubscriptionsSection_removeOrganizationPaymentMethodMutation$data = {
  readonly removeOrganizationPaymentMethod: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type organizationSettingsSubscriptionsSection_removeOrganizationPaymentMethodMutation = {
  response: organizationSettingsSubscriptionsSection_removeOrganizationPaymentMethodMutation$data;
  variables: organizationSettingsSubscriptionsSection_removeOrganizationPaymentMethodMutation$variables;
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
    "name": "organizationSettingsSubscriptionsSection_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsSubscriptionsSection_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "00fb322c22b5d0e58a1a3f1c687563e3",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsSubscriptionsSection_removeOrganizationPaymentMethodMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsSubscriptionsSection_removeOrganizationPaymentMethodMutation(\n  $input: RemoveOrganizationPaymentMethodInput!\n) {\n  removeOrganizationPaymentMethod(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "76d65b35319716acc3bfc4255fb4a66f";

export default node;
