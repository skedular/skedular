/**
 * @generated SignedSource<<49c299306e376a6b043a97c3cf659e32>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type BookingOrderField = "BookingType" | "FamilyName" | "From" | "GivenName" | "LocationName" | "MiddleName" | "Name" | "Notes" | "OrganizationName" | "TeamName" | "To" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationMemberOrderField = "FamilyName" | "GivenName" | "MiddleName" | "Name" | "PhoneNumber" | "Role" | "Status" | "%future added value";
export type BookingOrderInput = {
  direction: OrderDirection;
  field: BookingOrderField;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type oldBookings_rootQuery$variables = {
  bookingDetailsSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  bookingPeopleNameSearchText?: string | null | undefined;
  bookingSortingValues?: ReadonlyArray<BookingOrderInput> | null | undefined;
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaTo: any;
  dateToGetAvailableDesks: any;
  deskIdsToIncludeToGetAvailableDesks: ReadonlyArray<string>;
  locationExists: boolean;
  locationId: string;
  nullableOrganizationId?: string | null | undefined;
  organizationId: string;
  peopleNameSearchText?: string | null | undefined;
  teamExists: boolean;
  teamId: string;
};
export type oldBookings_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"oldBookings_bookings_query" | "oldBookings_query">;
};
export type oldBookings_rootQuery = {
  response: oldBookings_rootQuery$data;
  variables: oldBookings_rootQuery$variables;
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
  "name": "bookingSortingValues"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingsSearchCriteriaFrom"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingsSearchCriteriaTo"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateToGetAvailableDesks"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskIdsToIncludeToGetAvailableDesks"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationExists"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "nullableOrganizationId"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v11 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v12 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamExists"
},
v13 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamId"
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v21 = [
  (v20/*: any*/)
],
v22 = [
  (v14/*: any*/),
  (v15/*: any*/)
],
v23 = [
  (v20/*: any*/),
  (v15/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
],
v24 = [
  (v20/*: any*/),
  (v15/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingOrganizationCustomTagDetails",
    "kind": "LinkedField",
    "name": "customTags",
    "plural": true,
    "selections": (v23/*: any*/),
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingOrganizationZoneDetails",
    "kind": "LinkedField",
    "name": "zones",
    "plural": true,
    "selections": (v23/*: any*/),
    "storageKey": null
  }
],
v25 = [
  {
    "kind": "Variable",
    "name": "organizationId",
    "variableName": "nullableOrganizationId"
  }
],
v26 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v27 = [
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
      (v26/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v29 = [
  (v20/*: any*/),
  (v15/*: any*/),
  (v16/*: any*/),
  (v17/*: any*/),
  (v18/*: any*/),
  (v19/*: any*/)
],
v30 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v31 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v32 = {
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
v33 = {
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
v34 = [
  "where",
  "orderBy"
],
v35 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "bookingSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "fromGTE",
        "variableName": "bookingsSearchCriteriaFrom"
      },
      {
        "kind": "Variable",
        "name": "fromLTE",
        "variableName": "bookingsSearchCriteriaTo"
      },
      {
        "kind": "Literal",
        "name": "includeMineOnly",
        "value": false
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
        "name": "nameContains",
        "variableName": "peopleNameSearchText"
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
        "items": [
          {
            "kind": "Variable",
            "name": "teamIds.0",
            "variableName": "teamId"
          }
        ],
        "kind": "ListValue",
        "name": "teamIds"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v36 = [
  (v20/*: any*/),
  (v15/*: any*/)
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
      (v12/*: any*/),
      (v13/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "oldBookings_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "oldBookings_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "oldBookings_bookings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v10/*: any*/),
      (v9/*: any*/),
      (v8/*: any*/),
      (v7/*: any*/),
      (v13/*: any*/),
      (v12/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v0/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v11/*: any*/)
    ],
    "kind": "Operation",
    "name": "oldBookings_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v14/*: any*/),
          (v15/*: any*/),
          (v16/*: any*/),
          (v17/*: any*/),
          (v18/*: any*/),
          (v19/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v21/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "organizationId"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v22/*: any*/),
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
                "name": "id",
                "variableName": "locationId"
              }
            ],
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": (v22/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": [
              {
                "fields": [
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
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "BookingDeskDetails",
            "kind": "LinkedField",
            "name": "availableDesks",
            "plural": true,
            "selections": (v24/*: any*/),
            "storageKey": null
          }
        ]
      },
      {
        "condition": "teamExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "id",
                "variableName": "teamId"
              }
            ],
            "concreteType": "TeamDetails",
            "kind": "LinkedField",
            "name": "team",
            "plural": false,
            "selections": (v22/*: any*/),
            "storageKey": null
          }
        ]
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "myOrganizations",
        "plural": true,
        "selections": (v22/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v25/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "myLocations",
        "plural": true,
        "selections": [
          (v14/*: any*/),
          (v15/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": (v21/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          (v26/*: any*/)
        ],
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
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canAddBookingOnBehalf",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v25/*: any*/),
        "concreteType": "TeamDetails",
        "kind": "LinkedField",
        "name": "myTeams",
        "plural": true,
        "selections": [
          (v14/*: any*/),
          (v15/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": (v21/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v27/*: any*/),
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "organizationMembers",
        "plural": false,
        "selections": [
          (v28/*: any*/),
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
                  (v14/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": (v29/*: any*/),
                    "storageKey": null
                  },
                  (v30/*: any*/)
                ],
                "storageKey": null
              },
              (v31/*: any*/)
            ],
            "storageKey": null
          },
          (v32/*: any*/),
          (v33/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v27/*: any*/),
        "filters": (v34/*: any*/),
        "handle": "connection",
        "key": "bookingDetailsSelectorQuery_organizationMembers",
        "kind": "LinkedHandle",
        "name": "organizationMembers"
      },
      {
        "alias": null,
        "args": (v35/*: any*/),
        "concreteType": "BookingConnection",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v28/*: any*/),
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
                  (v14/*: any*/),
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
                    "concreteType": "BookingCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": (v29/*: any*/),
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
                    "kind": "ScalarField",
                    "name": "type",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v36/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v36/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v36/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": (v24/*: any*/),
                    "storageKey": null
                  },
                  (v30/*: any*/)
                ],
                "storageKey": null
              },
              (v31/*: any*/)
            ],
            "storageKey": null
          },
          (v32/*: any*/),
          (v33/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v35/*: any*/),
        "filters": (v34/*: any*/),
        "handle": "connection",
        "key": "bookings_bookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "139bf3bb9d7e68794da787440b706cac",
    "id": null,
    "metadata": {},
    "name": "oldBookings_rootQuery",
    "operationKind": "query",
    "text": "query oldBookings_rootQuery(\n  $organizationId: String!\n  $nullableOrganizationId: String\n  $locationId: String!\n  $locationExists: Boolean!\n  $teamId: String!\n  $teamExists: Boolean!\n  $dateToGetAvailableDesks: DateTime!\n  $deskIdsToIncludeToGetAvailableDesks: [String!]!\n  $bookingPeopleNameSearchText: String\n  $bookingSortingValues: [BookingOrderInput!]\n  $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaTo: DateTime!\n  $peopleNameSearchText: String\n) {\n  ...oldBookings_query\n  ...oldBookings_bookings_query\n}\n\nfragment bookingDetailsSelector_availableLocationDesks_query on Query {\n  availableDesks(where: {locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks}) @include(if: $locationExists) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment bookingDetailsSelector_organizationMembers_query on Query {\n  organizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment bookingDetailsSelector_query on Query {\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $nullableOrganizationId) {\n    id\n    name\n    organization {\n      uniqueId\n    }\n  }\n  myTeams(organizationId: $nullableOrganizationId) {\n    id\n    name\n    organization {\n      uniqueId\n    }\n  }\n}\n\nfragment newBookingDialog_query on Query {\n  me {\n    id\n  }\n  organizationBookingPermissions(organizationId: $organizationId) {\n    canAddBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n  ...bookingDetailsSelector_organizationMembers_query\n  ...bookingDetailsSelector_availableLocationDesks_query\n}\n\nfragment oldBookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  to\n  notes\n  type\n  customer {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    uniqueId\n    name\n  }\n  location {\n    uniqueId\n    name\n  }\n  team {\n    uniqueId\n    name\n  }\n  desks {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment oldBookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    preferredDesks {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $nullableOrganizationId) {\n    id\n    name\n  }\n  organizationBookingPermissions(organizationId: $organizationId) {\n    canUpdateBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n  ...bookingDetailsSelector_organizationMembers_query\n  ...bookingDetailsSelector_availableLocationDesks_query\n}\n\nfragment oldBookings_bookings_query on Query {\n  bookings(first: 50, where: {organizationIds: [$organizationId], locationIds: [$locationId], teamIds: [$teamId], fromGTE: $bookingsSearchCriteriaFrom, fromLTE: $bookingsSearchCriteriaTo, nameContains: $peopleNameSearchText, includeMineOnly: false}, orderBy: $bookingSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        to\n        customer {\n          uniqueId\n        }\n        ...oldBookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment oldBookings_query on Query {\n  me {\n    id\n  }\n  organization(id: $organizationId) {\n    id\n    name\n  }\n  location(id: $locationId) @include(if: $locationExists) {\n    id\n    name\n  }\n  team(id: $teamId) @include(if: $teamExists) {\n    id\n    name\n  }\n  ...oldBookingCard_query\n  ...newBookingDialog_query\n}\n"
  }
};
})();

(node as any).hash = "008a52c0df737965594945d9f743d10b";

export default node;
