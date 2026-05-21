/**
 * @generated SignedSource<<8ee3ce617028c5902953711866ddb972>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationStripeConnectAccountPatchField = "NAME" | "%future added value";
export type UpdateOrganizationStripeConnectAccountInput = {
  clientMutationId?: string | null | undefined;
  fieldsToUpdate: ReadonlyArray<OrganizationStripeConnectAccountPatchField>;
  id: string;
  name?: string | null | undefined;
};
export type editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation$variables = {
  input: UpdateOrganizationStripeConnectAccountInput;
};
export type editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation$data = {
  readonly updateOrganizationStripeConnectAccount: {
    readonly organizationStripeConnectAccount: {
      readonly id: string;
      readonly name: string;
    };
  };
};
export type editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation$rawResponse = {
  readonly updateOrganizationStripeConnectAccount: {
    readonly organizationStripeConnectAccount: {
      readonly id: string;
      readonly name: string;
    };
  };
};
export type editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation = {
  rawResponse: editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation$rawResponse;
  response: editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation$data;
  variables: editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation$variables;
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
    "concreteType": "OrganizationStripeConnectAccountPayload",
    "kind": "LinkedField",
    "name": "updateOrganizationStripeConnectAccount",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationStripeConnectAccountDetails",
        "kind": "LinkedField",
        "name": "organizationStripeConnectAccount",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          }
        ],
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
    "name": "editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "5be472ced67d8cfbfbeb2c96ac61bbbf",
    "id": null,
    "metadata": {},
    "name": "editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation",
    "operationKind": "mutation",
    "text": "mutation editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation(\n  $input: UpdateOrganizationStripeConnectAccountInput!\n) {\n  updateOrganizationStripeConnectAccount(input: $input) {\n    organizationStripeConnectAccount {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8f93657afbafbadc99c910887a4bf4c0";

export default node;
