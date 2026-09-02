/**
 * @generated SignedSource<<f552e68a3e748d9e496e14f49ba36f2b>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type MakeEntitlementPurchasePaymentNotRequiredInput = {
  clientMutationId?: string | null | undefined;
  purchaseId: string;
};
export type pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation$variables = {
  input: MakeEntitlementPurchasePaymentNotRequiredInput;
};
export type pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation$data = {
  readonly makeEntitlementPurchasePaymentNotRequired: {
    readonly error: string | null | undefined;
    readonly purchase: {
      readonly entitlement: {
        readonly id: string;
        readonly status: EntitlementStatus;
      } | null | undefined;
      readonly id: string;
      readonly lifecycleState: string;
      readonly paymentStatus: string;
    } | null | undefined;
  };
};
export type pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation = {
  response: pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation$data;
  variables: pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation$variables;
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
                "name": "status",
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
    "name": "pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "eb6f92fc07225c8fa973146ec4444f16",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation(\n  $input: MakeEntitlementPurchasePaymentNotRequiredInput!\n) {\n  makeEntitlementPurchasePaymentNotRequired(input: $input) {\n    error\n    purchase {\n      id\n      paymentStatus\n      lifecycleState\n      entitlement {\n        id\n        status\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "96452cb33d51d85915aa144a905d2860";

export default node;
