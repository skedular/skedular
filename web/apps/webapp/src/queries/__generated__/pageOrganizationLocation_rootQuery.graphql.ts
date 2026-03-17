/**
 * @generated SignedSource<<35704651eaacaa4551aafab13a57c4cb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type FloorPlanOrderField = "NAME" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagOrderField = "DESCRIPTION" | "NAME" | "TYPE" | "%future added value";
export type ResourceOrderField = "NAME" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type ResourceOrderInput = {
  direction: OrderDirection;
  field: ResourceOrderField;
};
export type FloorPlanOrderInput = {
  direction: OrderDirection;
  field: FloorPlanOrderField;
};
export type pageOrganizationLocation_rootQuery$variables = {
  customTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  floorPlansSortingValues?: ReadonlyArray<FloorPlanOrderInput> | null | undefined;
  locationId: string;
  organizationCustomDomain: string;
  resourceCustomTagIds?: ReadonlyArray<string> | null | undefined;
  resourceNameSearchText?: string | null | undefined;
  resourceZoneIds?: ReadonlyArray<string> | null | undefined;
  resourcesSortingValues?: ReadonlyArray<ResourceOrderInput> | null | undefined;
  zonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
};
export type pageOrganizationLocation_rootQuery$data = {
  readonly location: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationLocation_floorPlans_query" | "organizationLocation_query" | "organizationLocation_resources_query">;
};
export type pageOrganizationLocation_rootQuery = {
  response: pageOrganizationLocation_rootQuery$data;
  variables: pageOrganizationLocation_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "floorPlansSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourceCustomTagIds"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourceNameSearchText"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourceZoneIds"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourcesSortingValues"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zonesSortingValues"
},
v9 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "locationId"
  }
],
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
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v13 = [
  (v12/*: any*/),
  (v10/*: any*/)
],
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v15 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "url",
    "storageKey": null
  },
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
v16 = {
  "alias": null,
  "args": null,
  "concreteType": "CdnFile",
  "kind": "LinkedField",
  "name": "thumbnail",
  "plural": false,
  "selections": (v15/*: any*/),
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v18 = [
  (v11/*: any*/),
  (v10/*: any*/),
  (v17/*: any*/)
],
v19 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "amenities",
  "plural": true,
  "selections": (v18/*: any*/),
  "storageKey": null
},
v20 = [
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
  (v14/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "until",
    "storageKey": null
  }
],
v21 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "resourcesSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "customTagIds",
        "variableName": "resourceCustomTagIds"
      },
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "resourceNameSearchText"
      },
      {
        "kind": "Variable",
        "name": "zoneIds",
        "variableName": "resourceZoneIds"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v25 = {
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
  "where",
  "orderBy"
],
v28 = [
  (v22/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "OrganizationTagEdge",
    "kind": "LinkedField",
    "name": "edges",
    "plural": true,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTagDetails",
        "kind": "LinkedField",
        "name": "node",
        "plural": false,
        "selections": (v18/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  (v26/*: any*/)
],
v29 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "floorPlansSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "locationId",
        "variableName": "locationId"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v7/*: any*/),
      (v8/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationLocation_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v9/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v10/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationLocation_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationLocation_resources_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationLocation_floorPlans_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v3/*: any*/),
      (v2/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v4/*: any*/),
      (v8/*: any*/),
      (v0/*: any*/),
      (v7/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationLocation_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v9/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v10/*: any*/),
          (v11/*: any*/),
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
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": (v13/*: any*/),
            "storageKey": null
          },
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
                  (v14/*: any*/),
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
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "relatedVideoLinks",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "otherLinks",
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
                "selections": (v15/*: any*/),
                "storageKey": null
              },
              (v16/*: any*/)
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
              (v11/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "osmType",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "osmId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "placeId",
                "storageKey": null
              },
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
                "name": "formattedAddress",
                "storageKey": null
              },
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
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "countryCode",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "spaceTypes",
            "plural": true,
            "selections": (v18/*: any*/),
            "storageKey": null
          },
          (v19/*: any*/),
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
                    "selections": (v20/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "tuesday",
                    "plural": false,
                    "selections": (v20/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "wednesday",
                    "plural": false,
                    "selections": (v20/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "thursday",
                    "plural": false,
                    "selections": (v20/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "friday",
                    "plural": false,
                    "selections": (v20/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "saturday",
                    "plural": false,
                    "selections": (v20/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OpeningHoursDetails",
                    "kind": "LinkedField",
                    "name": "sunday",
                    "plural": false,
                    "selections": (v20/*: any*/),
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
            "args": (v21/*: any*/),
            "concreteType": "ConnectionOfResourceEdge",
            "kind": "LinkedField",
            "name": "resources",
            "plural": false,
            "selections": [
              (v22/*: any*/),
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
                      (v11/*: any*/),
                      (v10/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "inactive",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "requireBookingApproval",
                        "storageKey": null
                      },
                      (v17/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "capacity",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "customTags",
                        "plural": true,
                        "selections": (v18/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "zones",
                        "plural": true,
                        "selections": (v18/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "productTags",
                        "plural": true,
                        "selections": (v18/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "resourceType",
                        "plural": false,
                        "selections": (v18/*: any*/),
                        "storageKey": null
                      },
                      (v23/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v24/*: any*/)
                ],
                "storageKey": null
              },
              (v25/*: any*/),
              (v26/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v21/*: any*/),
            "filters": (v27/*: any*/),
            "handle": "connection",
            "key": "organizationLocation_resources",
            "kind": "LinkedHandle",
            "name": "resources"
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "emailsToShowLatestCapabilities",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v11/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "emails",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "ResourceDetails",
            "kind": "LinkedField",
            "name": "preferredResources",
            "plural": true,
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
            "args": null,
            "concreteType": "OrganizationTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": [
              (v12/*: any*/)
            ],
            "storageKey": null
          },
          (v11/*: any*/),
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "orderBy",
                "variableName": "customTagsSortingValues"
              }
            ],
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "customTags",
            "plural": false,
            "selections": (v28/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "orderBy",
                "variableName": "zonesSortingValues"
              }
            ],
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "zones",
            "plural": false,
            "selections": (v28/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "locationSpaceTypes",
            "plural": true,
            "selections": (v18/*: any*/),
            "storageKey": null
          },
          (v19/*: any*/)
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
        "args": null,
        "concreteType": "LocationTypeDetails",
        "kind": "LinkedField",
        "name": "locationTypes",
        "plural": true,
        "selections": (v13/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v29/*: any*/),
        "concreteType": "ConnectionOfFloorPlanEdge",
        "kind": "LinkedField",
        "name": "floorPlans",
        "plural": false,
        "selections": [
          (v22/*: any*/),
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
                  (v11/*: any*/),
                  (v10/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CdnImageFile",
                    "kind": "LinkedField",
                    "name": "image",
                    "plural": false,
                    "selections": [
                      (v16/*: any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "resourceCount",
                    "storageKey": null
                  },
                  (v23/*: any*/)
                ],
                "storageKey": null
              },
              (v24/*: any*/)
            ],
            "storageKey": null
          },
          (v25/*: any*/),
          (v26/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v29/*: any*/),
        "filters": (v27/*: any*/),
        "handle": "connection",
        "key": "organizationLocation_floorPlans",
        "kind": "LinkedHandle",
        "name": "floorPlans"
      }
    ]
  },
  "params": {
    "cacheID": "224a58946c9045a327add093c182af91",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationLocation_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationLocation_rootQuery(\n  $organizationCustomDomain: String!\n  $locationId: String!\n  $resourceNameSearchText: String\n  $resourceZoneIds: [String!]\n  $resourceCustomTagIds: [String!]\n  $zonesSortingValues: [OrganizationTagOrderInput!]\n  $customTagsSortingValues: [OrganizationTagOrderInput!]\n  $resourcesSortingValues: [ResourceOrderInput!]\n  $floorPlansSortingValues: [FloorPlanOrderInput!]\n) {\n  location(id: $locationId) {\n    name\n    id\n  }\n  ...organizationLocation_query\n  ...organizationLocation_resources_query\n  ...organizationLocation_floorPlans_query\n}\n\nfragment customTagSelector_allCustomTags_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    customTags(orderBy: $customTagsSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n        }\n      }\n    }\n    id\n  }\n}\n\nfragment floorPlanCard_FloorPlanDetails on FloorPlanDetails {\n  id\n  name\n  image {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n  resourceCount\n}\n\nfragment multipleChoicesAmenities_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    amenities {\n      id\n      name\n      color\n    }\n    id\n  }\n}\n\nfragment multipleChoicesLocationSpaceTypes_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    locationSpaceTypes {\n      id\n      name\n      color\n    }\n    id\n  }\n}\n\nfragment organizationLocation_floorPlans_query on Query {\n  floorPlans(where: {locationId: $locationId}, orderBy: $floorPlansSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        ...floorPlanCard_FloorPlanDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationLocation_query on Query {\n  emailsToShowLatestCapabilities\n  me {\n    id\n    emails\n    preferredResources {\n      id\n    }\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    type {\n      type\n    }\n    id\n  }\n  location(id: $locationId) {\n    id\n    name\n    listingMetadata {\n      title\n      subTitle\n      includedFeatures\n    }\n    timezone\n    type {\n      type\n      name\n    }\n    extraMetadata {\n      contactDetails {\n        contactPeople\n        contactEmails\n        contactPhones\n      }\n      areaRange {\n        fromInSqm\n        toInSqm\n      }\n      peopleCapacity {\n        from\n        to\n      }\n      website\n      relatedImageLinks\n      relatedVideoLinks\n      otherLinks\n    }\n    featureImages {\n      original {\n        url\n        height\n        width\n      }\n      thumbnail {\n        url\n        height\n        width\n      }\n    }\n    physicalAddress {\n      id\n      osmType\n      osmId\n      placeId\n      longitude\n      latitude\n      formattedAddress\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n      countryCode\n    }\n    spaceTypes {\n      id\n      name\n      color\n    }\n    amenities {\n      id\n      name\n      color\n    }\n    openingHours {\n      weekOpeningHours {\n        monday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        tuesday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        wednesday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        thursday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        friday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        saturday {\n          closed\n          openAllDay\n          from\n          until\n        }\n        sunday {\n          closed\n          openAllDay\n          from\n          until\n        }\n      }\n    }\n  }\n  ...weekOpeningHours_query\n  ...customTagSelector_allCustomTags_query\n  ...zoneSelector_allZones_query\n  ...singleChoiceLocationType_query\n  ...multipleChoicesLocationSpaceTypes_query\n  ...multipleChoicesAmenities_query\n}\n\nfragment organizationLocation_resources_query on Query {\n  location(id: $locationId) {\n    resources(where: {nameContains: $resourceNameSearchText, customTagIds: $resourceCustomTagIds, zoneIds: $resourceZoneIds}, orderBy: $resourcesSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          inactive\n          requireBookingApproval\n          color\n          capacity\n          customTags {\n            id\n            name\n            color\n          }\n          zones {\n            id\n            name\n            color\n          }\n          productTags {\n            id\n            name\n            color\n          }\n          resourceType {\n            id\n            name\n            color\n          }\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment singleChoiceLocationType_query on Query {\n  locationTypes {\n    type\n    name\n  }\n}\n\nfragment weekOpeningHours_query on Query {\n  bookingSlotSizeInMinutes\n}\n\nfragment zoneSelector_allZones_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    zones(orderBy: $zonesSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n        }\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "2f823040f6fdeb7f0cb7b783a191ca7f";

export default node;
