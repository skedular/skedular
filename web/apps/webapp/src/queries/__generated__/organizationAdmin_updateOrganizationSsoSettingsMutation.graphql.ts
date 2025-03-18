/**
 * @generated SignedSource<<371b335536ecaea40e0b56eae711215e>>
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
  id?: string | null | undefined;
  loginUrl: string;
  organizationId: string;
};
export type organizationAdmin_updateOrganizationSsoSettingsMutation$variables = {
  input: UpdateOrganizationSsoSettingsInput;
};
export type organizationAdmin_updateOrganizationSsoSettingsMutation$data = {
  readonly updateOrganizationSsoSettings: {
    readonly ssoSettings: {
      readonly appFederationMetadataUrl: string;
      readonly entityId: string;
      readonly id: string;
      readonly loginUrl: string;
    };
  } | null | undefined;
};
export type organizationAdmin_updateOrganizationSsoSettingsMutation$rawResponse = {
  readonly updateOrganizationSsoSettings: {
    readonly ssoSettings: {
      readonly appFederationMetadataUrl: string;
      readonly entityId: string;
      readonly id: string;
      readonly loginUrl: string;
    };
  } | null | undefined;
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
    "name": "updateOrganizationSsoSettings",
    "plural": false,
    "selections": [
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
            "name": "id",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdmin_updateOrganizationSsoSettingsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_updateOrganizationSsoSettingsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "30f05d33a3519e85cbba5ee2c42635fe",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_updateOrganizationSsoSettingsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_updateOrganizationSsoSettingsMutation(\n  $input: UpdateOrganizationSsoSettingsInput!\n) {\n  updateOrganizationSsoSettings(input: $input) {\n    ssoSettings {\n      id\n      entityId\n      loginUrl\n      appFederationMetadataUrl\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d41c045424882e9b556ca53ddfbbde93";

export default node;
