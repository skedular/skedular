/**
 * @generated SignedSource<<c37adad0f028eaad3496472a7f0c540c>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageMarketplaceLocation_rootQuery$variables = {
  floorPlanSelected: boolean;
  locationId: string;
  selectedFloorPlanId?: string | null | undefined;
};
export type pageMarketplaceLocation_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocation_query">;
};
export type pageMarketplaceLocation_rootQuery = {
  response: pageMarketplaceLocation_rootQuery$data;
  variables: pageMarketplaceLocation_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "floorPlanSelected"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "selectedFloorPlanId"
},
v3 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v4 = {
  "kind": "Literal",
  "name": "orderBy",
  "value": [
    {
      "direction": "ASCENDING",
      "field": "NAME"
    }
  ]
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
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "url",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "CdnFile",
  "kind": "LinkedField",
  "name": "original",
  "plural": false,
  "selections": [
    (v7/*:: as any*/),
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
},
v9 = [
  (v5/*:: as any*/)
],
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "amenities",
  "plural": true,
  "selections": [
    (v5/*:: as any*/),
    (v6/*:: as any*/)
  ],
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v15 = [
  (v7/*:: as any*/)
],
v16 = [
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
  (v14/*:: as any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "until",
    "storageKey": null
  }
],
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageMarketplaceLocation_rootQuery",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "floorPlanSelected",
            "variableName": "floorPlanSelected"
          },
          (v3/*:: as any*/),
          {
            "kind": "Variable",
            "name": "selectedFloorPlanId",
            "variableName": "selectedFloorPlanId"
          }
        ],
        "kind": "FragmentSpread",
        "name": "marketplaceLocation_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v0/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "pageMarketplaceLocation_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "deskResourceType",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "roomResourceType",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "parkingResourceType",
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          (v4/*:: as any*/),
          {
            "fields": [
              (v3/*:: as any*/)
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "ConnectionOfFloorPlanEdge",
        "kind": "LinkedField",
        "name": "floorPlans",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "FloorPlanEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "FloorPlanDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v5/*:: as any*/),
                  (v6/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "resourceCount",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CdnImageFile",
                    "kind": "LinkedField",
                    "name": "image",
                    "plural": false,
                    "selections": [
                      (v8/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ResourcePositionDetails",
                    "kind": "LinkedField",
                    "name": "resourcePositions",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "x",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "y",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ResourceDetails",
                        "kind": "LinkedField",
                        "name": "resource",
                        "plural": false,
                        "selections": (v9/*:: as any*/),
                        "storageKey": null
                      },
                      (v5/*:: as any*/)
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
        "selections": [
          (v10/*:: as any*/),
          (v6/*:: as any*/)
        ],
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
          (v5/*:: as any*/),
          (v6/*:: as any*/),
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
              (v5/*:: as any*/)
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
              (v11/*:: as any*/),
              (v12/*:: as any*/),
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
          (v13/*:: as any*/),
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
                  (v14/*:: as any*/),
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
              (v8/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v15/*:: as any*/),
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
              (v5/*:: as any*/)
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
                    "selections": (v16/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "tuesday",
                    "plural": false,
                    "selections": (v16/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "wednesday",
                    "plural": false,
                    "selections": (v16/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "thursday",
                    "plural": false,
                    "selections": (v16/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "friday",
                    "plural": false,
                    "selections": (v16/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "saturday",
                    "plural": false,
                    "selections": (v16/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "sunday",
                    "plural": false,
                    "selections": (v16/*:: as any*/),
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
              (v5/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ListingMetadata",
                "kind": "LinkedField",
                "name": "listingMetadata",
                "plural": false,
                "selections": [
                  (v11/*:: as any*/),
                  (v12/*:: as any*/)
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagDetails",
                "kind": "LinkedField",
                "name": "productTags",
                "plural": true,
                "selections": (v9/*:: as any*/),
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
                    "selections": (v15/*:: as any*/),
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
                  (v10/*:: as any*/)
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
                  (v5/*:: as any*/),
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
                      (v11/*:: as any*/)
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
              (v13/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "condition": "floorPlanSelected",
            "kind": "Condition",
            "passingValue": true,
            "selections": [
              {
                "alias": null,
                "args": [
                  (v4/*:: as any*/),
                  {
                    "fields": [
                      {
                        "kind": "Variable",
                        "name": "floorPlanId",
                        "variableName": "selectedFloorPlanId"
                      }
                    ],
                    "kind": "ObjectValue",
                    "name": "where"
                  }
                ],
                "concreteType": "ConnectionOfResourceEdge",
                "kind": "LinkedField",
                "name": "resources",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ResourceEdge",
                    "kind": "LinkedField",
                    "name": "edges",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ResourceDetails",
                        "kind": "LinkedField",
                        "name": "node",
                        "plural": false,
                        "selections": [
                          (v5/*:: as any*/),
                          (v6/*:: as any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "inactive",
                            "storageKey": null
                          },
                          (v17/*:: as any*/),
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OrganizationTagDetails",
                            "kind": "LinkedField",
                            "name": "productTags",
                            "plural": true,
                            "selections": [
                              (v5/*:: as any*/),
                              (v6/*:: as any*/),
                              (v17/*:: as any*/)
                            ],
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OrganizationTagDetails",
                            "kind": "LinkedField",
                            "name": "resourceType",
                            "plural": false,
                            "selections": [
                              (v5/*:: as any*/),
                              (v6/*:: as any*/),
                              (v17/*:: as any*/),
                              (v10/*:: as any*/)
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
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "a4a40b4a74cd3fa73ec6af4d9eb2bf04",
    "id": null,
    "metadata": {},
    "name": "pageMarketplaceLocation_rootQuery",
    "operationKind": "query",
    "text": "query pageMarketplaceLocation_rootQuery(\n  $locationId: String!\n  $selectedFloorPlanId: String\n  $floorPlanSelected: Boolean!\n) {\n  ...marketplaceLocation_query_4ndSC6\n}\n\nfragment marketplaceLocation_query_4ndSC6 on Query {\n  deskResourceType\n  roomResourceType\n  parkingResourceType\n  floorPlans(where: {locationId: $locationId}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    edges {\n      node {\n        id\n        name\n        resourceCount\n        image {\n          original {\n            url\n            height\n            width\n          }\n        }\n        resourcePositions {\n          x\n          y\n          resource {\n            id\n          }\n          id\n        }\n      }\n    }\n  }\n  currencies {\n    type\n    name\n  }\n  location(id: $locationId) {\n    id\n    name\n    organization {\n      customDomain\n      id\n    }\n    listingMetadata {\n      about\n      title\n      subTitle\n      includedFeatures\n    }\n    timezone\n    amenities {\n      id\n      name\n    }\n    extraMetadata {\n      contactDetails {\n        contactPeople\n        contactEmails\n        contactPhones\n      }\n      areaRange {\n        fromInSqm\n        toInSqm\n      }\n      peopleCapacity {\n        from\n        to\n      }\n      website\n      relatedImageLinks\n    }\n    featureImages {\n      original {\n        url\n        height\n        width\n      }\n      thumbnail {\n        url\n      }\n    }\n    physicalAddress {\n      longitude\n      latitude\n      multilinesFormattedAddress\n      id\n    }\n    openingHours {\n      weekOpeningHours {\n        monday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        tuesday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        wednesday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        thursday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        friday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        saturday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        sunday {\n          closed\n          openAllDay\n          from\n          until\n        }\n      }\n    }\n    products {\n      id\n      listingMetadata {\n        title\n        subTitle\n      }\n      productTags {\n        id\n      }\n      featureImages {\n        original {\n          url\n        }\n      }\n      currency {\n        type\n      }\n      pricingOptions {\n        id\n        index\n        listingMetadata {\n          title\n        }\n        purchaseCadence\n        price\n        isTaxInclusive\n        supportsSubscriptionAutoRenewal\n        availableDays\n      }\n      amenities {\n        id\n        name\n      }\n    }\n    resources(where: {floorPlanId: $selectedFloorPlanId}, orderBy: [{direction: ASCENDING, field: NAME}]) @include(if: $floorPlanSelected) {\n      edges {\n        node {\n          id\n          name\n          inactive\n          color\n          productTags {\n            id\n            name\n            color\n          }\n          resourceType {\n            id\n            name\n            color\n            type\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7015597a592497a93761cefcb7572198";

export default node;
