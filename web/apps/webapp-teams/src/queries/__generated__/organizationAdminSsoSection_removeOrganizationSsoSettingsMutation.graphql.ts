/**
 * @generated SignedSource<<56fc61f33656b88c006c730be6a798fd>>
 * @lightSyntaxTransform
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
export type organizationAdminSsoSection_removeOrganizationSsoSettingsMutation$variables = {
  input: RemoveOrganizationSsoSettingsInput;
};
export type organizationAdminSsoSection_removeOrganizationSsoSettingsMutation$data = {
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
export type organizationAdminSsoSection_removeOrganizationSsoSettingsMutation$rawResponse = {
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
export type organizationAdminSsoSection_removeOrganizationSsoSettingsMutation = {
  rawResponse: organizationAdminSsoSection_removeOrganizationSsoSettingsMutation$rawResponse;
  response: organizationAdminSsoSection_removeOrganizationSsoSettingsMutation$data;
  variables: organizationAdminSsoSection_removeOrganizationSsoSettingsMutation$variables;
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
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationSsoSettingsDetails",
            "kind": "LinkedField",
            "name": "ssoSettings",
            "plural": false,
            "selections": [
              (v1/*:: as any*/),
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdminSsoSection_removeOrganizationSsoSettingsMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationAdminSsoSection_removeOrganizationSsoSettingsMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "6dd9b2b96ae95b474129f6c990cf680d",
    "id": null,
    "metadata": {},
    "name": "organizationAdminSsoSection_removeOrganizationSsoSettingsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminSsoSection_removeOrganizationSsoSettingsMutation(\n  $input: RemoveOrganizationSsoSettingsInput!\n) {\n  removeOrganizationSsoSettings(input: $input) {\n    organization {\n      id\n      ssoSettings {\n        id\n        isActive\n        entityId\n        loginUrl\n        appFederationMetadataUrl\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4071726415f2fb2ab784f585d179d77a";

export default node;
