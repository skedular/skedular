/**
 * @generated SignedSource<<7301bff5d3a25a97e8d11f84bded4de0>>
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
export type marketplaceProductBookingDetails_deleteMarketplaceBookingMutation$variables = {
  input: DeleteMarketplaceBookingInput;
};
export type marketplaceProductBookingDetails_deleteMarketplaceBookingMutation$data = {
  readonly deleteMarketplaceBooking: {
    readonly booking: {
      readonly deletedByCustomer: {
        readonly id: string;
      } | null | undefined;
      readonly id: string;
    };
  };
};
export type marketplaceProductBookingDetails_deleteMarketplaceBookingMutation = {
  response: marketplaceProductBookingDetails_deleteMarketplaceBookingMutation$data;
  variables: marketplaceProductBookingDetails_deleteMarketplaceBookingMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
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
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDetails",
            "kind": "LinkedField",
            "name": "deletedByCustomer",
            "plural": false,
            "selections": [
              (v1/*:: as any*/)
            ],
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
    "name": "marketplaceProductBookingDetails_deleteMarketplaceBookingMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductBookingDetails_deleteMarketplaceBookingMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "c83d5b1197500f092effb1a2cd584b76",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductBookingDetails_deleteMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceProductBookingDetails_deleteMarketplaceBookingMutation(\n  $input: DeleteMarketplaceBookingInput!\n) {\n  deleteMarketplaceBooking(input: $input) {\n    booking {\n      id\n      deletedByCustomer {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7ab7484fdb78730c3fdc93b7d39f84d5";

export default node;
