/**
 * @generated SignedSource<<c5d5ef902af6007d15120e94d16acc5f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageMarketplaceLocation_rootQuery$variables = {
  locationId: string;
};
export type pageMarketplaceLocation_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocation_query">;
};
export type pageMarketplaceLocation_rootQuery = {
  response: pageMarketplaceLocation_rootQuery$data;
  variables: pageMarketplaceLocation_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  (v1/*: any*/),
  (v2/*: any*/)
],
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
  "name": "title",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "amenities",
  "plural": true,
  "selections": [
    (v4/*: any*/),
    (v2/*: any*/)
  ],
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "url",
  "storageKey": null
},
v10 = [
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
  (v8/*: any*/),
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageMarketplaceLocation_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "marketplaceLocation_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageMarketplaceLocation_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductPricingCadenceDetails",
        "kind": "LinkedField",
        "name": "productPricingCadences",
        "plural": true,
        "selections": (v3/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v3/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "locationId"
          }
        ],
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v4/*: any*/),
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "customDomain",
                "storageKey": null
              },
              (v4/*: any*/)
            ],
            "storageKey": null
          },
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
                "name": "about",
                "storageKey": null
              },
              (v5/*: any*/),
              (v6/*: any*/),
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
            "kind": "ScalarField",
            "name": "timezone",
            "storageKey": null
          },
          (v7/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationExtraMetadata",
            "kind": "LinkedField",
            "name": "extraMetadata",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "ContactDetails",
                "kind": "LinkedField",
                "name": "contactDetails",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "contactPeople",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "contactEmails",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "contactPhones",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "AreaRange",
                "kind": "LinkedField",
                "name": "areaRange",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "fromInSqm",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "toInSqm",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PeopleCapacity",
                "kind": "LinkedField",
                "name": "peopleCapacity",
                "plural": false,
                "selections": [
                  (v8/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "to",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "website",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "relatedImageLinks",
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
                  (v9/*: any*/),
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
                "name": "longitude",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "latitude",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "multilinesFormattedAddress",
                "storageKey": null
              },
              (v4/*: any*/)
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
                    "selections": (v10/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "tuesday",
                    "plural": false,
                    "selections": (v10/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "wednesday",
                    "plural": false,
                    "selections": (v10/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "thursday",
                    "plural": false,
                    "selections": (v10/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "friday",
                    "plural": false,
                    "selections": (v10/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "saturday",
                    "plural": false,
                    "selections": (v10/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "sunday",
                    "plural": false,
                    "selections": (v10/*: any*/),
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
            "concreteType": "ProductDetails",
            "kind": "LinkedField",
            "name": "products",
            "plural": true,
            "selections": [
              (v4/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ListingMetadata",
                "kind": "LinkedField",
                "name": "listingMetadata",
                "plural": false,
                "selections": [
                  (v5/*: any*/),
                  (v6/*: any*/)
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
                      (v9/*: any*/)
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
                "selections": [
                  (v1/*: any*/)
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductPricing",
                "kind": "LinkedField",
                "name": "pricingOptions",
                "plural": true,
                "selections": [
                  (v4/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "index",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ListingMetadata",
                    "kind": "LinkedField",
                    "name": "listingMetadata",
                    "plural": false,
                    "selections": [
                      (v5/*: any*/)
                    ],
                    "storageKey": null
                  },
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
              (v7/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "22cc9555bd8845703f970538c7907b21",
    "id": null,
    "metadata": {},
    "name": "pageMarketplaceLocation_rootQuery",
    "operationKind": "query",
    "text": "query pageMarketplaceLocation_rootQuery(\n  $locationId: String!\n) {\n  ...marketplaceLocation_query\n}\n\nfragment marketplaceLocation_query on Query {\n  productPricingCadences {\n    type\n    name\n  }\n  currencies {\n    type\n    name\n  }\n  location(id: $locationId) {\n    id\n    name\n    organization {\n      customDomain\n      id\n    }\n    listingMetadata {\n      about\n      title\n      subTitle\n      includedFeatures\n    }\n    timezone\n    amenities {\n      id\n      name\n    }\n    extraMetadata {\n      contactDetails {\n        contactPeople\n        contactEmails\n        contactPhones\n      }\n      areaRange {\n        fromInSqm\n        toInSqm\n      }\n      peopleCapacity {\n        from\n        to\n      }\n      website\n      relatedImageLinks\n    }\n    featureImages {\n      original {\n        url\n        height\n        width\n      }\n    }\n    physicalAddress {\n      longitude\n      latitude\n      multilinesFormattedAddress\n      id\n    }\n    openingHours {\n      weekOpeningHours {\n        monday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        tuesday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        wednesday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        thursday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        friday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        saturday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        sunday {\n          closed\n          openAllDay\n          from\n          until\n        }\n      }\n    }\n    products {\n      id\n      listingMetadata {\n        title\n        subTitle\n      }\n      featureImages {\n        original {\n          url\n        }\n      }\n      currency {\n        type\n      }\n      pricingOptions {\n        id\n        index\n        listingMetadata {\n          title\n        }\n        purchaseCadence\n        price\n        isTaxInclusive\n        supportsSubscriptionAutoRenewal\n      }\n      amenities {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7f451426b1e84ae262de19e9dd387734";

export default node;
