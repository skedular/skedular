/**
 * @generated SignedSource<<876c7ec4ce49b71d2e101b82775c294e>>
 * @lightSyntaxTransform
 * @nogrep
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationXeroConnection",
            "kind": "LinkedField",
            "name": "xeroConnection",
            "plural": false,
            "selections": [
              (v1/*: any*/)
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_disconnectOrganizationXeroConnectionMutation",
    "selections": (v2/*: any*/)
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
