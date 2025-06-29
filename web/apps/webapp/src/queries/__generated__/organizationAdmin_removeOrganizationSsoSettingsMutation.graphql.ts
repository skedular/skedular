/**
 * @generated SignedSource<<4a8c005c38f137f32ba80f6a3c61cb57>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveOrganizationSsoSettingsInput = {
  clientMutationId?: string | null | undefined;
  organizationId: string;
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
    "concreteType": "UpdateOrganizationSsoSettingsPayload",
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
            "concreteType": "OrganizationSsoSettingsDetails",
            "kind": "LinkedField",
            "name": "ssoSettings",
            "plural": false,
            "selections": [
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
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_removeOrganizationSsoSettingsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "9ad82db01ba81712907d2afc744631ff",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_removeOrganizationSsoSettingsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_removeOrganizationSsoSettingsMutation(\n  $input: RemoveOrganizationSsoSettingsInput!\n) {\n  removeOrganizationSsoSettings(input: $input) {\n    organization {\n      id\n      ssoSettings {\n        entityId\n        loginUrl\n        appFederationMetadataUrl\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "91b5fe838e4a3007f6ef26db9a62d6fc";

export default node;
