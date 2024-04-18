/**
 * @generated SignedSource<<efd98b9b2acd5de05f71039644a55715>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type RemoveOrganizationPaymentMethodInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationPaymentMethods_removeOrganizationPaymentMethodMutation$variables = {
  input: RemoveOrganizationPaymentMethodInput;
};
export type organizationPaymentMethods_removeOrganizationPaymentMethodMutation$data = {
  readonly removeOrganizationPaymentMethod: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type organizationPaymentMethods_removeOrganizationPaymentMethodMutation = {
  response: organizationPaymentMethods_removeOrganizationPaymentMethodMutation$data;
  variables: organizationPaymentMethods_removeOrganizationPaymentMethodMutation$variables;
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
    "concreteType": "RemoveOrganizationPaymentMethodResponse",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationPaymentMethods_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationPaymentMethods_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "5074c142a8c6c54a313a37f6d87a7381",
    "id": null,
    "metadata": {},
    "name": "organizationPaymentMethods_removeOrganizationPaymentMethodMutation",
    "operationKind": "mutation",
    "text": "mutation organizationPaymentMethods_removeOrganizationPaymentMethodMutation(\n  $input: RemoveOrganizationPaymentMethodInput!\n) {\n  removeOrganizationPaymentMethod(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "ab0eb3efef263e42aee2f30ba4683356";

export default node;
