/**
 * @generated SignedSource<<35baab7ca6566a3cfab68db4b4a27d8c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateRoomInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  customTagIds: ReadonlyArray<string>;
  deactivated: boolean;
  id: string;
  name: string;
  requireBookingApproval: boolean;
  zoneIds: ReadonlyArray<string>;
};
export type editRoom_updateRoomMutation$variables = {
  input: UpdateRoomInput;
};
export type editRoom_updateRoomMutation$data = {
  readonly updateRoom: {
    readonly room: {
      readonly color: string | null | undefined;
      readonly customTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly deactivated: boolean;
      readonly id: string;
      readonly name: string;
      readonly requireBookingApproval: boolean;
      readonly zones: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type editRoom_updateRoomMutation$rawResponse = {
  readonly updateRoom: {
    readonly room: {
      readonly color: string | null | undefined;
      readonly customTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly deactivated: boolean;
      readonly id: string;
      readonly name: string;
      readonly requireBookingApproval: boolean;
      readonly zones: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type editRoom_updateRoomMutation = {
  rawResponse: editRoom_updateRoomMutation$rawResponse;
  response: editRoom_updateRoomMutation$data;
  variables: editRoom_updateRoomMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v1/*: any*/),
  (v2/*: any*/)
],
v4 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "RoomPayload",
    "kind": "LinkedField",
    "name": "updateRoom",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "RoomDetails",
        "kind": "LinkedField",
        "name": "room",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "deactivated",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "requireBookingApproval",
            "storageKey": null
          },
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "Organization_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "customTags",
            "plural": true,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Organization_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "zones",
            "plural": true,
            "selections": (v3/*: any*/),
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
    "name": "editRoom_updateRoomMutation",
    "selections": (v4/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editRoom_updateRoomMutation",
    "selections": (v4/*: any*/)
  },
  "params": {
    "cacheID": "e5a2cf64fb6ff785e9591b7d1b1d8a64",
    "id": null,
    "metadata": {},
    "name": "editRoom_updateRoomMutation",
    "operationKind": "mutation",
    "text": "mutation editRoom_updateRoomMutation(\n  $input: UpdateRoomInput!\n) {\n  updateRoom(input: $input) {\n    room {\n      id\n      name\n      deactivated\n      requireBookingApproval\n      color\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6298e900988e4e672929961d232be99c";

export default node;
