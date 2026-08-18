/**
 * @generated SignedSource<<bd6e3a6a57ad35401e5bda8878864fea>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RejectEntitlementPurchaseInput = {
  clientMutationId?: string | null | undefined;
  purchaseId: string;
};
export type pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation$variables = {
  input: RejectEntitlementPurchaseInput;
};
export type pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation$data = {
  readonly rejectEntitlementPurchase: {
    readonly error: string | null | undefined;
    readonly purchase: {
      readonly id: string;
      readonly lifecycleState: string;
      readonly paymentStatus: string;
    } | null | undefined;
  };
};
export type pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation = {
  response: pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation$data;
  variables: pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation$variables;
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
    "name": "rejectEntitlementPurchase",
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
            "name": "paymentStatus",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "lifecycleState",
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
    "name": "pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "3efa5f28797828208333eb021054301e",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation(\n  $input: RejectEntitlementPurchaseInput!\n) {\n  rejectEntitlementPurchase(input: $input) {\n    error\n    purchase {\n      id\n      paymentStatus\n      lifecycleState\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b2eb05aba12270849e6cc7145b0d7eef";

export default node;
