/**
 * @generated SignedSource<<b5f9b3960a9128cde6bdba58e56ad751>>
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
    readonly organizationStripeConnectAccount: {
      readonly id: string;
      readonly isDefault: boolean;
    };
  };
};
export type organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation$rawResponse = {
  readonly setOrganizationStripeConnectAccountAsDefault: {
    readonly organizationStripeConnectAccount: {
      readonly id: string;
      readonly isDefault: boolean;
    };
  };
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
        "name": "organizationStripeConnectAccount",
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
    "cacheID": "2b4d18cb14f3d4da87fb4ac19167f9fe",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_setOrganizationStripeConnectAccountAsDefaultMutation(\n  $input: SetOrganizationStripeConnectAccountAsDefaultInput!\n) {\n  setOrganizationStripeConnectAccountAsDefault(input: $input) {\n    organizationStripeConnectAccount {\n      id\n      isDefault\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6e5c99b632f2cb52d5c1c6fec3e8c879";

export default node;
