/**
 * @generated SignedSource<<a3dde9e12ce86179081f94b73811cf3f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationAdminSsoSectionQuery$variables = {
  organizationCustomDomain: string;
};
export type organizationAdminSsoSectionQuery$data = {
  readonly organization: {
    readonly customDomain: string | null | undefined;
    readonly id: string;
    readonly name: string;
    readonly ssoSettings: {
      readonly appFederationMetadataUrl: string;
      readonly entityId: string;
      readonly id: string;
      readonly isActive: boolean;
      readonly loginUrl: string;
    } | null | undefined;
  } | null | undefined;
};
export type organizationAdminSsoSectionQuery = {
  response: organizationAdminSsoSectionQuery$data;
  variables: organizationAdminSsoSectionQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
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
        "name": "customDomain",
        "variableName": "organizationCustomDomain"
      }
    ],
    "concreteType": "OrganizationDetails",
    "kind": "LinkedField",
    "name": "organization",
    "plural": false,
    "selections": [
      (v1/*: any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "name",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "customDomain",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdminSsoSectionQuery",
    "selections": (v2/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdminSsoSectionQuery",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "d7cc4a4bc486fec6e1925000d4d8df85",
    "id": null,
    "metadata": {},
    "name": "organizationAdminSsoSectionQuery",
    "operationKind": "query",
    "text": "query organizationAdminSsoSectionQuery(\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n    customDomain\n    ssoSettings {\n      id\n      isActive\n      entityId\n      loginUrl\n      appFederationMetadataUrl\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e7bb009809865e3b922f60c50db459f4";

export default node;
