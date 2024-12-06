/**
 * @generated SignedSource<<8aae91887568256d7cfd7f61f889f8a6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateMyCustomerDetailsInput = {
  clientMutationId?: string | null | undefined;
  designation?: string | null | undefined;
  familyName?: string | null | undefined;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  name?: string | null | undefined;
  phoneNumber?: string | null | undefined;
  timezone?: string | null | undefined;
  title?: string | null | undefined;
};
export type customerSettingsPersonalTab_updateMyCustomerDetailsMutation$variables = {
  input: UpdateMyCustomerDetailsInput;
};
export type customerSettingsPersonalTab_updateMyCustomerDetailsMutation$data = {
  readonly updateMyCustomerDetails: {
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
  } | null | undefined;
};
export type customerSettingsPersonalTab_updateMyCustomerDetailsMutation$rawResponse = {
  readonly updateMyCustomerDetails: {
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
  } | null | undefined;
};
export type customerSettingsPersonalTab_updateMyCustomerDetailsMutation = {
  rawResponse: customerSettingsPersonalTab_updateMyCustomerDetailsMutation$rawResponse;
  response: customerSettingsPersonalTab_updateMyCustomerDetailsMutation$data;
  variables: customerSettingsPersonalTab_updateMyCustomerDetailsMutation$variables;
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
    "name": "updateMyCustomerDetails",
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
    "name": "customerSettingsPersonalTab_updateMyCustomerDetailsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "customerSettingsPersonalTab_updateMyCustomerDetailsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "d0ababb18171b61d3618db2704890d62",
    "id": null,
    "metadata": {},
    "name": "customerSettingsPersonalTab_updateMyCustomerDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation customerSettingsPersonalTab_updateMyCustomerDetailsMutation(\n  $input: UpdateMyCustomerDetailsInput!\n) {\n  updateMyCustomerDetails(input: $input) {\n    customer {\n      id\n      timezone\n      designation\n      title\n      name\n      givenName\n      middleName\n      familyName\n      phoneNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7c9cf3f21539ecf5b6a7b66aaf399d2b";

export default node;
