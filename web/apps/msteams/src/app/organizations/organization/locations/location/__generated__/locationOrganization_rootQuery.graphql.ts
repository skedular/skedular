/**
 * @generated SignedSource<<6d04eceb873580edb7b78724faf27d77>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Query } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type BookingOrderField = "familyName" | "from" | "givenName" | "locationName" | "middleName" | "name" | "notes" | "organizationName" | "teamName" | "to" | "%future added value";
export type CustomerOrderField = "designation" | "familyName" | "givenName" | "locale" | "middleName" | "name" | "timezone" | "title" | "%future added value";
export type DeskOrderField = "name" | "%future added value";
export type LocationMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type LocationTagOrderField = "description" | "name" | "tagType" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type BookingOrderInput = {
  direction: OrderDirection;
  field?: BookingOrderField | null | undefined;
};
export type LocationMemberOrderInput = {
  direction: OrderDirection;
  field?: LocationMemberOrderField | null | undefined;
};
export type CustomerOrderInput = {
  direction: OrderDirection;
  field?: CustomerOrderField | null | undefined;
};
export type LocationTagOrderInput = {
  direction: OrderDirection;
  field?: LocationTagOrderField | null | undefined;
};
export type DeskOrderInput = {
  direction: OrderDirection;
  field?: DeskOrderField | null | undefined;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field?: OrganizationMemberOrderField | null | undefined;
};
export type locationOrganization_rootQuery$variables = {
  bookingDetailsSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  bookingPeopleNameSearchText: string;
  bookingSortingValues: ReadonlyArray<BookingOrderInput>;
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaUntil: any;
  dateToGetAvailableDesks: any;
  deskIdsToIncludeToGetAvailableDesks: ReadonlyArray<string>;
  deskMultipleChoicesZonesSortingValues?: ReadonlyArray<LocationTagOrderInput> | null | undefined;
  deskNameSearchText: string;
  deskSortingValues: ReadonlyArray<DeskOrderInput>;
  fromToGetBookings?: any | null | undefined;
  locationAnalyticsFrom: any;
  locationAnalyticsUntil: any;
  locationId: string;
  locationOrganizationPeopleSortingValues?: ReadonlyArray<CustomerOrderInput> | null | undefined;
  locationPeopleSortingValues?: ReadonlyArray<LocationMemberOrderInput> | null | undefined;
  organizationId: string;
  peopleNameSearchText: string;
  toToGetBookings?: any | null | undefined;
  zoneNameSearchText: string;
  zoneSortingValues: ReadonlyArray<LocationTagOrderInput>;
  zoneTagType: string;
};
export type locationOrganization_rootQuery$data = {
  readonly locationCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"locationPage_query" | "rootShell_query">;
};
export type locationOrganization_rootQuery = {
  response: locationOrganization_rootQuery$data;
  variables: locationOrganization_rootQuery$variables;
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
  "name": "deskMultipleChoicesZonesSortingValues"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskNameSearchText"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskSortingValues"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "fromToGetBookings"
},
v11 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationAnalyticsFrom"
},
v12 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationAnalyticsUntil"
},
v13 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v14 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationOrganizationPeopleSortingValues"
},
v15 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationPeopleSortingValues"
},
v16 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v17 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v18 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "toToGetBookings"
},
v19 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneNameSearchText"
},
v20 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneSortingValues"
},
v21 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneTagType"
},
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "locationCustomerRecordSynced",
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v27 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v29 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v30 = [
  (v29/*: any*/)
],
v31 = {
  "kind": "Literal",
  "name": "first",
  "value": 50
},
v32 = {
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
v33 = [
  (v31/*: any*/),
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
      (v32/*: any*/)
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
  (v29/*: any*/),
  (v28/*: any*/),
  (v24/*: any*/),
  (v25/*: any*/),
  (v26/*: any*/),
  (v27/*: any*/)
],
v36 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingCustomerDetails",
  "kind": "LinkedField",
  "name": "customer",
  "plural": false,
  "selections": (v35/*: any*/),
  "storageKey": null
},
v37 = [
  (v29/*: any*/),
  (v28/*: any*/)
],
v38 = [
  (v29/*: any*/),
  (v28/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingLocationTagDetails",
    "kind": "LinkedField",
    "name": "locationTags",
    "plural": true,
    "selections": [
      (v29/*: any*/),
      (v28/*: any*/),
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
v39 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v40 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v41 = {
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
v42 = {
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
v43 = [
  "where",
  "orderBy"
],
v44 = [
  (v23/*: any*/),
  (v28/*: any*/)
],
v45 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v46 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v47 = [
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
      (v45/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v48 = {
  "fields": [
    (v46/*: any*/),
    {
      "kind": "Variable",
      "name": "nameContains",
      "variableName": "peopleNameSearchText"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v49 = [
  (v31/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationPeopleSortingValues"
  },
  (v48/*: any*/)
],
v50 = [
  (v31/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationOrganizationPeopleSortingValues"
  },
  (v48/*: any*/)
],
v51 = {
  "kind": "Variable",
  "name": "tagType",
  "variableName": "zoneTagType"
},
v52 = [
  (v31/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "zoneSortingValues"
  },
  {
    "fields": [
      (v46/*: any*/),
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "zoneNameSearchText"
      },
      (v51/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v53 = [
  (v34/*: any*/),
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
          (v23/*: any*/),
          (v28/*: any*/),
          (v39/*: any*/)
        ],
        "storageKey": null
      },
      (v40/*: any*/)
    ],
    "storageKey": null
  },
  (v41/*: any*/),
  (v42/*: any*/)
],
v54 = [
  (v31/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "deskSortingValues"
  },
  {
    "fields": [
      (v46/*: any*/),
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
v55 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "deskMultipleChoicesZonesSortingValues"
  },
  {
    "fields": [
      (v46/*: any*/),
      (v51/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v56 = {
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
      (v16/*: any*/),
      (v17/*: any*/),
      (v18/*: any*/),
      (v19/*: any*/),
      (v20/*: any*/),
      (v21/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "locationOrganization_rootQuery",
    "selections": [
      (v22/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "rootShell_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationPage_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v16/*: any*/),
      (v13/*: any*/),
      (v21/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v10/*: any*/),
      (v18/*: any*/),
      (v17/*: any*/),
      (v19/*: any*/),
      (v8/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v15/*: any*/),
      (v14/*: any*/),
      (v20/*: any*/),
      (v9/*: any*/),
      (v0/*: any*/),
      (v7/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v11/*: any*/),
      (v12/*: any*/)
    ],
    "kind": "Operation",
    "name": "locationOrganization_rootQuery",
    "selections": [
      (v22/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v23/*: any*/),
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
              (v23/*: any*/),
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
          (v24/*: any*/),
          (v25/*: any*/),
          (v26/*: any*/),
          (v27/*: any*/),
          (v28/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v30/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerLocationTagDetails",
            "kind": "LinkedField",
            "name": "preferredZones",
            "plural": true,
            "selections": (v30/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "tenantInstalled",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "adminConsentUrl",
        "storageKey": null
      },
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
        "selections": [
          (v28/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canViewAnalytics",
            "storageKey": null
          },
          (v23/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "about",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "timezone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              (v28/*: any*/)
            ],
            "storageKey": null
          },
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
                  (v23/*: any*/),
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
                  (v36/*: any*/),
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
                    "selections": (v37/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v37/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v37/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": (v38/*: any*/),
                    "storageKey": null
                  },
                  (v39/*: any*/)
                ],
                "storageKey": null
              },
              (v40/*: any*/)
            ],
            "storageKey": null
          },
          (v41/*: any*/),
          (v42/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v33/*: any*/),
        "filters": (v43/*: any*/),
        "handle": "connection",
        "key": "locationBookingsTab_bookings",
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
        "selections": (v44/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          (v45/*: any*/)
        ],
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "myLocations",
        "plural": true,
        "selections": (v44/*: any*/),
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
          (v23/*: any*/),
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
          (v46/*: any*/)
        ],
        "concreteType": "BookingDeskDetails",
        "kind": "LinkedField",
        "name": "availableLocationDesks",
        "plural": true,
        "selections": (v38/*: any*/),
        "storageKey": null
      },
      {
        "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
        "args": (v47/*: any*/),
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
                  (v23/*: any*/),
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
                  (v39/*: any*/)
                ],
                "storageKey": null
              },
              (v40/*: any*/)
            ],
            "storageKey": null
          },
          (v41/*: any*/),
          (v42/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
        "args": (v47/*: any*/),
        "filters": (v43/*: any*/),
        "handle": "connection",
        "key": "bookingDetailsSelectorQuery_bookingDetailsSelectorQueryPaginatedOrganizationMembers",
        "kind": "LinkedHandle",
        "name": "paginatedOrganizationMembers"
      },
      {
        "alias": null,
        "args": (v49/*: any*/),
        "concreteType": "LocationMemberConnection",
        "kind": "LinkedField",
        "name": "paginatedLocationMembers",
        "plural": false,
        "selections": [
          (v34/*: any*/),
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
                  (v23/*: any*/),
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
                      (v28/*: any*/),
                      (v24/*: any*/),
                      (v25/*: any*/),
                      (v26/*: any*/),
                      (v27/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v39/*: any*/)
                ],
                "storageKey": null
              },
              (v40/*: any*/)
            ],
            "storageKey": null
          },
          (v41/*: any*/),
          (v42/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v49/*: any*/),
        "filters": (v43/*: any*/),
        "handle": "connection",
        "key": "locationPeopleTab_paginatedLocationMembers",
        "kind": "LinkedHandle",
        "name": "paginatedLocationMembers"
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
        "args": (v50/*: any*/),
        "concreteType": "CustomerConnection",
        "kind": "LinkedField",
        "name": "customersByDefaultLocation",
        "plural": false,
        "selections": [
          (v34/*: any*/),
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
                  (v23/*: any*/),
                  (v28/*: any*/),
                  (v24/*: any*/),
                  (v25/*: any*/),
                  (v26/*: any*/),
                  (v27/*: any*/),
                  (v39/*: any*/)
                ],
                "storageKey": null
              },
              (v40/*: any*/)
            ],
            "storageKey": null
          },
          (v41/*: any*/),
          (v42/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v50/*: any*/),
        "filters": (v43/*: any*/),
        "handle": "connection",
        "key": "locationPeopleTab_customersByDefaultLocation",
        "kind": "LinkedHandle",
        "name": "customersByDefaultLocation"
      },
      {
        "alias": "locationZonesTabPaginatedTags",
        "args": (v52/*: any*/),
        "concreteType": "LocationTagConnection",
        "kind": "LinkedField",
        "name": "paginatedLocationTags",
        "plural": false,
        "selections": (v53/*: any*/),
        "storageKey": null
      },
      {
        "alias": "locationZonesTabPaginatedTags",
        "args": (v52/*: any*/),
        "filters": (v43/*: any*/),
        "handle": "connection",
        "key": "locationZonesTab_locationZonesTabPaginatedTags",
        "kind": "LinkedHandle",
        "name": "paginatedLocationTags"
      },
      {
        "alias": null,
        "args": (v54/*: any*/),
        "concreteType": "DeskConnection",
        "kind": "LinkedField",
        "name": "paginatedLocationDesks",
        "plural": false,
        "selections": [
          (v34/*: any*/),
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
                  (v23/*: any*/),
                  (v28/*: any*/),
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
                    "selections": (v44/*: any*/),
                    "storageKey": null
                  },
                  (v39/*: any*/)
                ],
                "storageKey": null
              },
              (v40/*: any*/)
            ],
            "storageKey": null
          },
          (v41/*: any*/),
          (v42/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v54/*: any*/),
        "filters": (v43/*: any*/),
        "handle": "connection",
        "key": "locationDesksTab_paginatedLocationDesks",
        "kind": "LinkedHandle",
        "name": "paginatedLocationDesks"
      },
      {
        "alias": null,
        "args": (v55/*: any*/),
        "concreteType": "LocationTagConnection",
        "kind": "LinkedField",
        "name": "paginatedLocationTags",
        "plural": false,
        "selections": (v53/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v55/*: any*/),
        "filters": (v43/*: any*/),
        "handle": "connection",
        "key": "locationZonesTab_paginatedLocationTags",
        "kind": "LinkedHandle",
        "name": "paginatedLocationTags"
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
              (v32/*: any*/),
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
          (v23/*: any*/),
          (v36/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDeskDetails",
            "kind": "LinkedField",
            "name": "desks",
            "plural": true,
            "selections": (v30/*: any*/),
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
            "variableName": "locationAnalyticsFrom"
          },
          (v46/*: any*/),
          {
            "kind": "Variable",
            "name": "until",
            "variableName": "locationAnalyticsUntil"
          }
        ],
        "concreteType": "LocationAnalytics",
        "kind": "LinkedField",
        "name": "locationAnalytics",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDesksOccupancyPercentage",
            "kind": "LinkedField",
            "name": "desksOccupancyPercentage",
            "plural": true,
            "selections": [
              (v56/*: any*/),
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
            "concreteType": "LocationDailyBookingsTotal",
            "kind": "LinkedField",
            "name": "dailyBookingsTotals",
            "plural": true,
            "selections": [
              (v56/*: any*/),
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
    "cacheID": "e22893c61415c1db78b35532a92266ad",
    "id": null,
    "metadata": {},
    "name": "locationOrganization_rootQuery",
    "operationKind": "query",
    "text": "query locationOrganization_rootQuery(\n  $organizationId: String!\n  $locationId: String!\n  $zoneTagType: String!\n  $dateToGetAvailableDesks: DateTime!\n  $deskIdsToIncludeToGetAvailableDesks: [String!]!\n  $fromToGetBookings: DateTime\n  $toToGetBookings: DateTime\n  $peopleNameSearchText: String!\n  $zoneNameSearchText: String!\n  $deskNameSearchText: String!\n  $bookingPeopleNameSearchText: String!\n  $bookingSortingValues: [BookingOrderInput!]!\n  $locationPeopleSortingValues: [LocationMemberOrderInput!]\n  $locationOrganizationPeopleSortingValues: [CustomerOrderInput!]\n  $zoneSortingValues: [LocationTagOrderInput!]!\n  $deskSortingValues: [DeskOrderInput!]!\n  $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $deskMultipleChoicesZonesSortingValues: [LocationTagOrderInput!]\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaUntil: DateTime!\n  $locationAnalyticsFrom: DateTime!\n  $locationAnalyticsUntil: DateTime!\n) {\n  locationCustomerRecordSynced\n  ...rootShell_query\n  ...locationPage_query\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  to\n  notes\n  customer {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    uniqueId\n    name\n  }\n  location {\n    uniqueId\n    name\n  }\n  team {\n    uniqueId\n    name\n  }\n  desks {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n}\n\nfragment bookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    preferredDesks {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  organization(id: $organizationId) {\n    canUpdateBookingOnBehalf\n    canDeleteBookingOnBehalf\n    id\n  }\n  ...bookingDetailsSelector_query\n}\n\nfragment bookingDetailsSelector_query on Query {\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  availableLocationDesks(locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks) {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n  bookingDetailsSelectorQueryPaginatedOrganizationMembers: paginatedOrganizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment bulkNewDeskDialog_query on Query {\n  ...deskMultipleChoicesZones_query\n}\n\nfragment customerCard_CustomerDetails on CustomerDetails {\n  name\n  givenName\n  middleName\n  familyName\n  photoUrl\n}\n\nfragment deskCard_DeskDetails on DeskDetails {\n  id\n  name\n  deactivated\n  requireBookingApproval\n  locationTags {\n    id\n    name\n  }\n}\n\nfragment deskCard_query on Query {\n  me {\n    id\n    preferredDesks {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    canModify\n    id\n  }\n}\n\nfragment deskMultipleChoicesZones_query on Query {\n  paginatedLocationTags(where: {locationId: $locationId, tagType: $zoneTagType}, orderBy: $deskMultipleChoicesZonesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationAboutTab_query on Query {\n  location(id: $locationId) {\n    id\n    name\n    about\n    timezone\n    organization {\n      name\n    }\n    canModify\n  }\n}\n\nfragment locationAnalyticsTab_query on Query {\n  locationAnalytics(locationId: $locationId, from: $locationAnalyticsFrom, until: $locationAnalyticsUntil) {\n    desksOccupancyPercentage {\n      date\n      percentage\n    }\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n}\n\nfragment locationBookingsTab_query on Query {\n  bookings(first: 50, where: {locationIds: [$locationId], fromGTE: $bookingsSearchCriteriaFrom, fromLTE: $bookingsSearchCriteriaUntil, includeMineOnly: false}, orderBy: $bookingSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        to\n        customer {\n          uniqueId\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  me {\n    id\n  }\n  ...bookingCard_query\n  ...newBookingDialog_query\n}\n\nfragment locationDesksTab_query on Query {\n  location(id: $locationId) {\n    canModify\n    id\n  }\n  paginatedLocationDesks(first: 50, where: {locationId: $locationId, nameContains: $deskNameSearchText}, orderBy: $deskSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...deskCard_DeskDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...deskCard_query\n  ...deskMultipleChoicesZones_query\n  allBookings(where: {locationIds: [$locationId], fromGTE: $fromToGetBookings, toLTE: $toToGetBookings}) {\n    id\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    desks {\n      uniqueId\n    }\n  }\n  ...newDeskDialog_query\n  ...bulkNewDeskDialog_query\n}\n\nfragment locationMemberCard_LocationMemberDetails on LocationMemberDetails {\n  id\n  membershipType\n  customer {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n}\n\nfragment locationPage_query on Query {\n  location(id: $locationId) {\n    name\n    canViewAnalytics\n    id\n  }\n  ...locationBookingsTab_query\n  ...locationAboutTab_query\n  ...locationPeopleTab_query\n  ...locationPeopleTab_query_organizationMembers\n  ...locationZonesTab_query\n  ...locationDesksTab_query\n  ...locationAnalyticsTab_query\n}\n\nfragment locationPeopleTab_query on Query {\n  location(id: $locationId) {\n    id\n    name\n  }\n  paginatedLocationMembers(first: 50, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationPeopleSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...locationMemberCard_LocationMemberDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...locationSingleChoiceMembershipType_query\n}\n\nfragment locationPeopleTab_query_organizationMembers on Query {\n  customersByDefaultLocation(first: 50, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationOrganizationPeopleSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...customerCard_CustomerDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationSingleChoiceMembershipType_query on Query {\n  locationMemberMembershipTypes\n}\n\nfragment locationZonesTab_query on Query {\n  location(id: $locationId) {\n    canModify\n    id\n  }\n  locationZonesTabPaginatedTags: paginatedLocationTags(first: 50, where: {locationId: $locationId, tagType: $zoneTagType, nameContains: $zoneNameSearchText}, orderBy: $zoneSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...zoneCard_LocationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...zoneCard_Query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment mainRootLayout_query on Query {\n  me {\n    email {\n      email\n      verified\n      id\n    }\n    givenName\n    middleName\n    familyName\n    photoUrl\n    id\n  }\n  ...newFeedbackDialog_query\n}\n\nfragment newBookingDialog_query on Query {\n  me {\n    id\n  }\n  organization(id: $organizationId) {\n    id\n    canAddBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n}\n\nfragment newDeskDialog_query on Query {\n  ...deskMultipleChoicesZones_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n\nfragment rootShell_query on Query {\n  me {\n    id\n  }\n  tenantInstalled\n  adminConsentUrl\n  ...observability_query\n  ...mainRootLayout_query\n}\n\nfragment zoneCard_LocationTagDetails on LocationTagDetails {\n  id\n  name\n}\n\nfragment zoneCard_Query on Query {\n  me {\n    id\n    preferredZones {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    canModify\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "725fd71608864529552ab44e627b2b46";

export default node;
