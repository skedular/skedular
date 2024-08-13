/**
 * @generated SignedSource<<158eb557cf23e339e37c121d4d716cb6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type AddCustomerDefaultDeskInput = {
  clientMutationId?: string | null | undefined;
  deskId: string;
};
export type deskCard_addCustomerDefaultDeskMutation$variables = {
  input: AddCustomerDefaultDeskInput;
};
export type deskCard_addCustomerDefaultDeskMutation$data = {
  readonly addCustomerDefaultDesk: {
    readonly customer: {
      readonly id: string;
      readonly preferredDesks: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type deskCard_addCustomerDefaultDeskMutation = {
  response: deskCard_addCustomerDefaultDeskMutation$data;
  variables: deskCard_addCustomerDefaultDeskMutation$variables;
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
    "name": "deskCard_addCustomerDefaultDeskMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "deskCard_addCustomerDefaultDeskMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "eea4770f9442ed92a52dfe44716d493b",
    "id": null,
    "metadata": {},
    "name": "deskCard_addCustomerDefaultDeskMutation",
    "operationKind": "mutation",
    "text": "mutation deskCard_addCustomerDefaultDeskMutation(\n  $input: AddCustomerDefaultDeskInput!\n) {\n  addCustomerDefaultDesk(input: $input) {\n    customer {\n      id\n      preferredDesks {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2bc23192384bf4b61d56b7e466da5dcd";

export default node;
