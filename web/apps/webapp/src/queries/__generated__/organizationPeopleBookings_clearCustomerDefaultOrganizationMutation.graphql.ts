/**
 * @generated SignedSource<<99494173711916fa6bbb2027f28a028a>>
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
export type organizationPeopleBookings_clearCustomerDefaultOrganizationMutation$variables = {
  input: ClearCustomerDefaultOrganizationInput;
};
export type organizationPeopleBookings_clearCustomerDefaultOrganizationMutation$data = {
  readonly clearCustomerDefaultOrganization: {
    readonly customer: {
      readonly defaultOrganization: {
        readonly uniqueId: string;
      } | null | undefined;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationPeopleBookings_clearCustomerDefaultOrganizationMutation = {
  response: organizationPeopleBookings_clearCustomerDefaultOrganizationMutation$data;
  variables: organizationPeopleBookings_clearCustomerDefaultOrganizationMutation$variables;
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
    "name": "organizationPeopleBookings_clearCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationPeopleBookings_clearCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "e3e4e6eece4a745b4859bb789f47c54c",
    "id": null,
    "metadata": {},
    "name": "organizationPeopleBookings_clearCustomerDefaultOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationPeopleBookings_clearCustomerDefaultOrganizationMutation(\n  $input: ClearCustomerDefaultOrganizationInput!\n) {\n  clearCustomerDefaultOrganization(input: $input) {\n    customer {\n      id\n      defaultOrganization {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "83588d524fcb1aa6bcbf68a3d0bbcf9b";

export default node;
