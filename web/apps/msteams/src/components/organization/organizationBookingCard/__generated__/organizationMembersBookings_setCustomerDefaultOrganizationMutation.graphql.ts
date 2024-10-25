/**
 * @generated SignedSource<<94557a140e2c0178c45e4f45d9517d0c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type SetCustomerDefaultOrganizationInput = {
  clientMutationId?: string | null | undefined;
  organizationId: string;
};
export type organizationMembersBookings_setCustomerDefaultOrganizationMutation$variables = {
  input: SetCustomerDefaultOrganizationInput;
};
export type organizationMembersBookings_setCustomerDefaultOrganizationMutation$data = {
  readonly setCustomerDefaultOrganization: {
    readonly customer: {
      readonly defaultOrganization: {
        readonly uniqueId: string;
      } | null | undefined;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationMembersBookings_setCustomerDefaultOrganizationMutation = {
  response: organizationMembersBookings_setCustomerDefaultOrganizationMutation$data;
  variables: organizationMembersBookings_setCustomerDefaultOrganizationMutation$variables;
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
    "name": "setCustomerDefaultOrganization",
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
    "name": "organizationMembersBookings_setCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMembersBookings_setCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "665f350f7e7f629001dc4fa6f84f977d",
    "id": null,
    "metadata": {},
    "name": "organizationMembersBookings_setCustomerDefaultOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMembersBookings_setCustomerDefaultOrganizationMutation(\n  $input: SetCustomerDefaultOrganizationInput!\n) {\n  setCustomerDefaultOrganization(input: $input) {\n    customer {\n      id\n      defaultOrganization {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "36d183209181133579b4294a1d420d36";

export default node;
