/**
 * @generated SignedSource<<7b47f23bf9ff1803cf3ab9e36f986b10>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationBankAccount_rootQuery$variables = {
  organizationBankAccountId: string;
};
export type pageOrganizationBankAccount_rootQuery$data = {
  readonly organizationBankAccount: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"editBankAccount_query">;
};
export type pageOrganizationBankAccount_rootQuery = {
  response: pageOrganizationBankAccount_rootQuery$data;
  variables: pageOrganizationBankAccount_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationBankAccountId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationBankAccountId"
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationBankAccount_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationBankAccountDetails",
        "kind": "LinkedField",
        "name": "organizationBankAccount",
        "plural": false,
        "selections": [
          (v2/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editBankAccount_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationBankAccount_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationBankAccountDetails",
        "kind": "LinkedField",
        "name": "organizationBankAccount",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
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
            "name": "bankName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "accountHolderName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "accountNumber",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "country",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "c62cce79f7331305a91ea346a827f129",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationBankAccount_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationBankAccount_rootQuery(\n  $organizationBankAccountId: String!\n) {\n  organizationBankAccount(id: $organizationBankAccountId) {\n    name\n    id\n  }\n  ...editBankAccount_query\n}\n\nfragment editBankAccount_query on Query {\n  organizationBankAccount(id: $organizationBankAccountId) {\n    id\n    name\n    bankName\n    accountHolderName\n    accountNumber\n    country\n  }\n}\n"
  }
};
})();

(node as any).hash = "4cd1a62bfe924574715ef1a711e0157e";

export default node;
