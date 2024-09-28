/**
 * @generated SignedSource<<ded887f416ba4ea21e6c4d2d9905d256>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ClearCustomerDefaultOrganizationInput = {
  clientMutationId?: string | null | undefined;
};
export type organizationPeopleBookingsMatrix_clearCustomerDefaultOrganizationMutation$variables = {
  input: ClearCustomerDefaultOrganizationInput;
};
export type organizationPeopleBookingsMatrix_clearCustomerDefaultOrganizationMutation$data = {
  readonly clearCustomerDefaultOrganization: {
    readonly customer: {
      readonly defaultOrganization: {
        readonly uniqueId: string;
      } | null | undefined;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationPeopleBookingsMatrix_clearCustomerDefaultOrganizationMutation = {
  response: organizationPeopleBookingsMatrix_clearCustomerDefaultOrganizationMutation$data;
  variables: organizationPeopleBookingsMatrix_clearCustomerDefaultOrganizationMutation$variables;
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
    "name": "clearCustomerDefaultOrganization",
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
            "concreteType": "CustomerOrganizationDetails",
            "kind": "LinkedField",
            "name": "defaultOrganization",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
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
    "name": "organizationPeopleBookingsMatrix_clearCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationPeopleBookingsMatrix_clearCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "c0bab894c69dcc1bf9fac021ab688ffe",
    "id": null,
    "metadata": {},
    "name": "organizationPeopleBookingsMatrix_clearCustomerDefaultOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationPeopleBookingsMatrix_clearCustomerDefaultOrganizationMutation(\n  $input: ClearCustomerDefaultOrganizationInput!\n) {\n  clearCustomerDefaultOrganization(input: $input) {\n    customer {\n      id\n      defaultOrganization {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3d85c39da5081bac1950e8e60ba4fe6a";

export default node;
