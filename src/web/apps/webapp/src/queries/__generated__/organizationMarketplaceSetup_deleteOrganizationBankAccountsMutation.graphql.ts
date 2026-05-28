/**
 * @generated SignedSource<<9c28e8b36d01da5a822278e93a5b7670>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteOrganizationBankAccountsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteOrganizationBankAccountsInput;
};
export type organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation$data = {
  readonly deleteOrganizationBankAccounts: {
    readonly organizationBankAccounts: ReadonlyArray<{
      readonly id: string;
    }>;
  };
};
export type organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation = {
  response: organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation$data;
  variables: organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation$variables;
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
    "name": "organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationBankAccountsPayload",
        "kind": "LinkedField",
        "name": "deleteOrganizationBankAccounts",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationBankAccountDetails",
            "kind": "LinkedField",
            "name": "organizationBankAccounts",
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
    "name": "organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationBankAccountsPayload",
        "kind": "LinkedField",
        "name": "deleteOrganizationBankAccounts",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationBankAccountDetails",
            "kind": "LinkedField",
            "name": "organizationBankAccounts",
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
    "cacheID": "8fb91e292b036e8fbb7c0659d387bb04",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_deleteOrganizationBankAccountsMutation(\n  $input: DeleteOrganizationBankAccountsInput!\n) {\n  deleteOrganizationBankAccounts(input: $input) {\n    organizationBankAccounts {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f4d02e53454ead79059472d5f4b30a62";

export default node;
