/**
 * @generated SignedSource<<9df165b4b37948b78c7ec979f996e88f>>
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
export type BookingOrderInput = {
  direction: OrderDirection;
  field: BookingOrderField;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type bookings_rootQuery$variables = {
  bookingDetailsSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  bookingPeopleNameSearchText?: string | null | undefined;
  bookingSortingValues: ReadonlyArray<BookingOrderInput>;
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaTo: any;
  dateToGetAvailableDesks: any;
  deskIdsToIncludeToGetAvailableDesks: ReadonlyArray<string>;
  locationExists: boolean;
  locationId: string;
  organizationExists: boolean;
  organizationId: string;
  peopleNameSearchText?: string | null | undefined;
  teamExists: boolean;
  teamId: string;
};
export type bookings_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"bookings_bookings_query" | "bookings_query">;
};
export type bookings_rootQuery = {
  response: bookings_rootQuery$data;
  variables: bookings_rootQuery$variables;
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
  "name": "organizationExists"
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
v23 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v24 = [
  (v23/*: any*/)
],
v25 = [
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
      (v23/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v26 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v27 = [
  (v20/*: any*/),
  (v15/*: any*/),
  (v16/*: any*/),
  (v17/*: any*/),
  (v18/*: any*/),
  (v19/*: any*/)
],
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v29 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v30 = {
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
v31 = {
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
v32 = [
  "where",
  "orderBy"
],
v33 = [
  (v20/*: any*/),
  (v15/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingLocationTagDetails",
    "kind": "LinkedField",
    "name": "locationTags",
    "plural": true,
    "selections": [
      (v20/*: any*/),
      (v15/*: any*/),
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
v34 = [
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
        "kind": "Literal",
        "name": "combineOrganizationsLocationsTeams",
        "value": true
      },
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
v35 = [
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
    "name": "bookings_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "bookings_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "bookings_bookings_query"
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
    "name": "bookings_rootQuery",
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
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
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
            "alias": null,
            "args": (v24/*: any*/),
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
            "concreteType": "OrganizationMemberConnection",
            "kind": "LinkedField",
            "name": "organizationMembers",
            "plural": false,
            "selections": [
              (v26/*: any*/),
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
                        "selections": (v27/*: any*/),
                        "storageKey": null
                      },
                      (v28/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v29/*: any*/)
                ],
                "storageKey": null
              },
              (v30/*: any*/),
              (v31/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v25/*: any*/),
            "filters": (v32/*: any*/),
            "handle": "connection",
            "key": "bookingDetailsSelectorQuery_organizationMembers",
            "kind": "LinkedHandle",
            "name": "organizationMembers"
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
            "selections": (v33/*: any*/),
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
        "args": (v24/*: any*/),
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
        "args": (v24/*: any*/),
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
        "args": (v34/*: any*/),
        "concreteType": "BookingConnection",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v26/*: any*/),
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
                    "selections": (v27/*: any*/),
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
                    "concreteType": "BookingOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v35/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v35/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v35/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": (v33/*: any*/),
                    "storageKey": null
                  },
                  (v28/*: any*/)
                ],
                "storageKey": null
              },
              (v29/*: any*/)
            ],
            "storageKey": null
          },
          (v30/*: any*/),
          (v31/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v34/*: any*/),
        "filters": (v32/*: any*/),
        "handle": "connection",
        "key": "bookings_bookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "a788b6396311af2c49fc73455970a040",
    "id": null,
    "metadata": {},
    "name": "bookings_rootQuery",
    "operationKind": "query",
    "text": "query bookings_rootQuery(\n  $organizationId: String!\n  $organizationExists: Boolean!\n  $locationId: String!\n  $locationExists: Boolean!\n  $teamId: String!\n  $teamExists: Boolean!\n  $dateToGetAvailableDesks: DateTime!\n  $deskIdsToIncludeToGetAvailableDesks: [String!]!\n  $bookingPeopleNameSearchText: String\n  $bookingSortingValues: [BookingOrderInput!]!\n  $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaTo: DateTime!\n  $peopleNameSearchText: String\n) {\n  ...bookings_query\n  ...bookings_bookings_query\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  to\n  notes\n  customer {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    uniqueId\n    name\n  }\n  location {\n    uniqueId\n    name\n  }\n  team {\n    uniqueId\n    name\n  }\n  desks {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n}\n\nfragment bookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    preferredDesks {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  organizationBookingPermissions(organizationId: $organizationId) @include(if: $organizationExists) {\n    canUpdateBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n  ...bookingDetailsSelector_organizationMembers_query\n  ...bookingDetailsSelector_availableLocationDesks_query\n}\n\nfragment bookingDetailsSelector_availableLocationDesks_query on Query {\n  availableLocationDesks(locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks) @include(if: $locationExists) {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n}\n\nfragment bookingDetailsSelector_organizationMembers_query on Query {\n  organizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues) @include(if: $organizationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment bookingDetailsSelector_query on Query {\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n    organization {\n      uniqueId\n    }\n  }\n  myTeams(organizationId: $organizationId) {\n    id\n    name\n    organization {\n      uniqueId\n    }\n  }\n}\n\nfragment bookings_bookings_query on Query {\n  bookings(first: 50, where: {organizationIds: [$organizationId], locationIds: [$locationId], teamIds: [$teamId], fromGTE: $bookingsSearchCriteriaFrom, fromLTE: $bookingsSearchCriteriaTo, nameContains: $peopleNameSearchText, includeMineOnly: false, combineOrganizationsLocationsTeams: true}, orderBy: $bookingSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        to\n        customer {\n          uniqueId\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment bookings_query on Query {\n  me {\n    id\n  }\n  organization(id: $organizationId) @include(if: $organizationExists) {\n    id\n    name\n  }\n  location(id: $locationId) @include(if: $locationExists) {\n    id\n    name\n  }\n  team(id: $teamId) @include(if: $teamExists) {\n    id\n    name\n  }\n  ...bookingCard_query\n  ...newBookingDialog_query\n}\n\nfragment newBookingDialog_query on Query {\n  me {\n    id\n  }\n  organizationBookingPermissions(organizationId: $organizationId) @include(if: $organizationExists) {\n    canAddBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n  ...bookingDetailsSelector_organizationMembers_query\n  ...bookingDetailsSelector_availableLocationDesks_query\n}\n"
  }
};
})();

(node as any).hash = "febc1c66a45a9418e8a9c936ab0e505f";

export default node;
