/**
 * @generated SignedSource<<28970b0a1959a139ff012b62f8620a23>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationStripeConnectAccountInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  name: string;
};
export type editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation$variables = {
  input: UpdateOrganizationStripeConnectAccountInput;
};
export type editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation$data = {
  readonly updateOrganizationStripeConnectAccount: {
    readonly account: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation$rawResponse = {
  readonly updateOrganizationStripeConnectAccount: {
    readonly account: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
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
        "name": "account",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "fc317b39e239d9a33b5f0ad772adfeb7",
    "id": null,
    "metadata": {},
    "name": "editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation",
    "operationKind": "mutation",
    "text": "mutation editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation(\n  $input: UpdateOrganizationStripeConnectAccountInput!\n) {\n  updateOrganizationStripeConnectAccount(input: $input) {\n    account {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "dbd4b67eedbcc8d133b7b2362c397e11";

export default node;
