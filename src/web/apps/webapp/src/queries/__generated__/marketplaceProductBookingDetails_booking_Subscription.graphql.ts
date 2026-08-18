/**
 * @generated SignedSource<<a36c583ec515c74f273a798b7c199ff5>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type MarketplaceBookingModificationActorKind = "CUSTOMER" | "ORGANIZATION_OPERATOR" | "%future added value";
export type MarketplaceRefundEventType = "ACCOUNTING_PROJECTED" | "ACCOUNTING_PROJECTION_REQUIRED" | "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "SENT_TO_XERO" | "UNDER_REVIEW" | "%future added value";
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
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
    readonly cancellationAvailability: {
      readonly canCancel: boolean;
      readonly creditOutcome: string | null | undefined;
      readonly isCreditFunded: boolean;
      readonly isPolicyOverride: boolean;
      readonly requiresReason: boolean;
      readonly unavailableReason: string | null | undefined;
    };
    readonly cancellationOverrideReason: string | null | undefined;
    readonly cancellationPolicyOverridden: boolean;
    readonly deletedByCustomer: {
      readonly id: string;
    } | null | undefined;
    readonly entityFrameworkVersion: any;
    readonly from: any;
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
            readonly type: MarketplaceRefundEventType;
          };
          readonly externalRefundNumber: string | null | undefined;
          readonly id: string;
          readonly lastError: string | null | undefined;
          readonly newStatus: string | null | undefined;
          readonly occurredAt: any;
          readonly previousStatus: string | null | undefined;
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
          readonly type: MarketplaceRefundStatus;
        };
      } | null | undefined;
    } | null | undefined;
    readonly marketplaceBookingModifications: ReadonlyArray<{
      readonly actorKind: MarketplaceBookingModificationActorKind;
      readonly id: string;
      readonly occurredAt: any;
      readonly originalFrom: any;
      readonly originalResourceNames: ReadonlyArray<string>;
      readonly originalUntil: any;
      readonly reason: string | null | undefined;
      readonly resultFrom: any;
      readonly resultResourceNames: ReadonlyArray<string>;
      readonly resultUntil: any;
    }>;
    readonly until: any;
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
  "name": "entityFrameworkVersion",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "deletedByCustomer",
  "plural": false,
  "selections": [
    (v5/*:: as any*/)
  ],
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceCancellationAvailabilityDetails",
  "kind": "LinkedField",
  "name": "cancellationAvailability",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canCancel",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "requiresReason",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "isPolicyOverride",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "unavailableReason",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "isCreditFunded",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "creditOutcome",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancellationPolicyOverridden",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancellationOverrideReason",
  "storageKey": null
},
v10 = [
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
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "occurredAt",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceNumber",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingDetails",
  "kind": "LinkedField",
  "name": "marketplaceBooking",
  "plural": false,
  "selections": [
    (v5/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureDetails",
      "kind": "LinkedField",
      "name": "failure",
      "plural": false,
      "selections": [
        (v5/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceBookingFailureChoiceDetails",
          "kind": "LinkedField",
          "name": "category",
          "plural": false,
          "selections": (v10/*:: as any*/),
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
          "selections": (v10/*:: as any*/),
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
          "selections": (v10/*:: as any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceRefundStatusDetails",
          "kind": "LinkedField",
          "name": "status",
          "plural": false,
          "selections": (v10/*:: as any*/),
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
        (v11/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "refundPercentage",
          "storageKey": null
        },
        (v12/*:: as any*/),
        (v13/*:: as any*/),
        (v14/*:: as any*/),
        (v15/*:: as any*/),
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
            (v5/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "MarketplaceRefundEventTypeDetails",
              "kind": "LinkedField",
              "name": "eventType",
              "plural": false,
              "selections": (v10/*:: as any*/),
              "storageKey": null
            },
            (v16/*:: as any*/),
            (v11/*:: as any*/),
            (v12/*:: as any*/),
            (v13/*:: as any*/),
            (v14/*:: as any*/),
            (v15/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "actorName",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "previousStatus",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "newStatus",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    (v17/*:: as any*/),
    (v18/*:: as any*/),
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
      "selections": (v10/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingModificationDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingModifications",
  "plural": true,
  "selections": [
    (v5/*:: as any*/),
    (v16/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "actorKind",
      "storageKey": null
    },
    (v13/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "originalFrom",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "originalUntil",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "resultFrom",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "resultUntil",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "originalResourceNames",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "resultResourceNames",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationArrearsInvoiceDetails",
  "kind": "LinkedField",
  "name": "arrearsInvoices",
  "plural": true,
  "selections": [
    (v18/*:: as any*/),
    (v17/*:: as any*/),
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
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/),
          (v9/*:: as any*/),
          (v19/*:: as any*/),
          (v20/*:: as any*/),
          (v21/*:: as any*/)
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
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/),
          (v9/*:: as any*/),
          (v19/*:: as any*/),
          (v20/*:: as any*/),
          (v21/*:: as any*/),
          (v5/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "ee0c94a6879e1bc3d4529de7b923a586",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductBookingDetails_booking_Subscription",
    "operationKind": "subscription",
    "text": "subscription marketplaceProductBookingDetails_booking_Subscription(\n  $bookingId: String!\n) {\n  booking(id: $bookingId) {\n    entityFrameworkVersion\n    from\n    until\n    deletedByCustomer {\n      id\n    }\n    cancellationAvailability {\n      canCancel\n      requiresReason\n      isPolicyOverride\n      unavailableReason\n      isCreditFunded\n      creditOutcome\n    }\n    cancellationPolicyOverridden\n    cancellationOverrideReason\n    marketplaceBooking {\n      id\n      failure {\n        id\n        category {\n          type\n          name\n        }\n        finalizedAt\n        customerAction {\n          type\n          name\n        }\n      }\n      refund {\n        currency {\n          type\n          name\n        }\n        status {\n          type\n          name\n        }\n        requestedAt\n        lastProcessedAt\n        refundAmount\n        refundPercentage\n        currencyToDisplay\n        reason\n        lastError\n        externalRefundNumber\n        requestedByCustomerName\n        events {\n          id\n          eventType {\n            type\n            name\n          }\n          occurredAt\n          refundAmount\n          currencyToDisplay\n          reason\n          lastError\n          externalRefundNumber\n          actorName\n          previousStatus\n          newStatus\n        }\n      }\n      invoiceUrl\n      invoiceNumber\n      isPaymentRequired\n      paymentExpiry\n      bookingCheckoutSession {\n        checkoutUrl\n      }\n      paymentStatus {\n        type\n        name\n      }\n    }\n    marketplaceBookingModifications {\n      id\n      occurredAt\n      actorKind\n      reason\n      originalFrom\n      originalUntil\n      resultFrom\n      resultUntil\n      originalResourceNames\n      resultResourceNames\n    }\n    arrearsInvoices {\n      invoiceNumber\n      invoiceUrl\n      billingPeriodStartInclusive\n      billingPeriodEndExclusive\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "a2fee66f61991727e1183472f1268c2e";

export default node;
