/**
 * @generated SignedSource<<7e1d8100171d29ee98a1cfee38ac712b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveOrganizationPaymentMethodInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationAdmin_removeOrganizationPaymentMethodMutation$variables = {
  input: RemoveOrganizationPaymentMethodInput;
};
export type organizationAdmin_removeOrganizationPaymentMethodMutation$data = {
  readonly removeOrganizationPaymentMethod: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type organizationAdmin_removeOrganizationPaymentMethodMutation = {
  response: organizationAdmin_removeOrganizationPaymentMethodMutation$data;
  variables: organizationAdmin_removeOrganizationPaymentMethodMutation$variables;
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdmin_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_removeOrganizationPaymentMethodMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "292ee7180f93db97b16b7902253c2bcc",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_removeOrganizationPaymentMethodMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_removeOrganizationPaymentMethodMutation(\n  $input: RemoveOrganizationPaymentMethodInput!\n) {\n  removeOrganizationPaymentMethod(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "ef88cdeddd124b179a851b4ca60c399c";

export default node;
