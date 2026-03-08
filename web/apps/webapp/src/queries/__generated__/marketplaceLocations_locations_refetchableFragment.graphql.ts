/**
 * @generated SignedSource<<5530534545804fc086de5dce600530a4>>
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
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type PolygonInput = {
  northEast: PointCoordinatesInput;
  southWest: PointCoordinatesInput;
};
export type PointCoordinatesInput = {
  latitude: number;
  longitude: number;
};
export type marketplaceLocations_locations_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  resourceTypeToFilterWith?: OrganizationTagType | null | undefined;
  searchBoundaries?: PolygonInput | null | undefined;
};
export type marketplaceLocations_locations_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocations_locations_query">;
};
export type marketplaceLocations_locations_refetchableFragment = {
  response: marketplaceLocations_locations_refetchableFragment$data;
  variables: marketplaceLocations_locations_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "count"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "cursor"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationsSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "resourceTypeToFilterWith"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "searchBoundaries"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "after",
    "variableName": "cursor"
  },
  {
    "kind": "Variable",
    "name": "first",
    "variableName": "count"
  },
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
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceLocations_locations_refetchableFragment",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "count",
            "variableName": "count"
          },
          {
            "kind": "Variable",
            "name": "cursor",
            "variableName": "cursor"
          }
        ],
        "kind": "FragmentSpread",
        "name": "marketplaceLocations_locations_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "marketplaceLocations_locations_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
                  (v2/*: any*/),
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
                      (v2/*: any*/),
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
        "args": (v1/*: any*/),
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
    "cacheID": "4620d7fe990c07378d0ecbb632ab1770",
    "id": null,
    "metadata": {},
    "name": "marketplaceLocations_locations_refetchableFragment",
    "operationKind": "query",
    "text": "query marketplaceLocations_locations_refetchableFragment(\n  $count: Int = null\n  $cursor: String\n  $locationsSortingValues: [LocationOrderInput!]\n  $resourceTypeToFilterWith: OrganizationTagType\n  $searchBoundaries: PolygonInput\n) {\n  ...marketplaceLocations_locations_query_1G22uz\n}\n\nfragment marketplaceLocationCard_LocationDetails on LocationDetails {\n  id\n  name\n  extraMetadata {\n    areaRange {\n      fromInSqm\n      toInSqm\n    }\n    peopleCapacity {\n      from\n      to\n    }\n  }\n  physicalAddress {\n    multilinesFormattedAddress\n    id\n  }\n  featureImages {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n}\n\nfragment marketplaceLocations_locations_query_1G22uz on Query {\n  marketplaceLocations(first: $count, after: $cursor, where: {searchBoundaries: $searchBoundaries, resourceType: $resourceTypeToFilterWith}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        physicalAddress {\n          longitude\n          latitude\n          id\n        }\n        ...marketplaceLocationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6152d51ecf4ac3a45aafd0a36f9715b7";

export default node;
