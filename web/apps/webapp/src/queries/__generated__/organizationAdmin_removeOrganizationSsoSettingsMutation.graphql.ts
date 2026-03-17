/**
 * @generated SignedSource<<480712501402926c9f5ddfa59829dfb6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveOrganizationSsoSettingsInput = {
  clientMutationId?: string | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type organizationAdmin_removeOrganizationSsoSettingsMutation$variables = {
  input: RemoveOrganizationSsoSettingsInput;
};
export type organizationAdmin_removeOrganizationSsoSettingsMutation$data = {
  readonly removeOrganizationSsoSettings: {
    readonly organization: {
      readonly id: string;
      readonly ssoSettings: {
        readonly appFederationMetadataUrl: string;
        readonly entityId: string;
        readonly id: string;
        readonly isActive: boolean;
        readonly loginUrl: string;
      } | null | undefined;
    };
  };
};
export type organizationAdmin_removeOrganizationSsoSettingsMutation$rawResponse = {
  readonly removeOrganizationSsoSettings: {
    readonly organization: {
      readonly id: string;
      readonly ssoSettings: {
        readonly appFederationMetadataUrl: string;
        readonly entityId: string;
        readonly id: string;
        readonly isActive: boolean;
        readonly loginUrl: string;
      } | null | undefined;
    };
  };
};
export type organizationAdmin_removeOrganizationSsoSettingsMutation = {
  rawResponse: organizationAdmin_removeOrganizationSsoSettingsMutation$rawResponse;
  response: organizationAdmin_removeOrganizationSsoSettingsMutation$data;
  variables: organizationAdmin_removeOrganizationSsoSettingsMutation$variables;
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
    "name": "removeOrganizationSsoSettings",
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
            "concreteType": "OrganizationSsoSettingsDetails",
            "kind": "LinkedField",
            "name": "ssoSettings",
            "plural": false,
            "selections": [
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "isActive",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "entityId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "loginUrl",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "appFederationMetadataUrl",
                "storageKey": null
              }
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
    "name": "organizationAdmin_removeOrganizationSsoSettingsMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_removeOrganizationSsoSettingsMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "a1594380b8ac594e78c01de2ed914de1",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_removeOrganizationSsoSettingsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_removeOrganizationSsoSettingsMutation(\n  $input: RemoveOrganizationSsoSettingsInput!\n) {\n  removeOrganizationSsoSettings(input: $input) {\n    organization {\n      id\n      ssoSettings {\n        id\n        isActive\n        entityId\n        loginUrl\n        appFederationMetadataUrl\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8452a772cf1dcf88432ca99ff9294359";

export default node;
