/**
 * @generated SignedSource<<85fb2979624efd3dc4992e17b0aa3a29>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveOrganizationTaxDetailsInput = {
  clientMutationId?: string | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation$variables = {
  input: RemoveOrganizationTaxDetailsInput;
};
export type organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation$data = {
  readonly removeOrganizationTaxDetails: {
    readonly organization: {
      readonly id: string;
      readonly taxDetails: {
        readonly taxId: string;
        readonly taxRatePercentage: any;
      } | null | undefined;
    };
  };
};
export type organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation$rawResponse = {
  readonly removeOrganizationTaxDetails: {
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
export type organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation = {
  rawResponse: organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation$rawResponse;
  response: organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation$data;
  variables: organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationPayload",
        "kind": "LinkedField",
        "name": "removeOrganizationTaxDetails",
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
                  (v4/*:: as any*/)
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
    "name": "organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationPayload",
        "kind": "LinkedField",
        "name": "removeOrganizationTaxDetails",
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
    "cacheID": "27cb47568d6a8dbba3f0954011377bad",
    "id": null,
    "metadata": {},
    "name": "organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminTaxDetailsSection_removeOrganizationTaxDetailsMutation(\n  $input: RemoveOrganizationTaxDetailsInput!\n) {\n  removeOrganizationTaxDetails(input: $input) {\n    organization {\n      id\n      taxDetails {\n        taxId\n        taxRatePercentage\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f79f9b614dd53b8d321438b42ac2552d";

export default node;
