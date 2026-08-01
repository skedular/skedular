/**
 * @generated SignedSource<<cf81f76a38f0b1a1e6c235c4eb292a26>>
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
export type marketplaceProductBookingDetails_acceptPartialMutation$variables = {
  input: ResolvePartialMarketplaceBookingInput;
};
export type marketplaceProductBookingDetails_acceptPartialMutation$data = {
  readonly acceptPartialMarketplaceBooking: {
    readonly id: string;
    readonly resolutionDecision: string | null | undefined;
  };
};
export type marketplaceProductBookingDetails_acceptPartialMutation = {
  response: marketplaceProductBookingDetails_acceptPartialMutation$data;
  variables: marketplaceProductBookingDetails_acceptPartialMutation$variables;
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
    "name": "acceptPartialMarketplaceBooking",
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
    "name": "marketplaceProductBookingDetails_acceptPartialMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductBookingDetails_acceptPartialMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "f004fa91d592fe37fff17a908403f2f9",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductBookingDetails_acceptPartialMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceProductBookingDetails_acceptPartialMutation(\n  $input: ResolvePartialMarketplaceBookingInput!\n) {\n  acceptPartialMarketplaceBooking(input: $input) {\n    id\n    resolutionDecision\n  }\n}\n"
  }
};
})();

(node as any).hash = "d77ab52f993a07d2913206d264dc1ea0";

export default node;
