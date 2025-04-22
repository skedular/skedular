/**
 * @generated SignedSource<<c2de3a26fa32ca3046216a087019cc78>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddOrganizationStripeConnectAccountInput = {
  clientMutationId?: string | null | undefined;
  name: string;
  organizationId: string;
};
export type addStripeConnectAccount_addStripeConnectAccountMutation$variables = {
  input: AddOrganizationStripeConnectAccountInput;
};
export type addStripeConnectAccount_addStripeConnectAccountMutation$data = {
  readonly addOrganizationStripeConnectAccount: {
    readonly account: {
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
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addStripeConnectAccount_addStripeConnectAccountMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addStripeConnectAccount_addStripeConnectAccountMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "id",
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "d26e0c304d4554e78e4bb7f6c14a27b0",
    "id": null,
    "metadata": {},
    "name": "addStripeConnectAccount_addStripeConnectAccountMutation",
    "operationKind": "mutation",
    "text": "mutation addStripeConnectAccount_addStripeConnectAccountMutation(\n  $input: AddOrganizationStripeConnectAccountInput!\n) {\n  addOrganizationStripeConnectAccount(input: $input) {\n    account {\n      name\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "22cfec45efd0b06addf95eddddc93bbc";

export default node;
