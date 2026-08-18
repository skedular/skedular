/**
 * @generated SignedSource<<2013d9112a918754cc1a97d3a0dd1006>>
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
export type pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation$variables = {
  input: RejectEntitlementPurchaseInput;
};
export type pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation$data = {
  readonly rejectEntitlementPurchase: {
    readonly error: string | null | undefined;
    readonly purchase: {
      readonly id: string;
      readonly lifecycleState: string;
      readonly paymentStatus: string;
    } | null | undefined;
  };
};
export type pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation = {
  response: pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation$data;
  variables: pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation$variables;
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
    "name": "pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "5a40072cff4ad8d58cbad70818cdfc5a",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation(\n  $input: RejectEntitlementPurchaseInput!\n) {\n  rejectEntitlementPurchase(input: $input) {\n    error\n    purchase {\n      id\n      paymentStatus\n      lifecycleState\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4fff42301078a45f86eb32e60702050e";

export default node;
