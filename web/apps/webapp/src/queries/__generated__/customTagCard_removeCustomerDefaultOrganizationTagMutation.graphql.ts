/**
 * @generated SignedSource<<46d69cef1e82daa7ab055a8e9fcf22ef>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerDefaultOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  organizationTagId: string;
};
export type customTagCard_removeCustomerDefaultOrganizationTagMutation$variables = {
  input: RemoveCustomerDefaultOrganizationTagInput;
};
export type customTagCard_removeCustomerDefaultOrganizationTagMutation$data = {
  readonly removeCustomerDefaultOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredCustomTags: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type customTagCard_removeCustomerDefaultOrganizationTagMutation = {
  response: customTagCard_removeCustomerDefaultOrganizationTagMutation$data;
  variables: customTagCard_removeCustomerDefaultOrganizationTagMutation$variables;
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
    "name": "removeCustomerDefaultOrganizationTag",
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
    "name": "customTagCard_removeCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "customTagCard_removeCustomerDefaultOrganizationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "e3cfabc55679f0884f6297110f084e4a",
    "id": null,
    "metadata": {},
    "name": "customTagCard_removeCustomerDefaultOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation customTagCard_removeCustomerDefaultOrganizationTagMutation(\n  $input: RemoveCustomerDefaultOrganizationTagInput!\n) {\n  removeCustomerDefaultOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredCustomTags {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "565ab9c21157ebfd7ba6256dfe1b31a0";

export default node;
