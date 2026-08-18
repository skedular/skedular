/**
 * @generated SignedSource<<f411d17b403114a7d1d7cbc70b79797c>>
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
export type entitlementPurchaseDetails_cancelEntitlementMutation$variables = {
  input: CancelEntitlementInput;
};
export type entitlementPurchaseDetails_cancelEntitlementMutation$data = {
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
export type entitlementPurchaseDetails_cancelEntitlementMutation = {
  response: entitlementPurchaseDetails_cancelEntitlementMutation$data;
  variables: entitlementPurchaseDetails_cancelEntitlementMutation$variables;
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
    "name": "entitlementPurchaseDetails_cancelEntitlementMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "entitlementPurchaseDetails_cancelEntitlementMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "72507f57a58530f8ae3f7e1855c7f46b",
    "id": null,
    "metadata": {},
    "name": "entitlementPurchaseDetails_cancelEntitlementMutation",
    "operationKind": "mutation",
    "text": "mutation entitlementPurchaseDetails_cancelEntitlementMutation(\n  $input: CancelEntitlementInput!\n) {\n  cancelEntitlement(input: $input) {\n    entitlement {\n      id\n      status\n      autoRenew\n      cancelAtPeriodEnd\n      nextRenewalAt\n      renewalFailureReason\n    }\n    error\n  }\n}\n"
  }
};
})();

(node as any).hash = "5b3c4c7ead376c134f4d957d2a540cb7";

export default node;
