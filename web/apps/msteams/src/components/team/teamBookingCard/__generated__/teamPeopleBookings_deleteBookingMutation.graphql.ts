/**
 * @generated SignedSource<<90dbc0409b7d249a96bdca6e72020419>>
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
export type teamPeopleBookings_deleteBookingMutation$variables = {
  input: DeleteBookingInput;
};
export type teamPeopleBookings_deleteBookingMutation$data = {
  readonly deleteBooking: {
    readonly booking: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamPeopleBookings_deleteBookingMutation = {
  response: teamPeopleBookings_deleteBookingMutation$data;
  variables: teamPeopleBookings_deleteBookingMutation$variables;
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
    "name": "teamPeopleBookings_deleteBookingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamPeopleBookings_deleteBookingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "d77b63e064c19c9042e265ae107e6616",
    "id": null,
    "metadata": {},
    "name": "teamPeopleBookings_deleteBookingMutation",
    "operationKind": "mutation",
    "text": "mutation teamPeopleBookings_deleteBookingMutation(\n  $input: DeleteBookingInput!\n) {\n  deleteBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7e022a9b46c4ee8421e52ca02df2bab2";

export default node;
