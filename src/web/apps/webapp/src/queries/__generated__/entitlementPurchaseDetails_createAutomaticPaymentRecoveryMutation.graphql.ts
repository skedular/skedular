/**
 * @generated SignedSource<<ce9c72eb957886f0a39a6c240b061d50>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CreateEntitlementAutomaticPaymentRecoveryInput = {
  clientMutationId: string;
  purchaseId: string;
  returnUrl: string;
};
export type entitlementPurchaseDetails_createAutomaticPaymentRecoveryMutation$variables = {
  input: CreateEntitlementAutomaticPaymentRecoveryInput;
};
export type entitlementPurchaseDetails_createAutomaticPaymentRecoveryMutation$data = {
  readonly createEntitlementAutomaticPaymentRecovery: {
    readonly automaticPaymentRecoveryUrl: string | null | undefined;
    readonly clientMutationId: string | null | undefined;
    readonly error: string | null | undefined;
  };
};
export type entitlementPurchaseDetails_createAutomaticPaymentRecoveryMutation = {
  response: entitlementPurchaseDetails_createAutomaticPaymentRecoveryMutation$data;
  variables: entitlementPurchaseDetails_createAutomaticPaymentRecoveryMutation$variables;
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
    "concreteType": "EntitlementPurchasePayload",
    "kind": "LinkedField",
    "name": "createEntitlementAutomaticPaymentRecovery",
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
    "name": "entitlementPurchaseDetails_createAutomaticPaymentRecoveryMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "entitlementPurchaseDetails_createAutomaticPaymentRecoveryMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "a8ae8f219d2bb5a3618a7cbb84e1a2f5",
    "id": null,
    "metadata": {},
    "name": "entitlementPurchaseDetails_createAutomaticPaymentRecoveryMutation",
    "operationKind": "mutation",
    "text": "mutation entitlementPurchaseDetails_createAutomaticPaymentRecoveryMutation(\n  $input: CreateEntitlementAutomaticPaymentRecoveryInput!\n) {\n  createEntitlementAutomaticPaymentRecovery(input: $input) {\n    automaticPaymentRecoveryUrl\n    error\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "17b8a22f15c1d26da1242a8562228cf8";

export default node;
