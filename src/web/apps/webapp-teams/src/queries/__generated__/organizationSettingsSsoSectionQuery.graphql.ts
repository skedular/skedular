/**
 * @generated SignedSource<<2649a8837e845d87457a80e2b5e7984f>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationSettingsSsoSectionQuery$variables = {
  organizationCustomDomain: string;
};
export type organizationSettingsSsoSectionQuery$data = {
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
export type organizationSettingsSsoSectionQuery = {
  response: organizationSettingsSsoSectionQuery$data;
  variables: organizationSettingsSsoSectionQuery$variables;
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
      (v1/*:: as any*/),
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationSettingsSsoSectionQuery",
    "selections": (v2/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsSsoSectionQuery",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "4d268855044812279280df5291928137",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsSsoSectionQuery",
    "operationKind": "query",
    "text": "query organizationSettingsSsoSectionQuery(\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n    customDomain\n    ssoSettings {\n      id\n      isActive\n      entityId\n      loginUrl\n      appFederationMetadataUrl\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cc95830268a8e2d53ddd13840837f430";

export default node;
