/**
 * @generated SignedSource<<3399e0a4bfa90d7a04711d5c0eff2cbd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "ABOUT" | "NAME" | "TIMEZONE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type organizationLocations_locations_availableOrganizationResources_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  customTagIds?: ReadonlyArray<string> | null | undefined;
  fromTodayDate: any;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationUniqueAlphanumericName?: string | null | undefined;
  untilTodayDate: any;
  zoneIds?: ReadonlyArray<string> | null | undefined;
};
export type organizationLocations_locations_availableOrganizationResources_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationLocations_locations_availableOrganizationResources_query">;
};
export type organizationLocations_locations_availableOrganizationResources_refetchableFragment = {
  response: organizationLocations_locations_availableOrganizationResources_refetchableFragment$data;
  variables: organizationLocations_locations_availableOrganizationResources_refetchableFragment$variables;
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
    "name": "customTagIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "fromTodayDate"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationsSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationUniqueAlphanumericName"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "untilTodayDate"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "zoneIds"
  }
],
v1 = {
  "kind": "Variable",
  "name": "customTagIds",
  "variableName": "customTagIds"
},
v2 = {
  "kind": "Variable",
  "name": "organizationUniqueAlphanumericName",
  "variableName": "organizationUniqueAlphanumericName"
},
v3 = {
  "kind": "Variable",
  "name": "zoneIds",
  "variableName": "zoneIds"
},
v4 = [
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
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
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
  "name": "uniqueId",
  "storageKey": null
},
v8 = [
  (v7/*: any*/),
  (v6/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocations_locations_availableOrganizationResources_refetchableFragment",
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
        "name": "organizationLocations_locations_availableOrganizationResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocations_locations_availableOrganizationResources_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
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
                  (v5/*: any*/),
                  (v6/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Location_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "customTags",
                    "plural": true,
                    "selections": (v8/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Location_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "zones",
                    "plural": true,
                    "selections": (v8/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ResourceDetails",
                    "kind": "LinkedField",
                    "name": "resources",
                    "plural": true,
                    "selections": [
                      (v5/*: any*/)
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
                        "name": "formattedAddress",
                        "storageKey": null
                      },
                      (v5/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "multilinesFormattedAddress",
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
                        "name": "longitude",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "hasFutureBooking",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "canModify",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "canDelete",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Location_OrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "uniqueAlphanumericName",
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
                    "name": "primaryFeatureImage",
                    "plural": false,
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
        "args": (v4/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationLocations_locations",
        "kind": "LinkedHandle",
        "name": "locations"
      },
      {
        "alias": null,
        "args": [
          {
            "fields": [
              (v1/*: any*/),
              {
                "kind": "Variable",
                "name": "from",
                "variableName": "fromTodayDate"
              },
              (v2/*: any*/),
              {
                "kind": "Variable",
                "name": "until",
                "variableName": "untilTodayDate"
              },
              (v3/*: any*/)
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
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": [
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
    "cacheID": "2528227441c6c8c85859e6aec84fd7f9",
    "id": null,
    "metadata": {},
    "name": "organizationLocations_locations_availableOrganizationResources_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationLocations_locations_availableOrganizationResources_refetchableFragment(\n  $count: Int = null\n  $cursor: String\n  $customTagIds: [String!]\n  $fromTodayDate: DateTime!\n  $locationsSortingValues: [LocationOrderInput!]\n  $organizationUniqueAlphanumericName: String\n  $untilTodayDate: DateTime!\n  $zoneIds: [String!]\n) {\n  ...organizationLocations_locations_availableOrganizationResources_query_1G22uz\n}\n\nfragment locationCard_LocationDetails on LocationDetails {\n  id\n  name\n  customTags {\n    uniqueId\n    name\n    color\n  }\n  zones {\n    uniqueId\n    name\n    color\n  }\n  resources {\n    id\n  }\n  physicalAddress {\n    multilinesFormattedAddress\n    latitude\n    longitude\n    id\n  }\n  primaryFeatureImage {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n  hasFutureBooking\n  canModify\n  canDelete\n  organization {\n    uniqueAlphanumericName\n  }\n}\n\nfragment organizationLocations_locations_availableOrganizationResources_query_1G22uz on Query {\n  locations(first: $count, after: $cursor, where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, zoneIds: $zoneIds, customTagIds: $customTagIds}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        customTags {\n          uniqueId\n          name\n          color\n        }\n        zones {\n          uniqueId\n          name\n          color\n        }\n        resources {\n          id\n        }\n        physicalAddress {\n          formattedAddress\n          id\n        }\n        hasFutureBooking\n        canModify\n        canDelete\n        organization {\n          uniqueAlphanumericName\n        }\n        ...locationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  availableResources(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, from: $fromTodayDate, until: $untilTodayDate, zoneIds: $zoneIds, customTagIds: $customTagIds}) {\n    location {\n      uniqueId\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "54427292b4b950aff0d4de74fb08a465";

export default node;
