/**
 * @generated SignedSource<<6062867df7aa7bcea59ecc151d2db5c0>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type CancelEntitlementInput = {
  clientMutationId?: string | null | undefined;
  entitlementId: string;
  reason: string;
};
export type entitlementBalanceCard_cancelEntitlementMutation$variables = {
  input: CancelEntitlementInput;
};
export type entitlementBalanceCard_cancelEntitlementMutation$data = {
  readonly cancelEntitlement: {
    readonly entitlement: {
      readonly autoRenew: boolean;
      readonly cancelAtPeriodEnd: boolean;
      readonly id: string;
      readonly nextRenewalAt: any | null | undefined;
      readonly renewalFailureReason: string | null | undefined;
      readonly status: EntitlementStatus;
    } | null | undefined;
    readonly error: string | null | undefined;
  };
};
export type entitlementBalanceCard_cancelEntitlementMutation = {
  response: entitlementBalanceCard_cancelEntitlementMutation$data;
  variables: entitlementBalanceCard_cancelEntitlementMutation$variables;
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
    "concreteType": "CancelEntitlementPayload",
    "kind": "LinkedField",
    "name": "cancelEntitlement",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "EntitlementDetails",
        "kind": "LinkedField",
        "name": "entitlement",
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
            "name": "status",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "autoRenew",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cancelAtPeriodEnd",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "nextRenewalAt",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "renewalFailureReason",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "error",
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
    "name": "entitlementBalanceCard_cancelEntitlementMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "entitlementBalanceCard_cancelEntitlementMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "daef692bb563c6f15ef896821058a097",
    "id": null,
    "metadata": {},
    "name": "entitlementBalanceCard_cancelEntitlementMutation",
    "operationKind": "mutation",
    "text": "mutation entitlementBalanceCard_cancelEntitlementMutation(\n  $input: CancelEntitlementInput!\n) {\n  cancelEntitlement(input: $input) {\n    entitlement {\n      id\n      status\n      autoRenew\n      cancelAtPeriodEnd\n      nextRenewalAt\n      renewalFailureReason\n    }\n    error\n  }\n}\n"
  }
};
})();

(node as any).hash = "543cefe0eb1e5fa9968bfcf8471cf1b7";

export default node;
