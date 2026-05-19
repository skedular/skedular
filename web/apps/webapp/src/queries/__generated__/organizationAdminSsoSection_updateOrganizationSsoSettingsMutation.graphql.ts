/**
 * @generated SignedSource<<00080fdf6a6ab709b0b919fca730c135>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationSsoSettingsInput = {
  appFederationMetadataUrl: string;
  clientMutationId?: string | null | undefined;
  entityId: string;
  isActive: boolean;
  loginUrl: string;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type organizationAdminSsoSection_updateOrganizationSsoSettingsMutation$variables = {
  input: UpdateOrganizationSsoSettingsInput;
};
export type organizationAdminSsoSection_updateOrganizationSsoSettingsMutation$data = {
  readonly updateOrganizationSsoSettings: {
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
export type organizationAdminSsoSection_updateOrganizationSsoSettingsMutation$rawResponse = {
  readonly updateOrganizationSsoSettings: {
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
export type organizationAdminSsoSection_updateOrganizationSsoSettingsMutation = {
  rawResponse: organizationAdminSsoSection_updateOrganizationSsoSettingsMutation$rawResponse;
  response: organizationAdminSsoSection_updateOrganizationSsoSettingsMutation$data;
  variables: organizationAdminSsoSection_updateOrganizationSsoSettingsMutation$variables;
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
    "name": "updateOrganizationSsoSettings",
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
    "name": "organizationAdminSsoSection_updateOrganizationSsoSettingsMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationAdminSsoSection_updateOrganizationSsoSettingsMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "68cbf42de08a3451d93903e983f0cfd3",
    "id": null,
    "metadata": {},
    "name": "organizationAdminSsoSection_updateOrganizationSsoSettingsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminSsoSection_updateOrganizationSsoSettingsMutation(\n  $input: UpdateOrganizationSsoSettingsInput!\n) {\n  updateOrganizationSsoSettings(input: $input) {\n    organization {\n      id\n      ssoSettings {\n        id\n        isActive\n        entityId\n        loginUrl\n        appFederationMetadataUrl\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "deb09c398a71f846cd05ecc125f277a8";

export default node;
