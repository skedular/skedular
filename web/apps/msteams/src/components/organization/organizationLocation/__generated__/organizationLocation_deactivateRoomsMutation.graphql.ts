/**
 * @generated SignedSource<<5c21a60827f3e463448cd51ec96f249e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeactivateRoomsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationLocation_deactivateRoomsMutation$variables = {
  input: DeactivateRoomsInput;
};
export type organizationLocation_deactivateRoomsMutation$data = {
  readonly deactivateRooms: {
    readonly rooms: ReadonlyArray<{
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
    }>;
  } | null | undefined;
};
export type organizationLocation_deactivateRoomsMutation = {
  response: organizationLocation_deactivateRoomsMutation$data;
  variables: organizationLocation_deactivateRoomsMutation$variables;
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
    "concreteType": "RoomsPayload",
    "kind": "LinkedField",
    "name": "deactivateRooms",
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
    "name": "organizationLocation_deactivateRoomsMutation",
    "selections": (v4/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_deactivateRoomsMutation",
    "selections": (v4/*: any*/)
  },
  "params": {
    "cacheID": "5e7babf09e17af26590d71f97612a0d8",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_deactivateRoomsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_deactivateRoomsMutation(\n  $input: DeactivateRoomsInput!\n) {\n  deactivateRooms(input: $input) {\n    rooms {\n      id\n      name\n      deactivated\n      requireBookingApproval\n      color\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8578a8ed25c9cd47a2b188ff9e1f123e";

export default node;
