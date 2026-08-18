/**
 * @generated SignedSource<<d8eedfe546fee2538d1d82bf4f02dc5a>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type ConfirmEntitlementPurchaseInput = {
  clientMutationId?: string | null | undefined;
  purchaseId: string;
};
export type pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation$variables = {
  input: ConfirmEntitlementPurchaseInput;
};
export type pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation$data = {
  readonly confirmEntitlementPurchase: {
    readonly error: string | null | undefined;
    readonly purchase: {
      readonly entitlement: {
        readonly autoRenew: boolean;
        readonly cancelAtPeriodEnd: boolean;
        readonly id: string;
        readonly nextRenewalAt: any | null | undefined;
        readonly renewalFailureReason: string | null | undefined;
        readonly status: EntitlementStatus;
      } | null | undefined;
      readonly id: string;
      readonly lifecycleState: string;
      readonly paymentStatus: string;
    } | null | undefined;
  };
};
export type pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation = {
  response: pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation$data;
  variables: pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation$variables;
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
    "concreteType": "EntitlementPurchasePayload",
    "kind": "LinkedField",
    "name": "confirmEntitlementPurchase",
    "plural": false,
    "selections": [
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
        "concreteType": "EntitlementPurchaseDetails",
        "kind": "LinkedField",
        "name": "purchase",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "paymentStatus",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "lifecycleState",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "EntitlementDetails",
            "kind": "LinkedField",
            "name": "entitlement",
            "plural": false,
            "selections": [
              (v1/*:: as any*/),
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
                "name": "status",
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
    "name": "pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "5c1fcd5684ea25c10b78c51085015967",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation(\n  $input: ConfirmEntitlementPurchaseInput!\n) {\n  confirmEntitlementPurchase(input: $input) {\n    error\n    purchase {\n      id\n      paymentStatus\n      lifecycleState\n      entitlement {\n        id\n        autoRenew\n        cancelAtPeriodEnd\n        status\n        nextRenewalAt\n        renewalFailureReason\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "41f11eabddec03b036628f830d98d296";

export default node;
