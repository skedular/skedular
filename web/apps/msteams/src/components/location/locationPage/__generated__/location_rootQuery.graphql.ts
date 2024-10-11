/**
 * @generated SignedSource<<f7702d0c5a7bf2d00536eefd623ee3d4>>
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
export type DeskOrderInput = {
  direction: OrderDirection;
  field: DeskOrderField;
};
export type LocationTagOrderInput = {
  direction: OrderDirection;
  field: LocationTagOrderField;
};
export type location_rootQuery$variables = {
  deskMultipleChoicesZonesSortingValues?: ReadonlyArray<LocationTagOrderInput> | null | undefined;
  deskNameSearchText?: string | null | undefined;
  deskSortingValues: ReadonlyArray<DeskOrderInput>;
  fromToGetBookings?: any | null | undefined;
  locationExists: boolean;
  locationId: string;
  toToGetBookings?: any | null | undefined;
  zoneTagType: string;
};
export type location_rootQuery$data = {
  readonly location: {
    readonly canViewAnalytics: boolean;
    readonly name: string;
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"locationDesksTab_query">;
};
export type location_rootQuery = {
  response: location_rootQuery$data;
  variables: location_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskMultipleChoicesZonesSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskNameSearchText"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskSortingValues"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "fromToGetBookings"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationExists"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "toToGetBookings"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneTagType"
},
v8 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "locationId"
  }
],
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canViewAnalytics",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v12 = [
  (v11/*: any*/)
],
v13 = {
  "alias": null,
  "args": null,
  "concreteType": "LocationOrganizationDetails",
  "kind": "LinkedField",
  "name": "organization",
  "plural": false,
  "selections": (v12/*: any*/),
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v15 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v16 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "deskSortingValues"
  },
  {
    "fields": [
      (v15/*: any*/),
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
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v20 = {
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
v21 = {
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
v22 = [
  "where",
  "orderBy"
],
v23 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "deskMultipleChoicesZonesSortingValues"
  },
  {
    "fields": [
      (v15/*: any*/),
      {
        "kind": "Variable",
        "name": "tagType",
        "variableName": "zoneTagType"
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
      (v7/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "location_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v8/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v9/*: any*/),
          (v10/*: any*/),
          (v13/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationDesksTab_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v5/*: any*/),
      (v4/*: any*/),
      (v7/*: any*/),
      (v3/*: any*/),
      (v6/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "location_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v8/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v9/*: any*/),
          (v10/*: any*/),
          (v13/*: any*/),
          (v14/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canModify",
            "storageKey": null
          }
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
            "args": (v16/*: any*/),
            "concreteType": "DeskConnection",
            "kind": "LinkedField",
            "name": "paginatedLocationDesks",
            "plural": false,
            "selections": [
              (v17/*: any*/),
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
                      (v14/*: any*/),
                      (v9/*: any*/),
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
                          (v14/*: any*/),
                          (v9/*: any*/)
                        ],
                        "storageKey": null
                      },
                      (v18/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v19/*: any*/)
                ],
                "storageKey": null
              },
              (v20/*: any*/),
              (v21/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v16/*: any*/),
            "filters": (v22/*: any*/),
            "handle": "connection",
            "key": "locationDesksTab_paginatedLocationDesks",
            "kind": "LinkedHandle",
            "name": "paginatedLocationDesks"
          },
          {
            "alias": null,
            "args": (v23/*: any*/),
            "concreteType": "LocationTagConnection",
            "kind": "LinkedField",
            "name": "paginatedLocationTags",
            "plural": false,
            "selections": [
              (v17/*: any*/),
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
                      (v14/*: any*/),
                      (v9/*: any*/),
                      (v18/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v19/*: any*/)
                ],
                "storageKey": null
              },
              (v20/*: any*/),
              (v21/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v23/*: any*/),
            "filters": (v22/*: any*/),
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
          (v14/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v12/*: any*/),
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
          (v14/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": [
              (v11/*: any*/),
              (v9/*: any*/),
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
            "selections": (v12/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "9dde98c423644365e4bd233a72f56284",
    "id": null,
    "metadata": {},
    "name": "location_rootQuery",
    "operationKind": "query",
    "text": "query location_rootQuery(\n  $locationId: String!\n  $locationExists: Boolean!\n  $zoneTagType: String!\n  $fromToGetBookings: DateTime\n  $toToGetBookings: DateTime\n  $deskNameSearchText: String\n  $deskSortingValues: [DeskOrderInput!]!\n  $deskMultipleChoicesZonesSortingValues: [LocationTagOrderInput!]\n) {\n  location(id: $locationId) {\n    name\n    canViewAnalytics\n    organization {\n      uniqueId\n    }\n    id\n  }\n  ...locationDesksTab_query\n}\n\nfragment bulkNewDeskDialog_query on Query {\n  ...deskMultipleChoicesZones_query\n}\n\nfragment deskCard_DeskDetails on DeskDetails {\n  id\n  name\n  deactivated\n  requireBookingApproval\n  locationTags {\n    id\n    name\n  }\n}\n\nfragment deskCard_query on Query {\n  me {\n    id\n    preferredDesks {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    canModify\n    id\n  }\n}\n\nfragment deskMultipleChoicesZones_query on Query {\n  paginatedLocationTags(where: {locationId: $locationId, tagType: $zoneTagType}, orderBy: $deskMultipleChoicesZonesSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationDesksTab_query on Query {\n  location(id: $locationId) {\n    canModify\n    id\n  }\n  paginatedLocationDesks(first: 50, where: {locationId: $locationId, nameContains: $deskNameSearchText}, orderBy: $deskSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...deskCard_DeskDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...deskCard_query\n  ...deskMultipleChoicesZones_query\n  allBookings(where: {locationIds: [$locationId], fromGTE: $fromToGetBookings, toLTE: $toToGetBookings}) {\n    id\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    desks {\n      uniqueId\n    }\n  }\n  ...newDeskDialog_query\n  ...bulkNewDeskDialog_query\n}\n\nfragment newDeskDialog_query on Query {\n  ...deskMultipleChoicesZones_query\n}\n"
  }
};
})();

(node as any).hash = "2e17cf0c294baf0c46c0412621823776";

export default node;
