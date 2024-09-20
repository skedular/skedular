/**
 * @generated SignedSource<<d82f0f8ea50f4d2b2d45112ff0909762>>
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
export type teamBookingsCard_deleteBookingMutation$variables = {
  input: DeleteBookingInput;
};
export type teamBookingsCard_deleteBookingMutation$data = {
  readonly deleteBooking: {
    readonly booking: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamBookingsCard_deleteBookingMutation = {
  response: teamBookingsCard_deleteBookingMutation$data;
  variables: teamBookingsCard_deleteBookingMutation$variables;
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
    "name": "teamBookingsCard_deleteBookingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamBookingsCard_deleteBookingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "4103547b989404df8d0bb0cea7054395",
    "id": null,
    "metadata": {},
    "name": "teamBookingsCard_deleteBookingMutation",
    "operationKind": "mutation",
    "text": "mutation teamBookingsCard_deleteBookingMutation(\n  $input: DeleteBookingInput!\n) {\n  deleteBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8da5613ef311346d3d8f88224ac94c71";

export default node;
