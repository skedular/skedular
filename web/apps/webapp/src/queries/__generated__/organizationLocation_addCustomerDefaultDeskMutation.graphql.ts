/**
 * @generated SignedSource<<9c3b4e9d141d9707a4cfbfdd008d25d1>>
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
export type organizationLocation_addCustomerDefaultDeskMutation$variables = {
  input: AddCustomerDefaultDeskInput;
};
export type organizationLocation_addCustomerDefaultDeskMutation$data = {
  readonly addCustomerDefaultDesk: {
    readonly customer: {
      readonly id: string;
      readonly preferredDesks: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type organizationLocation_addCustomerDefaultDeskMutation = {
  response: organizationLocation_addCustomerDefaultDeskMutation$data;
  variables: organizationLocation_addCustomerDefaultDeskMutation$variables;
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
    "name": "organizationLocation_addCustomerDefaultDeskMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_addCustomerDefaultDeskMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "2e7fb886f485089275e3c0a8b15ac0f9",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_addCustomerDefaultDeskMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_addCustomerDefaultDeskMutation(\n  $input: AddCustomerDefaultDeskInput!\n) {\n  addCustomerDefaultDesk(input: $input) {\n    customer {\n      id\n      preferredDesks {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "485ac0799865f249aba29e9227cb64dc";

export default node;
