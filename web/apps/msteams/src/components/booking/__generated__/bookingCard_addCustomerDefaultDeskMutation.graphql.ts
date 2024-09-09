/**
 * @generated SignedSource<<3fc0fe648efb9cc3d2dc8d351d6c1d2b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerDefaultDeskInput = {
  clientMutationId?: string | null | undefined;
  deskId: string;
};
export type bookingCard_addCustomerDefaultDeskMutation$variables = {
  input: AddCustomerDefaultDeskInput;
};
export type bookingCard_addCustomerDefaultDeskMutation$data = {
  readonly addCustomerDefaultDesk: {
    readonly customer: {
      readonly id: string;
      readonly preferredDesks: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type bookingCard_addCustomerDefaultDeskMutation = {
  response: bookingCard_addCustomerDefaultDeskMutation$data;
  variables: bookingCard_addCustomerDefaultDeskMutation$variables;
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
    "name": "addCustomerDefaultDesk",
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
    "name": "bookingCard_addCustomerDefaultDeskMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookingCard_addCustomerDefaultDeskMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "3f380236244a240fe52b83c7079862ed",
    "id": null,
    "metadata": {},
    "name": "bookingCard_addCustomerDefaultDeskMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_addCustomerDefaultDeskMutation(\n  $input: AddCustomerDefaultDeskInput!\n) {\n  addCustomerDefaultDesk(input: $input) {\n    customer {\n      id\n      preferredDesks {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a8a9ca9074c61cbd90dd3fed08298e86";

export default node;
