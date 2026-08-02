/**
 * @generated SignedSource<<734e950a698191d1b2882ba257af9b5b>>
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
export type bookingCard_deleteMarketplaceBookingMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteMarketplaceBookingInput;
};
export type bookingCard_deleteMarketplaceBookingMutation$data = {
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
export type bookingCard_deleteMarketplaceBookingMutation = {
  response: bookingCard_deleteMarketplaceBookingMutation$data;
  variables: bookingCard_deleteMarketplaceBookingMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
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
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "bookingCard_deleteMarketplaceBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "deleteMarketplaceBooking",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDetails",
            "kind": "LinkedField",
            "name": "booking",
            "plural": false,
            "selections": [
              (v3/*:: as any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "bookingCard_deleteMarketplaceBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "deleteMarketplaceBooking",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDetails",
            "kind": "LinkedField",
            "name": "booking",
            "plural": false,
            "selections": [
              (v3/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "e471d5b9690c6f813931fa2195322b9c",
    "id": null,
    "metadata": {},
    "name": "bookingCard_deleteMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_deleteMarketplaceBookingMutation(\n  $input: DeleteMarketplaceBookingInput!\n) {\n  deleteMarketplaceBooking(input: $input) {\n    cancellationError {\n      code\n      message\n    }\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "29b301011ce3fea33f1b331814349ca5";

export default node;
