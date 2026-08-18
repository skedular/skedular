/**
 * @generated SignedSource<<5c5d848fc819ccc356cd405a856688d8>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CreditLedgerTransactionType = "ADJUSTED" | "CONSUMED" | "EXPIRED" | "FORFEITED" | "GRANTED" | "RELEASED" | "%future added value";
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type marketplaceEntitlementsQuery$variables = {
  customerId: string;
};
export type marketplaceEntitlementsQuery$data = {
  readonly entitlementsByCustomer: ReadonlyArray<{
    readonly availableQuantity: number;
    readonly expiresAt: any;
    readonly grantedQuantity: number;
    readonly id: string;
    readonly ledger: ReadonlyArray<{
      readonly bookingId: string | null | undefined;
      readonly createdAt: any;
      readonly id: string;
      readonly quantity: number;
      readonly referenceKey: string;
      readonly transactionType: CreditLedgerTransactionType;
    }>;
    readonly refund: {
      readonly amount: any;
      readonly id: string;
      readonly paymentRefundStatus: string | null | undefined;
      readonly status: MarketplaceRefundStatus;
      readonly unusedCreditQuantity: number;
    } | null | undefined;
    readonly status: EntitlementStatus;
  }>;
};
export type marketplaceEntitlementsQuery = {
  response: marketplaceEntitlementsQuery$data;
  variables: marketplaceEntitlementsQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "customerId"
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
        "name": "customerId",
        "variableName": "customerId"
      }
    ],
    "concreteType": "EntitlementDetails",
    "kind": "LinkedField",
    "name": "entitlementsByCustomer",
    "plural": true,
    "selections": [
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "availableQuantity",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "grantedQuantity",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "expiresAt",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "EntitlementRefundDetails",
        "kind": "LinkedField",
        "name": "refund",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
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
            "name": "unusedCreditQuantity",
            "storageKey": null
          },
          (v2/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "paymentRefundStatus",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CreditLedgerEntryDetails",
        "kind": "LinkedField",
        "name": "ledger",
        "plural": true,
        "selections": [
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "bookingId",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "quantity",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "transactionType",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "referenceKey",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "createdAt",
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
    "name": "marketplaceEntitlementsQuery",
    "selections": (v3/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceEntitlementsQuery",
    "selections": (v3/*:: as any*/)
  },
  "params": {
    "cacheID": "594fa0831ed71da57ff12b0c0753c335",
    "id": null,
    "metadata": {},
    "name": "marketplaceEntitlementsQuery",
    "operationKind": "query",
    "text": "query marketplaceEntitlementsQuery(\n  $customerId: String!\n) {\n  entitlementsByCustomer(customerId: $customerId) {\n    id\n    status\n    availableQuantity\n    grantedQuantity\n    expiresAt\n    refund {\n      id\n      amount\n      unusedCreditQuantity\n      status\n      paymentRefundStatus\n    }\n    ledger {\n      id\n      bookingId\n      quantity\n      transactionType\n      referenceKey\n      createdAt\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5b6cf5c3debf30f3f487652aa29fc242";

export default node;
