/**
 * @generated SignedSource<<09a47d32e9221be22510dbdc096f1f84>>
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
export type deskTypeCard_addCustomerDefaultOrganizationTagMutation$variables = {
  input: AddCustomerDefaultOrganizationTagInput;
};
export type deskTypeCard_addCustomerDefaultOrganizationTagMutation$data = {
  readonly addCustomerDefaultOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredDeskTypes: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type deskTypeCard_addCustomerDefaultOrganizationTagMutation = {
  response: deskTypeCard_addCustomerDefaultOrganizationTagMutation$data;
  variables: deskTypeCard_addCustomerDefaultOrganizationTagMutation$variables;
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
            "name": "preferredDeskTypes",
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
    "name": "deskTypeCard_addCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "deskTypeCard_addCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "260ed76902243b8fcb9534b3f4f0f661",
    "id": null,
    "metadata": {},
    "name": "deskTypeCard_addCustomerDefaultOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation deskTypeCard_addCustomerDefaultOrganizationTagMutation(\n  $input: AddCustomerDefaultOrganizationTagInput!\n) {\n  addCustomerDefaultOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredDeskTypes {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f12eb918501e9955ddd04beea0a84491";

export default node;
