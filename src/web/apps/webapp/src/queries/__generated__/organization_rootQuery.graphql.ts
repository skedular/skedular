/**
 * @generated SignedSource<<dccab16073b48e18d470ffde6a7c0439>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "NAME" | "TIMEZONE" | "TYPE" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
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
  organizationCustomDomain: string;
  teamIds: ReadonlyArray<string>;
};
export type organization_rootQuery$data = {
  readonly marketplaceBookingSubscriptionCancellationModes: ReadonlyArray<{
    readonly name: string;
    readonly type: MarketplaceBookingSubscriptionCancellationMode;
  }>;
  readonly marketplaceBookingSubscriptions: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly recurringBookings: ReadonlyArray<{
          readonly id: string;
        }>;
      };
    }>;
  };
  readonly organization: {
    readonly canModify: boolean;
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
  "name": "organizationCustomDomain"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamIds"
},
v6 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v9 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v8/*:: as any*/)
],
v10 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
  "kind": "LinkedField",
  "name": "marketplaceBookingSubscriptionCancellationModes",
  "plural": true,
  "selections": (v9/*:: as any*/),
  "storageKey": null
},
v11 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v13 = [
  (v12/*:: as any*/)
],
v14 = {
  "alias": null,
  "args": [
    {
      "kind": "Literal",
      "name": "first",
      "value": 100
    },
    {
      "fields": [
        {
          "kind": "Literal",
          "name": "includeMineOnly",
          "value": true
        },
        (v11/*:: as any*/)
      ],
      "kind": "ObjectValue",
      "name": "where"
    }
  ],
  "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
  "kind": "LinkedField",
  "name": "marketplaceBookingSubscriptions",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingSubscriptionEdge",
      "kind": "LinkedField",
      "name": "edges",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceBookingSubscriptionDetails",
          "kind": "LinkedField",
          "name": "node",
          "plural": false,
          "selections": [
            (v12/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "RecurringBookingDetails",
              "kind": "LinkedField",
              "name": "recurringBookings",
              "plural": true,
              "selections": (v13/*:: as any*/),
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
v15 = {
  "fields": [
    (v11/*:: as any*/)
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v17 = [
  (v12/*:: as any*/),
  (v8/*:: as any*/)
],
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
      (v11/*:: as any*/),
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
  (v12/*:: as any*/),
  (v8/*:: as any*/),
  (v20/*:: as any*/)
],
v22 = [
  (v8/*:: as any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organization_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v6/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v7/*:: as any*/)
        ],
        "storageKey": null
      },
      (v10/*:: as any*/),
      (v14/*:: as any*/),
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
      (v4/*:: as any*/),
      (v2/*:: as any*/),
      (v5/*:: as any*/),
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v3/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "organization_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v6/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v7/*:: as any*/),
          (v12/*:: as any*/),
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
      (v10/*:: as any*/),
      (v14/*:: as any*/),
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          (v15/*:: as any*/)
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v16/*:: as any*/),
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
                "selections": (v17/*:: as any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v18/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          (v15/*:: as any*/)
        ],
        "concreteType": "ConnectionOfTeamEdge",
        "kind": "LinkedField",
        "name": "teams",
        "plural": false,
        "selections": [
          (v16/*:: as any*/),
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
                "selections": (v17/*:: as any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v18/*:: as any*/)
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
        "selections": (v13/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v16/*:: as any*/),
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
                  (v12/*:: as any*/),
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
                    "concreteType": "BookingChannelDetails",
                    "kind": "LinkedField",
                    "name": "channel",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "channel",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CustomerDetails",
                    "kind": "LinkedField",
                    "name": "involvedCustomers",
                    "plural": true,
                    "selections": [
                      (v12/*:: as any*/),
                      (v8/*:: as any*/),
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
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "uniqueId",
                        "storageKey": null
                      },
                      (v8/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamDetails",
                    "kind": "LinkedField",
                    "name": "involvedTeams",
                    "plural": true,
                    "selections": (v17/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingResourceDetails",
                    "kind": "LinkedField",
                    "name": "bookingResources",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ResourceDetails",
                        "kind": "LinkedField",
                        "name": "resource",
                        "plural": false,
                        "selections": [
                          (v12/*:: as any*/),
                          (v8/*:: as any*/),
                          (v20/*:: as any*/),
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OrganizationTagDetails",
                            "kind": "LinkedField",
                            "name": "customTags",
                            "plural": true,
                            "selections": (v21/*:: as any*/),
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "OrganizationTagDetails",
                            "kind": "LinkedField",
                            "name": "zones",
                            "plural": true,
                            "selections": (v21/*:: as any*/),
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
                    "concreteType": "MarketplaceBookingFailureDetails",
                    "kind": "LinkedField",
                    "name": "failure",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "MarketplaceBookingFailureChoiceDetails",
                        "kind": "LinkedField",
                        "name": "category",
                        "plural": false,
                        "selections": (v22/*:: as any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "MarketplaceBookingFailureChoiceDetails",
                        "kind": "LinkedField",
                        "name": "customerAction",
                        "plural": false,
                        "selections": (v22/*:: as any*/),
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "isPaymentRequired",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "PaymentStatusDetails",
                        "kind": "LinkedField",
                        "name": "paymentStatus",
                        "plural": false,
                        "selections": (v9/*:: as any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "invoiceUrl",
                        "storageKey": null
                      },
                      (v12/*:: as any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "RecurringBookingDetails",
                    "kind": "LinkedField",
                    "name": "recurringBooking",
                    "plural": false,
                    "selections": [
                      (v12/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "startDate",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "endDate",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "BookingFrequencyDetails",
                        "kind": "LinkedField",
                        "name": "frequency",
                        "plural": false,
                        "selections": (v22/*:: as any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "MarketplaceBookingDetails",
                        "kind": "LinkedField",
                        "name": "marketplaceBooking",
                        "plural": false,
                        "selections": (v13/*:: as any*/),
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
          (v18/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*:: as any*/),
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
    "cacheID": "3c5c3ba209ce690bfe5493af6e68ff56",
    "id": null,
    "metadata": {},
    "name": "organization_rootQuery",
    "operationKind": "query",
    "text": "query organization_rootQuery(\n  $organizationCustomDomain: String!\n  $locationIds: [String!]!\n  $teamIds: [String!]!\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaTo: DateTime!\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    canModify\n    id\n  }\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  marketplaceBookingSubscriptions(first: 100, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain}) {\n    edges {\n      node {\n        id\n        recurringBookings {\n          id\n        }\n      }\n    }\n  }\n  ...locationSelector_allLocations_query\n  ...teamSelector_allTeams_query\n  ...gettingStarted_query\n  ...myBookings_query\n  ...myBookings_bookings_query\n}\n\nfragment gettingStarted_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    isMyOnboardingDone\n    id\n  }\n}\n\nfragment locationSelector_allLocations_query on Query {\n  locations(where: {organizationCustomDomain: $organizationCustomDomain}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment myBookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  until\n  notes\n  channel {\n    channel\n  }\n  failure {\n    category {\n      name\n    }\n    customerAction {\n      name\n    }\n  }\n  involvedCustomers {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  involvedLocations {\n    uniqueId\n    name\n  }\n  involvedTeams {\n    id\n    name\n  }\n  bookingResources {\n    resource {\n      id\n      name\n      color\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n    }\n  }\n  marketplaceBooking {\n    isPaymentRequired\n    paymentStatus {\n      type\n      name\n    }\n    invoiceUrl\n    id\n  }\n  recurringBooking {\n    id\n    startDate\n    endDate\n    frequency {\n      name\n    }\n    marketplaceBooking {\n      id\n    }\n  }\n}\n\nfragment myBookings_bookings_query on Query {\n  bookings(where: {organizationCustomDomain: $organizationCustomDomain, locationIds: $locationIds, teamIds: $teamIds, fromGte: $bookingsSearchCriteriaFrom, fromLte: $bookingsSearchCriteriaTo}, orderBy: [{field: FROM, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        until\n        notes\n        channel {\n          channel\n        }\n        involvedCustomers {\n          id\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        involvedLocations {\n          uniqueId\n          name\n        }\n        involvedTeams {\n          id\n          name\n        }\n        bookingResources {\n          resource {\n            id\n            name\n            color\n            customTags {\n              id\n              name\n              color\n            }\n            zones {\n              id\n              name\n              color\n            }\n          }\n        }\n        ...myBookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment myBookings_query on Query {\n  me {\n    id\n  }\n  marketplaceBookingSubscriptionCancellationModes {\n    type\n    name\n  }\n  marketplaceBookingSubscriptions(first: 100, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain}) {\n    edges {\n      node {\n        id\n        recurringBookings {\n          id\n        }\n      }\n    }\n  }\n}\n\nfragment teamSelector_allTeams_query on Query {\n  teams(where: {organizationCustomDomain: $organizationCustomDomain}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ef2c0358ba6511c5cd40a6a4033cc103";

export default node;
