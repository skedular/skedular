/**
 * @generated SignedSource<<1b002090a3792477d328f3df0c33fba4>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type SetCustomerDefaultOrganizationInput = {
  clientMutationId?: string | null | undefined;
  organizationId: string;
};
export type organizationCard_setCustomerDefaultOrganizationMutation$variables = {
  input: SetCustomerDefaultOrganizationInput;
};
export type organizationCard_setCustomerDefaultOrganizationMutation$data = {
  readonly setCustomerDefaultOrganization: {
    readonly customer: {
      readonly defaultOrganization: {
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      } | null | undefined;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationCard_setCustomerDefaultOrganizationMutation = {
  response: organizationCard_setCustomerDefaultOrganizationMutation$data;
  variables: organizationCard_setCustomerDefaultOrganizationMutation$variables;
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
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "name",
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
    "name": "organizationCard_setCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationCard_setCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "a3433137c71dc242fc7500fd7449def3",
    "id": null,
    "metadata": {},
    "name": "organizationCard_setCustomerDefaultOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationCard_setCustomerDefaultOrganizationMutation(\n  $input: SetCustomerDefaultOrganizationInput!\n) {\n  setCustomerDefaultOrganization(input: $input) {\n    customer {\n      id\n      defaultOrganization {\n        uniqueId\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5f3c58b5ca7e31ffc127bc0587aff07d";

export default node;
