/**
 * @generated SignedSource<<81d3b55ca42d8be8df4f5013ae55e8ca>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationTaxDetailsInput = {
  clientMutationId?: string | null | undefined;
  organizationId: string;
  taxId: string;
  taxRatePercentage: string;
};
export type organizationAdmin_updateOrganizationTaxDetailsMutation$variables = {
  input: UpdateOrganizationTaxDetailsInput;
};
export type organizationAdmin_updateOrganizationTaxDetailsMutation$data = {
  readonly updateOrganizationTaxDetails: {
    readonly organization: {
      readonly id: string;
      readonly taxDetails: {
        readonly taxId: string;
        readonly taxRatePercentage: string;
      } | null | undefined;
    };
  };
};
export type organizationAdmin_updateOrganizationTaxDetailsMutation$rawResponse = {
  readonly updateOrganizationTaxDetails: {
    readonly organization: {
      readonly id: string;
      readonly taxDetails: {
        readonly id: string;
        readonly taxId: string;
        readonly taxRatePercentage: string;
      } | null | undefined;
    };
  };
};
export type organizationAdmin_updateOrganizationTaxDetailsMutation = {
  rawResponse: organizationAdmin_updateOrganizationTaxDetailsMutation$rawResponse;
  response: organizationAdmin_updateOrganizationTaxDetailsMutation$data;
  variables: organizationAdmin_updateOrganizationTaxDetailsMutation$variables;
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
    "name": "organizationAdmin_updateOrganizationTaxDetailsMutation",
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
    "name": "organizationAdmin_updateOrganizationTaxDetailsMutation",
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
    "cacheID": "ef01f072b46d17f2876576e3b30910c6",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_updateOrganizationTaxDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_updateOrganizationTaxDetailsMutation(\n  $input: UpdateOrganizationTaxDetailsInput!\n) {\n  updateOrganizationTaxDetails(input: $input) {\n    organization {\n      id\n      taxDetails {\n        taxId\n        taxRatePercentage\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c73545bd47f18d53fa4934db1763617e";

export default node;
