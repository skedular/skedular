/**
 * @generated SignedSource<<187d5e28fd4f497da8b48e7d446135e7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerDefaultRoomInput = {
  clientMutationId?: string | null | undefined;
  roomId: string;
};
export type organizationLocation_addCustomerDefaultRoomMutation$variables = {
  input: AddCustomerDefaultRoomInput;
};
export type organizationLocation_addCustomerDefaultRoomMutation$data = {
  readonly addCustomerDefaultRoom: {
    readonly customer: {
      readonly id: string;
      readonly preferredRooms: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type organizationLocation_addCustomerDefaultRoomMutation = {
  response: organizationLocation_addCustomerDefaultRoomMutation$data;
  variables: organizationLocation_addCustomerDefaultRoomMutation$variables;
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
    "name": "addCustomerDefaultRoom",
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
    "name": "organizationLocation_addCustomerDefaultRoomMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_addCustomerDefaultRoomMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "9232c6e2a01bbc979e7d99d0c217269d",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_addCustomerDefaultRoomMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_addCustomerDefaultRoomMutation(\n  $input: AddCustomerDefaultRoomInput!\n) {\n  addCustomerDefaultRoom(input: $input) {\n    customer {\n      id\n      preferredRooms {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7e3d3822ebe086a1f8377b619d1d9b48";

export default node;
