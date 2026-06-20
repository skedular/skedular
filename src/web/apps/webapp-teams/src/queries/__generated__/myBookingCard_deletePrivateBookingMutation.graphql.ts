/**
 * @generated SignedSource<<5e7ff87e63aeb141b95e817f1bf2ba6e>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeletePrivateBookingInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type myBookingCard_deletePrivateBookingMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeletePrivateBookingInput;
};
export type myBookingCard_deletePrivateBookingMutation$data = {
  readonly deletePrivateBooking: {
    readonly booking: {
      readonly id: string;
    } | null | undefined;
  };
};
export type myBookingCard_deletePrivateBookingMutation = {
  response: myBookingCard_deletePrivateBookingMutation$data;
  variables: myBookingCard_deletePrivateBookingMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "myBookingCard_deletePrivateBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "deletePrivateBooking",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDetails",
            "kind": "LinkedField",
            "name": "booking",
            "plural": false,
            "selections": [
              (v2/*:: as any*/)
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "myBookingCard_deletePrivateBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "deletePrivateBooking",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDetails",
            "kind": "LinkedField",
            "name": "booking",
            "plural": false,
            "selections": [
              (v2/*:: as any*/),
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
    "cacheID": "c2c24ccc003ff9b1db7170c5959af74c",
    "id": null,
    "metadata": {},
    "name": "myBookingCard_deletePrivateBookingMutation",
    "operationKind": "mutation",
    "text": "mutation myBookingCard_deletePrivateBookingMutation(\n  $input: DeletePrivateBookingInput!\n) {\n  deletePrivateBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "34edf9ae719e20e8f6c23dd8362d1a26";

export default node;
