/**
 * @generated SignedSource<<25d341e49a47aeb1425776fb2728225f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type BookingOrderField = "FamilyName" | "From" | "GivenName" | "LocationName" | "MiddleName" | "Name" | "Notes" | "OrganizationName" | "TeamName" | "To" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationMemberOrderField = "FamilyName" | "GivenName" | "MembershipType" | "MiddleName" | "Name" | "%future added value";
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type BookingOrderInput = {
  direction: OrderDirection;
  field: BookingOrderField;
};
export type smallMonthlyViewCalendar_rootQuery$variables = {
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
export type smallMonthlyViewCalendar_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"smallMonthlyViewCalendar_bookings_query" | "smallMonthlyViewCalendar_query">;
};
export type smallMonthlyViewCalendar_rootQuery = {
  response: smallMonthlyViewCalendar_rootQuery$data;
  variables: smallMonthlyViewCalendar_rootQuery$variables;
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
  "name": "id",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v18 = [
  (v17/*: any*/)
],
v19 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v20 = [
  (v19/*: any*/)
],
v21 = [
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
      (v19/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v22 = [
  (v17/*: any*/),
  (v12/*: any*/),
  (v13/*: any*/),
  (v14/*: any*/),
  (v15/*: any*/),
  (v16/*: any*/)
],
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
  (v17/*: any*/),
  (v12/*: any*/)
],
v29 = [
  (v17/*: any*/),
  (v12/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingOrganizationDeskTypeDetails",
    "kind": "LinkedField",
    "name": "deskTypes",
    "plural": true,
    "selections": (v28/*: any*/),
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingOrganizationZoneDetails",
    "kind": "LinkedField",
    "name": "zones",
    "plural": true,
    "selections": (v28/*: any*/),
    "storageKey": null
  }
],
v30 = [
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
    "name": "smallMonthlyViewCalendar_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "smallMonthlyViewCalendar_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "smallMonthlyViewCalendar_bookings_query"
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
    "name": "smallMonthlyViewCalendar_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v11/*: any*/),
          (v12/*: any*/),
          (v13/*: any*/),
          (v14/*: any*/),
          (v15/*: any*/),
          (v16/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerOrganizationDetails",
            "kind": "LinkedField",
            "name": "defaultOrganization",
            "plural": false,
            "selections": (v18/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v18/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "myOrganizations",
        "plural": true,
        "selections": [
          (v11/*: any*/),
          (v12/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v20/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "myLocations",
        "plural": true,
        "selections": [
          (v11/*: any*/),
          (v12/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": (v18/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v20/*: any*/),
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
            "alias": null,
            "args": (v21/*: any*/),
            "concreteType": "OrganizationMemberConnection",
            "kind": "LinkedField",
            "name": "organizationMembers",
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
                      (v11/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationCustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": (v22/*: any*/),
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
            "key": "bookingDetailsSelectorQuery_organizationMembers",
            "kind": "LinkedHandle",
            "name": "organizationMembers"
          }
        ]
      },
      {
        "alias": null,
        "args": (v20/*: any*/),
        "concreteType": "TeamDetails",
        "kind": "LinkedField",
        "name": "myTeams",
        "plural": true,
        "selections": [
          (v11/*: any*/),
          (v12/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": (v18/*: any*/),
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
            "selections": (v29/*: any*/),
            "storageKey": null
          }
        ]
      },
      {
        "alias": null,
        "args": (v30/*: any*/),
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
                  (v11/*: any*/),
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
                    "selections": (v22/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v28/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v28/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v28/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": (v29/*: any*/),
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
        "args": (v30/*: any*/),
        "filters": (v27/*: any*/),
        "handle": "connection",
        "key": "SmallMonthlyViewCalendar_bookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "76be225f5abb5e3f3f30748b3ae09d69",
    "id": null,
    "metadata": {},
    "name": "smallMonthlyViewCalendar_rootQuery",
    "operationKind": "query",
    "text": "query smallMonthlyViewCalendar_rootQuery(\n  $organizationId: String!\n  $organizationExists: Boolean!\n  $locationId: String!\n  $locationExists: Boolean!\n  $monthlyCalendarDateFrom: DateTime!\n  $monthlyCalendarDateTo: DateTime!\n  $dateToGetAvailableDesks: DateTime!\n  $deskIdsToIncludeToGetAvailableDesks: [String!]!\n  $bookingPeopleNameSearchText: String\n  $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $smallMonthlyViewCalendarBookingsSortingValues: [BookingOrderInput!]\n) {\n  ...smallMonthlyViewCalendar_query\n  ...smallMonthlyViewCalendar_bookings_query\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  to\n  notes\n  customer {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    uniqueId\n    name\n  }\n  location {\n    uniqueId\n    name\n  }\n  team {\n    uniqueId\n    name\n  }\n  desks {\n    uniqueId\n    name\n    deskTypes {\n      uniqueId\n      name\n    }\n    zones {\n      uniqueId\n      name\n    }\n  }\n}\n\nfragment bookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    preferredDesks {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  organizationBookingPermissions(organizationId: $organizationId) @include(if: $organizationExists) {\n    canUpdateBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n  ...bookingDetailsSelector_organizationMembers_query\n  ...bookingDetailsSelector_availableLocationDesks_query\n}\n\nfragment bookingDetailsSelector_availableLocationDesks_query on Query {\n  availableLocationDesks(locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks) @include(if: $locationExists) {\n    uniqueId\n    name\n    deskTypes {\n      uniqueId\n      name\n    }\n    zones {\n      uniqueId\n      name\n    }\n  }\n}\n\nfragment bookingDetailsSelector_organizationMembers_query on Query {\n  organizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues) @include(if: $organizationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment bookingDetailsSelector_query on Query {\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n    organization {\n      uniqueId\n    }\n  }\n  myTeams(organizationId: $organizationId) {\n    id\n    name\n    organization {\n      uniqueId\n    }\n  }\n}\n\nfragment smallMonthlyViewCalendar_bookings_query on Query {\n  bookings(first: 1000, where: {fromGTE: $monthlyCalendarDateFrom, toLT: $monthlyCalendarDateTo, includeMineOnly: true}, orderBy: $smallMonthlyViewCalendarBookingsSortingValues) {\n    edges {\n      node {\n        id\n        from\n        to\n        notes\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        organization {\n          uniqueId\n          name\n        }\n        location {\n          uniqueId\n          name\n        }\n        team {\n          uniqueId\n          name\n        }\n        desks {\n          uniqueId\n          name\n          deskTypes {\n            uniqueId\n            name\n          }\n          zones {\n            uniqueId\n            name\n          }\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment smallMonthlyViewCalendar_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    defaultOrganization {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  ...bookingCard_query\n}\n"
  }
};
})();

(node as any).hash = "755ecd97f758f0b896cb05d97f194d21";

export default node;
