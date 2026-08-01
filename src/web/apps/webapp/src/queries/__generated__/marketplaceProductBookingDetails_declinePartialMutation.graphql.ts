/**
 * @generated SignedSource<<83b00830d59a6674751afc068778f4b4>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ResolvePartialMarketplaceBookingInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type marketplaceProductBookingDetails_declinePartialMutation$variables = {
  input: ResolvePartialMarketplaceBookingInput;
};
export type marketplaceProductBookingDetails_declinePartialMutation$data = {
  readonly declinePartialMarketplaceBooking: {
    readonly id: string;
    readonly resolutionDecision: string | null | undefined;
  };
};
export type marketplaceProductBookingDetails_declinePartialMutation = {
  response: marketplaceProductBookingDetails_declinePartialMutation$data;
  variables: marketplaceProductBookingDetails_declinePartialMutation$variables;
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
    "concreteType": "MarketplaceBookingFailureDetails",
    "kind": "LinkedField",
    "name": "declinePartialMarketplaceBooking",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "id",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "resolutionDecision",
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
    "name": "marketplaceProductBookingDetails_declinePartialMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductBookingDetails_declinePartialMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "1b3f13b1c23e6f3700c32a623bcda46f",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductBookingDetails_declinePartialMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceProductBookingDetails_declinePartialMutation(\n  $input: ResolvePartialMarketplaceBookingInput!\n) {\n  declinePartialMarketplaceBooking(input: $input) {\n    id\n    resolutionDecision\n  }\n}\n"
  }
};
})();

(node as any).hash = "49027e635508ea90f8b72adf2c404cb3";

export default node;
