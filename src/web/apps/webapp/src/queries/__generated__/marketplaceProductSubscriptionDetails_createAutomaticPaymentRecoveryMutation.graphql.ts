/**
 * @generated SignedSource<<0f09c3fce0a9f70866f4fafade104c2e>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CreateMarketplaceBookingSubscriptionAutomaticPaymentRecoveryInput = {
  clientMutationId: string;
  returnUrl: string;
  subscriptionId: string;
};
export type marketplaceProductSubscriptionDetails_createAutomaticPaymentRecoveryMutation$variables = {
  input: CreateMarketplaceBookingSubscriptionAutomaticPaymentRecoveryInput;
};
export type marketplaceProductSubscriptionDetails_createAutomaticPaymentRecoveryMutation$data = {
  readonly createMarketplaceBookingSubscriptionAutomaticPaymentRecovery: {
    readonly automaticPaymentRecoveryUrl: string | null | undefined;
    readonly clientMutationId: string | null | undefined;
    readonly error: string | null | undefined;
  };
};
export type marketplaceProductSubscriptionDetails_createAutomaticPaymentRecoveryMutation = {
  response: marketplaceProductSubscriptionDetails_createAutomaticPaymentRecoveryMutation$data;
  variables: marketplaceProductSubscriptionDetails_createAutomaticPaymentRecoveryMutation$variables;
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
    "concreteType": "MarketplaceBookingSubscriptionPayload",
    "kind": "LinkedField",
    "name": "createMarketplaceBookingSubscriptionAutomaticPaymentRecovery",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "automaticPaymentRecoveryUrl",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "error",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "clientMutationId",
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
    "name": "marketplaceProductSubscriptionDetails_createAutomaticPaymentRecoveryMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductSubscriptionDetails_createAutomaticPaymentRecoveryMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "ca99add77f1153fafb71d07241d8763d",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductSubscriptionDetails_createAutomaticPaymentRecoveryMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceProductSubscriptionDetails_createAutomaticPaymentRecoveryMutation(\n  $input: CreateMarketplaceBookingSubscriptionAutomaticPaymentRecoveryInput!\n) {\n  createMarketplaceBookingSubscriptionAutomaticPaymentRecovery(input: $input) {\n    automaticPaymentRecoveryUrl\n    error\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "d9b03f9903ae458e740e1bf0322f8c4d";

export default node;
