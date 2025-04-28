/**
 * @generated SignedSource<<a8aa6a9b8d59624f298111be526a23b5>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddOrganizationStripeConnectAccountInput = {
  clientMutationId?: string | null | undefined;
  id?: string | null | undefined;
  name: string;
  organizationId: string;
  redirectUrl: string;
};
export type addStripeConnectAccount_addStripeConnectAccountMutation$variables = {
  input: AddOrganizationStripeConnectAccountInput;
};
export type addStripeConnectAccount_addStripeConnectAccountMutation$data = {
  readonly addOrganizationStripeConnectAccount: {
    readonly account: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type addStripeConnectAccount_addStripeConnectAccountMutation$rawResponse = {
  readonly addOrganizationStripeConnectAccount: {
    readonly account: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type addStripeConnectAccount_addStripeConnectAccountMutation = {
  rawResponse: addStripeConnectAccount_addStripeConnectAccountMutation$rawResponse;
  response: addStripeConnectAccount_addStripeConnectAccountMutation$data;
  variables: addStripeConnectAccount_addStripeConnectAccountMutation$variables;
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
    "name": "addOrganizationStripeConnectAccount",
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
    "name": "addStripeConnectAccount_addStripeConnectAccountMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addStripeConnectAccount_addStripeConnectAccountMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "f0c6de506bb4e839a5b1e5156ace8334",
    "id": null,
    "metadata": {},
    "name": "addStripeConnectAccount_addStripeConnectAccountMutation",
    "operationKind": "mutation",
    "text": "mutation addStripeConnectAccount_addStripeConnectAccountMutation(\n  $input: AddOrganizationStripeConnectAccountInput!\n) {\n  addOrganizationStripeConnectAccount(input: $input) {\n    account {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f1b6c4c6e87c1214ba514292840912b5";

export default node;
