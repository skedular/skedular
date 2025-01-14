/**
 * @generated SignedSource<<781e53eaa062a79a1d08dbe6e95ec390>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerDefaultOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  organizationTagId: string;
};
export type customTagCard_addCustomerDefaultOrganizationTagMutation$variables = {
  input: AddCustomerDefaultOrganizationTagInput;
};
export type customTagCard_addCustomerDefaultOrganizationTagMutation$data = {
  readonly addCustomerDefaultOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredCustomTags: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type customTagCard_addCustomerDefaultOrganizationTagMutation = {
  response: customTagCard_addCustomerDefaultOrganizationTagMutation$data;
  variables: customTagCard_addCustomerDefaultOrganizationTagMutation$variables;
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
    "name": "addCustomerDefaultOrganizationTag",
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
            "concreteType": "CustomerOrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredCustomTags",
            "plural": true,
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
    "name": "customTagCard_addCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "customTagCard_addCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "efb3b87ec2c17254e6d13881e82120c5",
    "id": null,
    "metadata": {},
    "name": "customTagCard_addCustomerDefaultOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation customTagCard_addCustomerDefaultOrganizationTagMutation(\n  $input: AddCustomerDefaultOrganizationTagInput!\n) {\n  addCustomerDefaultOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredCustomTags {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4e5d90af66d3bbd11bed24ebeaed3106";

export default node;
