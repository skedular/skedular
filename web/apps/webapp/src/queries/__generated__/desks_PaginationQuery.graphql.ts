/**
 * @generated SignedSource<<a9dfa195a373d770bbfbf1d5f9c2f0ea>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type DeskOrderField = "name" | "%future added value";
export type LocationTagOrderField = "description" | "name" | "tagType" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type LocationTagOrderInput = {
  direction: OrderDirection;
  field: LocationTagOrderField;
};
export type DeskOrderInput = {
  direction: OrderDirection;
  field: DeskOrderField;
};
export type desks_PaginationQuery$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  deskMultipleChoicesZonesSortingValues?: ReadonlyArray<LocationTagOrderInput> | null | undefined;
  deskNameSearchText?: string | null | undefined;
  deskSortingValues?: ReadonlyArray<DeskOrderInput> | null | undefined;
  fromToGetBookings?: any | null | undefined;
  locationExists: boolean;
  locationId: string;
  toToGetBookings?: any | null | undefined;
  zoneTagType?: string | null | undefined;
};
export type desks_PaginationQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationDesksTab_query">;
};
export type desks_PaginationQuery = {
  response: desks_PaginationQuery$data;
  variables: desks_PaginationQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": 50,
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
    "name": "deskMultipleChoicesZonesSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "deskNameSearchText"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "deskSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "fromToGetBookings"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationExists"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "toToGetBookings"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "zoneTagType"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v3 = [
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
    "variableName": "deskSortingValues"
  },
  {
    "fields": [
      (v2/*: any*/),
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "deskNameSearchText"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
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
  "name": "__typename",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v8 = {
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
v9 = {
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
v10 = [
  "where",
  "orderBy"
],
v11 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "deskMultipleChoicesZonesSortingValues"
  },
  {
    "fields": [
      (v2/*: any*/),
      {
        "kind": "Variable",
        "name": "tagType",
        "variableName": "zoneTagType"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v13 = [
  (v12/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "desks_PaginationQuery",
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
        "name": "locationDesksTab_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "desks_PaginationQuery",
    "selections": [
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canModify",
            "storageKey": null
          },
          (v1/*: any*/)
        ],
        "storageKey": null
      },
      {
        "condition": "locationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v3/*: any*/),
            "concreteType": "DeskConnection",
            "kind": "LinkedField",
            "name": "paginatedLocationDesks",
            "plural": false,
            "selections": [
              (v4/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "DeskEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "DeskDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v1/*: any*/),
                      (v5/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "deactivated",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "requireBookingApproval",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "LocationTagDetails",
                        "kind": "LinkedField",
                        "name": "locationTags",
                        "plural": true,
                        "selections": [
                          (v1/*: any*/),
                          (v5/*: any*/)
                        ],
                        "storageKey": null
                      },
                      (v6/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v7/*: any*/)
                ],
                "storageKey": null
              },
              (v8/*: any*/),
              (v9/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v3/*: any*/),
            "filters": (v10/*: any*/),
            "handle": "connection",
            "key": "locationDesksTab_paginatedLocationDesks",
            "kind": "LinkedHandle",
            "name": "paginatedLocationDesks"
          },
          {
            "alias": null,
            "args": (v11/*: any*/),
            "concreteType": "LocationTagConnection",
            "kind": "LinkedField",
            "name": "paginatedLocationTags",
            "plural": false,
            "selections": [
              (v4/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationTagEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationTagDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v1/*: any*/),
                      (v5/*: any*/),
                      (v6/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v7/*: any*/)
                ],
                "storageKey": null
              },
              (v8/*: any*/),
              (v9/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v11/*: any*/),
            "filters": (v10/*: any*/),
            "handle": "connection",
            "key": "locationZonesTab_paginatedLocationTags",
            "kind": "LinkedHandle",
            "name": "paginatedLocationTags"
          }
        ]
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v13/*: any*/),
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
                "kind": "Variable",
                "name": "fromGTE",
                "variableName": "fromToGetBookings"
              },
              {
                "items": [
                  {
                    "kind": "Variable",
                    "name": "locationIds.0",
                    "variableName": "locationId"
                  }
                ],
                "kind": "ListValue",
                "name": "locationIds"
              },
              {
                "kind": "Variable",
                "name": "toLTE",
                "variableName": "toToGetBookings"
              }
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "allBookings",
        "plural": true,
        "selections": [
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": [
              (v12/*: any*/),
              (v5/*: any*/),
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
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDeskDetails",
            "kind": "LinkedField",
            "name": "desks",
            "plural": true,
            "selections": (v13/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "546f5d93d1105db54e4451fca90e7301",
    "id": null,
    "metadata": {},
    "name": "desks_PaginationQuery",
    "operationKind": "query",
    "text": "query desks_PaginationQuery(\n  $count: Int = 50\n  $cursor: String\n  $deskMultipleChoicesZonesSortingValues: [LocationTagOrderInput!]\n  $deskNameSearchText: String\n  $deskSortingValues: [DeskOrderInput!]\n  $fromToGetBookings: DateTime\n  $locationExists: Boolean!\n  $locationId: String!\n  $toToGetBookings: DateTime\n  $zoneTagType: String\n) {\n  ...locationDesksTab_query_1G22uz\n}\n\nfragment bulkNewDeskDialog_query on Query {\n  ...deskMultipleChoicesZones_query\n}\n\nfragment deskCard_DeskDetails on DeskDetails {\n  id\n  name\n  deactivated\n  requireBookingApproval\n  locationTags {\n    id\n    name\n  }\n}\n\nfragment deskCard_query on Query {\n  me {\n    id\n    preferredDesks {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    canModify\n    id\n  }\n}\n\nfragment deskMultipleChoicesZones_query on Query {\n  paginatedLocationTags(where: {locationId: $locationId, tagType: $zoneTagType}, orderBy: $deskMultipleChoicesZonesSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationDesksTab_query_1G22uz on Query {\n  location(id: $locationId) {\n    canModify\n    id\n  }\n  paginatedLocationDesks(first: $count, after: $cursor, where: {locationId: $locationId, nameContains: $deskNameSearchText}, orderBy: $deskSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...deskCard_DeskDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...deskCard_query\n  ...deskMultipleChoicesZones_query\n  allBookings(where: {locationIds: [$locationId], fromGTE: $fromToGetBookings, toLTE: $toToGetBookings}) {\n    id\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    desks {\n      uniqueId\n    }\n  }\n  ...newDeskDialog_query\n  ...bulkNewDeskDialog_query\n}\n\nfragment newDeskDialog_query on Query {\n  ...deskMultipleChoicesZones_query\n}\n"
  }
};
})();

(node as any).hash = "b1be78d6a8bf7b7f2aa14cbce637face";

export default node;
