/**
 * @generated SignedSource<<28fc2acf6dd12bda35397f62089b07ef>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editBankAccount_query$data = {
  readonly organizationBankAccount: {
    readonly accountHolderName: string;
    readonly accountNumber: string;
    readonly bankName: string;
    readonly country: string;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentType": "editBankAccount_query";
};
export type editBankAccount_query$key = {
  readonly " $data"?: editBankAccount_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editBankAccount_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationBankAccountId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "editBankAccount_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "organizationBankAccountId"
        }
      ],
      "concreteType": "OrganizationBankAccountDetails",
      "kind": "LinkedField",
      "name": "organizationBankAccount",
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
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "380f6bfa40fa95704eab8704b574b002";

export default node;
