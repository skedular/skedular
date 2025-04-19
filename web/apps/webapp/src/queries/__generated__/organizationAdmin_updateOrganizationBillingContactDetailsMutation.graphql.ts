/**
 * @generated SignedSource<<774e89efa6d5e434e78ce401f23e0f65>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationBillingContactDetailsInput = {
  addressLine1?: string | null | undefined;
  addressLine2?: string | null | undefined;
  city?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  country?: string | null | undefined;
  email?: string | null | undefined;
  organizationId: string;
  province?: string | null | undefined;
  suburb?: string | null | undefined;
  zipcode?: string | null | undefined;
};
export type organizationAdmin_updateOrganizationBillingContactDetailsMutation$variables = {
  input: UpdateOrganizationBillingContactDetailsInput;
};
export type organizationAdmin_updateOrganizationBillingContactDetailsMutation$data = {
  readonly updateOrganizationBillingContactDetails: {
    readonly organizationBillingContactDetails: {
      readonly addressLine1: string | null | undefined;
      readonly addressLine2: string | null | undefined;
      readonly city: string | null | undefined;
      readonly country: string | null | undefined;
      readonly email: string | null | undefined;
      readonly id: string;
      readonly province: string | null | undefined;
      readonly suburb: string | null | undefined;
      readonly zipcode: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationAdmin_updateOrganizationBillingContactDetailsMutation$rawResponse = {
  readonly updateOrganizationBillingContactDetails: {
    readonly organizationBillingContactDetails: {
      readonly addressLine1: string | null | undefined;
      readonly addressLine2: string | null | undefined;
      readonly city: string | null | undefined;
      readonly country: string | null | undefined;
      readonly email: string | null | undefined;
      readonly id: string;
      readonly province: string | null | undefined;
      readonly suburb: string | null | undefined;
      readonly zipcode: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationAdmin_updateOrganizationBillingContactDetailsMutation = {
  rawResponse: organizationAdmin_updateOrganizationBillingContactDetailsMutation$rawResponse;
  response: organizationAdmin_updateOrganizationBillingContactDetailsMutation$data;
  variables: organizationAdmin_updateOrganizationBillingContactDetailsMutation$variables;
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
    "concreteType": "OrganizationBillingContactDetailsPayload",
    "kind": "LinkedField",
    "name": "updateOrganizationBillingContactDetails",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationBillingContactDetails",
        "kind": "LinkedField",
        "name": "organizationBillingContactDetails",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdmin_updateOrganizationBillingContactDetailsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_updateOrganizationBillingContactDetailsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "69e6c61d64ae1e2361d3eb95a51d68c2",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_updateOrganizationBillingContactDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_updateOrganizationBillingContactDetailsMutation(\n  $input: UpdateOrganizationBillingContactDetailsInput!\n) {\n  updateOrganizationBillingContactDetails(input: $input) {\n    organizationBillingContactDetails {\n      id\n      email\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "abdd45eb30ec66058423fee2411bfe05";

export default node;
