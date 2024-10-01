/**
 * @generated SignedSource<<284ea5b9987db16b973a82dc39e2f770>>
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
export type organizationPeopleBookings_deleteBookingMutation$variables = {
  input: DeleteBookingInput;
};
export type organizationPeopleBookings_deleteBookingMutation$data = {
  readonly deleteBooking: {
    readonly booking: {
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationPeopleBookings_deleteBookingMutation = {
  response: organizationPeopleBookings_deleteBookingMutation$data;
  variables: organizationPeopleBookings_deleteBookingMutation$variables;
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
    "name": "organizationPeopleBookings_deleteBookingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationPeopleBookings_deleteBookingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "dc0a591e7dc0b2fc176152ace2c9919c",
    "id": null,
    "metadata": {},
    "name": "organizationPeopleBookings_deleteBookingMutation",
    "operationKind": "mutation",
    "text": "mutation organizationPeopleBookings_deleteBookingMutation(\n  $input: DeleteBookingInput!\n) {\n  deleteBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "10074043915bdd22c5ebbe706214de96";

export default node;
