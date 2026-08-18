/**
 * @generated SignedSource<<b7552c6f314657dcbe2e65463b3ed069>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ConfirmEntitlementPurchaseInput = {
  clientMutationId?: string | null | undefined;
  purchaseId: string;
};
export type pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation$variables = {
  input: ConfirmEntitlementPurchaseInput;
};
export type pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation$data = {
  readonly confirmEntitlementPurchase: {
    readonly error: string | null | undefined;
    readonly purchase: {
      readonly id: string;
      readonly lifecycleState: string;
      readonly paymentStatus: string;
    } | null | undefined;
  };
};
export type pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation = {
  response: pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation$data;
  variables: pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation$variables;
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
    "name": "pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "ec761403c681703591b3dfc65f64727b",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation(\n  $input: ConfirmEntitlementPurchaseInput!\n) {\n  confirmEntitlementPurchase(input: $input) {\n    error\n    purchase {\n      id\n      paymentStatus\n      lifecycleState\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5e51fdd40b0f2607be1277f2eb4a12a6";

export default node;
