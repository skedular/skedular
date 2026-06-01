/**
 * @generated SignedSource<<49ad711971c956053d4db27079bfb68c>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFront_rootQuery$variables = {
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaTo: any;
  includeActiveSubscriptions: boolean;
  includeUpcomingBookings: boolean;
  organizationCustomDomain: string;
};
export type guestStoreFront_rootQuery$data = {
  readonly organizationPublic: {
    readonly featureImages: ReadonlyArray<{
      readonly original: {
        readonly height: number | null | undefined;
        readonly url: string;
        readonly width: number | null | undefined;
      } | null | undefined;
    }>;
    readonly marketplaceListingMetadata: {
      readonly about: string | null | undefined;
      readonly includedFeatures: ReadonlyArray<string> | null | undefined;
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontActivitySummary_query" | "guestStoreFrontFooter_query" | "guestStoreFrontLocationsStrip_query" | "guestStoreFrontProductCard_query" | "guestStoreFrontProducts_query">;
};
export type guestStoreFront_rootQuery = {
  response: guestStoreFront_rootQuery$data;
  variables: guestStoreFront_rootQuery$variables;
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
  "name": "includeActiveSubscriptions"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "includeUpcomingBookings"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v5 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "marketplaceListingMetadata",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "about",
      "storageKey": null
    },
    (v7/*:: as any*/),
    (v8/*:: as any*/),
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
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "url",
  "storageKey": null
},
v11 = {
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
        (v10/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "height",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "width",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v12 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v13 = [
  (v12/*:: as any*/)
],
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "listingMetadata",
  "plural": false,
  "selections": [
    (v7/*:: as any*/),
    (v8/*:: as any*/)
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
  (v6/*:: as any*/)
],
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v18 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "closed",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "openAllDay",
    "storageKey": null
  },
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
  }
],
v19 = {
  "kind": "Literal",
  "name": "first",
  "value": 0
},
v20 = {
  "kind": "Literal",
  "name": "includeMineOnly",
  "value": true
},
v21 = [
  (v17/*:: as any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "guestStoreFront_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v5/*:: as any*/),
        "concreteType": "OrganizationPublicDetails",
        "kind": "LinkedField",
        "name": "organizationPublic",
        "plural": false,
        "selections": [
          (v6/*:: as any*/),
          (v9/*:: as any*/),
          (v11/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "args": (v13/*:: as any*/),
        "kind": "FragmentSpread",
        "name": "guestStoreFrontProducts_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "guestStoreFrontLocationsStrip_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "guestStoreFrontProductCard_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "guestStoreFrontFooter_query"
      },
      {
        "args": [
          {
            "kind": "Variable",
            "name": "bookingsSearchCriteriaFrom",
            "variableName": "bookingsSearchCriteriaFrom"
          },
          {
            "kind": "Variable",
            "name": "bookingsSearchCriteriaTo",
            "variableName": "bookingsSearchCriteriaTo"
          },
          {
            "kind": "Variable",
            "name": "includeActiveSubscriptions",
            "variableName": "includeActiveSubscriptions"
          },
          {
            "kind": "Variable",
            "name": "includeUpcomingBookings",
            "variableName": "includeUpcomingBookings"
          },
          (v12/*:: as any*/)
        ],
        "kind": "FragmentSpread",
        "name": "guestStoreFrontActivitySummary_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v4/*:: as any*/),
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v3/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "guestStoreFront_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v5/*:: as any*/),
        "concreteType": "OrganizationPublicDetails",
        "kind": "LinkedField",
        "name": "organizationPublic",
        "plural": false,
        "selections": [
          (v6/*:: as any*/),
          (v9/*:: as any*/),
          (v11/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "contactPhone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "contactEmail",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationPhysicalAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "addressLine1",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "addressLine2",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "suburb",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "city",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "province",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "zipcode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "country",
                "storageKey": null
              },
              (v14/*:: as any*/)
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
                  (v14/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ProductPricing",
                    "kind": "LinkedField",
                    "name": "pricingOptions",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "index",
                        "storageKey": null
                      },
                      (v14/*:: as any*/),
                      (v15/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "purchaseCadence",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "price",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "isTaxInclusive",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "supportsSubscriptionAutoRenewal",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  (v15/*:: as any*/),
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
                          (v10/*:: as any*/)
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
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
                    "concreteType": "OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "amenities",
                    "plural": true,
                    "selections": [
                      (v14/*:: as any*/),
                      (v6/*:: as any*/)
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
      {
        "alias": null,
        "args": [
          {
            "fields": (v13/*:: as any*/),
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "marketplaceLocations",
        "plural": false,
        "selections": [
          (v17/*:: as any*/),
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
                "selections": [
                  (v14/*:: as any*/),
                  (v6/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "timezone",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "floorPlanCount",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationPhysicalAddressDetails",
                    "kind": "LinkedField",
                    "name": "physicalAddress",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "formattedAddress",
                        "storageKey": null
                      },
                      (v14/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHours",
                    "kind": "LinkedField",
                    "name": "openingHours",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "WeekOpeningHours",
                        "kind": "LinkedField",
                        "name": "weekOpeningHours",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "monday",
                            "plural": false,
                            "selections": (v18/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "tuesday",
                            "plural": false,
                            "selections": (v18/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "wednesday",
                            "plural": false,
                            "selections": (v18/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "thursday",
                            "plural": false,
                            "selections": (v18/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "friday",
                            "plural": false,
                            "selections": (v18/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "saturday",
                            "plural": false,
                            "selections": (v18/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "sunday",
                            "plural": false,
                            "selections": (v18/*:: as any*/),
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
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductPricingCadenceDetails",
        "kind": "LinkedField",
        "name": "productPricingCadences",
        "plural": true,
        "selections": (v16/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v16/*:: as any*/),
        "storageKey": null
      },
      {
        "condition": "includeUpcomingBookings",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              (v19/*:: as any*/),
              {
                "fields": [
                  {
                    "kind": "Literal",
                    "name": "channel",
                    "value": "MARKETPLACE"
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
                  (v20/*:: as any*/),
                  (v12/*:: as any*/)
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "ConnectionOfBookingEdge",
            "kind": "LinkedField",
            "name": "bookings",
            "plural": false,
            "selections": (v21/*:: as any*/),
            "storageKey": null
          }
        ]
      },
      {
        "condition": "includeActiveSubscriptions",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              (v19/*:: as any*/),
              {
                "fields": [
                  (v20/*:: as any*/),
                  (v12/*:: as any*/),
                  {
                    "kind": "Literal",
                    "name": "status",
                    "value": "ACTIVE"
                  }
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
            "kind": "LinkedField",
            "name": "marketplaceBookingSubscriptions",
            "plural": false,
            "selections": (v21/*:: as any*/),
            "storageKey": null
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "d7f7a820a6489ad88893db32a991a863",
    "id": null,
    "metadata": {},
    "name": "guestStoreFront_rootQuery",
    "operationKind": "query",
    "text": "query guestStoreFront_rootQuery(\n  $organizationCustomDomain: String!\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaTo: DateTime!\n  $includeUpcomingBookings: Boolean!\n  $includeActiveSubscriptions: Boolean!\n) {\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n    marketplaceListingMetadata {\n      about\n      title\n      subTitle\n      includedFeatures\n    }\n    featureImages {\n      original {\n        url\n        height\n        width\n      }\n    }\n  }\n  ...guestStoreFrontProducts_query_VqntA\n  ...guestStoreFrontLocationsStrip_query\n  ...guestStoreFrontProductCard_query\n  ...guestStoreFrontFooter_query\n  ...guestStoreFrontActivitySummary_query_2N1lMJ\n}\n\nfragment guestStoreFrontActivitySummary_query_2N1lMJ on Query {\n  bookings(first: 0, where: {organizationCustomDomain: $organizationCustomDomain, fromGte: $bookingsSearchCriteriaFrom, fromLte: $bookingsSearchCriteriaTo, includeMineOnly: true, channel: MARKETPLACE}) @include(if: $includeUpcomingBookings) {\n    totalCount\n  }\n  marketplaceBookingSubscriptions(first: 0, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, status: ACTIVE}) @include(if: $includeActiveSubscriptions) {\n    totalCount\n  }\n}\n\nfragment guestStoreFrontFooter_query on Query {\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n    contactPhone\n    contactEmail\n    physicalAddress {\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n      id\n    }\n  }\n}\n\nfragment guestStoreFrontLocationsStrip_query on Query {\n  marketplaceLocations(where: {organizationCustomDomain: $organizationCustomDomain}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        timezone\n        floorPlanCount\n        physicalAddress {\n          formattedAddress\n          id\n        }\n        openingHours {\n          weekOpeningHours {\n            monday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            tuesday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            wednesday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            thursday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            friday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            saturday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            sunday {\n              closed\n              openAllDay\n              from\n              until\n            }\n          }\n        }\n      }\n    }\n  }\n}\n\nfragment guestStoreFrontProductCard_product on ProductDetails {\n  id\n  listingMetadata {\n    title\n    subTitle\n  }\n  featureImages {\n    original {\n      url\n    }\n  }\n  currency {\n    type\n    name\n  }\n  amenities {\n    id\n    name\n  }\n  pricingOptions {\n    id\n    index\n    listingMetadata {\n      title\n      subTitle\n    }\n    purchaseCadence\n    price\n    isTaxInclusive\n    supportsSubscriptionAutoRenewal\n  }\n}\n\nfragment guestStoreFrontProductCard_query on Query {\n  productPricingCadences {\n    type\n    name\n  }\n  currencies {\n    type\n    name\n  }\n}\n\nfragment guestStoreFrontProducts_query_VqntA on Query {\n  products(where: {organizationCustomDomains: [$organizationCustomDomain], includeInactive: false}) {\n    edges {\n      node {\n        id\n        pricingOptions {\n          index\n        }\n        ...guestStoreFrontProductCard_product\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4d9694a89187746671cd9636cd9a8d27";

export default node;
