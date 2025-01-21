/**
 * @generated SignedSource<<91095feffcf7c367a1bb2a930c12f4de>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerDefaultDeskInput = {
  clientMutationId?: string | null | undefined;
  deskId: string;
};
export type organizationLocation_removeCustomerDefaultDeskMutation$variables = {
  input: RemoveCustomerDefaultDeskInput;
};
export type organizationLocation_removeCustomerDefaultDeskMutation$data = {
  readonly removeCustomerDefaultDesk: {
    readonly customer: {
      readonly id: string;
      readonly preferredDesks: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type organizationLocation_removeCustomerDefaultDeskMutation = {
  response: organizationLocation_removeCustomerDefaultDeskMutation$data;
  variables: organizationLocation_removeCustomerDefaultDeskMutation$variables;
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
    "name": "removeCustomerDefaultDesk",
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
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
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
    "name": "organizationLocation_removeCustomerDefaultDeskMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_removeCustomerDefaultDeskMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "b596018f95ac003cd687dd2f68e2ad29",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_removeCustomerDefaultDeskMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_removeCustomerDefaultDeskMutation(\n  $input: RemoveCustomerDefaultDeskInput!\n) {\n  removeCustomerDefaultDesk(input: $input) {\n    customer {\n      id\n      preferredDesks {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b183c1877a694a231f1432e0a75a34ab";

export default node;
