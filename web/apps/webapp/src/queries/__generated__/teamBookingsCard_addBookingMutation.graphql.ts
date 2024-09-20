/**
 * @generated SignedSource<<64450d1d4e3dc8fb11bc5dd5ee8dfa37>>
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
export type teamBookingsCard_addBookingMutation$variables = {
  input: AddBookingInput;
};
export type teamBookingsCard_addBookingMutation$data = {
  readonly addBooking: {
    readonly booking: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamBookingsCard_addBookingMutation = {
  response: teamBookingsCard_addBookingMutation$data;
  variables: teamBookingsCard_addBookingMutation$variables;
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
    "name": "teamBookingsCard_addBookingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamBookingsCard_addBookingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "7887d5d7bc6dd80d1fd7ca73631715c5",
    "id": null,
    "metadata": {},
    "name": "teamBookingsCard_addBookingMutation",
    "operationKind": "mutation",
    "text": "mutation teamBookingsCard_addBookingMutation(\n  $input: AddBookingInput!\n) {\n  addBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "647bc51d8fb7ba2f9f3085a936d619b2";

export default node;
