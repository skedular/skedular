/**
 * @generated SignedSource<<74ca7392eb21efd25edd724920c0a83a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteBookingInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type bookingsWeekGrid_deleteBookingMutation$variables = {
  input: DeleteBookingInput;
};
export type bookingsWeekGrid_deleteBookingMutation$data = {
  readonly deleteBooking: {
    readonly booking: {
      readonly id: string;
    };
  } | null | undefined;
};
export type bookingsWeekGrid_deleteBookingMutation = {
  response: bookingsWeekGrid_deleteBookingMutation$data;
  variables: bookingsWeekGrid_deleteBookingMutation$variables;
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
    "concreteType": "BookingPayload",
    "kind": "LinkedField",
    "name": "deleteBooking",
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
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
    "name": "bookingsWeekGrid_deleteBookingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookingsWeekGrid_deleteBookingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "5f810639760432b143d2422061eeeab9",
    "id": null,
    "metadata": {},
    "name": "bookingsWeekGrid_deleteBookingMutation",
    "operationKind": "mutation",
    "text": "mutation bookingsWeekGrid_deleteBookingMutation(\n  $input: DeleteBookingInput!\n) {\n  deleteBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f3946fbc243de2027eaabb587e4c4657";

export default node;
