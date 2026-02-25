/**
 * @generated SignedSource<<0e63317c314ecd82d1a3668bb1aaf34c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteMarketplaceBookingInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type bookings_deleteMarketplaceBookingMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteMarketplaceBookingInput;
};
export type bookings_deleteMarketplaceBookingMutation$data = {
  readonly deleteMarketplaceBooking: {
    readonly booking: {
      readonly id: string;
    };
  };
};
export type bookings_deleteMarketplaceBookingMutation = {
  response: bookings_deleteMarketplaceBookingMutation$data;
  variables: bookings_deleteMarketplaceBookingMutation$variables;
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
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "bookings_deleteMarketplaceBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "deleteMarketplaceBooking",
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
              (v2/*: any*/)
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookings_deleteMarketplaceBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "deleteMarketplaceBooking",
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
              (v2/*: any*/),
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
    "cacheID": "4f4d2cd6f9876c0b2fd4c20da32ed260",
    "id": null,
    "metadata": {},
    "name": "bookings_deleteMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation bookings_deleteMarketplaceBookingMutation(\n  $input: DeleteMarketplaceBookingInput!\n) {\n  deleteMarketplaceBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a64167c2356deccc269c9c7dc46eaa9a";

export default node;
