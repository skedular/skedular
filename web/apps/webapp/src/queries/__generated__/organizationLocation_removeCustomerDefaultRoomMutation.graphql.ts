/**
 * @generated SignedSource<<178d44940df5bb57fdf8f6c44d8466eb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerDefaultRoomInput = {
  clientMutationId?: string | null | undefined;
  roomId: string;
};
export type organizationLocation_removeCustomerDefaultRoomMutation$variables = {
  input: RemoveCustomerDefaultRoomInput;
};
export type organizationLocation_removeCustomerDefaultRoomMutation$data = {
  readonly removeCustomerDefaultRoom: {
    readonly customer: {
      readonly id: string;
      readonly preferredRooms: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type organizationLocation_removeCustomerDefaultRoomMutation = {
  response: organizationLocation_removeCustomerDefaultRoomMutation$data;
  variables: organizationLocation_removeCustomerDefaultRoomMutation$variables;
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
    "name": "removeCustomerDefaultRoom",
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
            "concreteType": "CustomerRoomDetails",
            "kind": "LinkedField",
            "name": "preferredRooms",
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
    "name": "organizationLocation_removeCustomerDefaultRoomMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_removeCustomerDefaultRoomMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "21088ddb82546bb83ad4779545570967",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_removeCustomerDefaultRoomMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_removeCustomerDefaultRoomMutation(\n  $input: RemoveCustomerDefaultRoomInput!\n) {\n  removeCustomerDefaultRoom(input: $input) {\n    customer {\n      id\n      preferredRooms {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "fe5f9e0a0cfb15be762a31ec7384a19d";

export default node;
