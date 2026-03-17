/**
 * @generated SignedSource<<00701d536e6c84e4813a71107c0d0ed2>>
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
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
  redirectUrl: string;
};
export type addStripeConnectAccount_addStripeConnectAccountMutation$variables = {
  input: AddOrganizationStripeConnectAccountInput;
};
export type addStripeConnectAccount_addStripeConnectAccountMutation$data = {
  readonly addOrganizationStripeConnectAccount: {
    readonly organizationStripeConnectAccount: {
      readonly id: string;
      readonly name: string;
    };
  };
};
export type addStripeConnectAccount_addStripeConnectAccountMutation$rawResponse = {
  readonly addOrganizationStripeConnectAccount: {
    readonly organizationStripeConnectAccount: {
      readonly id: string;
      readonly name: string;
    };
  };
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
    "cacheID": "1a75f231e8eb502f9a4a9cde56228178",
    "id": null,
    "metadata": {},
    "name": "addStripeConnectAccount_addStripeConnectAccountMutation",
    "operationKind": "mutation",
    "text": "mutation addStripeConnectAccount_addStripeConnectAccountMutation(\n  $input: AddOrganizationStripeConnectAccountInput!\n) {\n  addOrganizationStripeConnectAccount(input: $input) {\n    organizationStripeConnectAccount {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f490ab5fd527786bde910ebddb6c0e0b";

export default node;
