/**
 * @generated SignedSource<<a6519025d0be5cdb3ffc3e7b120266ab>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationTaxDetailsPatchField = "IS_REGISTERED" | "TAX_ID" | "TAX_RATE_PERCENTAGE" | "%future added value";
export type UpdateOrganizationTaxDetailsInput = {
  clientMutationId?: string | null | undefined;
  fieldsToUpdate: ReadonlyArray<OrganizationTaxDetailsPatchField>;
  isRegistered?: boolean | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
  taxId?: string | null | undefined;
  taxRatePercentage?: any | null | undefined;
};
export type organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation$variables = {
  input: UpdateOrganizationTaxDetailsInput;
};
export type organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation$data = {
  readonly updateOrganizationTaxDetails: {
    readonly organization: {
      readonly id: string;
      readonly taxDetails: {
        readonly isRegistered: boolean;
        readonly taxId: string;
        readonly taxRatePercentage: any;
      } | null | undefined;
    };
  };
};
export type organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation$rawResponse = {
  readonly updateOrganizationTaxDetails: {
    readonly organization: {
      readonly id: string;
      readonly taxDetails: {
        readonly id: string;
        readonly isRegistered: boolean;
        readonly taxId: string;
        readonly taxRatePercentage: any;
      } | null | undefined;
    };
  };
};
export type organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation = {
  rawResponse: organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation$rawResponse;
  response: organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation$data;
  variables: organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation$variables;
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
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
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
  "name": "isRegistered",
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
    "name": "organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationPayload",
        "kind": "LinkedField",
        "name": "updateOrganizationTaxDetails",
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
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTaxDetails",
                "kind": "LinkedField",
                "name": "taxDetails",
                "plural": false,
                "selections": [
                  (v3/*:: as any*/),
                  (v4/*:: as any*/),
                  (v5/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationPayload",
        "kind": "LinkedField",
        "name": "updateOrganizationTaxDetails",
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
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTaxDetails",
                "kind": "LinkedField",
                "name": "taxDetails",
                "plural": false,
                "selections": [
                  (v3/*:: as any*/),
                  (v4/*:: as any*/),
                  (v5/*:: as any*/),
                  (v2/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "0f96ef4aed455e5091d84f33b875c745",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsTaxDetailsSection_updateOrganizationTaxDetailsMutation(\n  $input: UpdateOrganizationTaxDetailsInput!\n) {\n  updateOrganizationTaxDetails(input: $input) {\n    organization {\n      id\n      taxDetails {\n        isRegistered\n        taxId\n        taxRatePercentage\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "66c4fc00106fb02eb3d39c6a1d012f4d";

export default node;
