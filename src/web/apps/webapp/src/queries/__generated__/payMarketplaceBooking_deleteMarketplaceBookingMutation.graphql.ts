/**
 * @generated SignedSource<<a259f8e7316dd16ca314ca2e0b81ba68>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CancellationErrorCode = "INSUFFICIENT_MANAGEMENT_PERMISSION" | "INVALID_TERMINAL_STATE" | "OVERRIDE_REASON_REQUIRED" | "POLICY_RESTRICTION" | "%future added value";
export type DeleteMarketplaceBookingInput = {
  cancellationOverrideReason?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  id: string;
};
export type payMarketplaceBooking_deleteMarketplaceBookingMutation$variables = {
  input: DeleteMarketplaceBookingInput;
};
export type payMarketplaceBooking_deleteMarketplaceBookingMutation$data = {
  readonly deleteMarketplaceBooking: {
    readonly booking: {
      readonly id: string;
    } | null | undefined;
    readonly cancellationError: {
      readonly code: CancellationErrorCode;
      readonly message: string;
    } | null | undefined;
  };
};
export type payMarketplaceBooking_deleteMarketplaceBookingMutation = {
  response: payMarketplaceBooking_deleteMarketplaceBookingMutation$data;
  variables: payMarketplaceBooking_deleteMarketplaceBookingMutation$variables;
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
    "name": "deleteMarketplaceBooking",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CancellationErrorDetails",
        "kind": "LinkedField",
        "name": "cancellationError",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "code",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "message",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "payMarketplaceBooking_deleteMarketplaceBookingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "payMarketplaceBooking_deleteMarketplaceBookingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "1286cdb96e44b1144a00277094dfed8b",
    "id": null,
    "metadata": {},
    "name": "payMarketplaceBooking_deleteMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation payMarketplaceBooking_deleteMarketplaceBookingMutation(\n  $input: DeleteMarketplaceBookingInput!\n) {\n  deleteMarketplaceBooking(input: $input) {\n    cancellationError {\n      code\n      message\n    }\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5d3bd1141da1d9ac9a25d17cd9770c6b";

export default node;
