/**
 * @generated SignedSource<<587862c377739d604143dcd67fe37640>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationTaxDetailsInput = {
  clientMutationId?: string | null | undefined;
  gstNumber: string;
  gstPercentage: string;
  organizationId: string;
};
export type organizationAdmin_updateOrganizationTaxDetailsMutation$variables = {
  input: UpdateOrganizationTaxDetailsInput;
};
export type organizationAdmin_updateOrganizationTaxDetailsMutation$data = {
  readonly updateOrganizationTaxDetails: {
    readonly organization: {
      readonly id: string;
      readonly taxDetails: {
        readonly gstNumber: string;
        readonly gstPercentage: string;
      } | null | undefined;
    };
  };
};
export type organizationAdmin_updateOrganizationTaxDetailsMutation$rawResponse = {
  readonly updateOrganizationTaxDetails: {
    readonly organization: {
      readonly id: string;
      readonly taxDetails: {
        readonly gstNumber: string;
        readonly gstPercentage: string;
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
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
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
            "concreteType": "OrganizationTaxDetails",
            "kind": "LinkedField",
            "name": "taxDetails",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "gstNumber",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "gstPercentage",
                "storageKey": null
              }
            ],
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdmin_updateOrganizationTaxDetailsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_updateOrganizationTaxDetailsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "d2794cdbed26baffb6420621c6a9c22c",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_updateOrganizationTaxDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_updateOrganizationTaxDetailsMutation(\n  $input: UpdateOrganizationTaxDetailsInput!\n) {\n  updateOrganizationTaxDetails(input: $input) {\n    organization {\n      id\n      taxDetails {\n        gstNumber\n        gstPercentage\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "18883ef4b0e7677729ec4014e67b24ff";

export default node;
