/**
 * @generated SignedSource<<f6e0b85920a202e82ea7ce540ae85c2f>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationAdminTaxDetailsSectionQuery$variables = {
  organizationCustomDomain: string;
};
export type organizationAdminTaxDetailsSectionQuery$data = {
  readonly organization: {
    readonly id: string;
    readonly name: string;
    readonly taxDetails: {
      readonly taxId: string;
      readonly taxRatePercentage: any;
    } | null | undefined;
  } | null | undefined;
};
export type organizationAdminTaxDetailsSectionQuery = {
  response: organizationAdminTaxDetailsSectionQuery$data;
  variables: organizationAdminTaxDetailsSectionQuery$variables;
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
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "taxId",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "taxRatePercentage",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdminTaxDetailsSectionQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTaxDetails",
            "kind": "LinkedField",
            "name": "taxDetails",
            "plural": false,
            "selections": [
              (v4/*:: as any*/),
              (v5/*:: as any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationAdminTaxDetailsSectionQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTaxDetails",
            "kind": "LinkedField",
            "name": "taxDetails",
            "plural": false,
            "selections": [
              (v4/*:: as any*/),
              (v5/*:: as any*/),
              (v2/*:: as any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "2ce8b35fc90dfe1ab9443c160e87de8d",
    "id": null,
    "metadata": {},
    "name": "organizationAdminTaxDetailsSectionQuery",
    "operationKind": "query",
    "text": "query organizationAdminTaxDetailsSectionQuery(\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n    taxDetails {\n      taxId\n      taxRatePercentage\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "88740e028c75787e669d593eeaec3534";

export default node;
