/**
 * @generated SignedSource<<5efd9e8f470fe4c361ccd9e91698db30>>
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
export type OrganizationMemberOrderField = "FAMILY_NAME" | "GIVEN_NAME" | "MIDDLE_NAME" | "NAME" | "PHONE_NUMBER" | "ROLE" | "STATUS" | "%future added value";
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type organizationBookings_rootQuery$variables = {
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaTo: any;
  customerIds: ReadonlyArray<string>;
  locationIds: ReadonlyArray<string>;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  organizationUniqueAlphanumericName: string;
  peopleNameSearchText?: string | null | undefined;
  teamIds: ReadonlyArray<string>;
};
export type organizationBookings_rootQuery$data = {
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
  readonly " $fragmentSpreads": FragmentRefs<"bookings_bookings_query" | "bookings_query" | "locationSelector_allLocations_query" | "organizationUserSelector_organizationMembers_query" | "teamSelector_allTeams_query">;
};
export type organizationBookings_rootQuery = {
  response: organizationBookings_rootQuery$data;
  variables: organizationBookings_rootQuery$variables;
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
  "name": "customerIds"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationIds"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamIds"
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v11 = [
  (v9/*: any*/),
  (v10/*: any*/)
],
v12 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "uniqueAlphanumericName",
      "variableName": "organizationUniqueAlphanumericName"
    }
  ],
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "organization",
  "plural": false,
  "selections": (v11/*: any*/),
  "storageKey": null
},
v13 = {
  "kind": "Variable",
  "name": "organizationUniqueAlphanumericName",
  "variableName": "organizationUniqueAlphanumericName"
},
v14 = [
  (v13/*: any*/)
],
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v16 = [
  (v15/*: any*/),
  (v10/*: any*/)
],
v17 = {
  "alias": null,
  "args": (v14/*: any*/),
  "concreteType": "LocationDetails",
  "kind": "LinkedField",
  "name": "myLocations",
  "plural": true,
  "selections": [
    (v9/*: any*/),
    (v10/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "Location_OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": (v16/*: any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": (v14/*: any*/),
  "concreteType": "TeamDetails",
  "kind": "LinkedField",
  "name": "myTeams",
  "plural": true,
  "selections": [
    (v9/*: any*/),
    (v10/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "Team_OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": (v16/*: any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
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
v24 = [
  (v15/*: any*/),
  (v10/*: any*/),
  (v20/*: any*/),
  (v21/*: any*/),
  (v22/*: any*/),
  (v23/*: any*/)
],
v25 = {
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
v26 = {
  "fields": (v14/*: any*/),
  "kind": "ObjectValue",
  "name": "where"
},
v27 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v10/*: any*/)
],
v28 = [
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
        "name": "customerIds",
        "variableName": "customerIds"
      },
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
            "name": "organizationUniqueAlphanumericNames.0",
            "variableName": "organizationUniqueAlphanumericName"
          }
        ],
        "kind": "ListValue",
        "name": "organizationUniqueAlphanumericNames"
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
v29 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v30 = [
  (v15/*: any*/),
  (v10/*: any*/),
  (v29/*: any*/)
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
    "name": "organizationBookings_rootQuery",
    "selections": [
      (v12/*: any*/),
      (v17/*: any*/),
      (v18/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationUserSelector_organizationMembers_query"
      },
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
      (v6/*: any*/),
      (v3/*: any*/),
      (v8/*: any*/),
      (v2/*: any*/),
      (v0/*: any*/),
      (v1/*: any*/),
      (v4/*: any*/),
      (v7/*: any*/),
      (v5/*: any*/)
    ],
    "kind": "Operation",
    "name": "organizationBookings_rootQuery",
    "selections": [
      (v12/*: any*/),
      (v17/*: any*/),
      (v18/*: any*/),
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "organizationMembersSortingValues"
          },
          {
            "fields": [
              {
                "kind": "Variable",
                "name": "nameContains",
                "variableName": "peopleNameSearchText"
              },
              (v13/*: any*/)
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "ConnectionOfOrganizationMemberEdge",
        "kind": "LinkedField",
        "name": "organizationMembers",
        "plural": false,
        "selections": [
          (v19/*: any*/),
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
                  (v9/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_CustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": (v24/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v25/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          (v26/*: any*/)
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v19/*: any*/),
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
                "selections": (v11/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v25/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          (v26/*: any*/)
        ],
        "concreteType": "ConnectionOfTeamEdge",
        "kind": "LinkedField",
        "name": "teams",
        "plural": false,
        "selections": [
          (v19/*: any*/),
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
                "selections": (v11/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v25/*: any*/)
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
          (v9/*: any*/),
          (v10/*: any*/),
          (v20/*: any*/),
          (v21/*: any*/),
          (v22/*: any*/),
          (v23/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v14/*: any*/),
        "concreteType": "OrganizationBookingPermissions",
        "kind": "LinkedField",
        "name": "organizationBookingPermissions",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canModifyPaymentMethod",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "PaymentStatusDetails",
        "kind": "LinkedField",
        "name": "paymentStatuses",
        "plural": true,
        "selections": (v27/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v28/*: any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v19/*: any*/),
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
                  (v9/*: any*/),
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
                    "concreteType": "BookingTypeDetails",
                    "kind": "LinkedField",
                    "name": "type",
                    "plural": false,
                    "selections": (v27/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Booking_CustomerDetails",
                    "kind": "LinkedField",
                    "name": "involvedCustomers",
                    "plural": true,
                    "selections": (v24/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Booking_OrganizationDetails",
                    "kind": "LinkedField",
                    "name": "involvedOrganizations",
                    "plural": true,
                    "selections": [
                      (v15/*: any*/)
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
                    "selections": (v16/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Booking_TeamDetails",
                    "kind": "LinkedField",
                    "name": "involvedTeams",
                    "plural": true,
                    "selections": (v16/*: any*/),
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
                      (v15/*: any*/),
                      (v10/*: any*/),
                      (v29/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Booking_OrganizationCustomTagDetails",
                        "kind": "LinkedField",
                        "name": "customTags",
                        "plural": true,
                        "selections": (v30/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Booking_OrganizationZoneDetails",
                        "kind": "LinkedField",
                        "name": "zones",
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
                    "selections": (v27/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "invoiceUrl",
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
          (v25/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v28/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "bookings_bookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "c208020daef043678827bf3eafff401a",
    "id": null,
    "metadata": {},
    "name": "organizationBookings_rootQuery",
    "operationKind": "query",
    "text": "query organizationBookings_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $locationIds: [String!]!\n  $teamIds: [String!]!\n  $customerIds: [String!]!\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaTo: DateTime!\n  $locationsSortingValues: [LocationOrderInput!]\n  $peopleNameSearchText: String\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n) {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    id\n    name\n  }\n  myLocations(organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    id\n    name\n    organization {\n      uniqueId\n      name\n    }\n  }\n  myTeams(organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    id\n    name\n    organization {\n      uniqueId\n      name\n    }\n  }\n  ...organizationUserSelector_organizationMembers_query\n  ...locationSelector_allLocations_query\n  ...teamSelector_allTeams_query\n  ...bookings_query\n  ...bookings_bookings_query\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  until\n  notes\n  type {\n    type\n    name\n  }\n  involvedCustomers {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  involvedOrganizations {\n    uniqueId\n  }\n  involvedLocations {\n    uniqueId\n    name\n  }\n  involvedTeams {\n    uniqueId\n    name\n  }\n  resources {\n    uniqueId\n    name\n    color\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n  isPaymentRequired\n  paymentStatus {\n    type\n    name\n  }\n  invoiceUrl\n}\n\nfragment bookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organizationBookingPermissions(organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    canModifyPaymentMethod\n  }\n  paymentStatuses {\n    type\n    name\n  }\n}\n\nfragment bookings_bookings_query on Query {\n  bookings(where: {organizationUniqueAlphanumericNames: [$organizationUniqueAlphanumericName], locationIds: $locationIds, teamIds: $teamIds, customerIds: $customerIds, fromGte: $bookingsSearchCriteriaFrom, fromLte: $bookingsSearchCriteriaTo}, orderBy: [{field: FROM, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        until\n        notes\n        type {\n          type\n          name\n        }\n        involvedCustomers {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        involvedOrganizations {\n          uniqueId\n        }\n        involvedLocations {\n          uniqueId\n          name\n        }\n        involvedTeams {\n          uniqueId\n          name\n        }\n        resources {\n          uniqueId\n          name\n          color\n          customTags {\n            uniqueId\n            name\n            color\n          }\n          zones {\n            uniqueId\n            name\n            color\n          }\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment bookings_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  ...bookingCard_query\n}\n\nfragment locationSelector_allLocations_query on Query {\n  locations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment organizationUserSelector_organizationMembers_query on Query {\n  organizationMembers(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n      }\n    }\n  }\n}\n\nfragment teamSelector_allTeams_query on Query {\n  teams(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d24f854dd1a70acf7e2c7083cc6da97a";

export default node;
