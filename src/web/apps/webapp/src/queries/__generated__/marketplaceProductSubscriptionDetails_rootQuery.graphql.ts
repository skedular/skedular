/**
 * @generated SignedSource<<95a06bea32d384f4d0ae184620cb35a8>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type MarketplaceBookingFailureAccountingCleanupStatus = "NOT_REQUIRED" | "PENDING" | "TRANSITION_REQUIRED" | "UNKNOWN" | "%future added value";
export type MarketplaceBookingFailureResourceReleaseStatus = "PENDING" | "RELEASED" | "UNKNOWN" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type MarketplaceRefundEventType = "ACCOUNTING_PROJECTED" | "ACCOUNTING_PROJECTION_REQUIRED" | "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "SENT_TO_XERO" | "UNDER_REVIEW" | "%future added value";
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type marketplaceProductSubscriptionDetails_rootQuery$variables = {
  subscriptionId: string;
};
export type marketplaceProductSubscriptionDetails_rootQuery$data = {
  readonly marketplaceBookingSubscription: {
    readonly arrearsInvoices: ReadonlyArray<{
      readonly billingPeriodEndExclusive: any;
      readonly billingPeriodStartInclusive: any;
      readonly invoiceNumber: string;
      readonly invoiceUrl: string;
    }>;
    readonly autoRenew: boolean;
    readonly cancelAtPeriodEnd: boolean;
    readonly cancellationAvailability: {
      readonly atPeriodEnd: {
        readonly canCancel: boolean;
        readonly isPolicyOverride: boolean;
        readonly requiresReason: boolean;
        readonly unavailableReason: string | null | undefined;
      };
      readonly immediate: {
        readonly canCancel: boolean;
        readonly isPolicyOverride: boolean;
        readonly requiresReason: boolean;
        readonly unavailableReason: string | null | undefined;
      };
    };
    readonly cancellationOverrideReason: string | null | undefined;
    readonly cancellationPolicyOverridden: boolean;
    readonly failure: {
      readonly accountingCleanupStatus: {
        readonly name: string;
        readonly type: MarketplaceBookingFailureAccountingCleanupStatus;
      };
      readonly category: {
        readonly name: string;
        readonly type: string;
      };
      readonly customerAction: {
        readonly name: string;
        readonly type: string;
      };
      readonly id: string;
      readonly resourceReleaseStatus: {
        readonly name: string;
        readonly type: MarketplaceBookingFailureResourceReleaseStatus;
      };
    } | null | undefined;
    readonly id: string;
    readonly involvedCustomers: ReadonlyArray<{
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly id: string;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
    }>;
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
      readonly productVersion: {
        readonly featureImages: ReadonlyArray<{
          readonly original: {
            readonly url: string;
          } | null | undefined;
        }>;
        readonly id: string;
        readonly listingMetadata: {
          readonly about: string | null | undefined;
          readonly includedFeatures: ReadonlyArray<string> | null | undefined;
          readonly subTitle: string | null | undefined;
          readonly title: string | null | undefined;
        };
        readonly organization: {
          readonly customerFacingTermsAndConditionsUrl: string | null | undefined;
        };
      };
      readonly quantity: number;
    };
    readonly nextRenewalAt: any | null | undefined;
    readonly recurringBookings: ReadonlyArray<{
      readonly endDate: any | null | undefined;
      readonly failure: {
        readonly accountingCleanupStatus: {
          readonly name: string;
          readonly type: MarketplaceBookingFailureAccountingCleanupStatus;
        };
        readonly category: {
          readonly name: string;
          readonly type: string;
        };
        readonly customerAction: {
          readonly name: string;
          readonly type: string;
        };
        readonly id: string;
        readonly resourceReleaseStatus: {
          readonly name: string;
          readonly type: MarketplaceBookingFailureResourceReleaseStatus;
        };
      } | null | undefined;
      readonly id: string;
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
        readonly productVersion: {
          readonly featureImages: ReadonlyArray<{
            readonly original: {
              readonly url: string;
            } | null | undefined;
          }>;
          readonly id: string;
          readonly listingMetadata: {
            readonly about: string | null | undefined;
            readonly includedFeatures: ReadonlyArray<string> | null | undefined;
            readonly subTitle: string | null | undefined;
            readonly title: string | null | undefined;
          };
          readonly organization: {
            readonly customerFacingTermsAndConditionsUrl: string | null | undefined;
          };
        };
        readonly quantity: number;
      } | null | undefined;
      readonly startDate: any;
    }>;
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
        readonly type: MarketplaceRefundStatus;
      };
    } | null | undefined;
    readonly startedAt: any;
    readonly status: {
      readonly name: string;
      readonly type: MarketplaceBookingSubscriptionStatus;
    };
    readonly weeklySelectedDays: ReadonlyArray<DayOfWeek>;
  } | null | undefined;
  readonly marketplaceBookingSubscriptionCancellationModes: ReadonlyArray<{
    readonly name: string;
    readonly type: MarketplaceBookingSubscriptionCancellationMode;
  }>;
  readonly marketplaceBookingSubscriptionRefundPreview: {
    readonly baseAmount: any | null | undefined;
    readonly currencyToDisplay: string;
    readonly isRefundable: boolean;
    readonly refundAmount: any | null | undefined;
    readonly refundPercentage: number;
  };
};
export type marketplaceProductSubscriptionDetails_rootQuery = {
  response: marketplaceProductSubscriptionDetails_rootQuery$data;
  variables: marketplaceProductSubscriptionDetails_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "subscriptionId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
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
  (v1/*:: as any*/)
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundPercentage",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v7 = [
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
  }
],
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingFailureDetails",
  "kind": "LinkedField",
  "name": "failure",
  "plural": false,
  "selections": [
    (v6/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureChoiceDetails",
      "kind": "LinkedField",
      "name": "category",
      "plural": false,
      "selections": (v2/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureChoiceDetails",
      "kind": "LinkedField",
      "name": "customerAction",
      "plural": false,
      "selections": (v2/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureResourceReleaseStatusDetails",
      "kind": "LinkedField",
      "name": "resourceReleaseStatus",
      "plural": false,
      "selections": (v2/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureAccountingCleanupStatusDetails",
      "kind": "LinkedField",
      "name": "accountingCleanupStatus",
      "plural": false,
      "selections": (v2/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingDetails",
  "kind": "LinkedField",
  "name": "marketplaceBooking",
  "plural": false,
  "selections": [
    (v6/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "quantity",
      "storageKey": null
    },
    (v9/*:: as any*/),
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
      "concreteType": "ProductVersionDetails",
      "kind": "LinkedField",
      "name": "productVersion",
      "plural": false,
      "selections": [
        (v6/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "ListingMetadata",
          "kind": "LinkedField",
          "name": "listingMetadata",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "title",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "subTitle",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "about",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "includedFeatures",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "CdnImageFile",
          "kind": "LinkedField",
          "name": "featureImages",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "CdnFile",
              "kind": "LinkedField",
              "name": "original",
              "plural": false,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "url",
                  "storageKey": null
                }
              ],
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Marketplace_OrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "customerFacingTermsAndConditionsUrl",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
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
      "concreteType": "PaymentMethodTypeDetails",
      "kind": "LinkedField",
      "name": "paymentMethod",
      "plural": false,
      "selections": (v2/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "PaymentStatusDetails",
      "kind": "LinkedField",
      "name": "paymentStatus",
      "plural": false,
      "selections": (v2/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v14 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
    "kind": "LinkedField",
    "name": "marketplaceBookingSubscriptionCancellationModes",
    "plural": true,
    "selections": (v2/*:: as any*/),
    "storageKey": null
  },
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "subscriptionId",
        "variableName": "subscriptionId"
      }
    ],
    "concreteType": "MarketplaceRefundPreviewDetails",
    "kind": "LinkedField",
    "name": "marketplaceBookingSubscriptionRefundPreview",
    "plural": false,
    "selections": [
      (v3/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "baseAmount",
        "storageKey": null
      },
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "isRefundable",
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "subscriptionId"
      }
    ],
    "concreteType": "MarketplaceBookingSubscriptionDetails",
    "kind": "LinkedField",
    "name": "marketplaceBookingSubscription",
    "plural": false,
    "selections": [
      (v6/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "cancellationPolicyOverridden",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "cancellationOverrideReason",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceSubscriptionCancellationAvailabilityDetails",
        "kind": "LinkedField",
        "name": "cancellationAvailability",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceCancellationAvailabilityDetails",
            "kind": "LinkedField",
            "name": "immediate",
            "plural": false,
            "selections": (v7/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceCancellationAvailabilityDetails",
            "kind": "LinkedField",
            "name": "atPeriodEnd",
            "plural": false,
            "selections": (v7/*:: as any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      (v8/*:: as any*/),
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
        "kind": "ScalarField",
        "name": "cancelAtPeriodEnd",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "weeklySelectedDays",
        "storageKey": null
      },
      (v10/*:: as any*/),
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
            "selections": (v2/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceRefundStatusDetails",
            "kind": "LinkedField",
            "name": "status",
            "plural": false,
            "selections": (v2/*:: as any*/),
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
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v11/*:: as any*/),
          (v12/*:: as any*/),
          (v13/*:: as any*/),
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
              (v6/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceRefundEventTypeDetails",
                "kind": "LinkedField",
                "name": "eventType",
                "plural": false,
                "selections": (v2/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "occurredAt",
                "storageKey": null
              },
              (v3/*:: as any*/),
              (v5/*:: as any*/),
              (v11/*:: as any*/),
              (v12/*:: as any*/),
              (v13/*:: as any*/),
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
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
        "kind": "LinkedField",
        "name": "status",
        "plural": false,
        "selections": (v2/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "involvedCustomers",
        "plural": true,
        "selections": [
          (v6/*:: as any*/),
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "givenName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "middleName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "familyName",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "RecurringBookingDetails",
        "kind": "LinkedField",
        "name": "recurringBookings",
        "plural": true,
        "selections": [
          (v6/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "startDate",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "endDate",
            "storageKey": null
          },
          (v8/*:: as any*/),
          (v10/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationArrearsInvoiceDetails",
        "kind": "LinkedField",
        "name": "arrearsInvoices",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "invoiceNumber",
            "storageKey": null
          },
          (v9/*:: as any*/),
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
    "name": "marketplaceProductSubscriptionDetails_rootQuery",
    "selections": (v14/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductSubscriptionDetails_rootQuery",
    "selections": (v14/*:: as any*/)
  },
  "params": {
    "cacheID": "f8f73a371d2ad7edd3e1f2452adb55d8",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductSubscriptionDetails_rootQuery",
    "operationKind": "query",
    "text": "query marketplaceProductSubscriptionDetails_rootQuery(\n  $subscriptionId: String!\n) {\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  marketplaceBookingSubscriptionRefundPreview(subscriptionId: $subscriptionId) {\n    refundAmount\n    baseAmount\n    refundPercentage\n    currencyToDisplay\n    isRefundable\n  }\n  marketplaceBookingSubscription(id: $subscriptionId) {\n    id\n    cancellationPolicyOverridden\n    cancellationOverrideReason\n    cancellationAvailability {\n      immediate {\n        canCancel\n        requiresReason\n        isPolicyOverride\n        unavailableReason\n      }\n      atPeriodEnd {\n        canCancel\n        requiresReason\n        isPolicyOverride\n        unavailableReason\n      }\n    }\n    failure {\n      id\n      category {\n        type\n        name\n      }\n      customerAction {\n        type\n        name\n      }\n      resourceReleaseStatus {\n        type\n        name\n      }\n      accountingCleanupStatus {\n        type\n        name\n      }\n    }\n    startedAt\n    nextRenewalAt\n    autoRenew\n    cancelAtPeriodEnd\n    weeklySelectedDays\n    marketplaceBooking {\n      id\n      quantity\n      invoiceUrl\n      isPaymentRequired\n      paymentExpiry\n      productVersion {\n        id\n        listingMetadata {\n          title\n          subTitle\n          about\n          includedFeatures\n        }\n        featureImages {\n          original {\n            url\n          }\n        }\n        organization {\n          customerFacingTermsAndConditionsUrl\n        }\n      }\n      bookingCheckoutSession {\n        checkoutUrl\n      }\n      paymentMethod {\n        type\n        name\n      }\n      paymentStatus {\n        type\n        name\n      }\n    }\n    refund {\n      currency {\n        type\n        name\n      }\n      status {\n        type\n        name\n      }\n      requestedAt\n      lastProcessedAt\n      refundAmount\n      refundPercentage\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n      requestedByCustomerName\n      events {\n        id\n        eventType {\n          type\n          name\n        }\n        occurredAt\n        refundAmount\n        currencyToDisplay\n        reason\n        lastError\n        externalRefundNumber\n        actorName\n      }\n    }\n    status {\n      type\n      name\n    }\n    involvedCustomers {\n      id\n      name\n      givenName\n      middleName\n      familyName\n    }\n    recurringBookings {\n      id\n      startDate\n      endDate\n      failure {\n        id\n        category {\n          type\n          name\n        }\n        customerAction {\n          type\n          name\n        }\n        resourceReleaseStatus {\n          type\n          name\n        }\n        accountingCleanupStatus {\n          type\n          name\n        }\n      }\n      marketplaceBooking {\n        id\n        quantity\n        invoiceUrl\n        isPaymentRequired\n        paymentExpiry\n        productVersion {\n          id\n          listingMetadata {\n            title\n            subTitle\n            about\n            includedFeatures\n          }\n          featureImages {\n            original {\n              url\n            }\n          }\n          organization {\n            customerFacingTermsAndConditionsUrl\n          }\n        }\n        bookingCheckoutSession {\n          checkoutUrl\n        }\n        paymentMethod {\n          type\n          name\n        }\n        paymentStatus {\n          type\n          name\n        }\n      }\n    }\n    arrearsInvoices {\n      invoiceNumber\n      invoiceUrl\n      billingPeriodStartInclusive\n      billingPeriodEndExclusive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "58945fcb516e5e43565fb98dcad8ecf1";

export default node;
