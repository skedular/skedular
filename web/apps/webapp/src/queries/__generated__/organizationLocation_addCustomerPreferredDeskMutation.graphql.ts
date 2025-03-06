/**
 * @generated SignedSource<<41bfe249beacaca1f8c0b6c191f24552>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerPreferredDeskInput = {
  clientMutationId?: string | null | undefined;
  deskId: string;
};
export type organizationLocation_addCustomerPreferredDeskMutation$variables = {
  input: AddCustomerPreferredDeskInput;
};
export type organizationLocation_addCustomerPreferredDeskMutation$data = {
  readonly addCustomerPreferredDesk: {
    readonly customer: {
      readonly id: string;
      readonly preferredDesks: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type organizationLocation_addCustomerPreferredDeskMutation = {
  response: organizationLocation_addCustomerPreferredDeskMutation$data;
  variables: organizationLocation_addCustomerPreferredDeskMutation$variables;
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
    "name": "addCustomerPreferredDesk",
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
    "name": "organizationLocation_addCustomerPreferredDeskMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_addCustomerPreferredDeskMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "86f494531bcd2c5c41973809127f8fc6",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_addCustomerPreferredDeskMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_addCustomerPreferredDeskMutation(\n  $input: AddCustomerPreferredDeskInput!\n) {\n  addCustomerPreferredDesk(input: $input) {\n    customer {\n      id\n      preferredDesks {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b1ac29183fee1e9ede1a1f7db03e2efc";

export default node;
