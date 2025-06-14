/**
 * @generated SignedSource<<65fe4554de2d9d6c11561515a28747de>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationBillingDetailsInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city: string;
  clientMutationId?: string | null | undefined;
  companyName?: string | null | undefined;
  country: string;
  email: string;
  id: string;
  province?: string | null | undefined;
  suburb: string;
  zipcode: string;
};
export type organizationAdmin_updateOrganizationBillingDetailsMutation$variables = {
  input: UpdateOrganizationBillingDetailsInput;
};
export type organizationAdmin_updateOrganizationBillingDetailsMutation$data = {
  readonly updateOrganizationBillingDetails: {
    readonly organization: {
      readonly billingDetails: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly companyName: string | null | undefined;
        readonly country: string;
        readonly email: string;
        readonly id: string;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      } | null | undefined;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationAdmin_updateOrganizationBillingDetailsMutation$rawResponse = {
  readonly updateOrganizationBillingDetails: {
    readonly organization: {
      readonly billingDetails: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly companyName: string | null | undefined;
        readonly country: string;
        readonly email: string;
        readonly id: string;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      } | null | undefined;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationAdmin_updateOrganizationBillingDetailsMutation = {
  rawResponse: organizationAdmin_updateOrganizationBillingDetailsMutation$rawResponse;
  response: organizationAdmin_updateOrganizationBillingDetailsMutation$data;
  variables: organizationAdmin_updateOrganizationBillingDetailsMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
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
    "name": "updateOrganizationBillingDetails",
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationBillingDetails",
            "kind": "LinkedField",
            "name": "billingDetails",
            "plural": false,
            "selections": [
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "companyName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "addressLine1",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "addressLine2",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "suburb",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "city",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "province",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "zipcode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "country",
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
    "name": "organizationAdmin_updateOrganizationBillingDetailsMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_updateOrganizationBillingDetailsMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "cc82de4b78f03d960eee0843931062d8",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_updateOrganizationBillingDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_updateOrganizationBillingDetailsMutation(\n  $input: UpdateOrganizationBillingDetailsInput!\n) {\n  updateOrganizationBillingDetails(input: $input) {\n    organization {\n      id\n      billingDetails {\n        id\n        companyName\n        email\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6ea9e1661a973d5fd580731d4a855318";

export default node;
