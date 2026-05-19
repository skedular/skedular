/**
 * @generated SignedSource<<938a90e7bb855a14ec5f24ae7b3ac434>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PersonalInformationVisibility = "REDACTED" | "VISIBLE" | "%future added value";
export type UpdateCustomerDetailsInput = {
  clientMutationId?: string | null | undefined;
  designation?: string | null | undefined;
  familyName?: string | null | undefined;
  givenName?: string | null | undefined;
  id: string;
  middleName?: string | null | undefined;
  name?: string | null | undefined;
  personalInformationVisibility: PersonalInformationVisibility;
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
      readonly personalInformationVisibility: {
        readonly name: string;
        readonly type: PersonalInformationVisibility;
      };
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
      readonly personalInformationVisibility: {
        readonly name: string;
        readonly type: PersonalInformationVisibility;
      };
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
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
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
          (v1/*:: as any*/),
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
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "PersonalInformationVisibilityDetails",
            "kind": "LinkedField",
            "name": "personalInformationVisibility",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "type",
                "storageKey": null
              },
              (v1/*:: as any*/)
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "mySettings_updateCustomerDetailsMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "mySettings_updateCustomerDetailsMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "f14feac77106870cc1cb21db26470007",
    "id": null,
    "metadata": {},
    "name": "mySettings_updateCustomerDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation mySettings_updateCustomerDetailsMutation(\n  $input: UpdateCustomerDetailsInput!\n) {\n  updateCustomerDetails(input: $input) {\n    customer {\n      id\n      timezone\n      designation\n      title\n      name\n      givenName\n      middleName\n      familyName\n      phoneNumber\n      personalInformationVisibility {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "86f28efb5ff608696feaf554b90d059f";

export default node;
