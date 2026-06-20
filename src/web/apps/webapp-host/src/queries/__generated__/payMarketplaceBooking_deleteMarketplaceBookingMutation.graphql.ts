/**
 * @generated SignedSource<<fa17a3f2594999b41f63f4f775ac88d0>>
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
export type payMarketplaceBooking_deleteMarketplaceBookingMutation$variables = {
  input: DeleteMarketplaceBookingInput;
};
export type payMarketplaceBooking_deleteMarketplaceBookingMutation$data = {
  readonly deleteMarketplaceBooking: {
    readonly booking: {
      readonly id: string;
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
    "cacheID": "71f7137c0e98c9de87c235e6f7151b5b",
    "id": null,
    "metadata": {},
    "name": "payMarketplaceBooking_deleteMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation payMarketplaceBooking_deleteMarketplaceBookingMutation(\n  $input: DeleteMarketplaceBookingInput!\n) {\n  deleteMarketplaceBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5946e5620f307438f389539fd0ac1658";

export default node;
