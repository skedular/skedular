/**
 * @generated SignedSource<<2c42467a8ef289fe0a719aa03be306fe>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type ProductPricingBillingMode = "IN_ARREARS" | "NOT_SET" | "UPFRONT" | "%future added value";
export type AddMarketplaceBookingSubscriptionInput = {
  autoRenew: boolean;
  cancelAtPeriodEnd: boolean;
  checkoutReturnUrl?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  id?: string | null | undefined;
  invoiceEmailList?: ReadonlyArray<string> | null | undefined;
  organizationCustomDomains?: ReadonlyArray<string> | null | undefined;
  organizationIds?: ReadonlyArray<string> | null | undefined;
  paymentMethod: PaymentMethod;
  pricingId: string;
  productVersionId: string;
  quantity: number;
  startedAt: any;
  teamIds?: ReadonlyArray<string> | null | undefined;
};
export type marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation$variables = {
  input: AddMarketplaceBookingSubscriptionInput;
};
export type marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation$data = {
  readonly addMarketplaceBookingSubscription: {
    readonly marketplaceBookingSubscription: {
      readonly autoRenew: boolean;
      readonly id: string;
      readonly marketplaceBooking: {
        readonly billingMode: ProductPricingBillingMode;
        readonly bookingCheckoutSession: {
          readonly checkoutUrl: string;
        } | null | undefined;
        readonly id: string;
        readonly invoiceEmailList: ReadonlyArray<string>;
        readonly invoiceNumber: string | null | undefined;
        readonly invoiceUrl: string | null | undefined;
        readonly isPaymentRequired: boolean;
        readonly paymentExpiry: any;
        readonly paymentMethod: {
          readonly name: string;
          readonly type: PaymentMethod;
        };
        readonly paymentStatus: {
          readonly name: string;
          readonly type: PaymentStatus;
        };
        readonly quantity: number;
        readonly taxAmountToDisplay: string;
        readonly totalAmountExcludeTaxToDisplay: string;
        readonly totalAmountToDisplay: string;
      };
      readonly nextRenewalAt: any | null | undefined;
      readonly startedAt: any;
      readonly status: {
        readonly name: string;
        readonly type: MarketplaceBookingSubscriptionStatus;
      };
    };
  };
};
export type marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation = {
  response: marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation$data;
  variables: marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation$variables;
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
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "name",
    "storageKey": null
  }
],
v3 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "MarketplaceBookingSubscriptionPayload",
    "kind": "LinkedField",
    "name": "addMarketplaceBookingSubscription",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceBookingSubscriptionDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscription",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "startedAt",
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
            "name": "autoRenew",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
            "kind": "LinkedField",
            "name": "status",
            "plural": false,
            "selections": (v2/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "isPaymentRequired",
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
                "concreteType": "BookingCheckoutSessionDetails",
                "kind": "LinkedField",
                "name": "bookingCheckoutSession",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "checkoutUrl",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PaymentStatusDetails",
                "kind": "LinkedField",
                "name": "paymentStatus",
                "plural": false,
                "selections": (v2/*: any*/),
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
                "name": "invoiceUrl",
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
                "name": "totalAmountToDisplay",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "totalAmountExcludeTaxToDisplay",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "taxAmountToDisplay",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "billingMode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PaymentMethodTypeDetails",
                "kind": "LinkedField",
                "name": "paymentMethod",
                "plural": false,
                "selections": (v2/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "invoiceEmailList",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation",
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "690cad005486fcabedbcd0f9149b6c24",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation(\n  $input: AddMarketplaceBookingSubscriptionInput!\n) {\n  addMarketplaceBookingSubscription(input: $input) {\n    marketplaceBookingSubscription {\n      id\n      startedAt\n      nextRenewalAt\n      autoRenew\n      status {\n        type\n        name\n      }\n      marketplaceBooking {\n        id\n        isPaymentRequired\n        paymentExpiry\n        bookingCheckoutSession {\n          checkoutUrl\n        }\n        paymentStatus {\n          type\n          name\n        }\n        quantity\n        invoiceUrl\n        invoiceNumber\n        totalAmountToDisplay\n        totalAmountExcludeTaxToDisplay\n        taxAmountToDisplay\n        billingMode\n        paymentMethod {\n          type\n          name\n        }\n        invoiceEmailList\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a69d40e4f3e3ae32e3a73a78b44b376e";

export default node;
