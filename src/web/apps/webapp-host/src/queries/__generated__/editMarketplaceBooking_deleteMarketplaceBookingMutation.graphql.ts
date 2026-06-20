/**
 * @generated SignedSource<<f72740a4e94ded951133a4948639df8d>>
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
export type editMarketplaceBooking_deleteMarketplaceBookingMutation$variables = {
  input: DeleteMarketplaceBookingInput;
};
export type editMarketplaceBooking_deleteMarketplaceBookingMutation$data = {
  readonly deleteMarketplaceBooking: {
    readonly booking: {
      readonly id: string;
    } | null | undefined;
  };
};
export type editMarketplaceBooking_deleteMarketplaceBookingMutation = {
  response: editMarketplaceBooking_deleteMarketplaceBookingMutation$data;
  variables: editMarketplaceBooking_deleteMarketplaceBookingMutation$variables;
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
    "name": "editMarketplaceBooking_deleteMarketplaceBookingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editMarketplaceBooking_deleteMarketplaceBookingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "a8a6056d31b23b2180d789bbdb51a846",
    "id": null,
    "metadata": {},
    "name": "editMarketplaceBooking_deleteMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation editMarketplaceBooking_deleteMarketplaceBookingMutation(\n  $input: DeleteMarketplaceBookingInput!\n) {\n  deleteMarketplaceBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "97424094aafb74764e21323fc567f862";

export default node;
