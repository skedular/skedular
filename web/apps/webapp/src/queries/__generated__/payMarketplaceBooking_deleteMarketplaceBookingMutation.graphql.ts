/**
 * @generated SignedSource<<6d5f6552a43605cdf6cfbc086151ff2b>>
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
export type payMarketplaceBooking_deleteMarketplaceBookingMutation$variables = {
  input: DeleteMarketplaceBookingInput;
};
export type payMarketplaceBooking_deleteMarketplaceBookingMutation$data = {
  readonly deleteMarketplaceBooking: {
    readonly booking: {
      readonly id: string;
    };
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "payMarketplaceBooking_deleteMarketplaceBookingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "payMarketplaceBooking_deleteMarketplaceBookingMutation",
    "selections": (v1/*: any*/)
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
