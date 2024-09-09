/**
 * @generated SignedSource<<b165519ec96fc9ff757e8b0e6ccb3b30>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type BookingOrderField = "familyName" | "from" | "givenName" | "locationName" | "middleName" | "name" | "notes" | "organizationName" | "teamName" | "to" | "%future added value";
export type LocationOrderField = "name" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type TeamOrderField = "about" | "name" | "website" | "%future added value";
export type BookingOrderInput = {
  direction: OrderDirection;
  field?: BookingOrderField | null | undefined;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field?: OrganizationMemberOrderField | null | undefined;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field?: LocationOrderField | null | undefined;
};
export type TeamOrderInput = {
  direction: OrderDirection;
  field?: TeamOrderField | null | undefined;
};
export type pageOrganization_rootQuery$variables = {
  bookingDetailsSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  bookingPeopleNameSearchText: string;
  bookingSortingValues: ReadonlyArray<BookingOrderInput>;
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaUntil: any;
  dateToGetAvailableDesks: any;
  deskIdsToIncludeToGetAvailableDesks: ReadonlyArray<string>;
  locationId: string;
  locationNameSearchText: string;
  organizationAnalyticsFrom: any;
  organizationAnalyticsUntil: any;
  organizationId: string;
  organizationLocationsSortingValues: ReadonlyArray<LocationOrderInput>;
  organizationPeopleSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  organizationTeamsSortingValues: ReadonlyArray<TeamOrderInput>;
  peopleNameSearchText: string;
  teamNameSearchText: string;
};
export type pageOrganization_rootQuery$data = {
  readonly organizationCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"organizationPage_query" | "rootShell_query">;
};
export type pageOrganization_rootQuery = {
  response: pageOrganization_rootQuery$data;
  variables: pageOrganization_rootQuery$variables;
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
  "name": "bookingsSearchCriteriaUntil"
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
  "name": "locationId"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationNameSearchText"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationAnalyticsFrom"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationAnalyticsUntil"
},
v11 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v12 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationLocationsSortingValues"
},
v13 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationPeopleSortingValues"
},
v14 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationTeamsSortingValues"
},
v15 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v16 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamNameSearchText"
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "organizationCustomerRecordSynced",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "email",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v26 = [
  (v25/*: any*/)
],
v27 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
},
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "about",
  "storageKey": null
},
v29 = [
  (v18/*: any*/),
  (v24/*: any*/)
],
v30 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "unitPrice",
  "storageKey": null
},
v31 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationFeatureSetDetails",
  "kind": "LinkedField",
  "name": "featureSet",
  "plural": true,
  "selections": [
    (v24/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "description",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v32 = {
  "kind": "Literal",
  "name": "first",
  "value": 50
},
v33 = [
  (v32/*: any*/),
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
        "variableName": "bookingsSearchCriteriaUntil"
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
            "name": "organizationIds.0",
            "variableName": "organizationId"
          }
        ],
        "kind": "ListValue",
        "name": "organizationIds"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v34 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v35 = [
  (v25/*: any*/),
  (v24/*: any*/),
  (v20/*: any*/),
  (v21/*: any*/),
  (v22/*: any*/),
  (v23/*: any*/)
],
v36 = [
  (v25/*: any*/),
  (v24/*: any*/)
],
v37 = [
  (v25/*: any*/),
  (v24/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingLocationTagDetails",
    "kind": "LinkedField",
    "name": "locationTags",
    "plural": true,
    "selections": [
      (v25/*: any*/),
      (v24/*: any*/),
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
v38 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v39 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v40 = {
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
v41 = {
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
v42 = [
  "where",
  "orderBy"
],
v43 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v44 = [
  (v43/*: any*/)
],
v45 = [
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
      (v43/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v46 = [
  (v32/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationPeopleSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "peopleNameSearchText"
      },
      (v43/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v47 = [
  (v32/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationLocationsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "locationNameSearchText"
      },
      (v43/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v48 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "hasFutureBooking",
  "storageKey": null
},
v49 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canDelete",
  "storageKey": null
},
v50 = [
  (v32/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationTeamsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "teamNameSearchText"
      },
      (v43/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v51 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "date",
  "storageKey": null
};
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
      (v13/*: any*/),
      (v14/*: any*/),
      (v15/*: any*/),
      (v16/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganization_rootQuery",
    "selections": [
      (v17/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "rootShell_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationPage_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v11/*: any*/),
      (v7/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v15/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v13/*: any*/),
      (v0/*: any*/),
      (v12/*: any*/),
      (v14/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v8/*: any*/),
      (v16/*: any*/),
      (v9/*: any*/),
      (v10/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganization_rootQuery",
    "selections": [
      (v17/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v18/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerEmail",
            "kind": "LinkedField",
            "name": "email",
            "plural": false,
            "selections": [
              (v19/*: any*/),
              (v18/*: any*/),
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
          (v20/*: any*/),
          (v21/*: any*/),
          (v22/*: any*/),
          (v23/*: any*/),
          (v24/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v26/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerLocationDetails",
            "kind": "LinkedField",
            "name": "defaultLocations",
            "plural": true,
            "selections": (v26/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerTeamDetails",
            "kind": "LinkedField",
            "name": "defaultTeams",
            "plural": true,
            "selections": (v26/*: any*/),
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
        "selections": [
          (v18/*: any*/),
          (v24/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "logoUrl",
            "storageKey": null
          },
          (v27/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canViewAnalytics",
            "storageKey": null
          },
          (v28/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "website",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
            "kind": "LinkedField",
            "name": "industrySubCategories",
            "plural": true,
            "selections": (v29/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canInvitePeople",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationOfferingDetails",
            "kind": "LinkedField",
            "name": "offering",
            "plural": false,
            "selections": [
              (v18/*: any*/),
              (v24/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "start",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "end",
                "storageKey": null
              },
              (v30/*: any*/),
              (v31/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "hasAttachedPaymentMethod",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationAvailableOfferingDetails",
            "kind": "LinkedField",
            "name": "availableOfferings",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "code",
                "storageKey": null
              },
              (v24/*: any*/),
              (v30/*: any*/),
              (v31/*: any*/)
            ],
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
            "selections": (v29/*: any*/),
            "storageKey": null
          },
          (v18/*: any*/),
          (v24/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v33/*: any*/),
        "concreteType": "BookingConnection",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v34/*: any*/),
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
                  (v18/*: any*/),
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
                    "selections": (v35/*: any*/),
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
                    "selections": (v37/*: any*/),
                    "storageKey": null
                  },
                  (v38/*: any*/)
                ],
                "storageKey": null
              },
              (v39/*: any*/)
            ],
            "storageKey": null
          },
          (v40/*: any*/),
          (v41/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v33/*: any*/),
        "filters": (v42/*: any*/),
        "handle": "connection",
        "key": "organizationBookingsTab_bookings",
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
        "args": (v44/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "myLocations",
        "plural": true,
        "selections": (v29/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v44/*: any*/),
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
        "selections": (v37/*: any*/),
        "storageKey": null
      },
      {
        "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
        "args": (v45/*: any*/),
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "paginatedOrganizationMembers",
        "plural": false,
        "selections": [
          (v34/*: any*/),
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
                  (v18/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": (v35/*: any*/),
                    "storageKey": null
                  },
                  (v38/*: any*/)
                ],
                "storageKey": null
              },
              (v39/*: any*/)
            ],
            "storageKey": null
          },
          (v40/*: any*/),
          (v41/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
        "args": (v45/*: any*/),
        "filters": (v42/*: any*/),
        "handle": "connection",
        "key": "bookingDetailsSelectorQuery_bookingDetailsSelectorQueryPaginatedOrganizationMembers",
        "kind": "LinkedHandle",
        "name": "paginatedOrganizationMembers"
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "organizationMemberMembershipTypes",
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v46/*: any*/),
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "paginatedOrganizationMembers",
        "plural": false,
        "selections": [
          (v34/*: any*/),
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
                  (v18/*: any*/),
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
                    "concreteType": "OrganizationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": [
                      (v24/*: any*/),
                      (v20/*: any*/),
                      (v21/*: any*/),
                      (v22/*: any*/),
                      (v23/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v38/*: any*/)
                ],
                "storageKey": null
              },
              (v39/*: any*/)
            ],
            "storageKey": null
          },
          (v40/*: any*/),
          (v41/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v46/*: any*/),
        "filters": (v42/*: any*/),
        "handle": "connection",
        "key": "organizationPeopleTab_paginatedOrganizationMembers",
        "kind": "LinkedHandle",
        "name": "paginatedOrganizationMembers"
      },
      {
        "alias": null,
        "args": (v47/*: any*/),
        "concreteType": "LocationConnection",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v34/*: any*/),
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
                "selections": [
                  (v18/*: any*/),
                  (v24/*: any*/),
                  (v28/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v36/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "deskCapacity",
                    "storageKey": null
                  },
                  (v48/*: any*/),
                  (v27/*: any*/),
                  (v49/*: any*/),
                  (v38/*: any*/)
                ],
                "storageKey": null
              },
              (v39/*: any*/)
            ],
            "storageKey": null
          },
          (v40/*: any*/),
          (v41/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v47/*: any*/),
        "filters": (v42/*: any*/),
        "handle": "connection",
        "key": "organizationLocationsTab_locations",
        "kind": "LinkedHandle",
        "name": "locations"
      },
      {
        "alias": null,
        "args": (v50/*: any*/),
        "concreteType": "TeamConnection",
        "kind": "LinkedField",
        "name": "teams",
        "plural": false,
        "selections": [
          (v34/*: any*/),
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
                "selections": [
                  (v18/*: any*/),
                  (v24/*: any*/),
                  (v28/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v36/*: any*/),
                    "storageKey": null
                  },
                  (v48/*: any*/),
                  (v27/*: any*/),
                  (v49/*: any*/),
                  (v38/*: any*/)
                ],
                "storageKey": null
              },
              (v39/*: any*/)
            ],
            "storageKey": null
          },
          (v40/*: any*/),
          (v41/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v50/*: any*/),
        "filters": (v42/*: any*/),
        "handle": "connection",
        "key": "organizationTeamsTab_teams",
        "kind": "LinkedHandle",
        "name": "teams"
      },
      {
        "alias": null,
        "args": (v44/*: any*/),
        "concreteType": "OrganizationPaymentMethod",
        "kind": "LinkedField",
        "name": "organizationPaymentMethodsDetails",
        "plural": true,
        "selections": [
          (v18/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardBrand",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardExpiryMonth",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardExpiryYear",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardLastFourDigit",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v44/*: any*/),
        "concreteType": "OrganizationBillingInfo",
        "kind": "LinkedField",
        "name": "organizationBillingInfo",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "organizationId",
            "storageKey": null
          },
          (v19/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "addressLine1",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "addressLine2",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "suburb",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "city",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "province",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "zipcode",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "country",
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
            "name": "from",
            "variableName": "organizationAnalyticsFrom"
          },
          (v43/*: any*/),
          {
            "kind": "Variable",
            "name": "until",
            "variableName": "organizationAnalyticsUntil"
          }
        ],
        "concreteType": "OrganizationAnalytics",
        "kind": "LinkedField",
        "name": "organizationAnalytics",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationMemberAttendancePercentage",
            "kind": "LinkedField",
            "name": "memberAttendancePercentage",
            "plural": true,
            "selections": [
              (v51/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "percentage",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationDailyBookingsTotal",
            "kind": "LinkedField",
            "name": "dailyBookingsTotals",
            "plural": true,
            "selections": [
              (v51/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "total",
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "8058c53e12aa2cf858c3722cce6ac5e8",
    "id": null,
    "metadata": {},
    "name": "pageOrganization_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganization_rootQuery(\n  $organizationId: String!\n  $locationId: String!\n  $dateToGetAvailableDesks: DateTime!\n  $deskIdsToIncludeToGetAvailableDesks: [String!]!\n  $peopleNameSearchText: String!\n  $bookingPeopleNameSearchText: String!\n  $bookingSortingValues: [BookingOrderInput!]!\n  $organizationPeopleSortingValues: [OrganizationMemberOrderInput!]\n  $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $organizationLocationsSortingValues: [LocationOrderInput!]!\n  $organizationTeamsSortingValues: [TeamOrderInput!]!\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaUntil: DateTime!\n  $locationNameSearchText: String!\n  $teamNameSearchText: String!\n  $organizationAnalyticsFrom: DateTime!\n  $organizationAnalyticsUntil: DateTime!\n) {\n  organizationCustomerRecordSynced\n  ...rootShell_query\n  ...organizationPage_query\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  to\n  notes\n  customer {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    uniqueId\n    name\n  }\n  location {\n    uniqueId\n    name\n  }\n  team {\n    uniqueId\n    name\n  }\n  desks {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n}\n\nfragment bookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    preferredDesks {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  organizationBookingPermissions(organizationId: $organizationId) {\n    canUpdateBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n}\n\nfragment bookingDetailsSelector_query on Query {\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  availableLocationDesks(locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks) {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n  bookingDetailsSelectorQueryPaginatedOrganizationMembers: paginatedOrganizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationCard_LocationDetails on LocationDetails {\n  id\n  name\n  about\n  organization {\n    uniqueId\n    name\n  }\n  deskCapacity\n  hasFutureBooking\n  canModify\n  canDelete\n}\n\nfragment locationCard_Query on Query {\n  me {\n    id\n    defaultLocations {\n      uniqueId\n    }\n  }\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment mainRootLayout_query on Query {\n  me {\n    email {\n      email\n      verified\n      id\n    }\n    givenName\n    middleName\n    familyName\n    photoUrl\n    id\n  }\n  ...newFeedbackDialog_query\n}\n\nfragment newBookingDialog_query on Query {\n  me {\n    id\n  }\n  organizationBookingPermissions(organizationId: $organizationId) {\n    canAddBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n\nfragment organizationAboutTab_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    logoUrl\n    about\n    website\n    canModify\n    industrySubCategories {\n      id\n      name\n    }\n  }\n  organizationIndustryMainCategoriesReferences {\n    subCategories {\n      id\n      name\n    }\n    id\n  }\n  ...organizationMultipleChoicesIndustries_query\n}\n\nfragment organizationAnalyticsTab_query on Query {\n  organizationAnalytics(organizationId: $organizationId, from: $organizationAnalyticsFrom, until: $organizationAnalyticsUntil) {\n    memberAttendancePercentage {\n      date\n      percentage\n    }\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n}\n\nfragment organizationAvailableOfferings_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    hasAttachedPaymentMethod\n    availableOfferings {\n      code\n      name\n      unitPrice\n      featureSet {\n        name\n        description\n      }\n    }\n  }\n}\n\nfragment organizationBillingInfo_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n  }\n  organizationBillingInfo(organizationId: $organizationId) {\n    organizationId\n    email\n    addressLine1\n    addressLine2\n    suburb\n    city\n    province\n    zipcode\n    country\n  }\n}\n\nfragment organizationBillingTab_query on Query {\n  ...organizationPaymentMethods_query\n  ...organizationBillingInfo_query\n}\n\nfragment organizationBookingsTab_query on Query {\n  bookings(first: 50, where: {organizationIds: [$organizationId], fromGTE: $bookingsSearchCriteriaFrom, fromLTE: $bookingsSearchCriteriaUntil, includeMineOnly: false}, orderBy: $bookingSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        to\n        customer {\n          uniqueId\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  me {\n    id\n  }\n  ...bookingCard_query\n  ...newBookingDialog_query\n}\n\nfragment organizationLocationsTab_query on Query {\n  locations(first: 50, where: {organizationId: $organizationId, nameContains: $locationNameSearchText}, orderBy: $organizationLocationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...locationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...locationCard_Query\n  organization(id: $organizationId) {\n    id\n    canModify\n  }\n}\n\nfragment organizationMemberCard_OrganizationMemberDetails on OrganizationMemberDetails {\n  id\n  membershipType\n  customer {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n}\n\nfragment organizationMultipleChoicesIndustries_query on Query {\n  organizationIndustryMainCategoriesReferences {\n    id\n    name\n    subCategories {\n      id\n      name\n    }\n  }\n}\n\nfragment organizationOfferingTab_query on Query {\n  ...organizationOffering_query\n  ...organizationAvailableOfferings_query\n  organization(id: $organizationId) {\n    id\n    availableOfferings {\n      code\n    }\n  }\n}\n\nfragment organizationOffering_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    offering {\n      id\n      name\n      start\n      end\n      unitPrice\n      featureSet {\n        name\n        description\n      }\n    }\n  }\n}\n\nfragment organizationPage_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    logoUrl\n    canModify\n    canViewAnalytics\n  }\n  ...organizationAboutTab_query\n  ...organizationBookingsTab_query\n  ...organizationMultipleChoicesIndustries_query\n  ...organizationPeopleTab_query\n  ...organizationLocationsTab_query\n  ...organizationTeamsTab_query\n  ...organizationBillingTab_query\n  ...organizationOfferingTab_query\n  ...organizationAnalyticsTab_query\n}\n\nfragment organizationPaymentMethods_query on Query {\n  organization(id: $organizationId) {\n    id\n  }\n  organizationPaymentMethodsDetails(organizationId: $organizationId) {\n    id\n    cardBrand\n    cardExpiryMonth\n    cardExpiryYear\n    cardLastFourDigit\n  }\n}\n\nfragment organizationPeopleTab_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    canInvitePeople\n  }\n  ...organizationSingleChoiceMembershipType_query\n  paginatedOrganizationMembers(first: 50, where: {organizationId: $organizationId, nameContains: $peopleNameSearchText}, orderBy: $organizationPeopleSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...organizationMemberCard_OrganizationMemberDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationSingleChoiceMembershipType_query on Query {\n  organizationMemberMembershipTypes\n}\n\nfragment organizationTeamsTab_query on Query {\n  teams(first: 50, where: {organizationId: $organizationId, nameContains: $teamNameSearchText}, orderBy: $organizationTeamsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...teamCard_TeamDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...teamCard_Query\n  organization(id: $organizationId) {\n    id\n    canModify\n  }\n}\n\nfragment rootShell_query on Query {\n  me {\n    id\n  }\n  ...observability_query\n  ...mainRootLayout_query\n}\n\nfragment teamCard_Query on Query {\n  me {\n    id\n    defaultTeams {\n      uniqueId\n    }\n  }\n}\n\nfragment teamCard_TeamDetails on TeamDetails {\n  id\n  name\n  about\n  organization {\n    uniqueId\n    name\n  }\n  hasFutureBooking\n  canModify\n  canDelete\n}\n"
  }
};
})();

(node as any).hash = "1e89ff6bd55aa324f9fbd9a577373f1e";

export default node;
