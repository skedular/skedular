/**
 * @generated SignedSource<<0aef235a003a513540b96f85cd4e5ccb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddBookingInput = {
  clientMutationId?: string | null | undefined;
  customerId: string;
  deskIds: ReadonlyArray<string>;
  from: any;
  id?: string | null | undefined;
  locationId?: string | null | undefined;
  notes?: string | null | undefined;
  organizationId?: string | null | undefined;
  teamId?: string | null | undefined;
  to: any;
};
export type teamPeopleBookingsMatrix_addBookingMutation$variables = {
  input: AddBookingInput;
};
export type teamPeopleBookingsMatrix_addBookingMutation$data = {
  readonly addBooking: {
    readonly booking: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamPeopleBookingsMatrix_addBookingMutation = {
  response: teamPeopleBookingsMatrix_addBookingMutation$data;
  variables: teamPeopleBookingsMatrix_addBookingMutation$variables;
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
    "name": "addBooking",
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
    "name": "teamPeopleBookingsMatrix_addBookingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamPeopleBookingsMatrix_addBookingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "41b82865265d4055229cfa11ec45f190",
    "id": null,
    "metadata": {},
    "name": "teamPeopleBookingsMatrix_addBookingMutation",
    "operationKind": "mutation",
    "text": "mutation teamPeopleBookingsMatrix_addBookingMutation(\n  $input: AddBookingInput!\n) {\n  addBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "be4dc5fd7f54df21e8e1ebb183e65c11";

export default node;
