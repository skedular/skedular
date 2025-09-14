/**
 * @generated SignedSource<<c6c441e55635c1ec6161b7348bea0803>>
 * @lightSyntaxTransform
 * @nogrep
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
export type organizationUser_updateCustomerDetailsMutation$variables = {
  input: UpdateCustomerDetailsInput;
};
export type organizationUser_updateCustomerDetailsMutation$data = {
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
export type organizationUser_updateCustomerDetailsMutation$rawResponse = {
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
export type organizationUser_updateCustomerDetailsMutation = {
  rawResponse: organizationUser_updateCustomerDetailsMutation$rawResponse;
  response: organizationUser_updateCustomerDetailsMutation$data;
  variables: organizationUser_updateCustomerDetailsMutation$variables;
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
          (v1/*: any*/),
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
              (v1/*: any*/)
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
    "name": "organizationUser_updateCustomerDetailsMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationUser_updateCustomerDetailsMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "db0544d6e0fe345d0f7e4b2d1cb003f2",
    "id": null,
    "metadata": {},
    "name": "organizationUser_updateCustomerDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationUser_updateCustomerDetailsMutation(\n  $input: UpdateCustomerDetailsInput!\n) {\n  updateCustomerDetails(input: $input) {\n    customer {\n      id\n      timezone\n      designation\n      title\n      name\n      givenName\n      middleName\n      familyName\n      phoneNumber\n      personalInformationVisibility {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ad3a45e98a468f89ccbf6fedac0e7aac";

export default node;
