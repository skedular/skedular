/**
 * @generated SignedSource<<add6b7272450b7fce41b4f8bc40c887f>>
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
export type organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation$variables = {
  input: RemoveOrganizationPaymentMethodInput;
};
export type organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation$data = {
  readonly removeOrganizationPaymentMethod: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation = {
  response: organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation$data;
  variables: organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation$variables;
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
    "name": "organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "c5d5662afb756921052f6cf3090683c2",
    "id": null,
    "metadata": {},
    "name": "organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminSubscriptionsSection_removeOrganizationPaymentMethodMutation(\n  $input: RemoveOrganizationPaymentMethodInput!\n) {\n  removeOrganizationPaymentMethod(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "5df0316f3f00aba4fa3a4dce95ce44c0";

export default node;
