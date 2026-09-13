/**
 * @generated SignedSource<<3acdd4d2564994183edca0e5db5f04a8>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type entitlementPurchaseDetails_subscription_EntitlementPurchase$variables = {
  purchaseId: string;
};
export type entitlementPurchaseDetails_subscription_EntitlementPurchase$data = {
  readonly entitlementPurchase: {
    readonly amount: any;
    readonly automaticPaymentStatus: {
      readonly cancelAtPeriodEnd: boolean;
      readonly configured: boolean;
      readonly currentPeriodEndsAt: any | null | undefined;
      readonly status: string;
    } | null | undefined;
    readonly creditQuantity: number;
    readonly currency: string;
    readonly entitlement: {
      readonly availableQuantity: number;
      readonly id: string;
      readonly status: EntitlementStatus;
    } | null | undefined;
    readonly id: string;
    readonly invoiceNumber: string | null | undefined;
    readonly invoiceUrl: string | null | undefined;
    readonly lifecycleState: string;
    readonly paymentAction: string | null | undefined;
    readonly paymentExpiry: any;
    readonly paymentMethod: string;
    readonly paymentStatus: string;
    readonly pricingId: string;
    readonly serviceStartAt: any;
    readonly validityDays: number;
  };
};
export type entitlementPurchaseDetails_subscription_EntitlementPurchase = {
  response: entitlementPurchaseDetails_subscription_EntitlementPurchase$data;
  variables: entitlementPurchaseDetails_subscription_EntitlementPurchase$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "purchaseId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "status",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "purchaseId",
        "variableName": "purchaseId"
      }
    ],
    "concreteType": "EntitlementPurchaseDetails",
    "kind": "LinkedField",
    "name": "entitlementPurchase",
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
        "concreteType": "MarketplaceAutomaticPaymentStatusDetails",
        "kind": "LinkedField",
        "name": "automaticPaymentStatus",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "configured",
            "storageKey": null
          },
          (v2/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "currentPeriodEndsAt",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cancelAtPeriodEnd",
            "storageKey": null
          }
        ],
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
        "kind": "ScalarField",
        "name": "paymentMethod",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "paymentExpiry",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "serviceStartAt",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "amount",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "currency",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "pricingId",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "creditQuantity",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "validityDays",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "invoiceNumber",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "invoiceUrl",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "paymentAction",
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
            "name": "availableQuantity",
            "storageKey": null
          },
          (v2/*:: as any*/)
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
    "name": "entitlementPurchaseDetails_subscription_EntitlementPurchase",
    "selections": (v3/*:: as any*/),
    "type": "Subscription",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "entitlementPurchaseDetails_subscription_EntitlementPurchase",
    "selections": (v3/*:: as any*/)
  },
  "params": {
    "cacheID": "e80ceb01bb92dd7643dc9706e73cc17c",
    "id": null,
    "metadata": {},
    "name": "entitlementPurchaseDetails_subscription_EntitlementPurchase",
    "operationKind": "subscription",
    "text": "subscription entitlementPurchaseDetails_subscription_EntitlementPurchase(\n  $purchaseId: String!\n) {\n  entitlementPurchase(purchaseId: $purchaseId) {\n    id\n    paymentStatus\n    automaticPaymentStatus {\n      configured\n      status\n      currentPeriodEndsAt\n      cancelAtPeriodEnd\n    }\n    lifecycleState\n    paymentMethod\n    paymentExpiry\n    serviceStartAt\n    amount\n    currency\n    pricingId\n    creditQuantity\n    validityDays\n    invoiceNumber\n    invoiceUrl\n    paymentAction\n    entitlement {\n      id\n      availableQuantity\n      status\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f5e413792d6eafe693cb7c39d9ebf01d";

export default node;
