/**
 * @generated SignedSource<<25ea28c0ad212bbae805aa4f060d3e94>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddOrganizationBankAccountInput = {
  accountHolderName: string;
  accountNumber: string;
  bankName: string;
  clientMutationId?: string | null | undefined;
  country: string;
  id?: string | null | undefined;
  name: string;
  organizationId: string;
};
export type addBankAccount_addBankAccountMutation$variables = {
  input: AddOrganizationBankAccountInput;
};
export type addBankAccount_addBankAccountMutation$data = {
  readonly addOrganizationBankAccount: {
    readonly organizationBankAccount: {
      readonly accountHolderName: string;
      readonly accountNumber: string;
      readonly bankName: string;
      readonly country: string;
      readonly id: string;
      readonly name: string;
    };
  };
};
export type addBankAccount_addBankAccountMutation$rawResponse = {
  readonly addOrganizationBankAccount: {
    readonly organizationBankAccount: {
      readonly accountHolderName: string;
      readonly accountNumber: string;
      readonly bankName: string;
      readonly country: string;
      readonly id: string;
      readonly name: string;
    };
  };
};
export type addBankAccount_addBankAccountMutation = {
  rawResponse: addBankAccount_addBankAccountMutation$rawResponse;
  response: addBankAccount_addBankAccountMutation$data;
  variables: addBankAccount_addBankAccountMutation$variables;
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
    "concreteType": "OrganizationBankAccountPayload",
    "kind": "LinkedField",
    "name": "addOrganizationBankAccount",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
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
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addBankAccount_addBankAccountMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addBankAccount_addBankAccountMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "4556595d5f0cfebcdf6bf9e5ff11eaab",
    "id": null,
    "metadata": {},
    "name": "addBankAccount_addBankAccountMutation",
    "operationKind": "mutation",
    "text": "mutation addBankAccount_addBankAccountMutation(\n  $input: AddOrganizationBankAccountInput!\n) {\n  addOrganizationBankAccount(input: $input) {\n    organizationBankAccount {\n      id\n      name\n      bankName\n      accountHolderName\n      accountNumber\n      country\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3fb4511e2b7fc0109e2de40c0d90b04d";

export default node;
