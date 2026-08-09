/**
 * @generated SignedSource<<9c3fee48df65e4c598b45807160b23de>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type Currency = "NZD" | "USD" | "%future added value";
export type MarketplaceBookingModificationActorKind = "CUSTOMER" | "ORGANIZATION_OPERATOR" | "%future added value";
export type MarketplaceRefundEventType = "ACCOUNTING_PROJECTED" | "ACCOUNTING_PROJECTION_REQUIRED" | "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "SENT_TO_XERO" | "UNDER_REVIEW" | "%future added value";
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type ProductType = "EVENT" | "RESOURCE" | "%future added value";
export type marketplaceProductBookingDetails_rootQuery$variables = {
  bookingId: string;
};
export type marketplaceProductBookingDetails_rootQuery$data = {
  readonly booking: {
    readonly arrearsInvoices: ReadonlyArray<{
      readonly billingPeriodEndExclusive: any;
      readonly billingPeriodStartInclusive: any;
      readonly invoiceNumber: string;
      readonly invoiceUrl: string;
    }>;
    readonly bookingResources: ReadonlyArray<{
      readonly resource: {
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly cancellationAvailability: {
      readonly canCancel: boolean;
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
    readonly id: string;
    readonly involvedCustomers: ReadonlyArray<{
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly id: string;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
    }>;
    readonly involvedLocations: ReadonlyArray<{
      readonly name: string;
      readonly uniqueId: string;
    }>;
    readonly marketplaceBooking: {
      readonly bookingCheckoutSession: {
        readonly checkoutUrl: string;
      } | null | undefined;
      readonly failure: {
        readonly allocatedRefundAmount: any | null | undefined;
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
        readonly resolutionDeadlineAt: any | null | undefined;
        readonly resolutionDecision: string | null | undefined;
      } | null | undefined;
      readonly id: string;
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
      readonly productVersion: {
        readonly featureImages: ReadonlyArray<{
          readonly original: {
            readonly url: string;
          } | null | undefined;
        }>;
        readonly listingMetadata: {
          readonly about: string | null | undefined;
          readonly includedFeatures: ReadonlyArray<string> | null | undefined;
          readonly subTitle: string | null | undefined;
          readonly title: string | null | undefined;
        };
        readonly type: {
          readonly name: string;
          readonly type: ProductType;
        };
      };
      readonly quantity: number;
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
    readonly recurringBooking: {
      readonly marketplaceBooking: {
        readonly paymentStatus: {
          readonly type: PaymentStatus;
        };
      } | null | undefined;
    } | null | undefined;
    readonly until: any;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"modifyMarketplaceBookingDialog_query">;
};
export type marketplaceProductBookingDetails_rootQuery = {
  response: marketplaceProductBookingDetails_rootQuery$data;
  variables: marketplaceProductBookingDetails_rootQuery$variables;
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
  "kind": "ScalarField",
  "name": "entityFrameworkVersion",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
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
    (v2/*:: as any*/)
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
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "involvedCustomers",
  "plural": true,
  "selections": [
    (v2/*:: as any*/),
    (v10/*:: as any*/),
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
v12 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v10/*:: as any*/)
],
v13 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_LocationDetails",
  "kind": "LinkedField",
  "name": "involvedLocations",
  "plural": true,
  "selections": (v12/*:: as any*/),
  "storageKey": null
},
v14 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "ResourceDetails",
    "kind": "LinkedField",
    "name": "resource",
    "plural": false,
    "selections": [
      (v2/*:: as any*/),
      (v10/*:: as any*/)
    ],
    "storageKey": null
  }
],
v15 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingResourceDetails",
  "kind": "LinkedField",
  "name": "bookingResources",
  "plural": true,
  "selections": (v14/*:: as any*/),
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": [
    (v16/*:: as any*/)
  ],
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v19 = [
  (v16/*:: as any*/),
  (v10/*:: as any*/)
],
v20 = {
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
      "selections": (v19/*:: as any*/),
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
      "selections": (v19/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "resolutionDeadlineAt",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "resolutionDecision",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "allocatedRefundAmount",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "occurredAt",
  "storageKey": null
},
v27 = {
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
      "selections": (v19/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceRefundStatusDetails",
      "kind": "LinkedField",
      "name": "status",
      "plural": false,
      "selections": (v19/*:: as any*/),
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
    (v21/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "refundPercentage",
      "storageKey": null
    },
    (v22/*:: as any*/),
    (v23/*:: as any*/),
    (v24/*:: as any*/),
    (v25/*:: as any*/),
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
          "selections": (v19/*:: as any*/),
          "storageKey": null
        },
        (v26/*:: as any*/),
        (v21/*:: as any*/),
        (v22/*:: as any*/),
        (v23/*:: as any*/),
        (v24/*:: as any*/),
        (v25/*:: as any*/),
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
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v29 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceNumber",
  "storageKey": null
},
v30 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isPaymentRequired",
  "storageKey": null
},
v31 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentExpiry",
  "storageKey": null
},
v32 = {
  "alias": null,
  "args": null,
  "concreteType": "ProductTypeDetails",
  "kind": "LinkedField",
  "name": "type",
  "plural": false,
  "selections": (v19/*:: as any*/),
  "storageKey": null
},
v33 = {
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
v34 = {
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
v35 = {
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
v36 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v19/*:: as any*/),
  "storageKey": null
},
v37 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v19/*:: as any*/),
  "storageKey": null
},
v38 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingModificationDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingModifications",
  "plural": true,
  "selections": [
    (v2/*:: as any*/),
    (v26/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "actorKind",
      "storageKey": null
    },
    (v23/*:: as any*/),
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
v39 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationArrearsInvoiceDetails",
  "kind": "LinkedField",
  "name": "arrearsInvoices",
  "plural": true,
  "selections": [
    (v29/*:: as any*/),
    (v28/*:: as any*/),
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
    "name": "marketplaceProductBookingDetails_rootQuery",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "bookingId",
            "variableName": "bookingId"
          }
        ],
        "kind": "FragmentSpread",
        "name": "modifyMarketplaceBookingDialog_query"
      },
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
          (v5/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/),
          (v9/*:: as any*/),
          (v11/*:: as any*/),
          (v13/*:: as any*/),
          (v15/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "RecurringBookingDetails",
            "kind": "LinkedField",
            "name": "recurringBooking",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingDetails",
                "kind": "LinkedField",
                "name": "marketplaceBooking",
                "plural": false,
                "selections": [
                  (v17/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
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
              (v2/*:: as any*/),
              (v18/*:: as any*/),
              (v20/*:: as any*/),
              (v27/*:: as any*/),
              (v28/*:: as any*/),
              (v29/*:: as any*/),
              (v30/*:: as any*/),
              (v31/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductVersionDetails",
                "kind": "LinkedField",
                "name": "productVersion",
                "plural": false,
                "selections": [
                  (v32/*:: as any*/),
                  (v33/*:: as any*/),
                  (v34/*:: as any*/)
                ],
                "storageKey": null
              },
              (v35/*:: as any*/),
              (v36/*:: as any*/),
              (v37/*:: as any*/)
            ],
            "storageKey": null
          },
          (v38/*:: as any*/),
          (v39/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductBookingDetails_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingResourceSelectionDetails",
            "kind": "LinkedField",
            "name": "marketplaceBookingResourceSelection",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "canSelectResources",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "maximumResourceCount",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "availableResourceIds",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "Booking_LocationDetails",
                "kind": "LinkedField",
                "name": "eligibleLocations",
                "plural": true,
                "selections": (v12/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingResourceDetails",
                "kind": "LinkedField",
                "name": "eligibleResources",
                "plural": true,
                "selections": (v14/*:: as any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/),
          (v9/*:: as any*/),
          (v11/*:: as any*/),
          (v13/*:: as any*/),
          (v15/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "RecurringBookingDetails",
            "kind": "LinkedField",
            "name": "recurringBooking",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingDetails",
                "kind": "LinkedField",
                "name": "marketplaceBooking",
                "plural": false,
                "selections": [
                  (v17/*:: as any*/),
                  (v2/*:: as any*/)
                ],
                "storageKey": null
              },
              (v2/*:: as any*/)
            ],
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
              (v2/*:: as any*/),
              (v18/*:: as any*/),
              (v20/*:: as any*/),
              (v27/*:: as any*/),
              (v28/*:: as any*/),
              (v29/*:: as any*/),
              (v30/*:: as any*/),
              (v31/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductVersionDetails",
                "kind": "LinkedField",
                "name": "productVersion",
                "plural": false,
                "selections": [
                  (v32/*:: as any*/),
                  (v33/*:: as any*/),
                  (v34/*:: as any*/),
                  (v2/*:: as any*/)
                ],
                "storageKey": null
              },
              (v35/*:: as any*/),
              (v36/*:: as any*/),
              (v37/*:: as any*/)
            ],
            "storageKey": null
          },
          (v38/*:: as any*/),
          (v39/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "8496982d2246f663d52c1bc5719c9628",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductBookingDetails_rootQuery",
    "operationKind": "query",
    "text": "query marketplaceProductBookingDetails_rootQuery(\n  $bookingId: String!\n) {\n  ...modifyMarketplaceBookingDialog_query_378Z3H\n  booking(id: $bookingId) {\n    id\n    entityFrameworkVersion\n    from\n    until\n    deletedByCustomer {\n      id\n    }\n    cancellationAvailability {\n      canCancel\n      requiresReason\n      isPolicyOverride\n      unavailableReason\n    }\n    cancellationPolicyOverridden\n    cancellationOverrideReason\n    involvedCustomers {\n      id\n      name\n      givenName\n      middleName\n      familyName\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    bookingResources {\n      resource {\n        id\n        name\n      }\n    }\n    recurringBooking {\n      marketplaceBooking {\n        paymentStatus {\n          type\n        }\n        id\n      }\n      id\n    }\n    marketplaceBooking {\n      id\n      quantity\n      failure {\n        id\n        category {\n          type\n          name\n        }\n        finalizedAt\n        customerAction {\n          type\n          name\n        }\n        resolutionDeadlineAt\n        resolutionDecision\n        allocatedRefundAmount\n      }\n      refund {\n        currency {\n          type\n          name\n        }\n        status {\n          type\n          name\n        }\n        requestedAt\n        lastProcessedAt\n        refundAmount\n        refundPercentage\n        currencyToDisplay\n        reason\n        lastError\n        externalRefundNumber\n        requestedByCustomerName\n        events {\n          id\n          eventType {\n            type\n            name\n          }\n          occurredAt\n          refundAmount\n          currencyToDisplay\n          reason\n          lastError\n          externalRefundNumber\n          actorName\n          previousStatus\n          newStatus\n        }\n      }\n      invoiceUrl\n      invoiceNumber\n      isPaymentRequired\n      paymentExpiry\n      productVersion {\n        type {\n          type\n          name\n        }\n        listingMetadata {\n          title\n          subTitle\n          about\n          includedFeatures\n        }\n        featureImages {\n          original {\n            url\n          }\n        }\n        id\n      }\n      bookingCheckoutSession {\n        checkoutUrl\n      }\n      paymentMethod {\n        type\n        name\n      }\n      paymentStatus {\n        type\n        name\n      }\n    }\n    marketplaceBookingModifications {\n      id\n      occurredAt\n      actorKind\n      reason\n      originalFrom\n      originalUntil\n      resultFrom\n      resultUntil\n      originalResourceNames\n      resultResourceNames\n    }\n    arrearsInvoices {\n      invoiceNumber\n      invoiceUrl\n      billingPeriodStartInclusive\n      billingPeriodEndExclusive\n    }\n  }\n}\n\nfragment modifyMarketplaceBookingDialog_query_378Z3H on Query {\n  booking(id: $bookingId) {\n    marketplaceBookingResourceSelection {\n      canSelectResources\n      maximumResourceCount\n      availableResourceIds\n      eligibleLocations {\n        uniqueId\n        name\n      }\n      eligibleResources {\n        resource {\n          id\n          name\n        }\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "513e5a94eb309a654837bbd7ad0afa34";

export default node;
