/**
 * @generated SignedSource<<56f74124418cf6e2559f2a935b679432>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateCustomerDetailsInput = {
  clientMutationId?: string | null | undefined;
  designation?: string | null | undefined;
  familyName?: string | null | undefined;
  givenName?: string | null | undefined;
  id: string;
  middleName?: string | null | undefined;
  name?: string | null | undefined;
  phoneNumber?: string | null | undefined;
  timezone?: string | null | undefined;
  title?: string | null | undefined;
};
export type mySettings_updateCustomerDetailsMutation$variables = {
  input: UpdateCustomerDetailsInput;
};
export type mySettings_updateCustomerDetailsMutation$data = {
  readonly updateCustomerDetails: {
    readonly customer: {
      readonly designation: string | null | undefined;
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly id: string;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
      readonly phoneNumber: string | null | undefined;
      readonly timezone: string | null | undefined;
      readonly title: string | null | undefined;
    };
  };
};
export type mySettings_updateCustomerDetailsMutation$rawResponse = {
  readonly updateCustomerDetails: {
    readonly customer: {
      readonly designation: string | null | undefined;
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly id: string;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
      readonly phoneNumber: string | null | undefined;
      readonly timezone: string | null | undefined;
      readonly title: string | null | undefined;
    };
  };
};
export type mySettings_updateCustomerDetailsMutation = {
  rawResponse: mySettings_updateCustomerDetailsMutation$rawResponse;
  response: mySettings_updateCustomerDetailsMutation$data;
  variables: mySettings_updateCustomerDetailsMutation$variables;
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
    "concreteType": "CustomerPayload",
    "kind": "LinkedField",
    "name": "updateCustomerDetails",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "customer",
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
            "name": "timezone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "designation",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "title",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "givenName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "middleName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "familyName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "phoneNumber",
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
    "name": "mySettings_updateCustomerDetailsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "mySettings_updateCustomerDetailsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "1f07c68b6a90606771e04ce22a6cfbe8",
    "id": null,
    "metadata": {},
    "name": "mySettings_updateCustomerDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation mySettings_updateCustomerDetailsMutation(\n  $input: UpdateCustomerDetailsInput!\n) {\n  updateCustomerDetails(input: $input) {\n    customer {\n      id\n      timezone\n      designation\n      title\n      name\n      givenName\n      middleName\n      familyName\n      phoneNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "785add8bfb66aa625b7c7be01937492a";

export default node;
