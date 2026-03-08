/**
 * @generated SignedSource<<b05253d7cb92f852123e96d51f421d5d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "ABOUT" | "NAME" | "TIMEZONE" | "TYPE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagType = "CUSTOM" | "LOCATION_SPACE_TYPE_CAR_PARK_SPACE" | "LOCATION_SPACE_TYPE_COMMERCIAL_KITCHEN" | "LOCATION_SPACE_TYPE_EVENT_SPACE" | "LOCATION_SPACE_TYPE_MEETING_SPACE" | "LOCATION_SPACE_TYPE_OFFICE_SPACE" | "LOCATION_SPACE_TYPE_OTHERS" | "LOCATION_SPACE_TYPE_RETAIL_SPACE" | "LOCATION_SPACE_TYPE_SHOOT_LOCATION" | "LOCATION_SPACE_TYPE_STORAGE_SPACE" | "LOCATION_SPACE_TYPE_STUDIO_SPACE" | "PRODUCT" | "RESOURCE_DESK" | "RESOURCE_OTHERS" | "RESOURCE_PARKING" | "RESOURCE_ROOM" | "ZONE" | "%future added value";
export type PolygonInput = {
  northEast: PointCoordinatesInput;
  southWest: PointCoordinatesInput;
};
export type PointCoordinatesInput = {
  latitude: number;
  longitude: number;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type pageHome_rootQuery$variables = {
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  resourceTypeToFilterWith?: OrganizationTagType | null | undefined;
  searchBoundaries?: PolygonInput | null | undefined;
  userSignedIn: boolean;
};
export type pageHome_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocations_locations_query" | "marketplaceLocations_query">;
};
export type pageHome_rootQuery = {
  response: pageHome_rootQuery$data;
  variables: pageHome_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourceTypeToFilterWith"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "searchBoundaries"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "userSignedIn"
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v5 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "resourceType",
        "variableName": "resourceTypeToFilterWith"
      },
      {
        "kind": "Variable",
        "name": "searchBoundaries",
        "variableName": "searchBoundaries"
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
      (v3/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageHome_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "marketplaceLocations_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "marketplaceLocations_locations_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v0/*: any*/),
      (v1/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageHome_rootQuery",
    "selections": [
      {
        "condition": "userSignedIn",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDetails",
            "kind": "LinkedField",
            "name": "me",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationDetails",
                "kind": "LinkedField",
                "name": "favouriteLocations",
                "plural": true,
                "selections": [
                  (v4/*: any*/)
                ],
                "storageKey": null
              },
              (v4/*: any*/)
            ],
            "storageKey": null
          }
        ]
      },
      {
        "alias": null,
        "args": (v5/*: any*/),
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
                  (v4/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "name",
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
                      (v4/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "multilinesFormattedAddress",
                        "storageKey": null
                      }
                    ],
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
                            "name": "to",
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
                        "name": "thumbnail",
                        "plural": false,
                        "selections": [
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
          {
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
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v5/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "locations_marketplaceLocations",
        "kind": "LinkedHandle",
        "name": "marketplaceLocations"
      }
    ]
  },
  "params": {
    "cacheID": "b3ed99e713ed20ae2a8fe6c898a7e4c4",
    "id": null,
    "metadata": {},
    "name": "pageHome_rootQuery",
    "operationKind": "query",
    "text": "query pageHome_rootQuery(\n  $searchBoundaries: PolygonInput\n  $locationsSortingValues: [LocationOrderInput!]\n  $resourceTypeToFilterWith: OrganizationTagType\n  $userSignedIn: Boolean!\n) {\n  ...marketplaceLocations_query\n  ...marketplaceLocations_locations_query\n}\n\nfragment marketplaceLocationCard_LocationDetails on LocationDetails {\n  id\n  name\n  extraMetadata {\n    areaRange {\n      fromInSqm\n      toInSqm\n    }\n    peopleCapacity {\n      from\n      to\n    }\n  }\n  physicalAddress {\n    multilinesFormattedAddress\n    id\n  }\n  featureImages {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n}\n\nfragment marketplaceLocationCard_query on Query {\n  me @include(if: $userSignedIn) {\n    favouriteLocations {\n      id\n    }\n    id\n  }\n}\n\nfragment marketplaceLocations_locations_query on Query {\n  marketplaceLocations(where: {searchBoundaries: $searchBoundaries, resourceType: $resourceTypeToFilterWith}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        physicalAddress {\n          longitude\n          latitude\n          id\n        }\n        ...marketplaceLocationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment marketplaceLocations_query on Query {\n  ...marketplaceLocationCard_query\n}\n"
  }
};
})();

(node as any).hash = "e73b1eb6d0d82fa11163d13f2580ca05";

export default node;
