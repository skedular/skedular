/**
 * @generated SignedSource<<267603fcda5147c0916cd0d895e10007>>
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
export type locationPeopleBookings_deleteBookingMutation$variables = {
  input: DeleteBookingInput;
};
export type locationPeopleBookings_deleteBookingMutation$data = {
  readonly deleteBooking: {
    readonly booking: {
      readonly id: string;
    };
  } | null | undefined;
};
export type locationPeopleBookings_deleteBookingMutation = {
  response: locationPeopleBookings_deleteBookingMutation$data;
  variables: locationPeopleBookings_deleteBookingMutation$variables;
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
    "name": "locationPeopleBookings_deleteBookingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationPeopleBookings_deleteBookingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "c8badb91f768bcbe927856477eaaaf19",
    "id": null,
    "metadata": {},
    "name": "locationPeopleBookings_deleteBookingMutation",
    "operationKind": "mutation",
    "text": "mutation locationPeopleBookings_deleteBookingMutation(\n  $input: DeleteBookingInput!\n) {\n  deleteBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7976a2de91c1d117850de64c99c161f4";

export default node;
