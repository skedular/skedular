/**
 * @generated SignedSource<<ece85e10c19e561c4855960610783afc>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingChannel = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type Currency = "NZD" | "USD" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type MarketplacePurchaseSourceType = "BOOKING" | "ENTITLEMENT" | "SUBSCRIPTION" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type customerBookingsHub_rootQuery$variables = {
  organizationCustomDomain: string;
  today: any;
};
export type customerBookingsHub_rootQuery$data = {
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
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
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
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v7 = [
  (v6/*:: as any*/)
],
v8 = {
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
        (v4/*:: as any*/)
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
            (v5/*:: as any*/),
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
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "currency",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "bookingId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "creditQuantity",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "isDeleted",
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
                  "selections": (v7/*:: as any*/),
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
v9 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v6/*:: as any*/)
],
v10 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingFailureDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingFailures",
  "plural": true,
  "selections": [
    (v5/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureChoiceDetails",
      "kind": "LinkedField",
      "name": "category",
      "plural": false,
      "selections": (v9/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingFailureChoiceDetails",
      "kind": "LinkedField",
      "name": "scope",
      "plural": false,
      "selections": (v9/*:: as any*/),
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
      "selections": (v9/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingSubscriptionCancellationModes",
  "plural": true,
  "selections": (v9/*:: as any*/),
  "storageKey": null
},
v12 = [
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
      (v3/*:: as any*/),
      (v4/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancellationPolicyOverridden",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancellationOverrideReason",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v18 = {
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
    (v6/*:: as any*/)
  ],
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "involvedOrganizations",
  "plural": true,
  "selections": [
    (v5/*:: as any*/),
    (v6/*:: as any*/),
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
v20 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_LocationDetails",
  "kind": "LinkedField",
  "name": "involvedLocations",
  "plural": true,
  "selections": (v7/*:: as any*/),
  "storageKey": null
},
v21 = [
  (v5/*:: as any*/),
  (v6/*:: as any*/)
],
v22 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamDetails",
  "kind": "LinkedField",
  "name": "involvedTeams",
  "plural": true,
  "selections": (v21/*:: as any*/),
  "storageKey": null
},
v23 = {
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
      "selections": (v21/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v9/*:: as any*/),
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBooking",
  "plural": false,
  "selections": [
    (v5/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingFrequencyDetails",
      "kind": "LinkedField",
      "name": "frequency",
      "plural": false,
      "selections": (v7/*:: as any*/),
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
        (v5/*:: as any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v27 = [
  (v13/*:: as any*/),
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
          (v5/*:: as any*/),
          (v14/*:: as any*/),
          (v15/*:: as any*/),
          (v16/*:: as any*/),
          (v17/*:: as any*/),
          (v18/*:: as any*/),
          (v19/*:: as any*/),
          (v20/*:: as any*/),
          (v22/*:: as any*/),
          (v23/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v24/*:: as any*/),
              (v25/*:: as any*/)
            ],
            "storageKey": null
          },
          (v26/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
],
v28 = [
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
      (v3/*:: as any*/),
      (v4/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v29 = [
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
    "fields": [
      (v3/*:: as any*/),
      (v4/*:: as any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v30 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startedAt",
  "storageKey": null
},
v31 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "nextRenewalAt",
  "storageKey": null
},
v32 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "autoRenew",
  "storageKey": null
},
v33 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancelAtPeriodEnd",
  "storageKey": null
},
v34 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v9/*:: as any*/),
  "storageKey": null
},
v35 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v7/*:: as any*/),
  "storageKey": null
},
v36 = {
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
v37 = {
  "alias": null,
  "args": null,
  "concreteType": "RecurringBookingDetails",
  "kind": "LinkedField",
  "name": "recurringBookings",
  "plural": true,
  "selections": [
    (v5/*:: as any*/),
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
v38 = [
  (v13/*:: as any*/),
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
          (v5/*:: as any*/),
          (v14/*:: as any*/),
          (v15/*:: as any*/),
          (v16/*:: as any*/),
          (v17/*:: as any*/),
          (v18/*:: as any*/),
          (v19/*:: as any*/),
          (v20/*:: as any*/),
          (v22/*:: as any*/),
          (v23/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v24/*:: as any*/),
              (v25/*:: as any*/),
              (v5/*:: as any*/)
            ],
            "storageKey": null
          },
          (v26/*:: as any*/)
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
      (v8/*:: as any*/),
      (v10/*:: as any*/),
      (v11/*:: as any*/),
      {
        "alias": "upcomingBookings",
        "args": (v12/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v27/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": "recentBookings",
        "args": (v28/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v27/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v29/*:: as any*/),
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
          (v13/*:: as any*/),
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
                  (v5/*:: as any*/),
                  (v14/*:: as any*/),
                  (v15/*:: as any*/),
                  (v30/*:: as any*/),
                  (v31/*:: as any*/),
                  (v32/*:: as any*/),
                  (v33/*:: as any*/),
                  (v34/*:: as any*/),
                  (v19/*:: as any*/),
                  (v22/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v24/*:: as any*/),
                      (v25/*:: as any*/),
                      (v35/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v36/*:: as any*/)
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  (v37/*:: as any*/)
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
      (v8/*:: as any*/),
      (v10/*:: as any*/),
      (v11/*:: as any*/),
      {
        "alias": "upcomingBookings",
        "args": (v12/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v38/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": "recentBookings",
        "args": (v28/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": (v38/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v29/*:: as any*/),
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
          (v13/*:: as any*/),
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
                  (v5/*:: as any*/),
                  (v14/*:: as any*/),
                  (v15/*:: as any*/),
                  (v30/*:: as any*/),
                  (v31/*:: as any*/),
                  (v32/*:: as any*/),
                  (v33/*:: as any*/),
                  (v34/*:: as any*/),
                  (v19/*:: as any*/),
                  (v22/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v24/*:: as any*/),
                      (v25/*:: as any*/),
                      (v35/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductVersionDetails",
                        "kind": "LinkedField",
                        "name": "productVersion",
                        "plural": false,
                        "selections": [
                          (v36/*:: as any*/),
                          (v5/*:: as any*/)
                        ],
                        "storageKey": null
                      },
                      (v5/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  (v37/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "83519417ff245678fef3dc4e0eac4982",
    "id": null,
    "metadata": {},
    "name": "customerBookingsHub_rootQuery",
    "operationKind": "query",
    "text": "query customerBookingsHub_rootQuery(\n  $today: DateTime!\n  $organizationCustomDomain: String!\n) {\n  marketplacePurchases(first: 48, where: {organizationCustomDomain: $organizationCustomDomain, includeMineOnly: true, lifecycleStates: [CANCELLED, DELETED, EXPIRED, PAYMENT_FAILED]}, orderBy: [{field: ACTIVITY_AT, direction: DESCENDING}]) {\n    edges {\n      node {\n        id\n        sourceId\n        sourceType\n        sourceTypeName\n        lifecycleStateName\n        renewalStateName\n        activityAt\n        bookingFrom\n        bookingUntil\n        productTitle\n        totalAmount\n        currency\n        bookingId\n        creditQuantity\n        isDeleted\n        paymentStatus\n        refund {\n          status {\n            name\n          }\n          refundAmount\n          currencyToDisplay\n        }\n      }\n    }\n  }\n  marketplaceBookingFailures {\n    id\n    category {\n      type\n      name\n    }\n    scope {\n      type\n      name\n    }\n    finalizedAt\n    requestedFrom\n    requestedUntil\n    customerAction {\n      type\n      name\n    }\n  }\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  upcomingBookings: bookings(first: 48, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, fromGte: $today}, orderBy: [{field: FROM, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        cancellationPolicyOverridden\n        cancellationOverrideReason\n        from\n        until\n        channel {\n          channel\n          name\n        }\n        involvedOrganizations {\n          id\n          name\n          customDomain\n        }\n        involvedLocations {\n          name\n        }\n        involvedTeams {\n          id\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n          }\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n        recurringBooking {\n          id\n          frequency {\n            name\n          }\n          marketplaceBooking {\n            id\n          }\n        }\n      }\n    }\n  }\n  recentBookings: bookings(first: 24, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, fromLt: $today}, orderBy: [{field: FROM, direction: DESCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        cancellationPolicyOverridden\n        cancellationOverrideReason\n        from\n        until\n        channel {\n          channel\n          name\n        }\n        involvedOrganizations {\n          id\n          name\n          customDomain\n        }\n        involvedLocations {\n          name\n        }\n        involvedTeams {\n          id\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n          }\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n        recurringBooking {\n          id\n          frequency {\n            name\n          }\n          marketplaceBooking {\n            id\n          }\n        }\n      }\n    }\n  }\n  marketplaceBookingSubscriptions(first: 48, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain}, orderBy: [{field: NEXT_RENEWAL_AT, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        cancellationPolicyOverridden\n        cancellationOverrideReason\n        startedAt\n        nextRenewalAt\n        autoRenew\n        cancelAtPeriodEnd\n        status {\n          type\n          name\n        }\n        involvedOrganizations {\n          id\n          name\n          customDomain\n        }\n        involvedTeams {\n          id\n          name\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          paymentMethod {\n            name\n          }\n          productVersion {\n            listingMetadata {\n              title\n              subTitle\n            }\n            id\n          }\n          id\n        }\n        recurringBookings {\n          id\n          startDate\n          endDate\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e52477aca5e9ee904df3452d76717adb";

export default node;
