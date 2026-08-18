/**
 * @generated SignedSource<<f58f9783027c2e489ef845729a6d1748>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "NAME" | "TIMEZONE" | "TYPE" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationMemberOrderField = "FAMILY_NAME" | "GIVEN_NAME" | "MIDDLE_NAME" | "NAME" | "PHONE_NUMBER" | "ROLE" | "STATUS" | "%future added value";
export type ProductPricingFulfillmentType = "ENTITLEMENT" | "RESERVATION" | "%future added value";
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type organizationBookings_rootQuery$variables = {
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaTo: any;
  customerId: string;
  customerIds: ReadonlyArray<string>;
  locationIds: ReadonlyArray<string>;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationCustomDomain: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
};
export type organizationBookings_rootQuery$data = {
  readonly entitlementsByCustomer: ReadonlyArray<{
    readonly availableQuantity: number;
    readonly expiresAt: any;
    readonly id: string;
    readonly pricingId: string;
  }>;
  readonly marketplaceBookingSubscriptionCancellationModes: ReadonlyArray<{
    readonly name: string;
    readonly type: MarketplaceBookingSubscriptionCancellationMode;
  }>;
  readonly marketplaceBookingSubscriptions: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly recurringBookings: ReadonlyArray<{
          readonly id: string;
        }>;
      };
    }>;
  };
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly id: string;
      readonly name: string;
    };
  }> | null | undefined;
  readonly organization: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly products: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly latestProductVersionId: string;
        readonly listingMetadata: {
          readonly title: string | null | undefined;
        };
        readonly pricingOptions: ReadonlyArray<{
          readonly fulfillmentType: ProductPricingFulfillmentType;
          readonly id: string;
          readonly listingMetadata: {
            readonly title: string | null | undefined;
          };
        }>;
      };
    }>;
  };
  readonly " $fragmentSpreads": FragmentRefs<"bookings_bookings_query" | "bookings_query" | "gettingStarted_query" | "locationSelector_allLocations_query" | "organizationUserSelector_organizationMembers_query">;
};
export type organizationBookings_rootQuery = {
  response: organizationBookings_rootQuery$data;
  variables: organizationBookings_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingsSearchCriteriaFrom"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingsSearchCriteriaTo"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customerId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customerIds"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationIds"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v9 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v12 = [
  (v10/*:: as any*/),
  (v11/*:: as any*/)
],
v13 = {
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
v14 = {
  "alias": null,
  "args": [
    {
      "fields": [
        {
          "kind": "Literal",
          "name": "includeInactive",
          "value": false
        },
        {
          "items": [
            {
              "kind": "Variable",
              "name": "organizationCustomDomains.0",
              "variableName": "organizationCustomDomain"
            }
          ],
          "kind": "ListValue",
          "name": "organizationCustomDomains"
        }
      ],
      "kind": "ObjectValue",
      "name": "where"
    }
  ],
  "concreteType": "ConnectionOfProductEdge",
  "kind": "LinkedField",
  "name": "products",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ProductEdge",
      "kind": "LinkedField",
      "name": "edges",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "ProductDetails",
          "kind": "LinkedField",
          "name": "node",
          "plural": false,
          "selections": [
            (v10/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "latestProductVersionId",
              "storageKey": null
            },
            (v13/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "ProductPricing",
              "kind": "LinkedField",
              "name": "pricingOptions",
              "plural": true,
              "selections": [
                (v10/*:: as any*/),
                (v13/*:: as any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "fulfillmentType",
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
v15 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "customerId",
      "variableName": "customerId"
    }
  ],
  "concreteType": "EntitlementDetails",
  "kind": "LinkedField",
  "name": "entitlementsByCustomer",
  "plural": true,
  "selections": [
    (v10/*:: as any*/),
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
      "name": "expiresAt",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v16 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v11/*:: as any*/)
],
v17 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingSubscriptionCancellationModes",
  "plural": true,
  "selections": (v16/*:: as any*/),
  "storageKey": null
},
v18 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v19 = [
  (v18/*:: as any*/)
],
v20 = {
  "fields": (v19/*:: as any*/),
  "kind": "ObjectValue",
  "name": "where"
},
v21 = [
  (v10/*:: as any*/)
],
v22 = {
  "alias": null,
  "args": [
    {
      "kind": "Literal",
      "name": "first",
      "value": 100
    },
    (v20/*:: as any*/)
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
            (v10/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "RecurringBookingDetails",
              "kind": "LinkedField",
              "name": "recurringBookings",
              "plural": true,
              "selections": (v21/*:: as any*/),
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
v23 = {
  "alias": null,
  "args": (v19/*:: as any*/),
  "concreteType": "LocationDetails",
  "kind": "LinkedField",
  "name": "myLocations",
  "plural": true,
  "selections": [
    (v10/*:: as any*/),
    (v11/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": (v12/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v25 = [
  (v10/*:: as any*/),
  (v11/*:: as any*/),
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
v26 = {
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
v27 = [
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
        "name": "customerIds",
        "variableName": "customerIds"
      },
      {
        "kind": "Variable",
        "name": "fromGte",
        "variableName": "bookingsSearchCriteriaFrom"
      },
      {
        "kind": "Variable",
        "name": "fromLte",
        "variableName": "bookingsSearchCriteriaTo"
      },
      {
        "kind": "Variable",
        "name": "locationIds",
        "variableName": "locationIds"
      },
      (v18/*:: as any*/),
      {
        "kind": "Literal",
        "name": "teamIds",
        "value": ([]/*:: as any*/)
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v29 = [
  (v10/*:: as any*/),
  (v11/*:: as any*/),
  (v28/*:: as any*/)
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
      (v7/*:: as any*/),
      (v8/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationBookings_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v9/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v12/*:: as any*/),
        "storageKey": null
      },
      (v14/*:: as any*/),
      (v15/*:: as any*/),
      (v17/*:: as any*/),
      (v22/*:: as any*/),
      (v23/*:: as any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationUserSelector_organizationMembers_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationSelector_allLocations_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "gettingStarted_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "bookings_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "bookings_bookings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v6/*:: as any*/),
      (v2/*:: as any*/),
      (v4/*:: as any*/),
      (v3/*:: as any*/),
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v5/*:: as any*/),
      (v8/*:: as any*/),
      (v7/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "organizationBookings_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v9/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v10/*:: as any*/),
          (v11/*:: as any*/),
          {
            "alias": null,
            "args": [
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
            ],
            "concreteType": "ConnectionOfOrganizationMemberEdge",
            "kind": "LinkedField",
            "name": "members",
            "plural": false,
            "selections": [
              (v24/*:: as any*/),
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
                      (v10/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": (v25/*:: as any*/),
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v26/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "isMyOnboardingDone",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      (v14/*:: as any*/),
      (v15/*:: as any*/),
      (v17/*:: as any*/),
      (v22/*:: as any*/),
      (v23/*:: as any*/),
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          (v20/*:: as any*/)
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v24/*:: as any*/),
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
                "selections": (v12/*:: as any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v26/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": (v25/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*:: as any*/),
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
        "args": null,
        "concreteType": "PaymentStatusDetails",
        "kind": "LinkedField",
        "name": "paymentStatuses",
        "plural": true,
        "selections": (v16/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v27/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v24/*:: as any*/),
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
                  (v10/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "from",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "until",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CustomerDetails",
                    "kind": "LinkedField",
                    "name": "involvedCustomers",
                    "plural": true,
                    "selections": (v25/*:: as any*/),
                    "storageKey": null
                  },
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
                    "concreteType": "MarketplaceCancellationAvailabilityDetails",
                    "kind": "LinkedField",
                    "name": "cancellationAvailability",
                    "plural": false,
                    "selections": [
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
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "category",
                        "storageKey": null
                      },
                      (v11/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  {
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
                      (v11/*:: as any*/)
                    ],
                    "storageKey": null
                  },
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
                      (v11/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamDetails",
                    "kind": "LinkedField",
                    "name": "involvedTeams",
                    "plural": true,
                    "selections": (v12/*:: as any*/),
                    "storageKey": null
                  },
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
                          (v10/*:: as any*/),
                          (v11/*:: as any*/),
                          (v28/*:: as any*/),
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OrganizationTagDetails",
                            "kind": "LinkedField",
                            "name": "customTags",
                            "plural": true,
                            "selections": (v29/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OrganizationTagDetails",
                            "kind": "LinkedField",
                            "name": "zones",
                            "plural": true,
                            "selections": (v29/*:: as any*/),
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
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v10/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "entitlementPurchaseId",
                        "storageKey": null
                      },
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
                        "concreteType": "PaymentStatusDetails",
                        "kind": "LinkedField",
                        "name": "paymentStatus",
                        "plural": false,
                        "selections": (v16/*:: as any*/),
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
                        "concreteType": "MarketplaceRefundDetails",
                        "kind": "LinkedField",
                        "name": "refund",
                        "plural": false,
                        "selections": [
                          (v10/*:: as any*/),
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "CurrencyDetails",
                            "kind": "LinkedField",
                            "name": "currency",
                            "plural": false,
                            "selections": (v16/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "MarketplaceRefundStatusDetails",
                            "kind": "LinkedField",
                            "name": "status",
                            "plural": false,
                            "selections": (v16/*:: as any*/),
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
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "RecurringBookingDetails",
                    "kind": "LinkedField",
                    "name": "recurringBooking",
                    "plural": false,
                    "selections": [
                      (v10/*:: as any*/),
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
                        "concreteType": "BookingFrequencyDetails",
                        "kind": "LinkedField",
                        "name": "frequency",
                        "plural": false,
                        "selections": [
                          (v11/*:: as any*/)
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
                        "selections": (v21/*:: as any*/),
                        "storageKey": null
                      }
                    ],
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
          (v26/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v27/*:: as any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "bookings_bookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "9f318d3630032ae068930345f3446830",
    "id": null,
    "metadata": {},
    "name": "organizationBookings_rootQuery",
    "operationKind": "query",
    "text": "query organizationBookings_rootQuery(\n  $organizationCustomDomain: String!\n  $customerId: String!\n  $locationIds: [String!]!\n  $customerIds: [String!]!\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaTo: DateTime!\n  $locationsSortingValues: [LocationOrderInput!]\n  $peopleNameSearchText: String\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n  }\n  products(where: {organizationCustomDomains: [$organizationCustomDomain], includeInactive: false}) {\n    edges {\n      node {\n        id\n        latestProductVersionId\n        listingMetadata {\n          title\n        }\n        pricingOptions {\n          id\n          listingMetadata {\n            title\n          }\n          fulfillmentType\n        }\n      }\n    }\n  }\n  entitlementsByCustomer(customerId: $customerId) {\n    id\n    pricingId\n    availableQuantity\n    expiresAt\n  }\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  marketplaceBookingSubscriptions(first: 100, where: {organizationCustomDomain: $organizationCustomDomain}) {\n    edges {\n      node {\n        id\n        recurringBookings {\n          id\n        }\n      }\n    }\n  }\n  myLocations(organizationCustomDomain: $organizationCustomDomain) {\n    id\n    name\n    organization {\n      id\n      name\n    }\n  }\n  ...organizationUserSelector_organizationMembers_query\n  ...locationSelector_allLocations_query\n  ...gettingStarted_query\n  ...bookings_query\n  ...bookings_bookings_query\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  cancellationPolicyOverridden\n  cancellationOverrideReason\n  cancellationAvailability {\n    isCreditFunded\n    creditOutcome\n  }\n  from\n  until\n  notes\n  category {\n    category\n    name\n  }\n  channel {\n    channel\n    name\n  }\n  involvedCustomers {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  involvedOrganizations {\n    id\n  }\n  involvedLocations {\n    uniqueId\n    name\n  }\n  involvedTeams {\n    id\n    name\n  }\n  bookingResources {\n    resource {\n      id\n      name\n      color\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n    }\n  }\n  marketplaceBooking {\n    id\n    entitlementPurchaseId\n    isPaymentRequired\n    paymentStatus {\n      type\n      name\n    }\n    invoiceUrl\n    refund {\n      id\n      currency {\n        type\n        name\n      }\n      status {\n        type\n        name\n      }\n      requestedAt\n      lastProcessedAt\n      refundAmount\n      refundPercentage\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n      requestedByCustomerName\n      canProcessInXero\n      xeroProcessingBlockedReason\n    }\n  }\n  recurringBooking {\n    id\n    startDate\n    endDate\n    frequency {\n      name\n    }\n    marketplaceBooking {\n      id\n    }\n  }\n}\n\nfragment bookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {\n    canModifyPaymentMethod\n  }\n  paymentStatuses {\n    type\n    name\n  }\n}\n\nfragment bookings_bookings_query on Query {\n  bookings(where: {organizationCustomDomain: $organizationCustomDomain, locationIds: $locationIds, teamIds: [], customerIds: $customerIds, fromGte: $bookingsSearchCriteriaFrom, fromLte: $bookingsSearchCriteriaTo}, orderBy: [{field: FROM, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        until\n        involvedCustomers {\n          id\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment bookings_query on Query {\n  me {\n    id\n  }\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  marketplaceBookingSubscriptions(first: 100, where: {organizationCustomDomain: $organizationCustomDomain}) {\n    edges {\n      node {\n        id\n        recurringBookings {\n          id\n        }\n      }\n    }\n  }\n  ...bookingCard_query\n}\n\nfragment gettingStarted_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    isMyOnboardingDone\n    id\n  }\n}\n\nfragment locationSelector_allLocations_query on Query {\n  locations(where: {organizationCustomDomain: $organizationCustomDomain}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment organizationUserSelector_organizationMembers_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    members(where: {nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          customer {\n            id\n            name\n            givenName\n            middleName\n            familyName\n            photoUrl\n          }\n        }\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "f80270ef0f4b2313c47c4fc0885c8e16";

export default node;
