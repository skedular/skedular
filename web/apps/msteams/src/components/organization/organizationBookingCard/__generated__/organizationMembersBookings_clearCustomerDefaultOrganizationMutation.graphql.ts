/**
 * @generated SignedSource<<0a6d1012a9d054e80c0ae7f0960bb3d8>>
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
export type organizationMembersBookings_clearCustomerDefaultOrganizationMutation$variables = {
  input: ClearCustomerDefaultOrganizationInput;
};
export type organizationMembersBookings_clearCustomerDefaultOrganizationMutation$data = {
  readonly clearCustomerDefaultOrganization: {
    readonly customer: {
      readonly defaultOrganization: {
        readonly uniqueId: string;
      } | null | undefined;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationMembersBookings_clearCustomerDefaultOrganizationMutation = {
  response: organizationMembersBookings_clearCustomerDefaultOrganizationMutation$data;
  variables: organizationMembersBookings_clearCustomerDefaultOrganizationMutation$variables;
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
    "name": "organizationMembersBookings_clearCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMembersBookings_clearCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "175f15178715d8ab90eb076a33fe5244",
    "id": null,
    "metadata": {},
    "name": "organizationMembersBookings_clearCustomerDefaultOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMembersBookings_clearCustomerDefaultOrganizationMutation(\n  $input: ClearCustomerDefaultOrganizationInput!\n) {\n  clearCustomerDefaultOrganization(input: $input) {\n    customer {\n      id\n      defaultOrganization {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "62358caa3b66d4e1bdb657203f12f00a";

export default node;
