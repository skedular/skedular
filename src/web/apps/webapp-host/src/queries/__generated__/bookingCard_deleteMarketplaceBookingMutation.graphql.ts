/**
 * @generated SignedSource<<40b679405e06f184e26acd35a931af33>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteMarketplaceBookingInput = {
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
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDetails",
            "kind": "LinkedField",
            "name": "booking",
            "plural": false,
            "selections": [
              (v2/*:: as any*/)
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
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDetails",
            "kind": "LinkedField",
            "name": "booking",
            "plural": false,
            "selections": [
              (v2/*:: as any*/),
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
    "cacheID": "64c5a60e281e7f361fa19ff29dc89d97",
    "id": null,
    "metadata": {},
    "name": "bookingCard_deleteMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_deleteMarketplaceBookingMutation(\n  $input: DeleteMarketplaceBookingInput!\n) {\n  deleteMarketplaceBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "50661ec7999e23dbf0ba86bb9a622ed3";

export default node;
