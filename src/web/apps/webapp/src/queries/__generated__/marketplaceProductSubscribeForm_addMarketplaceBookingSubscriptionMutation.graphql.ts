/**
 * @generated SignedSource<<25bc6261d2fa3bb59bb14870e759440a>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
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
  requestedResourceIds?: ReadonlyArray<string> | null | undefined;
  startedAt: any;
  teamIds?: ReadonlyArray<string> | null | undefined;
  weeklySelectedDays?: ReadonlyArray<DayOfWeek> | null | undefined;
};
export type marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation$variables = {
  input: AddMarketplaceBookingSubscriptionInput;
};
export type marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation$data = {
  readonly addMarketplaceBookingSubscription: {
    readonly accessError: {
      readonly message: string;
      readonly upgradeRequired: boolean;
    } | null | undefined;
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
      readonly recurringBookings: ReadonlyArray<{
        readonly marketplaceBooking: {
          readonly bookingCheckoutSession: {
            readonly checkoutUrl: string;
          } | null | undefined;
          readonly id: string;
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
        } | null | undefined;
        readonly startDate: any;
      }>;
      readonly startedAt: any;
      readonly status: {
        readonly name: string;
        readonly type: MarketplaceBookingSubscriptionStatus;
      };
    } | null | undefined;
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
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "concreteType": "SpacesAccessErrorDetails",
  "kind": "LinkedField",
  "name": "accessError",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "message",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "upgradeRequired",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startedAt",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "nextRenewalAt",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "autoRenew",
  "storageKey": null
},
v7 = [
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
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v7/*:: as any*/),
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startDate",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isPaymentRequired",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentExpiry",
  "storageKey": null
},
v12 = {
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
v13 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v7/*:: as any*/),
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v7/*:: as any*/),
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingDetails",
  "kind": "LinkedField",
  "name": "marketplaceBooking",
  "plural": false,
  "selections": [
    (v3/*:: as any*/),
    (v10/*:: as any*/),
    (v11/*:: as any*/),
    (v12/*:: as any*/),
    (v13/*:: as any*/),
    (v14/*:: as any*/),
    (v15/*:: as any*/),
    (v16/*:: as any*/)
  ],
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingDetails",
  "kind": "LinkedField",
  "name": "marketplaceBooking",
  "plural": false,
  "selections": [
    (v3/*:: as any*/),
    (v10/*:: as any*/),
    (v11/*:: as any*/),
    (v12/*:: as any*/),
    (v13/*:: as any*/),
    (v14/*:: as any*/),
    (v15/*:: as any*/),
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
    (v16/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "invoiceEmailList",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "MarketplaceBookingSubscriptionPayload",
        "kind": "LinkedField",
        "name": "addMarketplaceBookingSubscription",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingSubscriptionDetails",
            "kind": "LinkedField",
            "name": "marketplaceBookingSubscription",
            "plural": false,
            "selections": [
              (v3/*:: as any*/),
              (v4/*:: as any*/),
              (v5/*:: as any*/),
              (v6/*:: as any*/),
              (v8/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "RecurringBookingDetails",
                "kind": "LinkedField",
                "name": "recurringBookings",
                "plural": true,
                "selections": [
                  (v9/*:: as any*/),
                  (v17/*:: as any*/)
                ],
                "storageKey": null
              },
              (v18/*:: as any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "MarketplaceBookingSubscriptionPayload",
        "kind": "LinkedField",
        "name": "addMarketplaceBookingSubscription",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingSubscriptionDetails",
            "kind": "LinkedField",
            "name": "marketplaceBookingSubscription",
            "plural": false,
            "selections": [
              (v3/*:: as any*/),
              (v4/*:: as any*/),
              (v5/*:: as any*/),
              (v6/*:: as any*/),
              (v8/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "RecurringBookingDetails",
                "kind": "LinkedField",
                "name": "recurringBookings",
                "plural": true,
                "selections": [
                  (v9/*:: as any*/),
                  (v17/*:: as any*/),
                  (v3/*:: as any*/)
                ],
                "storageKey": null
              },
              (v18/*:: as any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "61249a6c4da54d2361e2ca471c8dffa0",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation(\n  $input: AddMarketplaceBookingSubscriptionInput!\n) {\n  addMarketplaceBookingSubscription(input: $input) {\n    accessError {\n      message\n      upgradeRequired\n    }\n    marketplaceBookingSubscription {\n      id\n      startedAt\n      nextRenewalAt\n      autoRenew\n      status {\n        type\n        name\n      }\n      recurringBookings {\n        startDate\n        marketplaceBooking {\n          id\n          isPaymentRequired\n          paymentExpiry\n          bookingCheckoutSession {\n            checkoutUrl\n          }\n          paymentStatus {\n            type\n            name\n          }\n          quantity\n          invoiceUrl\n          paymentMethod {\n            type\n            name\n          }\n        }\n        id\n      }\n      marketplaceBooking {\n        id\n        isPaymentRequired\n        paymentExpiry\n        bookingCheckoutSession {\n          checkoutUrl\n        }\n        paymentStatus {\n          type\n          name\n        }\n        quantity\n        invoiceUrl\n        invoiceNumber\n        totalAmountToDisplay\n        totalAmountExcludeTaxToDisplay\n        taxAmountToDisplay\n        billingMode\n        paymentMethod {\n          type\n          name\n        }\n        invoiceEmailList\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9f36feba4f7f408aca29914870d0a881";

export default node;
