/**
 * @generated SignedSource<<079fb7ed824376cc0d71c076f6c4b3af>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFront_rootQuery$variables = {
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
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontFooter_query" | "guestStoreFrontLocationsStrip_query" | "guestStoreFrontProductCard_query" | "guestStoreFrontProducts_query">;
};
export type guestStoreFront_rootQuery = {
  response: guestStoreFront_rootQuery$data;
  variables: guestStoreFront_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v5 = {
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
    (v3/*:: as any*/),
    (v4/*:: as any*/),
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
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "url",
  "storageKey": null
},
v7 = {
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
        (v6/*:: as any*/),
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
v8 = [
  {
    "kind": "Variable",
    "name": "organizationCustomDomain",
    "variableName": "organizationCustomDomain"
  }
],
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "listingMetadata",
  "plural": false,
  "selections": [
    (v3/*:: as any*/),
    (v4/*:: as any*/)
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
  (v2/*:: as any*/)
],
v12 = [
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "guestStoreFront_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationPublicDetails",
        "kind": "LinkedField",
        "name": "organizationPublic",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v5/*:: as any*/),
          (v7/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "args": (v8/*:: as any*/),
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
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "guestStoreFront_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationPublicDetails",
        "kind": "LinkedField",
        "name": "organizationPublic",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v5/*:: as any*/),
          (v7/*:: as any*/),
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
              (v9/*:: as any*/)
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
                  (v9/*:: as any*/),
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
                      (v9/*:: as any*/),
                      (v10/*:: as any*/),
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
                  },
                  (v10/*:: as any*/),
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
                          (v6/*:: as any*/)
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
                    "selections": (v11/*:: as any*/),
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
                      (v9/*:: as any*/),
                      (v2/*:: as any*/)
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
            "fields": (v8/*:: as any*/),
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "marketplaceLocations",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "totalCount",
            "storageKey": null
          },
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
                  (v9/*:: as any*/),
                  (v2/*:: as any*/),
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
                      (v9/*:: as any*/)
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
                            "selections": (v12/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "tuesday",
                            "plural": false,
                            "selections": (v12/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "wednesday",
                            "plural": false,
                            "selections": (v12/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "thursday",
                            "plural": false,
                            "selections": (v12/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "friday",
                            "plural": false,
                            "selections": (v12/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "saturday",
                            "plural": false,
                            "selections": (v12/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OpeningHoursDetails",
                            "kind": "LinkedField",
                            "name": "sunday",
                            "plural": false,
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
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v11/*:: as any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "d880b7a601493aafbb06dff8e0cbd20d",
    "id": null,
    "metadata": {},
    "name": "guestStoreFront_rootQuery",
    "operationKind": "query",
    "text": "query guestStoreFront_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n    marketplaceListingMetadata {\n      about\n      title\n      subTitle\n      includedFeatures\n    }\n    featureImages {\n      original {\n        url\n        height\n        width\n      }\n    }\n  }\n  ...guestStoreFrontProducts_query_VqntA\n  ...guestStoreFrontLocationsStrip_query\n  ...guestStoreFrontProductCard_query\n  ...guestStoreFrontFooter_query\n}\n\nfragment guestStoreFrontFooter_query on Query {\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n    contactPhone\n    contactEmail\n    physicalAddress {\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n      id\n    }\n  }\n}\n\nfragment guestStoreFrontLocationsStrip_query on Query {\n  marketplaceLocations(where: {organizationCustomDomain: $organizationCustomDomain}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        timezone\n        floorPlanCount\n        physicalAddress {\n          formattedAddress\n          id\n        }\n        openingHours {\n          weekOpeningHours {\n            monday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            tuesday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            wednesday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            thursday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            friday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            saturday {\n              closed\n              openAllDay\n              from\n              until\n            }\n            sunday {\n              closed\n              openAllDay\n              from\n              until\n            }\n          }\n        }\n      }\n    }\n  }\n}\n\nfragment guestStoreFrontProductCard_product on ProductDetails {\n  id\n  listingMetadata {\n    title\n    subTitle\n  }\n  featureImages {\n    original {\n      url\n    }\n  }\n  currency {\n    type\n    name\n  }\n  amenities {\n    id\n    name\n  }\n  pricingOptions {\n    id\n    index\n    listingMetadata {\n      title\n      subTitle\n    }\n    purchaseCadence\n    price\n    isTaxInclusive\n    supportsSubscriptionAutoRenewal\n    availableDays\n  }\n}\n\nfragment guestStoreFrontProductCard_query on Query {\n  currencies {\n    type\n    name\n  }\n}\n\nfragment guestStoreFrontProducts_query_VqntA on Query {\n  products(where: {organizationCustomDomains: [$organizationCustomDomain], includeInactive: false}) {\n    edges {\n      node {\n        id\n        pricingOptions {\n          index\n        }\n        ...guestStoreFrontProductCard_product\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "826c1b407aa860579bb44675cb85e00d";

export default node;
