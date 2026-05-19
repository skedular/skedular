/**
 * @generated SignedSource<<84c2e92081b166c96cec47bfaaa4ea8a>>
 * @lightSyntaxTransform
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
    readonly organizationStripeConnectAccounts: ReadonlyArray<{
      readonly id: string;
    }>;
  };
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
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
            "name": "organizationStripeConnectAccounts",
            "plural": true,
            "selections": [
              (v2/*:: as any*/)
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
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
            "name": "organizationStripeConnectAccounts",
            "plural": true,
            "selections": [
              (v2/*:: as any*/),
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
    "cacheID": "8a07ad36e65c93ed1c0aaf2defd03ba7",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_deleteOrganizationStripeConnectAccountsMutation(\n  $input: DeleteOrganizationStripeConnectAccountsInput!\n) {\n  deleteOrganizationStripeConnectAccounts(input: $input) {\n    organizationStripeConnectAccounts {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "467aa4d03eb43b844c5ff6718989a229";

export default node;
