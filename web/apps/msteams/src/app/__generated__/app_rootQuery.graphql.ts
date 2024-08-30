/**
 * @generated SignedSource<<6550544f979d31c0f48e3cfc17965be8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Query } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type BookingOrderField = "familyName" | "from" | "givenName" | "locationName" | "middleName" | "name" | "notes" | "organizationName" | "teamName" | "to" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field?: OrganizationMemberOrderField | null | undefined;
};
export type BookingOrderInput = {
  direction: OrderDirection;
  field?: BookingOrderField | null | undefined;
};
export type app_rootQuery$variables = {
  bookingDetailsSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  bookingPeopleNameSearchText: string;
  dateToGetAvailableDesks: any;
  deskIdsToIncludeToGetAvailableDesks: ReadonlyArray<string>;
  locationId: string;
  monthlyCalendarDateFrom: any;
  monthlyCalendarDateTo: any;
  organizationId: string;
  smallMonthlyViewCalendarBookingsSortingValues?: ReadonlyArray<BookingOrderInput> | null | undefined;
};
export type app_rootQuery$data = {
  readonly bookingCustomerRecordSynced: boolean;
  readonly organizationCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"organizationOnboarding_query" | "rootShell_query" | "smallMonthlyViewCalendar_query">;
};
export type app_rootQuery = {
  response: app_rootQuery$data;
  variables: app_rootQuery$variables;
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
  "name": "locationId"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "monthlyCalendarDateFrom"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "monthlyCalendarDateTo"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "smallMonthlyViewCalendarBookingsSortingValues"
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "bookingCustomerRecordSynced",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "organizationCustomerRecordSynced",
  "storageKey": null
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
  "name": "givenName",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
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
v19 = [
  (v11/*: any*/),
  (v15/*: any*/)
],
v20 = [
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
v21 = [
  (v17/*: any*/),
  (v15/*: any*/)
],
v22 = [
  (v17/*: any*/),
  (v15/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingLocationTagDetails",
    "kind": "LinkedField",
    "name": "locationTags",
    "plural": true,
    "selections": [
      (v17/*: any*/),
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
v28 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v29 = [
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
      (v28/*: any*/)
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
      (v8/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "app_rootQuery",
    "selections": [
      (v9/*: any*/),
      (v10/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "rootShell_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationOnboarding_query"
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
      (v7/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/),
      (v8/*: any*/)
    ],
    "kind": "Operation",
    "name": "app_rootQuery",
    "selections": [
      (v9/*: any*/),
      (v10/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v11/*: any*/),
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
              (v11/*: any*/)
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
          (v12/*: any*/),
          (v13/*: any*/),
          (v14/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "isOrganizationOnboardingDone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "isLocationOnboardingDone",
            "storageKey": null
          },
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
        "kind": "ScalarField",
        "name": "isAzureTenantInstalled",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "azureTenantAdminConsentUrl",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTermsOfUse",
        "kind": "LinkedField",
        "name": "activeOrganizationTermsOfUse",
        "plural": false,
        "selections": [
          (v11/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "terms",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationIndustryMainCategoryReferenceDetails",
        "kind": "LinkedField",
        "name": "organizationIndustryMainCategoriesReferences",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
            "kind": "LinkedField",
            "name": "subCategories",
            "plural": true,
            "selections": (v19/*: any*/),
            "storageKey": null
          },
          (v11/*: any*/),
          (v15/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": "monthlyBookings",
        "args": (v20/*: any*/),
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
                    "selections": [
                      (v16/*: any*/),
                      (v17/*: any*/),
                      (v15/*: any*/),
                      (v12/*: any*/),
                      (v13/*: any*/),
                      (v14/*: any*/)
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
                    "selections": (v21/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v21/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v21/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
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
        "alias": "monthlyBookings",
        "args": (v20/*: any*/),
        "filters": (v27/*: any*/),
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
        "selections": (v19/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          (v28/*: any*/)
        ],
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "myLocations",
        "plural": true,
        "selections": (v19/*: any*/),
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
          (v11/*: any*/)
        ],
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
        "selections": (v22/*: any*/),
        "storageKey": null
      },
      {
        "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
        "args": (v29/*: any*/),
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
                  (v11/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": [
                      (v17/*: any*/),
                      (v15/*: any*/),
                      (v12/*: any*/),
                      (v13/*: any*/),
                      (v14/*: any*/),
                      (v16/*: any*/)
                    ],
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
        "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
        "args": (v29/*: any*/),
        "filters": (v27/*: any*/),
        "handle": "connection",
        "key": "bookingDetailsSelectorQuery_bookingDetailsSelectorQueryPaginatedOrganizationMembers",
        "kind": "LinkedHandle",
        "name": "paginatedOrganizationMembers"
      }
    ]
  },
  "params": {
    "cacheID": "aff23e88ac0e30b3b73b69399357410c",
    "id": null,
    "metadata": {},
    "name": "app_rootQuery",
    "operationKind": "query",
    "text": "query app_rootQuery(\n  $organizationId: String!\n  $locationId: String!\n  $monthlyCalendarDateFrom: DateTime!\n  $monthlyCalendarDateTo: DateTime!\n  $dateToGetAvailableDesks: DateTime!\n  $deskIdsToIncludeToGetAvailableDesks: [String!]!\n  $bookingPeopleNameSearchText: String!\n  $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $smallMonthlyViewCalendarBookingsSortingValues: [BookingOrderInput!]\n) {\n  bookingCustomerRecordSynced\n  organizationCustomerRecordSynced\n  ...rootShell_query\n  ...organizationOnboarding_query\n  ...smallMonthlyViewCalendar_query\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  to\n  notes\n  customer {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    uniqueId\n    name\n  }\n  location {\n    uniqueId\n    name\n  }\n  team {\n    uniqueId\n    name\n  }\n  desks {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n}\n\nfragment bookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    preferredDesks {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  organization(id: $organizationId) {\n    canUpdateBookingOnBehalf\n    canDeleteBookingOnBehalf\n    id\n  }\n  ...bookingDetailsSelector_query\n}\n\nfragment bookingDetailsSelector_query on Query {\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  availableLocationDesks(locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks) {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n  bookingDetailsSelectorQueryPaginatedOrganizationMembers: paginatedOrganizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n\nfragment organizationMultipleChoicesIndustries_query on Query {\n  organizationIndustryMainCategoriesReferences {\n    id\n    name\n    subCategories {\n      id\n      name\n    }\n  }\n}\n\nfragment organizationOnboarding_query on Query {\n  me {\n    id\n    isOrganizationOnboardingDone\n    isLocationOnboardingDone\n  }\n  activeOrganizationTermsOfUse {\n    id\n  }\n  organizationIndustryMainCategoriesReferences {\n    subCategories {\n      id\n      name\n    }\n    id\n  }\n  ...organizationMultipleChoicesIndustries_query\n  ...organizationTermsOfUse_query\n}\n\nfragment organizationTermsOfUse_query on Query {\n  activeOrganizationTermsOfUse {\n    id\n    terms\n  }\n}\n\nfragment rootShell_query on Query {\n  me {\n    id\n  }\n  isAzureTenantInstalled\n  azureTenantAdminConsentUrl\n  ...observability_query\n}\n\nfragment smallMonthlyViewCalendar_query on Query {\n  monthlyBookings: bookings(first: 1000, where: {fromGTE: $monthlyCalendarDateFrom, toLT: $monthlyCalendarDateTo, includeMineOnly: true}, orderBy: $smallMonthlyViewCalendarBookingsSortingValues) {\n    edges {\n      node {\n        id\n        from\n        to\n        notes\n        customer {\n          photoUrl\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    defaultOrganization {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  ...bookingCard_query\n}\n"
  }
};
})();

(node as any).hash = "8a4d7529d3cee5cb01cc336e1ab2f544";

export default node;
