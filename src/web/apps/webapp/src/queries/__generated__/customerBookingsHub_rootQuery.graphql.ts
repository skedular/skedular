/**
 * @generated SignedSource<<b36aed4242b90a6d2aae2ba7b32973a3>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingChannel = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type Currency = "NZD" | "USD" | "%future added value";
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type MarketplacePurchaseSourceType = "BOOKING" | "ENTITLEMENT" | "SUBSCRIPTION" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type customerBookingsHub_rootQuery$variables = {
  organizationCustomDomain: string;
  today: any;
};
export type customerBookingsHub_rootQuery$data = {
  readonly entitlementPurchases: ReadonlyArray<{
    readonly amount: any;
    readonly creditQuantity: number;
    readonly currency: string;
    readonly id: string;
    readonly invoiceNumber: string | null | undefined;
    readonly paymentExpiry: any;
    readonly paymentMethod: string;
    readonly paymentStatus: string;
  }>;
  readonly marketplaceBookingFailures: ReadonlyArray<{
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
    readonly requestedFrom: any | null | undefined;
    readonly requestedUntil: any | null | undefined;
    readonly scope: {
      readonly name: string;
      readonly type: string;
    };
  }>;
  readonly marketplaceBookingSubscriptionCancellationModes: ReadonlyArray<{
    readonly name: string;
    readonly type: MarketplaceBookingSubscriptionCancellationMode;
  }>;
  readonly marketplaceBookingSubscriptions: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly autoRenew: boolean;
        readonly cancelAtPeriodEnd: boolean;
        readonly cancellationOverrideReason: string | null | undefined;
        readonly cancellationPolicyOverridden: boolean;
        readonly id: string;
        readonly involvedOrganizations: ReadonlyArray<{
          readonly customDomain: string | null | undefined;
          readonly id: string;
          readonly name: string;
        }>;
        readonly involvedTeams: ReadonlyArray<{
          readonly id: string;
          readonly name: string;
        }>;
        readonly marketplaceBooking: {
          readonly paymentMethod: {
            readonly name: string;
          };
          readonly paymentStatus: {
            readonly name: string;
            readonly type: PaymentStatus;
          };
          readonly productVersion: {
            readonly listingMetadata: {
              readonly subTitle: string | null | undefined;
              readonly title: string | null | undefined;
            };
          };
          readonly quantity: number;
        };
        readonly nextRenewalAt: any | null | undefined;
        readonly recurringBookings: ReadonlyArray<{
          readonly endDate: any | null | undefined;
          readonly id: string;
          readonly startDate: any;
        }>;
        readonly startedAt: any;
        readonly status: {
          readonly name: string;
          readonly type: MarketplaceBookingSubscriptionStatus;
        };
      };
    }>;
    readonly totalCount: number;
  };
  readonly marketplacePurchases: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly activityAt: any;
        readonly bookingFrom: any | null | undefined;
        readonly bookingId: string | null | undefined;
        readonly bookingUntil: any | null | undefined;
        readonly creditQuantity: number;
        readonly currency: Currency | null | undefined;
        readonly id: string;
        readonly isDeleted: boolean;
        readonly lifecycleStateName: string;
        readonly paymentStatus: PaymentStatus;
        readonly productTitle: string | null | undefined;
        readonly refund: {
          readonly currencyToDisplay: string;
          readonly refundAmount: any | null | undefined;
          readonly status: {
            readonly name: string;
          };
        } | null | undefined;
        readonly renewalStateName: string;
        readonly sourceId: string;
        readonly sourceType: MarketplacePurchaseSourceType;
        readonly sourceTypeName: string;
        readonly totalAmount: any | null | undefined;
      };
    }>;
  };
  readonly myEntitlements: ReadonlyArray<{
    readonly availableQuantity: number;
    readonly expiresAt: any;
    readonly grantedQuantity: number;
    readonly id: string;
    readonly pricingId: string;
    readonly purchaseReference: string;
    readonly restrictions: {
      readonly availableDays: ReadonlyArray<DayOfWeek>;
      readonly productId: string;
      readonly productVersionId: string;
    } | null | undefined;
    readonly status: EntitlementStatus;
  }>;
  readonly recentBookings: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly bookingResources: ReadonlyArray<{
          readonly resource: {
            readonly id: string;
            readonly name: string;
          };
        }>;
        readonly cancellationOverrideReason: string | null | undefined;
        readonly cancellationPolicyOverridden: boolean;
        readonly channel: {
          readonly channel: BookingChannel;
          readonly name: string;
        };
        readonly from: any;
        readonly id: string;
        readonly involvedLocations: ReadonlyArray<{
          readonly name: string;
        }>;
        readonly involvedOrganizations: ReadonlyArray<{
          readonly customDomain: string | null | undefined;
          readonly id: string;
          readonly name: string;
        }>;
        readonly involvedTeams: ReadonlyArray<{
          readonly id: string;
          readonly name: string;
        }>;
        readonly marketplaceBooking: {
          readonly paymentStatus: {
            readonly name: string;
            readonly type: PaymentStatus;
          };
          readonly quantity: number;
        } | null | undefined;
        readonly recurringBooking: {
          readonly frequency: {
            readonly name: string;
          };
          readonly id: string;
          readonly marketplaceBooking: {
            readonly id: string;
          } | null | undefined;
        } | null | undefined;
        readonly until: any;
      };
    }>;
    readonly totalCount: number;
  };
  readonly upcomingBookings: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly bookingResources: ReadonlyArray<{
          readonly resource: {
            readonly id: string;
            readonly name: string;
          };
        }>;
        readonly cancellationOverrideReason: string | null | undefined;
        readonly cancellationPolicyOverridden: boolean;
        readonly channel: {
          readonly channel: BookingChannel;
          readonly name: string;
        };
        readonly from: any;
        readonly id: string;
        readonly involvedLocations: ReadonlyArray<{
          readonly name: string;
        }>;
        readonly involvedOrganizations: ReadonlyArray<{
          readonly customDomain: string | null | undefined;
          readonly id: string;
          readonly name: string;
        }>;
        readonly involvedTeams: ReadonlyArray<{
          readonly id: string;
          readonly name: string;
        }>;
        readonly marketplaceBooking: {
          readonly paymentStatus: {
            readonly name: string;
            readonly type: PaymentStatus;
          };
          readonly quantity: number;
        } | null | undefined;
        readonly recurringBooking: {
          readonly frequency: {
            readonly name: string;
          };
          readonly id: string;
          readonly marketplaceBooking: {
            readonly id: string;
          } | null | undefined;
        } | null | undefined;
        readonly until: any;
      };
    }>;
    readonly totalCount: number;
  };
};
export type customerBookingsHub_rootQuery = {
  response: customerBookingsHub_rootQuery$data;
  variables: customerBookingsHub_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "today"
},
v2 = {
  "kind": "Literal",
  "name": "first",
  "value": 48
},
v3 = {
  "kind": "Literal",
  "name": "includeMineOnly",
  "value": true
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currency",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "creditQuantity",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentStatus",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v9 = [
  (v8/*:: as any*/)
],
v10 = {
  "alias": null,
  "args": [
    (v2/*:: as any*/),
    {
      "kind": "Literal",
      "name": "orderBy",
      "value": [
        {
          "direction": "DESCENDING",
          "field": "ACTIVITY_AT"
        }
      ]
    },
    {
      "fields": [
        (v3/*:: as any*/),
        {
          "kind": "Literal",
          "name": "lifecycleStates",
          "value": [
            "CANCELLED",
            "DELETED",
            "EXPIRED",
            "PAYMENT_FAILED"
          ]
        },
        {
          "kind": "Variable",
          "name": "organizationCustomDomain",
          "variableName": "organizationCustomDomain"
        }
      ],
      "kind": "ObjectValue",
      "name": "where"
    }
  ],
  "concreteType": "ConnectionOfMarketplacePurchaseHistoryEdge",
  "kind": "LinkedField",
  "name": "marketplacePurchases",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplacePurchaseHistoryEdge",
      "kind": "LinkedField",
      "name": "edges",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplacePurchaseHistoryDetails",
          "kind": "LinkedField",
          "name": "node",
          "plural": false,
          "selections": [
            (v4/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "sourceId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "sourceType",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "sourceTypeName",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "lifecycleStateName",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "renewalStateName",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "activityAt",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "bookingFrom",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "bookingUntil",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "productTitle",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "totalAmount",
              "storageKey": null
            },
            (v5/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "bookingId",
              "storageKey": null
            },
            (v6/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "isDeleted",
              "storageKey": null
            },
            (v7/*:: as any*/),
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
                  "concreteType": "MarketplaceRefundStatusDetails",
                  "kind": "LinkedField",
                  "name": "status",
                  "plural": false,
                  "selections": (v9/*:: as any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "refundAmount",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "currencyToDisplay",
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
  ],
  "storageKey": null
},
v11 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v8/*:: as any*/)
],
v12 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingFailureDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingFailures",
  "plural": true,
  "selections": [
    (v4/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureChoiceDetails",
      "kind": "LinkedField",
      "name": "category",
      "plural": false,
      "selections": (v11/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureChoiceDetails",
      "kind": "LinkedField",
      "name": "scope",
      "plural": false,
      "selections": (v11/*:: as any*/),
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
      "kind": "ScalarField",
      "name": "requestedFrom",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "requestedUntil",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureChoiceDetails",
      "kind": "LinkedField",
      "name": "customerAction",
      "plural": false,
      "selections": (v11/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "concreteType": "EntitlementDetails",
  "kind": "LinkedField",
  "name": "myEntitlements",
  "plural": true,
  "selections": [
    (v4/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "purchaseReference",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "pricingId",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "availableQuantity",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "grantedQuantity",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "expiresAt",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "status",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "EntitlementRestrictionsDetails",
      "kind": "LinkedField",
      "name": "restrictions",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "productId",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "productVersionId",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "availableDays",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "concreteType": "EntitlementPurchaseDetails",
  "kind": "LinkedField",
  "name": "entitlementPurchases",
  "plural": true,
  "selections": [
    (v4/*:: as any*/),
    (v7/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "paymentMethod",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "amount",
      "storageKey": null
    },
    (v5/*:: as any*/),
    (v6/*:: as any*/),
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
      "kind": "ScalarField",
      "name": "invoiceNumber",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingSubscriptionCancellationModes",
  "plural": true,
  "selections": (v11/*:: as any*/),
  "storageKey": null
},
v16 = [
  (v2/*:: as any*/),
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "ASCENDING",
        "field": "FROM"
      }
    ]
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "fromGte",
        "variableName": "today"
      },
      (v3/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancellationPolicyOverridden",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancellationOverrideReason",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingChannelDetails",
  "kind": "LinkedField",
  "name": "channel",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "channel",
      "storageKey": null
    },
    (v8/*:: as any*/)
  ],
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "involvedOrganizations",
  "plural": true,
  "selections": [
    (v4/*:: as any*/),
    (v8/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "customDomain",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v24 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_LocationDetails",
  "kind": "LinkedField",
  "name": "involvedLocations",
  "plural": true,
  "selections": (v9/*:: as any*/),
  "storageKey": null
},
v25 = [
  (v4/*:: as any*/),
  (v8/*:: as any*/)
],
v26 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamDetails",
  "kind": "LinkedField",
  "name": "involvedTeams",
  "plural": true,
  "selections": (v25/*:: as any*/),
  "storageKey": null
},
v27 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingResourceDetails",
  "kind": "LinkedField",
  "name": "bookingResources",
  "plural": true,
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ResourceDetails",
      "kind": "LinkedField",
      "name": "resource",
      "plural": false,
      "selections": (v25/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v29 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v11/*:: as any*/),
  "storageKey": null
},
v30 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBooking",
  "plural": false,
  "selections": [
    (v4/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingFrequencyDetails",
      "kind": "LinkedField",
      "name": "frequency",
      "plural": false,
      "selections": (v9/*:: as any*/),
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
        (v4/*:: as any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v31 = [
  (v17/*:: as any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingEdge",
    "kind": "LinkedField",
    "name": "edges",
    "plural": true,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "node",
        "plural": false,
        "selections": [
          (v4/*:: as any*/),
          (v18/*:: as any*/),
          (v19/*:: as any*/),
          (v20/*:: as any*/),
          (v21/*:: as any*/),
          (v22/*:: as any*/),
          (v23/*:: as any*/),
          (v24/*:: as any*/),
          (v26/*:: as any*/),
          (v27/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v28/*:: as any*/),
              (v29/*:: as any*/)
            ],
            "storageKey": null
          },
          (v30/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
],
v32 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 24
  },
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "DESCENDING",
        "field": "FROM"
      }
    ]
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "fromLt",
        "variableName": "today"
      },
      (v3/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v33 = [
  (v2/*:: as any*/),
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "ASCENDING",
        "field": "NEXT_RENEWAL_AT"
      }
    ]
  },
  {
    "kind": "Literal",
    "name": "where",
    "value": {
      "includeMineOnly": true
    }
  }
],
v34 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startedAt",
  "storageKey": null
},
v35 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "nextRenewalAt",
  "storageKey": null
},
v36 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "autoRenew",
  "storageKey": null
},
v37 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancelAtPeriodEnd",
  "storageKey": null
},
v38 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v11/*:: as any*/),
  "storageKey": null
},
v39 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v9/*:: as any*/),
  "storageKey": null
},
v40 = {
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
    }
  ],
  "storageKey": null
},
v41 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBookings",
  "plural": true,
  "selections": [
    (v4/*:: as any*/),
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
    }
  ],
  "storageKey": null
},
v42 = [
  (v17/*:: as any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingEdge",
    "kind": "LinkedField",
    "name": "edges",
    "plural": true,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "node",
        "plural": false,
        "selections": [
          (v4/*:: as any*/),
          (v18/*:: as any*/),
          (v19/*:: as any*/),
          (v20/*:: as any*/),
          (v21/*:: as any*/),
          (v22/*:: as any*/),
          (v23/*:: as any*/),
          (v24/*:: as any*/),
          (v26/*:: as any*/),
          (v27/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v28/*:: as any*/),
              (v29/*:: as any*/),
              (v4/*:: as any*/)
            ],
            "storageKey": null
          },
          (v30/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "customerBookingsHub_rootQuery",
    "selections": [
      (v10/*:: as any*/),
      (v12/*:: as any*/),
      (v13/*:: as any*/),
      (v14/*:: as any*/),
      (v15/*:: as any*/),
      {
        "alias": "upcomingBookings",
        "args": (v16/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v31/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": "recentBookings",
        "args": (v32/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v31/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v33/*:: as any*/),
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
          (v17/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingSubscriptionEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingSubscriptionDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v4/*:: as any*/),
                  (v18/*:: as any*/),
                  (v19/*:: as any*/),
                  (v34/*:: as any*/),
                  (v35/*:: as any*/),
                  (v36/*:: as any*/),
                  (v37/*:: as any*/),
                  (v38/*:: as any*/),
                  (v23/*:: as any*/),
                  (v26/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v28/*:: as any*/),
                      (v29/*:: as any*/),
                      (v39/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v40/*:: as any*/)
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  (v41/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": "marketplaceBookingSubscriptions(first:48,orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"NEXT_RENEWAL_AT\"}],where:{\"includeMineOnly\":true})"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*:: as any*/),
      (v0/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "customerBookingsHub_rootQuery",
    "selections": [
      (v10/*:: as any*/),
      (v12/*:: as any*/),
      (v13/*:: as any*/),
      (v14/*:: as any*/),
      (v15/*:: as any*/),
      {
        "alias": "upcomingBookings",
        "args": (v16/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v42/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": "recentBookings",
        "args": (v32/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v42/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v33/*:: as any*/),
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
          (v17/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingSubscriptionEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingSubscriptionDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v4/*:: as any*/),
                  (v18/*:: as any*/),
                  (v19/*:: as any*/),
                  (v34/*:: as any*/),
                  (v35/*:: as any*/),
                  (v36/*:: as any*/),
                  (v37/*:: as any*/),
                  (v38/*:: as any*/),
                  (v23/*:: as any*/),
                  (v26/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v28/*:: as any*/),
                      (v29/*:: as any*/),
                      (v39/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v40/*:: as any*/),
                          (v4/*:: as any*/)
                        ],
                        "storageKey": null
                      },
                      (v4/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  (v41/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": "marketplaceBookingSubscriptions(first:48,orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"NEXT_RENEWAL_AT\"}],where:{\"includeMineOnly\":true})"
      }
    ]
  },
  "params": {
    "cacheID": "665e72cc9eba3255dd0353d265cb2fd3",
    "id": null,
    "metadata": {},
    "name": "customerBookingsHub_rootQuery",
    "operationKind": "query",
    "text": "query customerBookingsHub_rootQuery(\n  $today: DateTime!\n  $organizationCustomDomain: String!\n) {\n  marketplacePurchases(first: 48, where: {organizationCustomDomain: $organizationCustomDomain, includeMineOnly: true, lifecycleStates: [CANCELLED, DELETED, EXPIRED, PAYMENT_FAILED]}, orderBy: [{field: ACTIVITY_AT, direction: DESCENDING}]) {\n    edges {\n      node {\n        id\n        sourceId\n        sourceType\n        sourceTypeName\n        lifecycleStateName\n        renewalStateName\n        activityAt\n        bookingFrom\n        bookingUntil\n        productTitle\n        totalAmount\n        currency\n        bookingId\n        creditQuantity\n        isDeleted\n        paymentStatus\n        refund {\n          status {\n            name\n          }\n          refundAmount\n          currencyToDisplay\n        }\n      }\n    }\n  }\n  marketplaceBookingFailures {\n    id\n    category {\n      type\n      name\n    }\n    scope {\n      type\n      name\n    }\n    finalizedAt\n    requestedFrom\n    requestedUntil\n    customerAction {\n      type\n      name\n    }\n  }\n  myEntitlements {\n    id\n    purchaseReference\n    pricingId\n    availableQuantity\n    grantedQuantity\n    expiresAt\n    status\n    restrictions {\n      productId\n      productVersionId\n      availableDays\n    }\n  }\n  entitlementPurchases {\n    id\n    paymentStatus\n    paymentMethod\n    amount\n    currency\n    creditQuantity\n    paymentExpiry\n    invoiceNumber\n  }\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  upcomingBookings: bookings(first: 48, where: {includeMineOnly: true, fromGte: $today}, orderBy: [{field: FROM, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        cancellationPolicyOverridden\n        cancellationOverrideReason\n        from\n        until\n        channel {\n          channel\n          name\n        }\n        involvedOrganizations {\n          id\n          name\n          customDomain\n        }\n        involvedLocations {\n          name\n        }\n        involvedTeams {\n          id\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n          }\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n        recurringBooking {\n          id\n          frequency {\n            name\n          }\n          marketplaceBooking {\n            id\n          }\n        }\n      }\n    }\n  }\n  recentBookings: bookings(first: 24, where: {includeMineOnly: true, fromLt: $today}, orderBy: [{field: FROM, direction: DESCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        cancellationPolicyOverridden\n        cancellationOverrideReason\n        from\n        until\n        channel {\n          channel\n          name\n        }\n        involvedOrganizations {\n          id\n          name\n          customDomain\n        }\n        involvedLocations {\n          name\n        }\n        involvedTeams {\n          id\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n          }\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n        recurringBooking {\n          id\n          frequency {\n            name\n          }\n          marketplaceBooking {\n            id\n          }\n        }\n      }\n    }\n  }\n  marketplaceBookingSubscriptions(first: 48, where: {includeMineOnly: true}, orderBy: [{field: NEXT_RENEWAL_AT, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        cancellationPolicyOverridden\n        cancellationOverrideReason\n        startedAt\n        nextRenewalAt\n        autoRenew\n        cancelAtPeriodEnd\n        status {\n          type\n          name\n        }\n        involvedOrganizations {\n          id\n          name\n          customDomain\n        }\n        involvedTeams {\n          id\n          name\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          paymentMethod {\n            name\n          }\n          productVersion {\n            listingMetadata {\n              title\n              subTitle\n            }\n            id\n          }\n          id\n        }\n        recurringBookings {\n          id\n          startDate\n          endDate\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "02738bbffbf4a782aa71020e27f66b41";

export default node;
