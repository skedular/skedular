/**
 * @generated SignedSource<<bac71845bd4a9f9525252c2d8fb87134>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationSettingsTaxDetailsSectionQuery$variables = {
  organizationCustomDomain: string;
};
export type organizationSettingsTaxDetailsSectionQuery$data = {
  readonly organization: {
    readonly id: string;
    readonly name: string;
    readonly taxDetails: {
      readonly isRegistered: boolean;
      readonly taxId: string;
      readonly taxRatePercentage: any;
    } | null | undefined;
  } | null | undefined;
};
export type organizationSettingsTaxDetailsSectionQuery = {
  response: organizationSettingsTaxDetailsSectionQuery$data;
  variables: organizationSettingsTaxDetailsSectionQuery$variables;
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
  "name": "isRegistered",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "taxId",
  "storageKey": null
},
v6 = {
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
    "name": "organizationSettingsTaxDetailsSectionQuery",
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
              (v6/*:: as any*/)
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
    "name": "organizationSettingsTaxDetailsSectionQuery",
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
              (v6/*:: as any*/),
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
    "cacheID": "c2b1c5b748f4916cbe3335978f0e8921",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsTaxDetailsSectionQuery",
    "operationKind": "query",
    "text": "query organizationSettingsTaxDetailsSectionQuery(\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n    taxDetails {\n      isRegistered\n      taxId\n      taxRatePercentage\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "220b2a1f549c7413254509f11f55e7bf";

export default node;
