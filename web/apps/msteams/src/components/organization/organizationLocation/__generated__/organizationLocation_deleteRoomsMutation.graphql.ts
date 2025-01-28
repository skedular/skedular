/**
 * @generated SignedSource<<daa0238540a3df2b1267d31b1ecd3d91>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteRoomsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationLocation_deleteRoomsMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteRoomsInput;
};
export type organizationLocation_deleteRoomsMutation$data = {
  readonly deleteRooms: {
    readonly rooms: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationLocation_deleteRoomsMutation = {
  response: organizationLocation_deleteRoomsMutation$data;
  variables: organizationLocation_deleteRoomsMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocation_deleteRoomsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "RoomsPayload",
        "kind": "LinkedField",
        "name": "deleteRooms",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "RoomDetails",
            "kind": "LinkedField",
            "name": "rooms",
            "plural": true,
            "selections": [
              (v2/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_deleteRoomsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "RoomsPayload",
        "kind": "LinkedField",
        "name": "deleteRooms",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "RoomDetails",
            "kind": "LinkedField",
            "name": "rooms",
            "plural": true,
            "selections": [
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "1909427fa73852a33c9f394426d8f8ce",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_deleteRoomsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_deleteRoomsMutation(\n  $input: DeleteRoomsInput!\n) {\n  deleteRooms(input: $input) {\n    rooms {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "17480e27aca7231e3ceade8d67ad84d1";

export default node;
