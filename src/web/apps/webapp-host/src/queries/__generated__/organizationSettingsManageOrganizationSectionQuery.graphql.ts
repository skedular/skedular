/**
 * @generated SignedSource<<fe299c01e0b7d83697426573f8c0b8fe>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationSettingsManageOrganizationSectionQuery$variables = {
  organizationCustomDomain: string;
};
export type organizationSettingsManageOrganizationSectionQuery$data = {
  readonly organization: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type organizationSettingsManageOrganizationSectionQuery = {
  response: organizationSettingsManageOrganizationSectionQuery$data;
  variables: organizationSettingsManageOrganizationSectionQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  }
],
v1 = [
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
        "name": "name",
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
    "name": "organizationSettingsManageOrganizationSectionQuery",
    "selections": (v1/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsManageOrganizationSectionQuery",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "c75e9b0b35964423a496a1ed3a362bc9",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsManageOrganizationSectionQuery",
    "operationKind": "query",
    "text": "query organizationSettingsManageOrganizationSectionQuery(\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "55ca17cc4b435b2ac17d8f4f2738d38c";

export default node;
