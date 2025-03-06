/**
 * @generated SignedSource<<83dab52361fab5d36e1e769cb3ab5402>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPreferredDeskInput = {
  clientMutationId?: string | null | undefined;
  deskId: string;
};
export type organizationLocation_removeCustomerPreferredDeskMutation$variables = {
  input: RemoveCustomerPreferredDeskInput;
};
export type organizationLocation_removeCustomerPreferredDeskMutation$data = {
  readonly removeCustomerPreferredDesk: {
    readonly customer: {
      readonly id: string;
      readonly preferredDesks: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type organizationLocation_removeCustomerPreferredDeskMutation = {
  response: organizationLocation_removeCustomerPreferredDeskMutation$data;
  variables: organizationLocation_removeCustomerPreferredDeskMutation$variables;
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
    "name": "removeCustomerPreferredDesk",
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
    "name": "organizationLocation_removeCustomerPreferredDeskMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_removeCustomerPreferredDeskMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "bdb6c4633d00ff18acb3f22a8cc0036a",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_removeCustomerPreferredDeskMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_removeCustomerPreferredDeskMutation(\n  $input: RemoveCustomerPreferredDeskInput!\n) {\n  removeCustomerPreferredDesk(input: $input) {\n    customer {\n      id\n      preferredDesks {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e3016ebcab396b524f20b538ae6c1721";

export default node;
