/**
 * @generated SignedSource<<f0c3dad2cab14d692e9a91a275d0ef4e>>
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
export type myBookings_deleteMarketplaceBookingMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteMarketplaceBookingInput;
};
export type myBookings_deleteMarketplaceBookingMutation$data = {
  readonly deleteMarketplaceBooking: {
    readonly booking: {
      readonly id: string;
    };
  };
};
export type myBookings_deleteMarketplaceBookingMutation = {
  response: myBookings_deleteMarketplaceBookingMutation$data;
  variables: myBookings_deleteMarketplaceBookingMutation$variables;
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
    "name": "myBookings_deleteMarketplaceBookingMutation",
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
    "name": "myBookings_deleteMarketplaceBookingMutation",
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
    "cacheID": "ed72f0ef3aeecfd90cc0e7d9f1d592a8",
    "id": null,
    "metadata": {},
    "name": "myBookings_deleteMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation myBookings_deleteMarketplaceBookingMutation(\n  $input: DeleteMarketplaceBookingInput!\n) {\n  deleteMarketplaceBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9eb053a60c3ef834945febcf0506bbb9";

export default node;
