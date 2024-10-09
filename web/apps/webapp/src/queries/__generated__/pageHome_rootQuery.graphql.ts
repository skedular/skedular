/**
 * @generated SignedSource<<3c5949bce0e084fd84c71ed4ec88b5e8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type BookingOrderField = "familyName" | "from" | "givenName" | "locationName" | "middleName" | "name" | "notes" | "organizationName" | "teamName" | "to" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type BookingOrderInput = {
  direction: OrderDirection;
  field: BookingOrderField;
};
export type pageHome_rootQuery$variables = {
  bookingDetailsSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  bookingPeopleNameSearchText?: string | null | undefined;
  dateToGetAvailableDesks: any;
  deskIdsToIncludeToGetAvailableDesks: ReadonlyArray<string>;
  locationExists: boolean;
  locationId: string;
  monthlyCalendarDateFrom: any;
  monthlyCalendarDateTo: any;
  organizationExists: boolean;
  organizationId: string;
  smallMonthlyViewCalendarBookingsSortingValues?: ReadonlyArray<BookingOrderInput> | null | undefined;
};
export type pageHome_rootQuery$data = {
  readonly bookingCustomerRecordSynced: boolean;
  readonly organizationCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"rootShell_query" | "smallMonthlyViewCalendar_query">;
};
export type pageHome_rootQuery = {
  response: pageHome_rootQuery$data;
  variables: pageHome_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingDetailsSelectorOrganizationMembersSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingPeopleNameSearchText"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateToGetAvailableDesks"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskIdsToIncludeToGetAvailableDesks"
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
  "name": "monthlyCalendarDateFrom"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "monthlyCalendarDateTo"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "smallMonthlyViewCalendarBookingsSortingValues"
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "bookingCustomerRecordSynced",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "organizationCustomerRecordSynced",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v20 = [
  (v19/*: any*/)
],
v21 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 1000
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "smallMonthlyViewCalendarBookingsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "fromGTE",
        "variableName": "monthlyCalendarDateFrom"
      },
      {
        "kind": "Literal",
        "name": "includeMineOnly",
        "value": true
      },
      {
        "kind": "Variable",
        "name": "toLT",
        "variableName": "monthlyCalendarDateTo"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v22 = [
  (v19/*: any*/),
  (v18/*: any*/)
],
v23 = [
  (v19/*: any*/),
  (v18/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingLocationTagDetails",
    "kind": "LinkedField",
    "name": "locationTags",
    "plural": true,
    "selections": [
      (v19/*: any*/),
      (v18/*: any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "tagType",
        "storageKey": null
      }
    ],
    "storageKey": null
  }
],
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v26 = {
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
v27 = {
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
v28 = [
  "where",
  "orderBy"
],
v29 = [
  (v13/*: any*/),
  (v18/*: any*/)
],
v30 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v31 = [
  (v30/*: any*/)
],
v32 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 20
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "bookingDetailsSelectorOrganizationMembersSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "bookingPeopleNameSearchText"
      },
      (v30/*: any*/)
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
      (v10/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageHome_rootQuery",
    "selections": [
      (v11/*: any*/),
      (v12/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "rootShell_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "smallMonthlyViewCalendar_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v9/*: any*/),
      (v8/*: any*/),
      (v5/*: any*/),
      (v4/*: any*/),
      (v6/*: any*/),
      (v7/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/),
      (v10/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageHome_rootQuery",
    "selections": [
      (v11/*: any*/),
      (v12/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v13/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerEmail",
            "kind": "LinkedField",
            "name": "email",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
              (v13/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "verified",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "title",
            "storageKey": null
          },
          (v14/*: any*/),
          (v15/*: any*/),
          (v16/*: any*/),
          (v17/*: any*/),
          (v18/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerOrganizationDetails",
            "kind": "LinkedField",
            "name": "defaultOrganization",
            "plural": false,
            "selections": (v20/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v20/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": "monthlyBookings",
        "args": (v21/*: any*/),
        "concreteType": "BookingConnection",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
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
                  (v13/*: any*/),
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
                    "concreteType": "BookingCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": [
                      (v17/*: any*/),
                      (v19/*: any*/),
                      (v18/*: any*/),
                      (v14/*: any*/),
                      (v15/*: any*/),
                      (v16/*: any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v22/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v22/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v22/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": (v23/*: any*/),
                    "storageKey": null
                  },
                  (v24/*: any*/)
                ],
                "storageKey": null
              },
              (v25/*: any*/)
            ],
            "storageKey": null
          },
          (v26/*: any*/),
          (v27/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": "monthlyBookings",
        "args": (v21/*: any*/),
        "filters": (v28/*: any*/),
        "handle": "connection",
        "key": "SmallMonthlyViewCalendar_monthlyBookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "myOrganizations",
        "plural": true,
        "selections": (v29/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v31/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "myLocations",
        "plural": true,
        "selections": (v29/*: any*/),
        "storageKey": null
      },
      {
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v31/*: any*/),
            "concreteType": "OrganizationBookingPermissions",
            "kind": "LinkedField",
            "name": "organizationBookingPermissions",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "canUpdateBookingOnBehalf",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "canDeleteBookingOnBehalf",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
            "args": (v32/*: any*/),
            "concreteType": "OrganizationMemberConnection",
            "kind": "LinkedField",
            "name": "paginatedOrganizationMembers",
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
                "concreteType": "OrganizationMemberEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationMemberDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v13/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationCustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": [
                          (v19/*: any*/),
                          (v18/*: any*/),
                          (v14/*: any*/),
                          (v15/*: any*/),
                          (v16/*: any*/),
                          (v17/*: any*/)
                        ],
                        "storageKey": null
                      },
                      (v24/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v25/*: any*/)
                ],
                "storageKey": null
              },
              (v26/*: any*/),
              (v27/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
            "args": (v32/*: any*/),
            "filters": (v28/*: any*/),
            "handle": "connection",
            "key": "bookingDetailsSelectorQuery_bookingDetailsSelectorQueryPaginatedOrganizationMembers",
            "kind": "LinkedHandle",
            "name": "paginatedOrganizationMembers"
          }
        ]
      },
      {
        "condition": "locationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "date",
                "variableName": "dateToGetAvailableDesks"
              },
              {
                "kind": "Variable",
                "name": "deskIdsToInclude",
                "variableName": "deskIdsToIncludeToGetAvailableDesks"
              },
              {
                "kind": "Variable",
                "name": "locationId",
                "variableName": "locationId"
              }
            ],
            "concreteType": "BookingDeskDetails",
            "kind": "LinkedField",
            "name": "availableLocationDesks",
            "plural": true,
            "selections": (v23/*: any*/),
            "storageKey": null
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "9fba53050452f800f47d885a9678de98",
    "id": null,
    "metadata": {},
    "name": "pageHome_rootQuery",
    "operationKind": "query",
    "text": "query pageHome_rootQuery(\n  $organizationId: String!\n  $organizationExists: Boolean!\n  $locationId: String!\n  $locationExists: Boolean!\n  $monthlyCalendarDateFrom: DateTime!\n  $monthlyCalendarDateTo: DateTime!\n  $dateToGetAvailableDesks: DateTime!\n  $deskIdsToIncludeToGetAvailableDesks: [String!]!\n  $bookingPeopleNameSearchText: String\n  $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $smallMonthlyViewCalendarBookingsSortingValues: [BookingOrderInput!]\n) {\n  bookingCustomerRecordSynced\n  organizationCustomerRecordSynced\n  ...rootShell_query\n  ...smallMonthlyViewCalendar_query\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  to\n  notes\n  customer {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    uniqueId\n    name\n  }\n  location {\n    uniqueId\n    name\n  }\n  team {\n    uniqueId\n    name\n  }\n  desks {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n}\n\nfragment bookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    preferredDesks {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  organizationBookingPermissions(organizationId: $organizationId) @include(if: $organizationExists) {\n    canUpdateBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n}\n\nfragment bookingDetailsSelector_query on Query {\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  availableLocationDesks(locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks) @include(if: $locationExists) {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n  bookingDetailsSelectorQueryPaginatedOrganizationMembers: paginatedOrganizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues) @include(if: $organizationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment mainRootLayout_query on Query {\n  me {\n    email {\n      email\n      verified\n      id\n    }\n    givenName\n    middleName\n    familyName\n    photoUrl\n    id\n  }\n  ...newFeedbackDialog_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n\nfragment rootShell_query on Query {\n  me {\n    id\n  }\n  ...observability_query\n  ...mainRootLayout_query\n}\n\nfragment smallMonthlyViewCalendar_query on Query {\n  monthlyBookings: bookings(first: 1000, where: {fromGTE: $monthlyCalendarDateFrom, toLT: $monthlyCalendarDateTo, includeMineOnly: true}, orderBy: $smallMonthlyViewCalendarBookingsSortingValues) {\n    edges {\n      node {\n        id\n        from\n        to\n        notes\n        customer {\n          photoUrl\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    defaultOrganization {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  ...bookingCard_query\n}\n"
  }
};
})();

(node as any).hash = "3a568845708b02a3fc1d0e5c2a685437";

export default node;
