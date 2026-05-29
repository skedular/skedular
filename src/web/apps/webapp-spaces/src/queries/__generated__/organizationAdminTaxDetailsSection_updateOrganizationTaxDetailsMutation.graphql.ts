/**
 * @generated SignedSource<<dfc78bfb82feda8cf64f25c16b741fa7>>
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
export type organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation$variables = {
  input: UpdateOrganizationTaxDetailsInput;
};
export type organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation$data = {
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
export type organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation$rawResponse = {
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
export type organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation = {
  rawResponse: organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation$rawResponse;
  response: organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation$data;
  variables: organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation$variables;
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
    "name": "organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation",
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
    "name": "organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation",
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
    "cacheID": "54f4740ef10258dd0b0ebdf5ee5afc3b",
    "id": null,
    "metadata": {},
    "name": "organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation(\n  $input: UpdateOrganizationTaxDetailsInput!\n) {\n  updateOrganizationTaxDetails(input: $input) {\n    organization {\n      id\n      taxDetails {\n        isRegistered\n        taxId\n        taxRatePercentage\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "994753b793d1b3ae14ab924613bcdebe";

export default node;
