/**
 * @generated SignedSource<<095382779486d0801d2198c0344d6851>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type SetOrganizationStripeConnectAccountAsDefaultInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation$variables = {
  input: SetOrganizationStripeConnectAccountAsDefaultInput;
};
export type organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation$data = {
  readonly setOrganizationStripeConnectAccountAsDefault: {
    readonly account: {
      readonly id: string;
      readonly isDefault: boolean;
    };
  } | null | undefined;
};
export type organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation$rawResponse = {
  readonly setOrganizationStripeConnectAccountAsDefault: {
    readonly account: {
      readonly id: string;
      readonly isDefault: boolean;
    };
  } | null | undefined;
};
export type organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation = {
  rawResponse: organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation$rawResponse;
  response: organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation$data;
  variables: organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation$variables;
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
    "name": "setOrganizationStripeConnectAccountAsDefault",
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
    "name": "organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "80c5395aff2cff39c2170f3bed8364b7",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation(\n  $input: SetOrganizationStripeConnectAccountAsDefaultInput!\n) {\n  setOrganizationStripeConnectAccountAsDefault(input: $input) {\n    account {\n      id\n      isDefault\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7b9a9ebc950d6a0dbbd43ff18580474c";

export default node;
