/**
 * @generated SignedSource<<9d31702ddd31ef9cfecb66af701a6fae>>
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
export type TeamMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type BookingOrderInput = {
  direction: OrderDirection;
  field: BookingOrderField;
};
export type TeamMemberOrderInput = {
  direction: OrderDirection;
  field: TeamMemberOrderField;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type pageOrganizationTeam_rootQuery$variables = {
  bookingDetailsSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  bookingPeopleNameSearchText: string;
  bookingSortingValues: ReadonlyArray<BookingOrderInput>;
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaUntil: any;
  dateToGetAvailableDesks: any;
  deskIdsToIncludeToGetAvailableDesks: ReadonlyArray<string>;
  locationExists: boolean;
  locationId: string;
  organizationExists: boolean;
  organizationId: string;
  organizationMemberSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  peopleNameSearchText: string;
  teamExists: boolean;
  teamId: string;
  teamPeopleSortingValues?: ReadonlyArray<TeamMemberOrderInput> | null | undefined;
};
export type pageOrganizationTeam_rootQuery$data = {
  readonly teamCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"rootShell_query" | "teamPage_query">;
};
export type pageOrganizationTeam_rootQuery = {
  response: pageOrganizationTeam_rootQuery$data;
  variables: pageOrganizationTeam_rootQuery$variables;
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
  "name": "organizationMemberSelectorOrganizationMembersSortingValues"
},
v12 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v13 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamExists"
},
v14 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamId"
},
v15 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamPeopleSortingValues"
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "teamCustomerRecordSynced",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v24 = [
  (v23/*: any*/)
],
v25 = [
  (v23/*: any*/),
  (v22/*: any*/)
],
v26 = {
  "kind": "Literal",
  "name": "first",
  "value": 50
},
v27 = [
  (v26/*: any*/),
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
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v29 = [
  (v23/*: any*/),
  (v22/*: any*/),
  (v18/*: any*/),
  (v19/*: any*/),
  (v20/*: any*/),
  (v21/*: any*/)
],
v30 = [
  (v23/*: any*/),
  (v22/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingLocationTagDetails",
    "kind": "LinkedField",
    "name": "locationTags",
    "plural": true,
    "selections": [
      (v23/*: any*/),
      (v22/*: any*/),
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
v31 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v32 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v33 = {
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
v34 = {
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
v35 = [
  "where",
  "orderBy"
],
v36 = [
  (v17/*: any*/),
  (v22/*: any*/)
],
v37 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v38 = [
  (v37/*: any*/)
],
v39 = {
  "kind": "Literal",
  "name": "first",
  "value": 20
},
v40 = {
  "fields": [
    {
      "kind": "Variable",
      "name": "nameContains",
      "variableName": "bookingPeopleNameSearchText"
    },
    (v37/*: any*/)
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v41 = [
  (v39/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "bookingDetailsSelectorOrganizationMembersSortingValues"
  },
  (v40/*: any*/)
],
v42 = [
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
          (v17/*: any*/),
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
          (v31/*: any*/)
        ],
        "storageKey": null
      },
      (v32/*: any*/)
    ],
    "storageKey": null
  },
  (v33/*: any*/),
  (v34/*: any*/)
],
v43 = [
  (v39/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationMemberSelectorOrganizationMembersSortingValues"
  },
  (v40/*: any*/)
],
v44 = [
  (v26/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "teamPeopleSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "peopleNameSearchText"
      },
      {
        "kind": "Variable",
        "name": "teamId",
        "variableName": "teamId"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v45 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamCustomerDetails",
  "kind": "LinkedField",
  "name": "customer",
  "plural": false,
  "selections": [
    (v22/*: any*/),
    (v18/*: any*/),
    (v19/*: any*/),
    (v20/*: any*/),
    (v21/*: any*/)
  ],
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
      (v15/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationTeam_rootQuery",
    "selections": [
      (v16/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "rootShell_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "teamPage_query"
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
      (v14/*: any*/),
      (v13/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v15/*: any*/),
      (v0/*: any*/),
      (v11/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v12/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationTeam_rootQuery",
    "selections": [
      (v16/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v17/*: any*/),
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
              (v17/*: any*/),
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
          (v18/*: any*/),
          (v19/*: any*/),
          (v20/*: any*/),
          (v21/*: any*/),
          (v22/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v24/*: any*/),
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
            "variableName": "teamId"
          }
        ],
        "concreteType": "TeamDetails",
        "kind": "LinkedField",
        "name": "team",
        "plural": false,
        "selections": [
          (v22/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": (v25/*: any*/),
            "storageKey": null
          },
          (v17/*: any*/),
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
            "kind": "ScalarField",
            "name": "canModify",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamMemberDetails",
            "kind": "LinkedField",
            "name": "members",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamCustomerDetails",
                "kind": "LinkedField",
                "name": "customer",
                "plural": false,
                "selections": (v24/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamOrganizationMemberDetails",
                "kind": "LinkedField",
                "name": "organizationMember",
                "plural": false,
                "selections": (v24/*: any*/),
                "storageKey": null
              },
              (v17/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v27/*: any*/),
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
                  (v17/*: any*/),
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
                    "concreteType": "BookingOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v25/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v25/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v25/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": (v30/*: any*/),
                    "storageKey": null
                  },
                  (v31/*: any*/)
                ],
                "storageKey": null
              },
              (v32/*: any*/)
            ],
            "storageKey": null
          },
          (v33/*: any*/),
          (v34/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v27/*: any*/),
        "filters": (v35/*: any*/),
        "handle": "connection",
        "key": "teamBookingsTab_bookings",
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
        "selections": (v36/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v38/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "myLocations",
        "plural": true,
        "selections": (v36/*: any*/),
        "storageKey": null
      },
      {
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v38/*: any*/),
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
            "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
            "args": (v41/*: any*/),
            "concreteType": "OrganizationMemberConnection",
            "kind": "LinkedField",
            "name": "paginatedOrganizationMembers",
            "plural": false,
            "selections": (v42/*: any*/),
            "storageKey": null
          },
          {
            "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
            "args": (v41/*: any*/),
            "filters": (v35/*: any*/),
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
            "selections": (v30/*: any*/),
            "storageKey": null
          }
        ]
      },
      {
        "alias": "organizationMemberSelectorPaginatedOrganizationMembers",
        "args": (v43/*: any*/),
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "paginatedOrganizationMembers",
        "plural": false,
        "selections": (v42/*: any*/),
        "storageKey": null
      },
      {
        "alias": "organizationMemberSelectorPaginatedOrganizationMembers",
        "args": (v43/*: any*/),
        "filters": (v35/*: any*/),
        "handle": "connection",
        "key": "organizationMemberSelector_organizationMemberSelectorPaginatedOrganizationMembers",
        "kind": "LinkedHandle",
        "name": "paginatedOrganizationMembers"
      },
      {
        "condition": "teamExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v44/*: any*/),
            "concreteType": "TeamMemberConnection",
            "kind": "LinkedField",
            "name": "paginatedTeamMembers",
            "plural": false,
            "selections": [
              (v28/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamMemberEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamMemberDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v17/*: any*/),
                      (v45/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "TeamOrganizationMemberDetails",
                        "kind": "LinkedField",
                        "name": "organizationMember",
                        "plural": false,
                        "selections": [
                          (v45/*: any*/)
                        ],
                        "storageKey": null
                      },
                      (v31/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v32/*: any*/)
                ],
                "storageKey": null
              },
              (v33/*: any*/),
              (v34/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v44/*: any*/),
            "filters": (v35/*: any*/),
            "handle": "connection",
            "key": "teamPeopleTab_paginatedTeamMembers",
            "kind": "LinkedHandle",
            "name": "paginatedTeamMembers"
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "1f5386b409bf9f0776851132290d9806",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationTeam_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationTeam_rootQuery(\n  $organizationId: String!\n  $organizationExists: Boolean!\n  $locationId: String!\n  $locationExists: Boolean!\n  $teamId: String!\n  $teamExists: Boolean!\n  $dateToGetAvailableDesks: DateTime!\n  $deskIdsToIncludeToGetAvailableDesks: [String!]!\n  $bookingPeopleNameSearchText: String!\n  $bookingSortingValues: [BookingOrderInput!]!\n  $teamPeopleSortingValues: [TeamMemberOrderInput!]\n  $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaUntil: DateTime!\n  $peopleNameSearchText: String!\n) {\n  teamCustomerRecordSynced\n  ...rootShell_query\n  ...teamPage_query\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  to\n  notes\n  customer {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    uniqueId\n    name\n  }\n  location {\n    uniqueId\n    name\n  }\n  team {\n    uniqueId\n    name\n  }\n  desks {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n}\n\nfragment bookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n    preferredDesks {\n      uniqueId\n    }\n  }\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  organizationBookingPermissions(organizationId: $organizationId) @include(if: $organizationExists) {\n    canUpdateBookingOnBehalf\n    canDeleteBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n}\n\nfragment bookingDetailsSelector_query on Query {\n  myOrganizations {\n    id\n    name\n  }\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n  }\n  availableLocationDesks(locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks) @include(if: $locationExists) {\n    uniqueId\n    name\n    locationTags {\n      uniqueId\n      name\n      tagType\n    }\n  }\n  bookingDetailsSelectorQueryPaginatedOrganizationMembers: paginatedOrganizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $bookingDetailsSelectorOrganizationMembersSortingValues) @include(if: $organizationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment mainRootLayout_query on Query {\n  me {\n    email {\n      email\n      verified\n      id\n    }\n    givenName\n    middleName\n    familyName\n    photoUrl\n    id\n  }\n  ...newFeedbackDialog_query\n}\n\nfragment newBookingDialog_query on Query {\n  me {\n    id\n  }\n  organizationBookingPermissions(organizationId: $organizationId) @include(if: $organizationExists) {\n    canAddBookingOnBehalf\n  }\n  ...bookingDetailsSelector_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n\nfragment organizationMemberSelector_query on Query {\n  organizationMemberSelectorPaginatedOrganizationMembers: paginatedOrganizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $organizationMemberSelectorOrganizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment rootShell_query on Query {\n  me {\n    id\n  }\n  ...observability_query\n  ...mainRootLayout_query\n}\n\nfragment teamAboutTab_query on Query {\n  team(id: $teamId) {\n    id\n    name\n    about\n    timezone\n    organization {\n      name\n    }\n    canModify\n    members {\n      customer {\n        uniqueId\n      }\n      organizationMember {\n        uniqueId\n      }\n      id\n    }\n  }\n  ...organizationMemberSelector_query\n}\n\nfragment teamBookingsTab_query on Query {\n  bookings(first: 50, where: {teamIds: [$teamId], fromGTE: $bookingsSearchCriteriaFrom, fromLTE: $bookingsSearchCriteriaUntil, includeMineOnly: false}, orderBy: $bookingSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        to\n        customer {\n          uniqueId\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  me {\n    id\n  }\n  ...bookingCard_query\n  ...newBookingDialog_query\n}\n\nfragment teamMemberCard_TeamMemberDetails on TeamMemberDetails {\n  id\n  customer {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organizationMember {\n    customer {\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n  }\n}\n\nfragment teamMemberCard_query on Query {\n  team(id: $teamId) {\n    id\n    name\n    about\n    canModify\n    members {\n      id\n      customer {\n        uniqueId\n      }\n      organizationMember {\n        uniqueId\n      }\n    }\n  }\n}\n\nfragment teamPage_query on Query {\n  team(id: $teamId) {\n    name\n    organization {\n      uniqueId\n    }\n    id\n  }\n  ...teamBookingsTab_query\n  ...teamAboutTab_query\n  ...teamPeopleTab_query\n}\n\nfragment teamPeopleTab_query on Query {\n  team(id: $teamId) {\n    id\n    name\n    about\n    organization {\n      name\n    }\n    canModify\n    members {\n      customer {\n        uniqueId\n      }\n      organizationMember {\n        uniqueId\n      }\n      id\n    }\n  }\n  paginatedTeamMembers(first: 50, where: {teamId: $teamId, nameContains: $peopleNameSearchText}, orderBy: $teamPeopleSortingValues) @include(if: $teamExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...teamMemberCard_TeamMemberDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...teamMemberCard_query\n  ...organizationMemberSelector_query\n}\n"
  }
};
})();

(node as any).hash = "aba86f1d014cfc83bd4a2be1c19df81b";

export default node;
