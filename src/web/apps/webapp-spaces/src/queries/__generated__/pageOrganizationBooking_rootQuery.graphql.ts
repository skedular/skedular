/**
 * @generated SignedSource<<6eb9f967ed3561b1427096f5f4a61bc3>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type BookingChannel = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type LocationOrderField = "NAME" | "TIMEZONE" | "TYPE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationMemberOrderField = "FAMILY_NAME" | "GIVEN_NAME" | "MIDDLE_NAME" | "NAME" | "PHONE_NUMBER" | "ROLE" | "STATUS" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type pageOrganizationBooking_rootQuery$variables = {
  bookingId: string;
  dateFromToGetAvailableResources: any;
  dateUntilToGetAvailableResources: any;
  locationId: string;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationCustomDomain: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
};
export type pageOrganizationBooking_rootQuery$data = {
  readonly booking: {
    readonly channel: {
      readonly channel: BookingChannel;
    };
    readonly from: any;
    readonly marketplaceBooking: {
      readonly isPaymentRequired: boolean;
      readonly paymentStatus: {
        readonly type: PaymentStatus;
      };
    } | null | undefined;
    readonly recurringBooking: {
      readonly id: string;
    } | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"editMarketplaceBooking_booking_query" | "editMarketplaceBooking_query" | "editPrivateBooking_availableResources_query" | "editPrivateBooking_organizationMembers_query" | "editPrivateBooking_query" | "editPrivateRecurringBooking_availableResources_query" | "editPrivateRecurringBooking_organizationMembers_query" | "editPrivateRecurringBooking_query" | "payMarketplaceBooking_booking_query">;
};
export type pageOrganizationBooking_rootQuery = {
  response: pageOrganizationBooking_rootQuery$data;
  variables: pageOrganizationBooking_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingId"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateFromToGetAvailableResources"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateUntilToGetAvailableResources"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v8 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "bookingId"
  }
],
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v10 = {
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
    }
  ],
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v12 = [
  (v11/*:: as any*/)
],
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isPaymentRequired",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v15 = [
  (v14/*:: as any*/)
],
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "category",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v19 = [
  (v11/*:: as any*/),
  (v18/*:: as any*/),
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
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "photoUrl",
    "storageKey": null
  }
],
v20 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "involvedCustomers",
  "plural": true,
  "selections": (v19/*:: as any*/),
  "storageKey": null
},
v21 = [
  (v11/*:: as any*/),
  (v18/*:: as any*/)
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
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v24 = [
  (v11/*:: as any*/),
  (v18/*:: as any*/),
  (v23/*:: as any*/)
],
v25 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "customTags",
  "plural": true,
  "selections": (v24/*:: as any*/),
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "zones",
  "plural": true,
  "selections": (v24/*:: as any*/),
  "storageKey": null
},
v27 = [
  (v11/*:: as any*/),
  (v18/*:: as any*/),
  (v25/*:: as any*/),
  (v26/*:: as any*/)
],
v28 = [
  (v14/*:: as any*/),
  (v18/*:: as any*/)
],
v29 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v30 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v31 = [
  (v30/*:: as any*/)
],
v32 = {
  "fields": (v31/*:: as any*/),
  "kind": "ObjectValue",
  "name": "where"
},
v33 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v34 = {
  "kind": "ClientExtension",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "__id",
      "storageKey": null
    }
  ]
},
v35 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationMembersSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "peopleNameSearchText"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      (v6/*:: as any*/),
      (v7/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationBooking_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v8/*:: as any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v9/*:: as any*/),
          (v10/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "RecurringBookingDetails",
            "kind": "LinkedField",
            "name": "recurringBooking",
            "plural": false,
            "selections": (v12/*:: as any*/),
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
              (v13/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "PaymentStatusDetails",
                "kind": "LinkedField",
                "name": "paymentStatus",
                "plural": false,
                "selections": (v15/*:: as any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateBooking_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateRecurringBooking_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateBooking_organizationMembers_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateBooking_availableResources_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateRecurringBooking_organizationMembers_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateRecurringBooking_availableResources_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editMarketplaceBooking_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editMarketplaceBooking_booking_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "payMarketplaceBooking_booking_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v5/*:: as any*/),
      (v0/*:: as any*/),
      (v7/*:: as any*/),
      (v6/*:: as any*/),
      (v3/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v4/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationBooking_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v8/*:: as any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v9/*:: as any*/),
          (v10/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "RecurringBookingDetails",
            "kind": "LinkedField",
            "name": "recurringBooking",
            "plural": false,
            "selections": [
              (v11/*:: as any*/),
              (v9/*:: as any*/),
              (v16/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingCategoryDetails",
                "kind": "LinkedField",
                "name": "category",
                "plural": false,
                "selections": [
                  (v17/*:: as any*/),
                  (v18/*:: as any*/)
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingFrequencyDetails",
                "kind": "LinkedField",
                "name": "frequency",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "frequency",
                    "storageKey": null
                  },
                  (v18/*:: as any*/)
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "interval",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "byMonthDay",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "bySetPosition",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "DayOfWeekDetails",
                "kind": "LinkedField",
                "name": "byWeekDays",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "dayOfWeek",
                    "storageKey": null
                  },
                  (v18/*:: as any*/)
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingRecurrenceEndTypeDetails",
                "kind": "LinkedField",
                "name": "endType",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "endType",
                    "storageKey": null
                  },
                  (v18/*:: as any*/)
                ],
                "storageKey": null
              },
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
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "occurrenceCount",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "skippedDates",
                "storageKey": null
              },
              (v20/*:: as any*/),
              (v22/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ResourceDetails",
                "kind": "LinkedField",
                "name": "requestedResources",
                "plural": true,
                "selections": (v27/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingDetails",
                "kind": "LinkedField",
                "name": "marketplaceBooking",
                "plural": false,
                "selections": (v12/*:: as any*/),
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
              (v13/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "PaymentStatusDetails",
                "kind": "LinkedField",
                "name": "paymentStatus",
                "plural": false,
                "selections": (v28/*:: as any*/),
                "storageKey": null
              },
              (v11/*:: as any*/),
              (v29/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceRefundDetails",
                "kind": "LinkedField",
                "name": "refund",
                "plural": false,
                "selections": [
                  (v11/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CurrencyDetails",
                    "kind": "LinkedField",
                    "name": "currency",
                    "plural": false,
                    "selections": (v28/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceRefundStatusDetails",
                    "kind": "LinkedField",
                    "name": "status",
                    "plural": false,
                    "selections": (v28/*:: as any*/),
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
                    "name": "refundPercentage",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "currencyToDisplay",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "reason",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "lastError",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "externalRefundNumber",
                    "storageKey": null
                  },
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
                    "kind": "ScalarField",
                    "name": "canProcessInXero",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "xeroProcessingBlockedReason",
                    "storageKey": null
                  }
                ],
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
                "name": "totalAmountToDisplay",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PaymentMethodTypeDetails",
                "kind": "LinkedField",
                "name": "paymentMethod",
                "plural": false,
                "selections": (v15/*:: as any*/),
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
                "kind": "ScalarField",
                "name": "paymentExpiry",
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
                "concreteType": "ProductPricing",
                "kind": "LinkedField",
                "name": "productPricing",
                "plural": false,
                "selections": [
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
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "price",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v11/*:: as any*/),
          (v16/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "notes",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCategoryDetails",
            "kind": "LinkedField",
            "name": "category",
            "plural": false,
            "selections": [
              (v17/*:: as any*/)
            ],
            "storageKey": null
          },
          (v20/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationDetails",
            "kind": "LinkedField",
            "name": "involvedOrganizations",
            "plural": true,
            "selections": (v21/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "involvedLocations",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
                "storageKey": null
              },
              (v18/*:: as any*/)
            ],
            "storageKey": null
          },
          (v22/*:: as any*/),
          {
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
                "selections": [
                  (v11/*:: as any*/),
                  (v18/*:: as any*/),
                  (v23/*:: as any*/),
                  (v25/*:: as any*/),
                  (v26/*:: as any*/)
                ],
                "storageKey": null
              }
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
              (v29/*:: as any*/),
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
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          (v32/*:: as any*/)
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v33/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": (v21/*:: as any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v34/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "bookingSlotSizeInMinutes",
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "customDomain",
            "variableName": "organizationCustomDomain"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": (v35/*:: as any*/),
            "concreteType": "ConnectionOfOrganizationMemberEdge",
            "kind": "LinkedField",
            "name": "members",
            "plural": false,
            "selections": [
              (v33/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationMemberEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationMemberDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v11/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": (v19/*:: as any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "__typename",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "cursor",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PageInfo",
                "kind": "LinkedField",
                "name": "pageInfo",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "endCursor",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "hasNextPage",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v34/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v35/*:: as any*/),
            "filters": [
              "where",
              "orderBy"
            ],
            "handle": "connection",
            "key": "bookingDetailsSelectorQuery_members",
            "kind": "LinkedHandle",
            "name": "members"
          },
          (v11/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "fields": [
              {
                "kind": "Variable",
                "name": "from",
                "variableName": "dateFromToGetAvailableResources"
              },
              {
                "kind": "Variable",
                "name": "locationId",
                "variableName": "locationId"
              },
              (v30/*:: as any*/),
              {
                "kind": "Variable",
                "name": "until",
                "variableName": "dateUntilToGetAvailableResources"
              }
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "BookingResourceDetails",
        "kind": "LinkedField",
        "name": "availableResources",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "ResourceDetails",
            "kind": "LinkedField",
            "name": "resource",
            "plural": false,
            "selections": (v27/*:: as any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v31/*:: as any*/),
        "concreteType": "OrganizationBookingPermissions",
        "kind": "LinkedField",
        "name": "organizationBookingPermissions",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canModifyPaymentMethod",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Literal",
            "name": "first",
            "value": 100
          },
          (v32/*:: as any*/)
        ],
        "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscriptions",
        "plural": false,
        "selections": [
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
                  (v11/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "RecurringBookingDetails",
                    "kind": "LinkedField",
                    "name": "recurringBookings",
                    "plural": true,
                    "selections": (v12/*:: as any*/),
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
      {
        "alias": null,
        "args": null,
        "concreteType": "PaymentStatusDetails",
        "kind": "LinkedField",
        "name": "paymentStatuses",
        "plural": true,
        "selections": (v28/*:: as any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "f8f476122192aa94369eb452433e5d87",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationBooking_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationBooking_rootQuery(\n  $organizationCustomDomain: String!\n  $bookingId: String!\n  $peopleNameSearchText: String\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $locationId: String!\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  booking(id: $bookingId) {\n    from\n    channel {\n      channel\n    }\n    recurringBooking {\n      id\n    }\n    marketplaceBooking {\n      isPaymentRequired\n      paymentStatus {\n        type\n      }\n      id\n    }\n    id\n  }\n  ...editPrivateBooking_query\n  ...editPrivateRecurringBooking_query\n  ...editPrivateBooking_organizationMembers_query\n  ...editPrivateBooking_availableResources_query\n  ...editPrivateRecurringBooking_organizationMembers_query\n  ...editPrivateRecurringBooking_availableResources_query\n  ...editMarketplaceBooking_query\n  ...editMarketplaceBooking_booking_query\n  ...payMarketplaceBooking_booking_query\n}\n\nfragment editMarketplaceBooking_booking_query on Query {\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    category {\n      category\n    }\n    involvedCustomers {\n      id\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    involvedOrganizations {\n      id\n      name\n    }\n    involvedLocations {\n      name\n    }\n    involvedTeams {\n      id\n      name\n    }\n    bookingResources {\n      resource {\n        id\n        name\n        color\n        customTags {\n          id\n          name\n          color\n        }\n        zones {\n          id\n          name\n          color\n        }\n      }\n    }\n    marketplaceBooking {\n      id\n      isPaymentRequired\n      paymentStatus {\n        type\n        name\n      }\n      invoiceUrl\n      refund {\n        id\n        currency {\n          type\n          name\n        }\n        status {\n          type\n          name\n        }\n        requestedAt\n        lastProcessedAt\n        refundAmount\n        refundPercentage\n        currencyToDisplay\n        reason\n        lastError\n        externalRefundNumber\n        requestedByCustomerName\n        canProcessInXero\n        xeroProcessingBlockedReason\n      }\n    }\n    recurringBooking {\n      id\n      startDate\n      endDate\n      frequency {\n        name\n      }\n      marketplaceBooking {\n        id\n      }\n    }\n    arrearsInvoices {\n      invoiceNumber\n      invoiceUrl\n      billingPeriodStartInclusive\n      billingPeriodEndExclusive\n    }\n  }\n}\n\nfragment editMarketplaceBooking_query on Query {\n  organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {\n    canModifyPaymentMethod\n  }\n  marketplaceBookingSubscriptions(first: 100, where: {organizationCustomDomain: $organizationCustomDomain}) {\n    edges {\n      node {\n        id\n        recurringBookings {\n          id\n        }\n      }\n    }\n  }\n  paymentStatuses {\n    type\n    name\n  }\n}\n\nfragment editPrivateBooking_availableResources_query on Query {\n  availableResources(where: {organizationCustomDomain: $organizationCustomDomain, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    resource {\n      id\n      name\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n\nfragment editPrivateBooking_organizationMembers_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    members(where: {nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          customer {\n            id\n            name\n            givenName\n            middleName\n            familyName\n            photoUrl\n          }\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment editPrivateBooking_query on Query {\n  locations(where: {organizationCustomDomain: $organizationCustomDomain}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    category {\n      category\n    }\n    involvedCustomers {\n      id\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    involvedOrganizations {\n      id\n      name\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    involvedTeams {\n      id\n      name\n    }\n    bookingResources {\n      resource {\n        id\n        name\n        color\n        customTags {\n          id\n          name\n          color\n        }\n        zones {\n          id\n          name\n          color\n        }\n      }\n    }\n  }\n  bookingSlotSizeInMinutes\n}\n\nfragment editPrivateRecurringBooking_availableResources_query on Query {\n  availableResources(where: {organizationCustomDomain: $organizationCustomDomain, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    resource {\n      id\n      name\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n\nfragment editPrivateRecurringBooking_organizationMembers_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    members(where: {nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n      edges {\n        node {\n          id\n          customer {\n            id\n            name\n            givenName\n            middleName\n            familyName\n            photoUrl\n          }\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment editPrivateRecurringBooking_query on Query {\n  booking(id: $bookingId) {\n    id\n    involvedOrganizations {\n      id\n      name\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    recurringBooking {\n      id\n      from\n      until\n      category {\n        category\n        name\n      }\n      frequency {\n        frequency\n        name\n      }\n      interval\n      byMonthDay\n      bySetPosition\n      byWeekDays {\n        dayOfWeek\n        name\n      }\n      endType {\n        endType\n        name\n      }\n      startDate\n      endDate\n      occurrenceCount\n      skippedDates\n      involvedCustomers {\n        id\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n      involvedTeams {\n        id\n        name\n      }\n      requestedResources {\n        id\n        name\n        customTags {\n          id\n          name\n          color\n        }\n        zones {\n          id\n          name\n          color\n        }\n      }\n    }\n  }\n  locations(where: {organizationCustomDomain: $organizationCustomDomain}, orderBy: $locationsSortingValues) {\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  bookingSlotSizeInMinutes\n}\n\nfragment payMarketplaceBooking_booking_query on Query {\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    category {\n      category\n    }\n    involvedCustomers {\n      id\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    involvedOrganizations {\n      id\n      name\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    involvedTeams {\n      id\n      name\n    }\n    bookingResources {\n      resource {\n        id\n        name\n        color\n        customTags {\n          id\n          name\n          color\n        }\n        zones {\n          id\n          name\n          color\n        }\n      }\n    }\n    marketplaceBooking {\n      totalAmountExcludeTaxToDisplay\n      taxAmountToDisplay\n      totalAmountToDisplay\n      paymentMethod {\n        type\n      }\n      bookingCheckoutSession {\n        checkoutUrl\n      }\n      paymentExpiry\n      invoiceUrl\n      quantity\n      productPricing {\n        listingMetadata {\n          title\n        }\n        price\n      }\n      isPaymentRequired\n      paymentStatus {\n        type\n        name\n      }\n      id\n    }\n    arrearsInvoices {\n      invoiceNumber\n      invoiceUrl\n      billingPeriodStartInclusive\n      billingPeriodEndExclusive\n    }\n  }\n  organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {\n    canModifyPaymentMethod\n  }\n  paymentStatuses {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "725d59221cf292b577a615b783131998";

export default node;
