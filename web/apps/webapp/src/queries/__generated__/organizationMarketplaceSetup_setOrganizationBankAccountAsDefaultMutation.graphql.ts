/**
 * @generated SignedSource<<65557ab56ca0d023b8e0590c7082795b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type SetOrganizationBankAccountAsDefaultInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation$variables = {
  input: SetOrganizationBankAccountAsDefaultInput;
};
export type organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation$data = {
  readonly setOrganizationBankAccountAsDefault: {
    readonly organizationBankAccount: {
      readonly id: string;
      readonly isDefault: boolean;
    };
  };
};
export type organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation$rawResponse = {
  readonly setOrganizationBankAccountAsDefault: {
    readonly organizationBankAccount: {
      readonly id: string;
      readonly isDefault: boolean;
    };
  };
};
export type organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation = {
  rawResponse: organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation$rawResponse;
  response: organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation$data;
  variables: organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation$variables;
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
    "name": "setOrganizationBankAccountAsDefault",
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
            "name": "isDefault",
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
    "name": "organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "0dfac59099827597d39526c6248df6b0",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_setOrganizationBankAccountAsDefaultMutation(\n  $input: SetOrganizationBankAccountAsDefaultInput!\n) {\n  setOrganizationBankAccountAsDefault(input: $input) {\n    organizationBankAccount {\n      id\n      isDefault\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f7a4dddba56fe43d60e9660cf8294028";

export default node;
