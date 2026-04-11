/**
 * @generated SignedSource<<28517533fc24f4d47975a88ed4c7ef4d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationTaxDetailsInput = {
  clientMutationId?: string | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
  taxId: string;
  taxRatePercentage: any;
};
export type organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation$variables = {
  input: UpdateOrganizationTaxDetailsInput;
};
export type organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation$data = {
  readonly updateOrganizationTaxDetails: {
    readonly organization: {
      readonly id: string;
      readonly taxDetails: {
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
  "name": "taxId",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "taxRatePercentage",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTaxDetails",
                "kind": "LinkedField",
                "name": "taxDetails",
                "plural": false,
                "selections": [
                  (v3/*: any*/),
                  (v4/*: any*/)
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTaxDetails",
                "kind": "LinkedField",
                "name": "taxDetails",
                "plural": false,
                "selections": [
                  (v3/*: any*/),
                  (v4/*: any*/),
                  (v2/*: any*/)
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
    "cacheID": "88114606545b20b34ca81cc49921db62",
    "id": null,
    "metadata": {},
    "name": "organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminTaxDetailsSection_updateOrganizationTaxDetailsMutation(\n  $input: UpdateOrganizationTaxDetailsInput!\n) {\n  updateOrganizationTaxDetails(input: $input) {\n    organization {\n      id\n      taxDetails {\n        taxId\n        taxRatePercentage\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1ba115004f237dfff079038cf7ec5060";

export default node;
