/**
 * @generated SignedSource<<b474bd3eb35836fd89b79e5237d4d555>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPreferredRoomInput = {
  clientMutationId?: string | null | undefined;
  roomId: string;
};
export type organizationLocation_removeCustomerPreferredRoomMutation$variables = {
  input: RemoveCustomerPreferredRoomInput;
};
export type organizationLocation_removeCustomerPreferredRoomMutation$data = {
  readonly removeCustomerPreferredRoom: {
    readonly customer: {
      readonly id: string;
      readonly preferredRooms: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type organizationLocation_removeCustomerPreferredRoomMutation = {
  response: organizationLocation_removeCustomerPreferredRoomMutation$data;
  variables: organizationLocation_removeCustomerPreferredRoomMutation$variables;
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
    "name": "removeCustomerPreferredRoom",
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
    "name": "organizationLocation_removeCustomerPreferredRoomMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_removeCustomerPreferredRoomMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "7c3bf669a2245a161e5cbc023dce8062",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_removeCustomerPreferredRoomMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_removeCustomerPreferredRoomMutation(\n  $input: RemoveCustomerPreferredRoomInput!\n) {\n  removeCustomerPreferredRoom(input: $input) {\n    customer {\n      id\n      preferredRooms {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f11014a626d47f8c886cf000b77f3e48";

export default node;
