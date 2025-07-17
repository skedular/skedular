/**
 * @generated SignedSource<<980a65b04c5643948f4886b901be3a83>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveOrganizationTaxDetailsInput = {
  clientMutationId?: string | null | undefined;
  organizationId: string;
};
export type organizationAdmin_removeOrganizationTaxDetailsMutation$variables = {
  input: RemoveOrganizationTaxDetailsInput;
};
export type organizationAdmin_removeOrganizationTaxDetailsMutation$data = {
  readonly removeOrganizationTaxDetails: {
    readonly organization: {
      readonly id: string;
      readonly taxDetails: {
        readonly taxId: string;
        readonly taxRatePercentage: string;
      } | null | undefined;
    };
  };
};
export type organizationAdmin_removeOrganizationTaxDetailsMutation$rawResponse = {
  readonly removeOrganizationTaxDetails: {
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
export type organizationAdmin_removeOrganizationTaxDetailsMutation = {
  rawResponse: organizationAdmin_removeOrganizationTaxDetailsMutation$rawResponse;
  response: organizationAdmin_removeOrganizationTaxDetailsMutation$data;
  variables: organizationAdmin_removeOrganizationTaxDetailsMutation$variables;
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
    "name": "organizationAdmin_removeOrganizationTaxDetailsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
    "name": "organizationAdmin_removeOrganizationTaxDetailsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
    "cacheID": "b743fbaa19325c7c77dd9bb30042826f",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_removeOrganizationTaxDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_removeOrganizationTaxDetailsMutation(\n  $input: RemoveOrganizationTaxDetailsInput!\n) {\n  removeOrganizationTaxDetails(input: $input) {\n    organization {\n      id\n      taxDetails {\n        taxId\n        taxRatePercentage\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "79f55e78de3a77303dab298eed1471cd";

export default node;
