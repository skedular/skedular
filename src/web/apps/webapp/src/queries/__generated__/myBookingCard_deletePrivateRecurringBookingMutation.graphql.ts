/**
 * @generated SignedSource<<f3ab200f9ad5331047caa2e395d38c86>>
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
export type myBookingCard_deletePrivateRecurringBookingMutation$variables = {
  input: DeletePrivateRecurringBookingInput;
};
export type myBookingCard_deletePrivateRecurringBookingMutation$data = {
  readonly deletePrivateRecurringBooking: {
    readonly recurringBooking: {
      readonly id: string;
    } | null | undefined;
  };
};
export type myBookingCard_deletePrivateRecurringBookingMutation = {
  response: myBookingCard_deletePrivateRecurringBookingMutation$data;
  variables: myBookingCard_deletePrivateRecurringBookingMutation$variables;
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
    "name": "myBookingCard_deletePrivateRecurringBookingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "myBookingCard_deletePrivateRecurringBookingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "69b2eb146a27096705e186b6ff248307",
    "id": null,
    "metadata": {},
    "name": "myBookingCard_deletePrivateRecurringBookingMutation",
    "operationKind": "mutation",
    "text": "mutation myBookingCard_deletePrivateRecurringBookingMutation(\n  $input: DeletePrivateRecurringBookingInput!\n) {\n  deletePrivateRecurringBooking(input: $input) {\n    recurringBooking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5062697d85735db0f205d84632f78e2b";

export default node;
