/**
 * @generated SignedSource<<e2971881882ed52cbe28b4b713e9d94d>>
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
export type organizationPeopleBookingsMatrix_setCustomerDefaultOrganizationMutation$variables = {
  input: SetCustomerDefaultOrganizationInput;
};
export type organizationPeopleBookingsMatrix_setCustomerDefaultOrganizationMutation$data = {
  readonly setCustomerDefaultOrganization: {
    readonly customer: {
      readonly defaultOrganization: {
        readonly uniqueId: string;
      } | null | undefined;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationPeopleBookingsMatrix_setCustomerDefaultOrganizationMutation = {
  response: organizationPeopleBookingsMatrix_setCustomerDefaultOrganizationMutation$data;
  variables: organizationPeopleBookingsMatrix_setCustomerDefaultOrganizationMutation$variables;
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
    "name": "organizationPeopleBookingsMatrix_setCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationPeopleBookingsMatrix_setCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "0f5af1cc112b05f0c1ac1aefd5f3d354",
    "id": null,
    "metadata": {},
    "name": "organizationPeopleBookingsMatrix_setCustomerDefaultOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationPeopleBookingsMatrix_setCustomerDefaultOrganizationMutation(\n  $input: SetCustomerDefaultOrganizationInput!\n) {\n  setCustomerDefaultOrganization(input: $input) {\n    customer {\n      id\n      defaultOrganization {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "71f3e0b2b1fc7c41926d59c65537eec6";

export default node;
