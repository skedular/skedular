/**
 * @generated SignedSource<<36a2fb751f3932d076f2afec1fc04556>>
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
        readonly gstNumber: string;
        readonly gstPercentage: string;
      } | null | undefined;
    };
  };
};
export type organizationAdmin_removeOrganizationTaxDetailsMutation$rawResponse = {
  readonly removeOrganizationTaxDetails: {
    readonly organization: {
      readonly id: string;
      readonly taxDetails: {
        readonly gstNumber: string;
        readonly gstPercentage: string;
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
    "name": "organizationAdmin_removeOrganizationTaxDetailsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_removeOrganizationTaxDetailsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "b62bc1577b4be43fc4b6fcc73f2b64a6",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_removeOrganizationTaxDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_removeOrganizationTaxDetailsMutation(\n  $input: RemoveOrganizationTaxDetailsInput!\n) {\n  removeOrganizationTaxDetails(input: $input) {\n    organization {\n      id\n      taxDetails {\n        gstNumber\n        gstPercentage\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5fe2897370114298e7bf8ce7a9b739ef";

export default node;
