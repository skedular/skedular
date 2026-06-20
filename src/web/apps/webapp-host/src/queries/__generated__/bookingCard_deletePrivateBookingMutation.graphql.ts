/**
 * @generated SignedSource<<1ba03ae4e6e449d5c8c5a9fc992ab8a2>>
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
export type bookingCard_deletePrivateBookingMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeletePrivateBookingInput;
};
export type bookingCard_deletePrivateBookingMutation$data = {
  readonly deletePrivateBooking: {
    readonly booking: {
      readonly id: string;
    } | null | undefined;
  };
};
export type bookingCard_deletePrivateBookingMutation = {
  response: bookingCard_deletePrivateBookingMutation$data;
  variables: bookingCard_deletePrivateBookingMutation$variables;
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
    "name": "bookingCard_deletePrivateBookingMutation",
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
    "name": "bookingCard_deletePrivateBookingMutation",
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
    "cacheID": "dca4a27b852466d3cdd8303792ca2d5e",
    "id": null,
    "metadata": {},
    "name": "bookingCard_deletePrivateBookingMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_deletePrivateBookingMutation(\n  $input: DeletePrivateBookingInput!\n) {\n  deletePrivateBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ebb9215533a40aca044cb1aea0175821";

export default node;
