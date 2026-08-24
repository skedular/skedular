/**
 * @generated SignedSource<<c1c8b9674e4bc51f23c8cadbed3e9c9a>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationSsoSettingsPatchField = "SSO_SETTINGS" | "%future added value";
export type UpdateOrganizationSsoSettingsInput = {
  appFederationMetadataUrl: string;
  clientMutationId?: string | null | undefined;
  entityId: string;
  fieldsToUpdate: ReadonlyArray<OrganizationSsoSettingsPatchField>;
  isActive: boolean;
  loginUrl: string;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation$variables = {
  input: UpdateOrganizationSsoSettingsInput;
};
export type organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation$data = {
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
export type organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation$rawResponse = {
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
export type organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation = {
  rawResponse: organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation$rawResponse;
  response: organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation$data;
  variables: organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation$variables;
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
    "name": "organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "69ea65b081b651e4d47db7b75e34b259",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsSsoSection_updateOrganizationSsoSettingsMutation(\n  $input: UpdateOrganizationSsoSettingsInput!\n) {\n  updateOrganizationSsoSettings(input: $input) {\n    organization {\n      id\n      ssoSettings {\n        id\n        isActive\n        entityId\n        loginUrl\n        appFederationMetadataUrl\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1b7dc96f2ab8e1ad8f28115dd59e7e42";

export default node;
