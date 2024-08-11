/**
 * @generated SignedSource<<c3c7986c9d93f546ee01f2dc267305cd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type UpdateMyCustomerDetailsInput = {
  clientMutationId?: string | null | undefined;
  designation?: string | null | undefined;
  familyName?: string | null | undefined;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  name?: string | null | undefined;
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
    "cacheID": "af292bf964f1f5f9670a5e84ddceb5d0",
    "id": null,
    "metadata": {},
    "name": "customerSettingsPersonalTab_updateMyCustomerDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation customerSettingsPersonalTab_updateMyCustomerDetailsMutation(\n  $input: UpdateMyCustomerDetailsInput!\n) {\n  updateMyCustomerDetails(input: $input) {\n    customer {\n      id\n      timezone\n      designation\n      title\n      name\n      givenName\n      middleName\n      familyName\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "04d3e426f6791d5732645dcbada88047";

export default node;
