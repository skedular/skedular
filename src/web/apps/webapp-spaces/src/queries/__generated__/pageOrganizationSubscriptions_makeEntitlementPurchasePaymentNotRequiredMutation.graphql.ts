/**
 * @generated SignedSource<<ef2404d3039a9edcd10261ec3e7e15a4>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MakeEntitlementPurchasePaymentNotRequiredInput = {
  clientMutationId?: string | null | undefined;
  purchaseId: string;
};
export type pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation$variables = {
  input: MakeEntitlementPurchasePaymentNotRequiredInput;
};
export type pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation$data = {
  readonly makeEntitlementPurchasePaymentNotRequired: {
    readonly error: string | null | undefined;
    readonly purchase: {
      readonly id: string;
      readonly lifecycleState: string;
      readonly paymentStatus: string;
    } | null | undefined;
  };
};
export type pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation = {
  response: pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation$data;
  variables: pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation$variables;
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
    "name": "makeEntitlementPurchasePaymentNotRequired",
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
    "name": "pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "89afc876c325478dafdfead583765928",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation(\n  $input: MakeEntitlementPurchasePaymentNotRequiredInput!\n) {\n  makeEntitlementPurchasePaymentNotRequired(input: $input) {\n    error\n    purchase {\n      id\n      paymentStatus\n      lifecycleState\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ce47ebd87ca946a883e360c591e2867c";

export default node;
