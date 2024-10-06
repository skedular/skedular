/**
 * @generated SignedSource<<84aa94616de853db9c8f98d6ef82022e>>
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
export type organizationPeopleBookings_setCustomerDefaultOrganizationMutation$variables = {
  input: SetCustomerDefaultOrganizationInput;
};
export type organizationPeopleBookings_setCustomerDefaultOrganizationMutation$data = {
  readonly setCustomerDefaultOrganization: {
    readonly customer: {
      readonly defaultOrganization: {
        readonly uniqueId: string;
      } | null | undefined;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationPeopleBookings_setCustomerDefaultOrganizationMutation = {
  response: organizationPeopleBookings_setCustomerDefaultOrganizationMutation$data;
  variables: organizationPeopleBookings_setCustomerDefaultOrganizationMutation$variables;
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
    "name": "organizationPeopleBookings_setCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationPeopleBookings_setCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "e92e23f8003bae459c5c8094da3e57c0",
    "id": null,
    "metadata": {},
    "name": "organizationPeopleBookings_setCustomerDefaultOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationPeopleBookings_setCustomerDefaultOrganizationMutation(\n  $input: SetCustomerDefaultOrganizationInput!\n) {\n  setCustomerDefaultOrganization(input: $input) {\n    customer {\n      id\n      defaultOrganization {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "05af62216a475319644d46f4cb3c76d3";

export default node;
