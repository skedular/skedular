/**
 * @generated SignedSource<<b6db401fc692423cad9a5f7f064ef724>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type CreateEntitlementPurchaseInput = {
  autoRenew: boolean;
  checkoutReturnUrl?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  invoiceEmailList: ReadonlyArray<string>;
  organizationId: string;
  paymentMethod: PaymentMethod;
  pricingId: string;
  productVersionId: string;
  serviceStartAt: any;
};
export type marketplaceProductBookingForm_createEntitlementPurchaseMutation$variables = {
  input: CreateEntitlementPurchaseInput;
};
export type marketplaceProductBookingForm_createEntitlementPurchaseMutation$data = {
  readonly createEntitlementPurchase: {
    readonly error: string | null | undefined;
    readonly purchase: {
      readonly id: string;
      readonly paymentAction: string | null | undefined;
      readonly paymentInstructions: string | null | undefined;
    } | null | undefined;
  };
};
export type marketplaceProductBookingForm_createEntitlementPurchaseMutation = {
  response: marketplaceProductBookingForm_createEntitlementPurchaseMutation$data;
  variables: marketplaceProductBookingForm_createEntitlementPurchaseMutation$variables;
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
    "name": "createEntitlementPurchase",
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
            "name": "paymentAction",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "paymentInstructions",
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
    "name": "marketplaceProductBookingForm_createEntitlementPurchaseMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductBookingForm_createEntitlementPurchaseMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "52b780cf31f8d9391973b6729a55ca4d",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductBookingForm_createEntitlementPurchaseMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceProductBookingForm_createEntitlementPurchaseMutation(\n  $input: CreateEntitlementPurchaseInput!\n) {\n  createEntitlementPurchase(input: $input) {\n    error\n    purchase {\n      id\n      paymentAction\n      paymentInstructions\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f45b6454d8dd4e96d9951ab7bd89780f";

export default node;
