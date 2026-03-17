/**
 * @generated SignedSource<<77cff4afcab19dca268e4c56dbaf5709>>
 * @lightSyntaxTransform
 * @nogrep
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
export type organizationAdmin_updateOrganizationSsoSettingsMutation$variables = {
  input: UpdateOrganizationSsoSettingsInput;
};
export type organizationAdmin_updateOrganizationSsoSettingsMutation$data = {
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
export type organizationAdmin_updateOrganizationSsoSettingsMutation$rawResponse = {
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
export type organizationAdmin_updateOrganizationSsoSettingsMutation = {
  rawResponse: organizationAdmin_updateOrganizationSsoSettingsMutation$rawResponse;
  response: organizationAdmin_updateOrganizationSsoSettingsMutation$data;
  variables: organizationAdmin_updateOrganizationSsoSettingsMutation$variables;
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
    "name": "organizationAdmin_updateOrganizationSsoSettingsMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_updateOrganizationSsoSettingsMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "5b7165fe673176cb2cb86dbfdd44d547",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_updateOrganizationSsoSettingsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_updateOrganizationSsoSettingsMutation(\n  $input: UpdateOrganizationSsoSettingsInput!\n) {\n  updateOrganizationSsoSettings(input: $input) {\n    organization {\n      id\n      ssoSettings {\n        id\n        isActive\n        entityId\n        loginUrl\n        appFederationMetadataUrl\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d8eb46d644b6df200499868688c8c58d";

export default node;
