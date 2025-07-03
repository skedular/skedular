/**
 * @generated SignedSource<<702853b93e3963087bbd40ebc05ac59d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationBankAccountInput = {
  accountHolderName: string;
  accountNumber: string;
  bankName: string;
  clientMutationId?: string | null | undefined;
  country: string;
  id: string;
  name: string;
};
export type editBankAccount_updateOrganizationBankAccountMutation$variables = {
  input: UpdateOrganizationBankAccountInput;
};
export type editBankAccount_updateOrganizationBankAccountMutation$data = {
  readonly updateOrganizationBankAccount: {
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
export type editBankAccount_updateOrganizationBankAccountMutation$rawResponse = {
  readonly updateOrganizationBankAccount: {
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
export type editBankAccount_updateOrganizationBankAccountMutation = {
  rawResponse: editBankAccount_updateOrganizationBankAccountMutation$rawResponse;
  response: editBankAccount_updateOrganizationBankAccountMutation$data;
  variables: editBankAccount_updateOrganizationBankAccountMutation$variables;
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
    "name": "updateOrganizationBankAccount",
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
    "name": "editBankAccount_updateOrganizationBankAccountMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editBankAccount_updateOrganizationBankAccountMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "f74703f6d465e666fa6d673c9c3b9d92",
    "id": null,
    "metadata": {},
    "name": "editBankAccount_updateOrganizationBankAccountMutation",
    "operationKind": "mutation",
    "text": "mutation editBankAccount_updateOrganizationBankAccountMutation(\n  $input: UpdateOrganizationBankAccountInput!\n) {\n  updateOrganizationBankAccount(input: $input) {\n    organizationBankAccount {\n      id\n      name\n      bankName\n      accountHolderName\n      accountNumber\n      country\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "924e44a96b732eed2fe4c96287706ffe";

export default node;
