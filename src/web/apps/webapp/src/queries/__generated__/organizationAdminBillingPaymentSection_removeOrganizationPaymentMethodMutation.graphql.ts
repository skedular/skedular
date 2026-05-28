/**
 * @generated SignedSource<<932d7f7ec99550f54874b6d4a930abf1>>
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
export type organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation$variables = {
  input: RemoveOrganizationPaymentMethodInput;
};
export type organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation$data = {
  readonly removeOrganizationPaymentMethod: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation = {
  response: organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation$data;
  variables: organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation$variables;
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
    "name": "organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "c47df1d2b98564d3cb6f08b31d2c4644",
    "id": null,
    "metadata": {},
    "name": "organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminBillingPaymentSection_removeOrganizationPaymentMethodMutation(\n  $input: RemoveOrganizationPaymentMethodInput!\n) {\n  removeOrganizationPaymentMethod(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "f3b561a120879d4691d9dd35a6267802";

export default node;
