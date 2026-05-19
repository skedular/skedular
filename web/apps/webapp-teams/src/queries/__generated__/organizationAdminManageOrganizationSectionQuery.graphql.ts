/**
 * @generated SignedSource<<5e0cc6b334df078477d833dd447df4bc>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationAdminManageOrganizationSectionQuery$variables = {
  organizationCustomDomain: string;
};
export type organizationAdminManageOrganizationSectionQuery$data = {
  readonly organization: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type organizationAdminManageOrganizationSectionQuery = {
  response: organizationAdminManageOrganizationSectionQuery$data;
  variables: organizationAdminManageOrganizationSectionQuery$variables;
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
    "name": "organizationAdminManageOrganizationSectionQuery",
    "selections": (v1/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationAdminManageOrganizationSectionQuery",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "ce0940288f96b7b94b9bb5ce8b3e4887",
    "id": null,
    "metadata": {},
    "name": "organizationAdminManageOrganizationSectionQuery",
    "operationKind": "query",
    "text": "query organizationAdminManageOrganizationSectionQuery(\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "8c50cb84ae8b8fdedb72dad598f697ab";

export default node;
