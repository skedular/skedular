/**
 * @generated SignedSource<<c6ffc825bc9420f61663ebea09354ea2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFront_rootQuery$variables = {
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaTo: any;
  includeUpcomingBookings: boolean;
  organizationUniqueAlphanumericName: string;
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
    readonly listingMetadata: {
      readonly includedFeatures: ReadonlyArray<string> | null | undefined;
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly marketplaceListingMetadata: {
      readonly about: string | null | undefined;
      readonly includedFeatures: ReadonlyArray<string> | null | undefined;
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontFooter_query" | "guestStoreFrontLocationsStrip_query" | "guestStoreFrontProductCard_query" | "guestStoreFrontProducts_query" | "guestStoreFrontUpcomingBookingsStrip_query">;
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
  "name": "includeUpcomingBookings"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v4 = [
  {
    "kind": "Variable",
    "name": "uniqueAlphanumericName",
    "variableName": "organizationUniqueAlphanumericName"
  }
],
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "includedFeatures",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "listingMetadata",
  "plural": false,
  "selections": [
    (v6/*: any*/),
    (v7/*: any*/),
    (v8/*: any*/)
  ],
  "storageKey": null
},
v10 = {
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
    (v6/*: any*/),
    (v7/*: any*/),
    (v8/*: any*/)
  ],
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "url",
  "storageKey": null
},
v12 = {
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
        (v11/*: any*/),
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
v13 = {
  "kind": "Variable",
  "name": "organizationUniqueAlphanumericName",
  "variableName": "organizationUniqueAlphanumericName"
},
v14 = [
  (v13/*: any*/)
],
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v16 = {
  "items": [
    {
      "kind": "Variable",
      "name": "organizationUniqueAlphanumericNames.0",
      "variableName": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "ListValue",
  "name": "organizationUniqueAlphanumericNames"
},
v17 = {
  "alias": null,
  "args": null,
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "listingMetadata",
  "plural": false,
  "selections": [
    (v6/*: any*/),
    (v7/*: any*/)
  ],
  "storageKey": null
},
v18 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v5/*: any*/)
],
v19 = [
  (v15/*: any*/),
  (v5/*: any*/)
],
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v23 = [
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
  (v21/*: any*/),
  (v22/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "guestStoreFront_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "OrganizationPublicDetails",
        "kind": "LinkedField",
        "name": "organizationPublic",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          (v9/*: any*/),
          (v10/*: any*/),
          (v12/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": (v14/*: any*/),
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
            "name": "includeUpcomingBookings",
            "variableName": "includeUpcomingBookings"
          },
          (v13/*: any*/)
        ],
        "kind": "FragmentSpread",
        "name": "guestStoreFrontUpcomingBookingsStrip_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v3/*: any*/),
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "guestStoreFront_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "OrganizationPublicDetails",
        "kind": "LinkedField",
        "name": "organizationPublic",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          (v9/*: any*/),
          (v10/*: any*/),
          (v12/*: any*/),
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
              (v15/*: any*/)
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
              (v16/*: any*/)
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
                  (v15/*: any*/),
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
                      (v15/*: any*/),
                      (v17/*: any*/),
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
                  (v17/*: any*/),
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
                          (v11/*: any*/)
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
                    "selections": (v18/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "amenities",
                    "plural": true,
                    "selections": (v19/*: any*/),
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
            "fields": (v14/*: any*/),
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "marketplaceLocations",
        "plural": false,
        "selections": [
          (v20/*: any*/),
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
                  (v15/*: any*/),
                  (v5/*: any*/),
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
                      (v15/*: any*/)
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
                            "selections": (v23/*: any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "tuesday",
                            "plural": false,
                            "selections": (v23/*: any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "wednesday",
                            "plural": false,
                            "selections": (v23/*: any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "thursday",
                            "plural": false,
                            "selections": (v23/*: any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "friday",
                            "plural": false,
                            "selections": (v23/*: any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "saturday",
                            "plural": false,
                            "selections": (v23/*: any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "sunday",
                            "plural": false,
                            "selections": (v23/*: any*/),
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
        "selections": (v18/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v18/*: any*/),
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
              {
                "kind": "Literal",
                "name": "first",
                "value": 6
              },
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
                  {
                    "kind": "Literal",
                    "name": "includeMineOnly",
                    "value": true
                  },
                  (v16/*: any*/)
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "ConnectionOfBookingEdge",
            "kind": "LinkedField",
            "name": "bookings",
            "plural": false,
            "selections": [
              (v20/*: any*/),
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
                      (v15/*: any*/),
                      (v21/*: any*/),
                      (v22/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "LocationDetails",
                        "kind": "LinkedField",
                        "name": "involvedLocations",
                        "plural": true,
                        "selections": (v19/*: any*/),
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
                            "selections": (v19/*: any*/),
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
                            "concreteType": "PaymentStatusDetails",
                            "kind": "LinkedField",
                            "name": "paymentStatus",
                            "plural": false,
                            "selections": (v18/*: any*/),
                            "storageKey": null
                          },
                          (v15/*: any*/)
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
        ]
      }
    ]
  },
  "params": {
    "cacheID": "0e14db73c178031a22b1e6385c4d7044",
    "id": null,
    "metadata": {},
    "name": "guestStoreFront_rootQuery",
    "operationKind": "query",
    "text": "query guestStoreFront_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaTo: DateTime!\n  $includeUpcomingBookings: Boolean!\n) {\n  organizationPublic(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    name\n    listingMetadata {\n      title\n      subTitle\n      includedFeatures\n    }\n    marketplaceListingMetadata {\n      about\n      title\n      subTitle\n      includedFeatures\n    }\n    featureImages {\n      original {\n        url\n        height\n        width\n      }\n    }\n  }\n  ...guestStoreFrontProducts_query_Lxbsd\n  ...guestStoreFrontLocationsStrip_query\n  ...guestStoreFrontProductCard_query\n  ...guestStoreFrontFooter_query\n  ...guestStoreFrontUpcomingBookingsStrip_query_29yRhj\n}\n\nfragment guestStoreFrontFooter_query on Query {\n  organizationPublic(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    name\n    contactPhone\n    contactEmail\n    physicalAddress {\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n      id\n    }\n  }\n}\n\nfragment guestStoreFrontLocationsStrip_query on Query {\n  marketplaceLocations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        timezone\n        physicalAddress {\n          formattedAddress\n          id\n        }\n        openingHours {\n          weekOpeningHours {\n            monday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            tuesday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            wednesday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            thursday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            friday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            saturday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            sunday {\n              closed\n              openAllDay\n              from\n              until\n            }\n          }\n        }\n      }\n    }\n  }\n}\n\nfragment guestStoreFrontProductCard_product on ProductDetails {\n  id\n  listingMetadata {\n    title\n    subTitle\n  }\n  featureImages {\n    original {\n      url\n    }\n  }\n  currency {\n    type\n    name\n  }\n  amenities {\n    id\n    name\n  }\n  pricingOptions {\n    id\n    index\n    listingMetadata {\n      title\n      subTitle\n    }\n    purchaseCadence\n    price\n    isTaxInclusive\n    supportsSubscriptionAutoRenewal\n  }\n}\n\nfragment guestStoreFrontProductCard_query on Query {\n  productPricingCadences {\n    type\n    name\n  }\n  currencies {\n    type\n    name\n  }\n}\n\nfragment guestStoreFrontProducts_query_Lxbsd on Query {\n  products(where: {organizationUniqueAlphanumericNames: [$organizationUniqueAlphanumericName], includeInactive: false}) {\n    edges {\n      node {\n        id\n        pricingOptions {\n          index\n        }\n        ...guestStoreFrontProductCard_product\n      }\n    }\n  }\n}\n\nfragment guestStoreFrontUpcomingBookingsStrip_query_29yRhj on Query {\n  bookings(first: 6, where: {organizationUniqueAlphanumericNames: [$organizationUniqueAlphanumericName], fromGte: $bookingsSearchCriteriaFrom, fromLte: $bookingsSearchCriteriaTo, includeMineOnly: true, channel: MARKETPLACE}, orderBy: [{field: FROM, direction: ASCENDING}]) @include(if: $includeUpcomingBookings) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        until\n        involvedLocations {\n          id\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n          }\n        }\n        marketplaceBooking {\n          quantity\n          paymentStatus {\n            type\n            name\n          }\n          id\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "651c27f59459b061ee8a43fd32d0094d";

export default node;
