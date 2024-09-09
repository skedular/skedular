/**
 * @generated SignedSource<<6f54c8a7eaeca12d511f70aff6fdbec3>>
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
export type organizationCard_clearCustomerDefaultOrganizationMutation$variables = {
  input: ClearCustomerDefaultOrganizationInput;
};
export type organizationCard_clearCustomerDefaultOrganizationMutation$data = {
  readonly clearCustomerDefaultOrganization: {
    readonly customer: {
      readonly defaultOrganization: {
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      } | null | undefined;
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationCard_clearCustomerDefaultOrganizationMutation = {
  response: organizationCard_clearCustomerDefaultOrganizationMutation$data;
  variables: organizationCard_clearCustomerDefaultOrganizationMutation$variables;
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
    "name": "organizationCard_clearCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationCard_clearCustomerDefaultOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "684f778ffd15b5e8aa66a4e7fe7769ef",
    "id": null,
    "metadata": {},
    "name": "organizationCard_clearCustomerDefaultOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationCard_clearCustomerDefaultOrganizationMutation(\n  $input: ClearCustomerDefaultOrganizationInput!\n) {\n  clearCustomerDefaultOrganization(input: $input) {\n    customer {\n      id\n      defaultOrganization {\n        uniqueId\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f89ba4b9b08017781eef519b44cccc9d";

export default node;
