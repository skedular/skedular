/**
 * @generated SignedSource<<0242846ad6e0e9c76a784d17559864d7>>
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
export type organization_rootQuery$variables = {
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaTo: any;
  locationIds: ReadonlyArray<string>;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  nullableOrganizationId?: string | null | undefined;
  organizationId: string;
  teamIds: ReadonlyArray<string>;
};
export type organization_rootQuery$data = {
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly name: string;
      readonly uniqueId: string;
    };
  }> | null | undefined;
  readonly myTeams: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly name: string;
      readonly uniqueId: string;
    };
  }>;
  readonly organization: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"gettingStarted_query" | "locationSelector_allLocations_query" | "myBookings_bookings_query" | "myBookings_query" | "teamSelector_allTeams_query">;
};
export type organization_rootQuery = {
  response: organization_rootQuery$data;
  variables: organization_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingsSearchCriteriaFrom"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingsSearchCriteriaTo"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationIds"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "nullableOrganizationId"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamIds"
},
v7 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationId"
  }
],
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v10 = [
  (v8/*: any*/),
  (v9/*: any*/)
],
v11 = [
  {
    "kind": "Variable",
    "name": "organizationId",
    "variableName": "nullableOrganizationId"
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
  (v12/*: any*/),
  (v9/*: any*/)
],
v14 = {
  "alias": null,
  "args": (v11/*: any*/),
  "concreteType": "LocationDetails",
  "kind": "LinkedField",
  "name": "myLocations",
  "plural": true,
  "selections": [
    (v8/*: any*/),
    (v9/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "Location_OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": (v13/*: any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": (v11/*: any*/),
  "concreteType": "TeamDetails",
  "kind": "LinkedField",
  "name": "myTeams",
  "plural": true,
  "selections": [
    (v8/*: any*/),
    (v9/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "Team_OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": (v13/*: any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v16 = {
  "fields": [
    {
      "kind": "Variable",
      "name": "organizationId",
      "variableName": "organizationId"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v18 = {
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
v19 = [
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "ASCENDING",
        "field": "FROM"
      }
    ]
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "fromGte",
        "variableName": "bookingsSearchCriteriaFrom"
      },
      {
        "kind": "Variable",
        "name": "fromLte",
        "variableName": "bookingsSearchCriteriaTo"
      },
      {
        "kind": "Variable",
        "name": "locationIds",
        "variableName": "locationIds"
      },
      {
        "items": [
          {
            "kind": "Variable",
            "name": "organizationIds.0",
            "variableName": "organizationId"
          }
        ],
        "kind": "ListValue",
        "name": "organizationIds"
      },
      {
        "kind": "Variable",
        "name": "teamIds",
        "variableName": "teamIds"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v21 = [
  (v12/*: any*/),
  (v9/*: any*/),
  (v20/*: any*/)
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
      (v6/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organization_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v7/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v10/*: any*/),
        "storageKey": null
      },
      (v14/*: any*/),
      (v15/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationSelector_allLocations_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "teamSelector_allTeams_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "gettingStarted_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "myBookings_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "myBookings_bookings_query"
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
      (v2/*: any*/),
      (v6/*: any*/),
      (v0/*: any*/),
      (v1/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Operation",
    "name": "organization_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v7/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v8/*: any*/),
          (v9/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "isMyOnboardingDone",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      (v14/*: any*/),
      (v15/*: any*/),
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          (v16/*: any*/)
        ],
        "concreteType": "LocationConnection",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v17/*: any*/),
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
                "selections": (v10/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v18/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          (v16/*: any*/)
        ],
        "concreteType": "TeamConnection",
        "kind": "LinkedField",
        "name": "teams",
        "plural": false,
        "selections": [
          (v17/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": (v10/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v18/*: any*/)
        ],
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
          (v8/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*: any*/),
        "concreteType": "BookingConnection",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v17/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v8/*: any*/),
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
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "notes",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Booking_CustomerDetails",
                    "kind": "LinkedField",
                    "name": "involvedCustomers",
                    "plural": true,
                    "selections": [
                      (v12/*: any*/),
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
                    "concreteType": "Booking_LocationDetails",
                    "kind": "LinkedField",
                    "name": "involvedLocations",
                    "plural": true,
                    "selections": (v13/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Booking_TeamDetails",
                    "kind": "LinkedField",
                    "name": "involvedTeams",
                    "plural": true,
                    "selections": (v13/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingResourceDetails",
                    "kind": "LinkedField",
                    "name": "resources",
                    "plural": true,
                    "selections": [
                      (v12/*: any*/),
                      (v9/*: any*/),
                      (v20/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Booking_OrganizationCustomTagDetails",
                        "kind": "LinkedField",
                        "name": "customTags",
                        "plural": true,
                        "selections": (v21/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Booking_OrganizationZoneDetails",
                        "kind": "LinkedField",
                        "name": "zones",
                        "plural": true,
                        "selections": (v21/*: any*/),
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
          (v18/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "myBookings_bookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "275fdf608ab738b7f3d3f84107d5d8f4",
    "id": null,
    "metadata": {},
    "name": "organization_rootQuery",
    "operationKind": "query",
    "text": "query organization_rootQuery(\n  $organizationId: String!\n  $nullableOrganizationId: String\n  $locationIds: [String!]!\n  $teamIds: [String!]!\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaTo: DateTime!\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  organization(id: $organizationId) {\n    id\n    name\n  }\n  myLocations(organizationId: $nullableOrganizationId) {\n    id\n    name\n    organization {\n      uniqueId\n      name\n    }\n  }\n  myTeams(organizationId: $nullableOrganizationId) {\n    id\n    name\n    organization {\n      uniqueId\n      name\n    }\n  }\n  ...locationSelector_allLocations_query\n  ...teamSelector_allTeams_query\n  ...gettingStarted_query\n  ...myBookings_query\n  ...myBookings_bookings_query\n}\n\nfragment gettingStarted_query on Query {\n  organization(id: $organizationId) {\n    isMyOnboardingDone\n    id\n  }\n}\n\nfragment locationSelector_allLocations_query on Query {\n  locations(where: {organizationId: $organizationId}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment myBookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  until\n  notes\n  involvedCustomers {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  involvedLocations {\n    uniqueId\n    name\n  }\n  involvedTeams {\n    uniqueId\n    name\n  }\n  resources {\n    uniqueId\n    name\n    color\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment myBookings_bookings_query on Query {\n  bookings(where: {organizationIds: [$organizationId], locationIds: $locationIds, teamIds: $teamIds, fromGte: $bookingsSearchCriteriaFrom, fromLte: $bookingsSearchCriteriaTo}, orderBy: [{field: FROM, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        until\n        notes\n        involvedCustomers {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        involvedLocations {\n          uniqueId\n          name\n        }\n        involvedTeams {\n          uniqueId\n          name\n        }\n        resources {\n          uniqueId\n          name\n          color\n          customTags {\n            uniqueId\n            name\n            color\n          }\n          zones {\n            uniqueId\n            name\n            color\n          }\n        }\n        ...myBookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment myBookings_query on Query {\n  me {\n    id\n  }\n}\n\nfragment teamSelector_allTeams_query on Query {\n  teams(where: {organizationId: $organizationId}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6e12d62e256b41f28ce42435fca29247";

export default node;
