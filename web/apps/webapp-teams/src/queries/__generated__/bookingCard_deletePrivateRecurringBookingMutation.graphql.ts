/**
 * @generated SignedSource<<68a01421712d577acb50217413a48aa1>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeletePrivateRecurringBookingInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type bookingCard_deletePrivateRecurringBookingMutation$variables = {
  input: DeletePrivateRecurringBookingInput;
};
export type bookingCard_deletePrivateRecurringBookingMutation$data = {
  readonly deletePrivateRecurringBooking: {
    readonly recurringBooking: {
      readonly id: string;
    };
  };
};
export type bookingCard_deletePrivateRecurringBookingMutation = {
  response: bookingCard_deletePrivateRecurringBookingMutation$data;
  variables: bookingCard_deletePrivateRecurringBookingMutation$variables;
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
    "concreteType": "RecurringBookingPayload",
    "kind": "LinkedField",
    "name": "deletePrivateRecurringBooking",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "RecurringBookingDetails",
        "kind": "LinkedField",
        "name": "recurringBooking",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "bookingCard_deletePrivateRecurringBookingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "bookingCard_deletePrivateRecurringBookingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "54a0718dedd089c6d00aea0a6948ad7c",
    "id": null,
    "metadata": {},
    "name": "bookingCard_deletePrivateRecurringBookingMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_deletePrivateRecurringBookingMutation(\n  $input: DeletePrivateRecurringBookingInput!\n) {\n  deletePrivateRecurringBooking(input: $input) {\n    recurringBooking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "eb2430c37497c960aaea861fa653fa95";

export default node;
