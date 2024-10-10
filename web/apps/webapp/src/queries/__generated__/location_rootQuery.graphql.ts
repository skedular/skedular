/**
 * @generated SignedSource<<83da1ab651fad76c59884516c46c8fbb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CustomerOrderField = "designation" | "familyName" | "givenName" | "locale" | "middleName" | "name" | "timezone" | "title" | "%future added value";
export type DeskOrderField = "name" | "%future added value";
export type LocationMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type LocationTagOrderField = "description" | "name" | "tagType" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type LocationMemberOrderInput = {
  direction: OrderDirection;
  field: LocationMemberOrderField;
};
export type CustomerOrderInput = {
  direction: OrderDirection;
  field: CustomerOrderField;
};
export type LocationTagOrderInput = {
  direction: OrderDirection;
  field: LocationTagOrderField;
};
export type DeskOrderInput = {
  direction: OrderDirection;
  field: DeskOrderField;
};
export type location_rootQuery$variables = {
  deskMultipleChoicesZonesSortingValues?: ReadonlyArray<LocationTagOrderInput> | null | undefined;
  deskNameSearchText?: string | null | undefined;
  deskSortingValues: ReadonlyArray<DeskOrderInput>;
  fromToGetBookings?: any | null | undefined;
  locationExists: boolean;
  locationId: string;
  locationOrganizationPeopleSortingValues?: ReadonlyArray<CustomerOrderInput> | null | undefined;
  locationPeopleSortingValues?: ReadonlyArray<LocationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
  toToGetBookings?: any | null | undefined;
  zoneNameSearchText?: string | null | undefined;
  zoneSortingValues: ReadonlyArray<LocationTagOrderInput>;
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
  readonly " $fragmentSpreads": FragmentRefs<"locationDesksTab_query" | "locationPeopleTab_query" | "locationPeopleTab_query_organizationMembers" | "locationZonesTab_query">;
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
  "name": "locationOrganizationPeopleSortingValues"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationPeopleSortingValues"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "toToGetBookings"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneNameSearchText"
},
v11 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneSortingValues"
},
v12 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneTagType"
},
v13 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "locationId"
  }
],
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canViewAnalytics",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v17 = [
  (v16/*: any*/)
],
v18 = {
  "alias": null,
  "args": null,
  "concreteType": "LocationOrganizationDetails",
  "kind": "LinkedField",
  "name": "organization",
  "plural": false,
  "selections": (v17/*: any*/),
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v20 = {
  "kind": "Literal",
  "name": "first",
  "value": 50
},
v21 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v22 = {
  "fields": [
    (v21/*: any*/),
    {
      "kind": "Variable",
      "name": "nameContains",
      "variableName": "peopleNameSearchText"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v23 = [
  (v20/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationPeopleSortingValues"
  },
  (v22/*: any*/)
],
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v27 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v29 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v30 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v31 = {
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
v32 = {
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
v33 = [
  "where",
  "orderBy"
],
v34 = [
  (v20/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationOrganizationPeopleSortingValues"
  },
  (v22/*: any*/)
],
v35 = {
  "kind": "Variable",
  "name": "tagType",
  "variableName": "zoneTagType"
},
v36 = [
  (v20/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "zoneSortingValues"
  },
  {
    "fields": [
      (v21/*: any*/),
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "zoneNameSearchText"
      },
      (v35/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v37 = [
  (v24/*: any*/),
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
          (v19/*: any*/),
          (v14/*: any*/),
          (v29/*: any*/)
        ],
        "storageKey": null
      },
      (v30/*: any*/)
    ],
    "storageKey": null
  },
  (v31/*: any*/),
  (v32/*: any*/)
],
v38 = [
  (v20/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "deskSortingValues"
  },
  {
    "fields": [
      (v21/*: any*/),
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
v39 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "deskMultipleChoicesZonesSortingValues"
  },
  {
    "fields": [
      (v21/*: any*/),
      (v35/*: any*/)
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
      (v8/*: any*/),
      (v9/*: any*/),
      (v10/*: any*/),
      (v11/*: any*/),
      (v12/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "location_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v13/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v14/*: any*/),
          (v15/*: any*/),
          (v18/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationPeopleTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationPeopleTab_query_organizationMembers"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationZonesTab_query"
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
      (v12/*: any*/),
      (v3/*: any*/),
      (v9/*: any*/),
      (v8/*: any*/),
      (v10/*: any*/),
      (v1/*: any*/),
      (v7/*: any*/),
      (v6/*: any*/),
      (v11/*: any*/),
      (v2/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "location_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v13/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v14/*: any*/),
          (v15/*: any*/),
          (v18/*: any*/),
          (v19/*: any*/),
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
            "args": (v23/*: any*/),
            "concreteType": "LocationMemberConnection",
            "kind": "LinkedField",
            "name": "paginatedLocationMembers",
            "plural": false,
            "selections": [
              (v24/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationMemberEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationMemberDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v19/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "membershipType",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "LocationCustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": [
                          (v14/*: any*/),
                          (v25/*: any*/),
                          (v26/*: any*/),
                          (v27/*: any*/),
                          (v28/*: any*/)
                        ],
                        "storageKey": null
                      },
                      (v29/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v30/*: any*/)
                ],
                "storageKey": null
              },
              (v31/*: any*/),
              (v32/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v23/*: any*/),
            "filters": (v33/*: any*/),
            "handle": "connection",
            "key": "locationPeopleTab_paginatedLocationMembers",
            "kind": "LinkedHandle",
            "name": "paginatedLocationMembers"
          },
          {
            "alias": null,
            "args": (v34/*: any*/),
            "concreteType": "CustomerConnection",
            "kind": "LinkedField",
            "name": "paginatedCustomersByDefaultLocation",
            "plural": false,
            "selections": [
              (v24/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "CustomerEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CustomerDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v19/*: any*/),
                      (v14/*: any*/),
                      (v25/*: any*/),
                      (v26/*: any*/),
                      (v27/*: any*/),
                      (v28/*: any*/),
                      (v29/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v30/*: any*/)
                ],
                "storageKey": null
              },
              (v31/*: any*/),
              (v32/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v34/*: any*/),
            "filters": (v33/*: any*/),
            "handle": "connection",
            "key": "locationPeopleTab_paginatedCustomersByDefaultLocation",
            "kind": "LinkedHandle",
            "name": "paginatedCustomersByDefaultLocation"
          },
          {
            "alias": "locationZonesTabPaginatedTags",
            "args": (v36/*: any*/),
            "concreteType": "LocationTagConnection",
            "kind": "LinkedField",
            "name": "paginatedLocationTags",
            "plural": false,
            "selections": (v37/*: any*/),
            "storageKey": null
          },
          {
            "alias": "locationZonesTabPaginatedTags",
            "args": (v36/*: any*/),
            "filters": (v33/*: any*/),
            "handle": "connection",
            "key": "locationZonesTab_locationZonesTabPaginatedTags",
            "kind": "LinkedHandle",
            "name": "paginatedLocationTags"
          },
          {
            "alias": null,
            "args": (v38/*: any*/),
            "concreteType": "DeskConnection",
            "kind": "LinkedField",
            "name": "paginatedLocationDesks",
            "plural": false,
            "selections": [
              (v24/*: any*/),
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
                      (v19/*: any*/),
                      (v14/*: any*/),
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
                          (v19/*: any*/),
                          (v14/*: any*/)
                        ],
                        "storageKey": null
                      },
                      (v29/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v30/*: any*/)
                ],
                "storageKey": null
              },
              (v31/*: any*/),
              (v32/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v38/*: any*/),
            "filters": (v33/*: any*/),
            "handle": "connection",
            "key": "locationDesksTab_paginatedLocationDesks",
            "kind": "LinkedHandle",
            "name": "paginatedLocationDesks"
          },
          {
            "alias": null,
            "args": (v39/*: any*/),
            "concreteType": "LocationTagConnection",
            "kind": "LinkedField",
            "name": "paginatedLocationTags",
            "plural": false,
            "selections": (v37/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v39/*: any*/),
            "filters": (v33/*: any*/),
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
        "kind": "ScalarField",
        "name": "locationMemberMembershipTypes",
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
          (v19/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerLocationTagDetails",
            "kind": "LinkedField",
            "name": "preferredZones",
            "plural": true,
            "selections": (v17/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v17/*: any*/),
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
          (v19/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": [
              (v16/*: any*/),
              (v14/*: any*/),
              (v25/*: any*/),
              (v26/*: any*/),
              (v27/*: any*/),
              (v28/*: any*/)
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
            "selections": (v17/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "58ce1e5d9dfa947f92b655c0d211b071",
    "id": null,
    "metadata": {},
    "name": "location_rootQuery",
    "operationKind": "query",
    "text": "query location_rootQuery(\n  $locationId: String!\n  $locationExists: Boolean!\n  $zoneTagType: String!\n  $fromToGetBookings: DateTime\n  $toToGetBookings: DateTime\n  $peopleNameSearchText: String\n  $zoneNameSearchText: String\n  $deskNameSearchText: String\n  $locationPeopleSortingValues: [LocationMemberOrderInput!]\n  $locationOrganizationPeopleSortingValues: [CustomerOrderInput!]\n  $zoneSortingValues: [LocationTagOrderInput!]!\n  $deskSortingValues: [DeskOrderInput!]!\n  $deskMultipleChoicesZonesSortingValues: [LocationTagOrderInput!]\n) {\n  location(id: $locationId) {\n    name\n    canViewAnalytics\n    organization {\n      uniqueId\n    }\n    id\n  }\n  ...locationPeopleTab_query\n  ...locationPeopleTab_query_organizationMembers\n  ...locationZonesTab_query\n  ...locationDesksTab_query\n}\n\nfragment bulkNewDeskDialog_query on Query {\n  ...deskMultipleChoicesZones_query\n}\n\nfragment customerCard_CustomerDetails on CustomerDetails {\n  name\n  givenName\n  middleName\n  familyName\n  photoUrl\n}\n\nfragment deskCard_DeskDetails on DeskDetails {\n  id\n  name\n  deactivated\n  requireBookingApproval\n  locationTags {\n    id\n    name\n  }\n}\n\nfragment deskCard_query on Query {\n  me {\n    id\n    preferredDesks {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    canModify\n    id\n  }\n}\n\nfragment deskMultipleChoicesZones_query on Query {\n  paginatedLocationTags(where: {locationId: $locationId, tagType: $zoneTagType}, orderBy: $deskMultipleChoicesZonesSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationDesksTab_query on Query {\n  location(id: $locationId) {\n    canModify\n    id\n  }\n  paginatedLocationDesks(first: 50, where: {locationId: $locationId, nameContains: $deskNameSearchText}, orderBy: $deskSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...deskCard_DeskDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...deskCard_query\n  ...deskMultipleChoicesZones_query\n  allBookings(where: {locationIds: [$locationId], fromGTE: $fromToGetBookings, toLTE: $toToGetBookings}) {\n    id\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    desks {\n      uniqueId\n    }\n  }\n  ...newDeskDialog_query\n  ...bulkNewDeskDialog_query\n}\n\nfragment locationMemberCard_LocationMemberDetails on LocationMemberDetails {\n  id\n  membershipType\n  customer {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n}\n\nfragment locationPeopleTab_query on Query {\n  location(id: $locationId) {\n    id\n    name\n  }\n  paginatedLocationMembers(first: 50, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationPeopleSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...locationMemberCard_LocationMemberDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...locationSingleChoiceMembershipType_query\n}\n\nfragment locationPeopleTab_query_organizationMembers on Query {\n  paginatedCustomersByDefaultLocation(first: 50, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationOrganizationPeopleSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...customerCard_CustomerDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationSingleChoiceMembershipType_query on Query {\n  locationMemberMembershipTypes\n}\n\nfragment locationZonesTab_query on Query {\n  location(id: $locationId) {\n    canModify\n    id\n  }\n  locationZonesTabPaginatedTags: paginatedLocationTags(first: 50, where: {locationId: $locationId, tagType: $zoneTagType, nameContains: $zoneNameSearchText}, orderBy: $zoneSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...zoneCard_LocationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...zoneCard_Query\n}\n\nfragment newDeskDialog_query on Query {\n  ...deskMultipleChoicesZones_query\n}\n\nfragment zoneCard_LocationTagDetails on LocationTagDetails {\n  id\n  name\n}\n\nfragment zoneCard_Query on Query {\n  me {\n    id\n    preferredZones {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    canModify\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "cd20e7b6da5054b75f674ddd442fd84d";

export default node;
