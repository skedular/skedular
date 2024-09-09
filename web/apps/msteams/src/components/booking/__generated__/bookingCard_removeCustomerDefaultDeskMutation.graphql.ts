/**
 * @generated SignedSource<<78afda7a786865efa6b584951d577dee>>
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
export type bookingCard_removeCustomerDefaultDeskMutation$variables = {
  input: RemoveCustomerDefaultDeskInput;
};
export type bookingCard_removeCustomerDefaultDeskMutation$data = {
  readonly removeCustomerDefaultDesk: {
    readonly customer: {
      readonly id: string;
      readonly preferredDesks: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type bookingCard_removeCustomerDefaultDeskMutation = {
  response: bookingCard_removeCustomerDefaultDeskMutation$data;
  variables: bookingCard_removeCustomerDefaultDeskMutation$variables;
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
    "name": "bookingCard_removeCustomerDefaultDeskMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookingCard_removeCustomerDefaultDeskMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "c4d150beb41963b885e74b79923b9880",
    "id": null,
    "metadata": {},
    "name": "bookingCard_removeCustomerDefaultDeskMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_removeCustomerDefaultDeskMutation(\n  $input: RemoveCustomerDefaultDeskInput!\n) {\n  removeCustomerDefaultDesk(input: $input) {\n    customer {\n      id\n      preferredDesks {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e25714b95c6a18c887f99026d3eb9a7e";

export default node;
