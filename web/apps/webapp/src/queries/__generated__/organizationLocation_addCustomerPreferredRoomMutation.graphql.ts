/**
 * @generated SignedSource<<632b2dab8985626b24d28d7985980921>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerPreferredRoomInput = {
  clientMutationId?: string | null | undefined;
  roomId: string;
};
export type organizationLocation_addCustomerPreferredRoomMutation$variables = {
  input: AddCustomerPreferredRoomInput;
};
export type organizationLocation_addCustomerPreferredRoomMutation$data = {
  readonly addCustomerPreferredRoom: {
    readonly customer: {
      readonly id: string;
      readonly preferredRooms: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type organizationLocation_addCustomerPreferredRoomMutation = {
  response: organizationLocation_addCustomerPreferredRoomMutation$data;
  variables: organizationLocation_addCustomerPreferredRoomMutation$variables;
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
    "name": "addCustomerPreferredRoom",
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
    "name": "organizationLocation_addCustomerPreferredRoomMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_addCustomerPreferredRoomMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "d355c391a4b9d8187ba25d0e1a748423",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_addCustomerPreferredRoomMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_addCustomerPreferredRoomMutation(\n  $input: AddCustomerPreferredRoomInput!\n) {\n  addCustomerPreferredRoom(input: $input) {\n    customer {\n      id\n      preferredRooms {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f1d5a3e92ce664e4d81aca245ef87e07";

export default node;
