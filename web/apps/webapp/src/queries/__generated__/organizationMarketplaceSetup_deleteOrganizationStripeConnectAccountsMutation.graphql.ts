/**
 * @generated SignedSource<<c3faf78ceadc161c252043be074d990a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteOrganizationStripeConnectAccountsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteOrganizationStripeConnectAccountsInput;
};
export type organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation$data = {
  readonly deleteOrganizationStripeConnectAccounts: {
    readonly accounts: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation = {
  response: organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation$data;
  variables: organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
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
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationStripeConnectAccountsPayload",
        "kind": "LinkedField",
        "name": "deleteOrganizationStripeConnectAccounts",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationStripeConnectAccountDetails",
            "kind": "LinkedField",
            "name": "accounts",
            "plural": true,
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
    "name": "organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationStripeConnectAccountsPayload",
        "kind": "LinkedField",
        "name": "deleteOrganizationStripeConnectAccounts",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationStripeConnectAccountDetails",
            "kind": "LinkedField",
            "name": "accounts",
            "plural": true,
            "selections": [
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
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
    "cacheID": "1455c638d8fcd9cd17cfcc367474f514",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation(\n  $input: DeleteOrganizationStripeConnectAccountsInput!\n) {\n  deleteOrganizationStripeConnectAccounts(input: $input) {\n    accounts {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b24d6e0925ca141a558d0e5cd090887a";

export default node;
