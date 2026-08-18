/**
 * @generated SignedSource<<b40c2fc68b418a3f3be2d0491e589d2c>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CreditLedgerTransactionType = "ADJUSTED" | "CONSUMED" | "EXPIRED" | "FORFEITED" | "GRANTED" | "RELEASED" | "%future added value";
export type ConsumeEntitlementCreditInput = {
  bookingAt: any;
  bookingId: string;
  clientMutationId?: string | null | undefined;
  idempotencyKey: string;
};
export type marketplaceEntitlementsConsumeMutation$variables = {
  input: ConsumeEntitlementCreditInput;
};
export type marketplaceEntitlementsConsumeMutation$data = {
  readonly consumeEntitlementCredit: {
    readonly clientMutationId: string | null | undefined;
    readonly error: string | null | undefined;
    readonly ledgerEntry: {
      readonly bookingId: string | null | undefined;
      readonly createdAt: any;
      readonly id: string;
      readonly quantity: number;
      readonly referenceKey: string;
      readonly transactionType: CreditLedgerTransactionType;
    } | null | undefined;
  };
};
export type marketplaceEntitlementsConsumeMutation = {
  response: marketplaceEntitlementsConsumeMutation$data;
  variables: marketplaceEntitlementsConsumeMutation$variables;
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
    "concreteType": "ConsumeEntitlementCreditPayload",
    "kind": "LinkedField",
    "name": "consumeEntitlementCredit",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "clientMutationId",
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
        "concreteType": "CreditLedgerEntryDetails",
        "kind": "LinkedField",
        "name": "ledgerEntry",
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
    "name": "marketplaceEntitlementsConsumeMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceEntitlementsConsumeMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "4b51dca2281200b3057c671051a0845c",
    "id": null,
    "metadata": {},
    "name": "marketplaceEntitlementsConsumeMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceEntitlementsConsumeMutation(\n  $input: ConsumeEntitlementCreditInput!\n) {\n  consumeEntitlementCredit(input: $input) {\n    clientMutationId\n    error\n    ledgerEntry {\n      id\n      bookingId\n      quantity\n      transactionType\n      referenceKey\n      createdAt\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "94d63627401ed255c680241b12375930";

export default node;
