/**
 * @generated SignedSource<<967df9ce29e61417cc6703fe8f475e81>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type SetOrganizationBillingInfoInput = {
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
export type organizationAdmin_setOrganizationBillingInfoMutation$variables = {
  input: SetOrganizationBillingInfoInput;
};
export type organizationAdmin_setOrganizationBillingInfoMutation$data = {
  readonly setOrganizationBillingInfo: {
    readonly organizationBillingInfo: {
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
export type organizationAdmin_setOrganizationBillingInfoMutation$rawResponse = {
  readonly setOrganizationBillingInfo: {
    readonly organizationBillingInfo: {
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
export type organizationAdmin_setOrganizationBillingInfoMutation = {
  rawResponse: organizationAdmin_setOrganizationBillingInfoMutation$rawResponse;
  response: organizationAdmin_setOrganizationBillingInfoMutation$data;
  variables: organizationAdmin_setOrganizationBillingInfoMutation$variables;
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
    "concreteType": "OrganizationBillingInfoPayload",
    "kind": "LinkedField",
    "name": "setOrganizationBillingInfo",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationBillingInfo",
        "kind": "LinkedField",
        "name": "organizationBillingInfo",
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
    "name": "organizationAdmin_setOrganizationBillingInfoMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_setOrganizationBillingInfoMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "4cc4577fd8a2d7cb351aebfc8bdf5c20",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_setOrganizationBillingInfoMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_setOrganizationBillingInfoMutation(\n  $input: SetOrganizationBillingInfoInput!\n) {\n  setOrganizationBillingInfo(input: $input) {\n    organizationBillingInfo {\n      id\n      email\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2c427d41bfc4810e0533735875d8c2a1";

export default node;
