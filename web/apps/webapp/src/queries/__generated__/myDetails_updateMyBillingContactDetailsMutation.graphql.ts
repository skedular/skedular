/**
 * @generated SignedSource<<668f48324925f1d45b57c361c1d86897>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateMyBillingContactDetailsInput = {
  addressLine1?: string | null | undefined;
  addressLine2?: string | null | undefined;
  city?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  companyName?: string | null | undefined;
  country?: string | null | undefined;
  email?: string | null | undefined;
  province?: string | null | undefined;
  suburb?: string | null | undefined;
  zipcode?: string | null | undefined;
};
export type myDetails_updateMyBillingContactDetailsMutation$variables = {
  input: UpdateMyBillingContactDetailsInput;
};
export type myDetails_updateMyBillingContactDetailsMutation$data = {
  readonly updateMyBillingContactDetails: {
    readonly customerBillingContactDetails: {
      readonly addressLine1: string | null | undefined;
      readonly addressLine2: string | null | undefined;
      readonly city: string | null | undefined;
      readonly companyName: string | null | undefined;
      readonly country: string | null | undefined;
      readonly email: string | null | undefined;
      readonly id: string;
      readonly province: string | null | undefined;
      readonly suburb: string | null | undefined;
      readonly zipcode: string | null | undefined;
    };
  } | null | undefined;
};
export type myDetails_updateMyBillingContactDetailsMutation$rawResponse = {
  readonly updateMyBillingContactDetails: {
    readonly customerBillingContactDetails: {
      readonly addressLine1: string | null | undefined;
      readonly addressLine2: string | null | undefined;
      readonly city: string | null | undefined;
      readonly companyName: string | null | undefined;
      readonly country: string | null | undefined;
      readonly email: string | null | undefined;
      readonly id: string;
      readonly province: string | null | undefined;
      readonly suburb: string | null | undefined;
      readonly zipcode: string | null | undefined;
    };
  } | null | undefined;
};
export type myDetails_updateMyBillingContactDetailsMutation = {
  rawResponse: myDetails_updateMyBillingContactDetailsMutation$rawResponse;
  response: myDetails_updateMyBillingContactDetailsMutation$data;
  variables: myDetails_updateMyBillingContactDetailsMutation$variables;
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
    "concreteType": "MyBillingContactDetailsPayload",
    "kind": "LinkedField",
    "name": "updateMyBillingContactDetails",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerBillingContactDetails",
        "kind": "LinkedField",
        "name": "customerBillingContactDetails",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "myDetails_updateMyBillingContactDetailsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myDetails_updateMyBillingContactDetailsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "4e6ed680f1d631c909accb3644784534",
    "id": null,
    "metadata": {},
    "name": "myDetails_updateMyBillingContactDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation myDetails_updateMyBillingContactDetailsMutation(\n  $input: UpdateMyBillingContactDetailsInput!\n) {\n  updateMyBillingContactDetails(input: $input) {\n    customerBillingContactDetails {\n      id\n      companyName\n      email\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0d5880d0626f6f9f7017d829c2e2a1db";

export default node;
