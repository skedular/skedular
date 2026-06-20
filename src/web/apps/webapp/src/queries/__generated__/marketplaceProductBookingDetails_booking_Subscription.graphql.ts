/**
 * @generated SignedSource<<8f4881b0cfd4cbb9f64f5fa45e7c5736>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type marketplaceProductBookingDetails_booking_Subscription$variables = {
  bookingId: string;
};
export type marketplaceProductBookingDetails_booking_Subscription$data = {
  readonly booking: {
    readonly arrearsInvoices: ReadonlyArray<{
      readonly billingPeriodEndExclusive: any;
      readonly billingPeriodStartInclusive: any;
      readonly invoiceNumber: string;
      readonly invoiceUrl: string;
    }>;
    readonly deletedByCustomer: {
      readonly id: string;
    } | null | undefined;
    readonly marketplaceBooking: {
      readonly bookingCheckoutSession: {
        readonly checkoutUrl: string;
      } | null | undefined;
      readonly failure: {
        readonly category: {
          readonly name: string;
          readonly type: string;
        };
        readonly customerAction: {
          readonly name: string;
          readonly type: string;
        };
        readonly finalizedAt: any;
        readonly id: string;
      } | null | undefined;
      readonly id: string;
      readonly invoiceNumber: string | null | undefined;
      readonly invoiceUrl: string | null | undefined;
      readonly isPaymentRequired: boolean;
      readonly paymentExpiry: any;
      readonly paymentStatus: {
        readonly name: string;
        readonly type: PaymentStatus;
      };
      readonly refund: {
        readonly currency: {
          readonly name: string;
          readonly type: Currency;
        } | null | undefined;
        readonly currencyToDisplay: string;
        readonly events: ReadonlyArray<{
          readonly actorName: string | null | undefined;
          readonly currencyToDisplay: string;
          readonly eventType: {
            readonly name: string;
            readonly type: string;
          };
          readonly externalRefundNumber: string | null | undefined;
          readonly id: string;
          readonly lastError: string | null | undefined;
          readonly occurredAt: any;
          readonly reason: string | null | undefined;
          readonly refundAmount: any | null | undefined;
        }>;
        readonly externalRefundNumber: string | null | undefined;
        readonly lastError: string | null | undefined;
        readonly lastProcessedAt: any | null | undefined;
        readonly reason: string | null | undefined;
        readonly refundAmount: any | null | undefined;
        readonly refundPercentage: number;
        readonly requestedAt: any;
        readonly requestedByCustomerName: string | null | undefined;
        readonly status: {
          readonly name: string;
          readonly type: string;
        };
      } | null | undefined;
    } | null | undefined;
  };
};
export type marketplaceProductBookingDetails_booking_Subscription = {
  response: marketplaceProductBookingDetails_booking_Subscription$data;
  variables: marketplaceProductBookingDetails_booking_Subscription$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "bookingId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "bookingId"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "deletedByCustomer",
  "plural": false,
  "selections": [
    (v2/*:: as any*/)
  ],
  "storageKey": null
},
v4 = [
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
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceNumber",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingDetails",
  "kind": "LinkedField",
  "name": "marketplaceBooking",
  "plural": false,
  "selections": [
    (v2/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureDetails",
      "kind": "LinkedField",
      "name": "failure",
      "plural": false,
      "selections": [
        (v2/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceBookingFailureChoiceDetails",
          "kind": "LinkedField",
          "name": "category",
          "plural": false,
          "selections": (v4/*:: as any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "finalizedAt",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceBookingFailureChoiceDetails",
          "kind": "LinkedField",
          "name": "customerAction",
          "plural": false,
          "selections": (v4/*:: as any*/),
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceRefundDetails",
      "kind": "LinkedField",
      "name": "refund",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "CurrencyDetails",
          "kind": "LinkedField",
          "name": "currency",
          "plural": false,
          "selections": (v4/*:: as any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceRefundStatusDetails",
          "kind": "LinkedField",
          "name": "status",
          "plural": false,
          "selections": (v4/*:: as any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "requestedAt",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "lastProcessedAt",
          "storageKey": null
        },
        (v5/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "refundPercentage",
          "storageKey": null
        },
        (v6/*:: as any*/),
        (v7/*:: as any*/),
        (v8/*:: as any*/),
        (v9/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "requestedByCustomerName",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceRefundEventDetails",
          "kind": "LinkedField",
          "name": "events",
          "plural": true,
          "selections": [
            (v2/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "MarketplaceRefundEventTypeDetails",
              "kind": "LinkedField",
              "name": "eventType",
              "plural": false,
              "selections": (v4/*:: as any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "occurredAt",
              "storageKey": null
            },
            (v5/*:: as any*/),
            (v6/*:: as any*/),
            (v7/*:: as any*/),
            (v8/*:: as any*/),
            (v9/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "actorName",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    (v10/*:: as any*/),
    (v11/*:: as any*/),
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
      "selections": (v4/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationArrearsInvoiceDetails",
  "kind": "LinkedField",
  "name": "arrearsInvoices",
  "plural": true,
  "selections": [
    (v11/*:: as any*/),
    (v10/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "billingPeriodStartInclusive",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "billingPeriodEndExclusive",
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
    "name": "marketplaceProductBookingDetails_booking_Subscription",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v3/*:: as any*/),
          (v12/*:: as any*/),
          (v13/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Subscription",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductBookingDetails_booking_Subscription",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v3/*:: as any*/),
          (v12/*:: as any*/),
          (v13/*:: as any*/),
          (v2/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "5e6f567b997b91868002dddf72add1fc",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductBookingDetails_booking_Subscription",
    "operationKind": "subscription",
    "text": "subscription marketplaceProductBookingDetails_booking_Subscription(\n  $bookingId: String!\n) {\n  booking(id: $bookingId) {\n    deletedByCustomer {\n      id\n    }\n    marketplaceBooking {\n      id\n      failure {\n        id\n        category {\n          type\n          name\n        }\n        finalizedAt\n        customerAction {\n          type\n          name\n        }\n      }\n      refund {\n        currency {\n          type\n          name\n        }\n        status {\n          type\n          name\n        }\n        requestedAt\n        lastProcessedAt\n        refundAmount\n        refundPercentage\n        currencyToDisplay\n        reason\n        lastError\n        externalRefundNumber\n        requestedByCustomerName\n        events {\n          id\n          eventType {\n            type\n            name\n          }\n          occurredAt\n          refundAmount\n          currencyToDisplay\n          reason\n          lastError\n          externalRefundNumber\n          actorName\n        }\n      }\n      invoiceUrl\n      invoiceNumber\n      isPaymentRequired\n      paymentExpiry\n      bookingCheckoutSession {\n        checkoutUrl\n      }\n      paymentStatus {\n        type\n        name\n      }\n    }\n    arrearsInvoices {\n      invoiceNumber\n      invoiceUrl\n      billingPeriodStartInclusive\n      billingPeriodEndExclusive\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "7c2aa8ef388deb8df2335015e2e94be2";

export default node;
