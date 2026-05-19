/**
 * @generated SignedSource<<7270cffb520689d8093b422117747e29>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DisconnectOrganizationXeroConnectionInput = {
  clientMutationId?: string | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation$variables = {
  input: DisconnectOrganizationXeroConnectionInput;
};
export type organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation$data = {
  readonly disconnectOrganizationXeroConnection: {
    readonly organization: {
      readonly id: string;
      readonly xeroConnection: {
        readonly id: string;
      } | null | undefined;
    };
  };
};
export type organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation$rawResponse = {
  readonly disconnectOrganizationXeroConnection: {
    readonly organization: {
      readonly id: string;
      readonly xeroConnection: {
        readonly id: string;
      } | null | undefined;
    };
  };
};
export type organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation = {
  rawResponse: organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation$rawResponse;
  response: organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation$data;
  variables: organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "disconnectOrganizationXeroConnection",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationXeroConnection",
            "kind": "LinkedField",
            "name": "xeroConnection",
            "plural": false,
            "selections": [
              (v1/*:: as any*/)
            ],
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "afa4b7ad1f09d71d93677e6e652a6d2c",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation(\n  $input: DisconnectOrganizationXeroConnectionInput!\n) {\n  disconnectOrganizationXeroConnection(input: $input) {\n    organization {\n      id\n      xeroConnection {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "589f3a0751896e889d119365f4974baa";

export default node;
