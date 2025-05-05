/**
 * @generated SignedSource<<70be2a8511e36aff89ebd77b9a8903af>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "About" | "Name" | "Timezone" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationMemberOrderField = "FamilyName" | "GivenName" | "MiddleName" | "Name" | "PhoneNumber" | "Role" | "Status" | "%future added value";
export type PaymentStatus = "Expired" | "NoPaymentRequired" | "Paid" | "Pending" | "Unpaid" | "%future added value";
export type TeamOrderField = "About" | "Name" | "%future added value";
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type TeamOrderInput = {
  direction: OrderDirection;
  field: TeamOrderField;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type pageOrganizationBooking_rootQuery$variables = {
  bookingId: string;
  customerExists: boolean;
  customerId: string;
  dateFromToGetAvailableResources: any;
  dateUntilToGetAvailableResources: any;
  locationId: string;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationId: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
  teamsSortingValues?: ReadonlyArray<TeamOrderInput> | null | undefined;
};
export type pageOrganizationBooking_rootQuery$data = {
  readonly booking: {
    readonly bookedOnMarketplace: boolean;
    readonly bookingCheckoutSession: {
      readonly paymentStatus: PaymentStatus;
    } | null | undefined;
    readonly from: any;
    readonly isPaymentRequired: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"editMarketplaceBooking_booking_query" | "editMarketplaceBooking_customerTeams_query" | "editMarketplaceBooking_organizationMembers_query" | "editMarketplaceBooking_query" | "editPrivateBooking_availableResources_query" | "editPrivateBooking_customerTeams_query" | "editPrivateBooking_organizationMembers_query" | "editPrivateBooking_query" | "payMarketplaceBooking_booking_query">;
};
export type pageOrganizationBooking_rootQuery = {
  response: pageOrganizationBooking_rootQuery$data;
  variables: pageOrganizationBooking_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingId"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customerExists"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customerId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateFromToGetAvailableResources"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateUntilToGetAvailableResources"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamsSortingValues"
},
v11 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "bookingId"
  }
],
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "bookedOnMarketplace",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isPaymentRequired",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentStatus",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v20 = [
  (v18/*: any*/),
  (v19/*: any*/),
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
v21 = [
  (v18/*: any*/),
  (v19/*: any*/)
],
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v23 = [
  (v18/*: any*/),
  (v19/*: any*/),
  (v22/*: any*/)
],
v24 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_OrganizationCustomTagDetails",
  "kind": "LinkedField",
  "name": "customTags",
  "plural": true,
  "selections": (v23/*: any*/),
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_OrganizationZoneDetails",
  "kind": "LinkedField",
  "name": "zones",
  "plural": true,
  "selections": (v23/*: any*/),
  "storageKey": null
},
v26 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v27 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v28 = [
  (v16/*: any*/),
  (v19/*: any*/)
],
v29 = {
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
v30 = [
  (v17/*: any*/),
  (v19/*: any*/)
],
v31 = [
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
      (v26/*: any*/)
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
    "name": "pageOrganizationBooking_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v11/*: any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v12/*: any*/),
          (v13/*: any*/),
          (v14/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCheckoutSessionDetails",
            "kind": "LinkedField",
            "name": "bookingCheckoutSession",
            "plural": false,
            "selections": [
              (v15/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateBooking_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateBooking_organizationMembers_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateBooking_customerTeams_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateBooking_availableResources_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editMarketplaceBooking_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editMarketplaceBooking_booking_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editMarketplaceBooking_organizationMembers_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editMarketplaceBooking_customerTeams_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "payMarketplaceBooking_booking_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v7/*: any*/),
      (v0/*: any*/),
      (v9/*: any*/),
      (v8/*: any*/),
      (v5/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v2/*: any*/),
      (v1/*: any*/),
      (v10/*: any*/),
      (v6/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationBooking_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v11/*: any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v12/*: any*/),
          (v13/*: any*/),
          (v14/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCheckoutSessionDetails",
            "kind": "LinkedField",
            "name": "bookingCheckoutSession",
            "plural": false,
            "selections": [
              (v15/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "checkoutUrl",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "amountTotalToDisplay",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v16/*: any*/),
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
            "selections": [
              (v17/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_CustomerDetails",
            "kind": "LinkedField",
            "name": "involvedCustomers",
            "plural": true,
            "selections": (v20/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_OrganizationDetails",
            "kind": "LinkedField",
            "name": "involvedOrganizations",
            "plural": true,
            "selections": (v21/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "involvedLocations",
            "plural": true,
            "selections": (v21/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_TeamDetails",
            "kind": "LinkedField",
            "name": "involvedTeams",
            "plural": true,
            "selections": (v21/*: any*/),
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
              (v18/*: any*/),
              (v19/*: any*/),
              (v22/*: any*/),
              (v24/*: any*/),
              (v25/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "bookingCheckoutSessionExpiry",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "LineItemDetails",
            "kind": "LinkedField",
            "name": "lineItems",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "quantity",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "Booking_ProductVersionDetails",
                "kind": "LinkedField",
                "name": "productVersion",
                "plural": false,
                "selections": [
                  (v18/*: any*/),
                  (v19/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "priceToDisplay",
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
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          {
            "fields": [
              (v26/*: any*/)
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "LocationConnection",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v27/*: any*/),
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
                "selections": (v28/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v29/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "openingHoursMinutesStep",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingTypeDetails",
        "kind": "LinkedField",
        "name": "bookingTypes",
        "plural": true,
        "selections": (v30/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v31/*: any*/),
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "organizationMembers",
        "plural": false,
        "selections": [
          (v27/*: any*/),
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
                  (v16/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_CustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": (v20/*: any*/),
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
          (v29/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v31/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "bookingDetailsSelectorQuery_organizationMembers",
        "kind": "LinkedHandle",
        "name": "organizationMembers"
      },
      {
        "condition": "customerExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "orderBy",
                "variableName": "teamsSortingValues"
              },
              {
                "fields": [
                  {
                    "kind": "Variable",
                    "name": "customerId",
                    "variableName": "customerId"
                  },
                  (v26/*: any*/)
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "TeamConnection",
            "kind": "LinkedField",
            "name": "customerTeams",
            "plural": false,
            "selections": [
              (v27/*: any*/),
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
                    "selections": (v28/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v29/*: any*/)
            ],
            "storageKey": null
          }
        ]
      },
      {
        "alias": null,
        "args": [
          {
            "fields": [
              {
                "kind": "Variable",
                "name": "from",
                "variableName": "dateFromToGetAvailableResources"
              },
              {
                "kind": "Variable",
                "name": "locationId",
                "variableName": "locationId"
              },
              (v26/*: any*/),
              {
                "kind": "Variable",
                "name": "until",
                "variableName": "dateUntilToGetAvailableResources"
              }
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "BookingResourceDetails",
        "kind": "LinkedField",
        "name": "availableResources",
        "plural": true,
        "selections": [
          (v18/*: any*/),
          (v19/*: any*/),
          (v24/*: any*/),
          (v25/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingTypeDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingTypes",
        "plural": true,
        "selections": (v30/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "cc84fa5c46f9f004b8ef4772c6f9e143",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationBooking_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationBooking_rootQuery(\n  $organizationId: String!\n  $bookingId: String!\n  $peopleNameSearchText: String\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $locationId: String!\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $customerId: String!\n  $customerExists: Boolean!\n  $teamsSortingValues: [TeamOrderInput!]\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  booking(id: $bookingId) {\n    from\n    bookedOnMarketplace\n    isPaymentRequired\n    bookingCheckoutSession {\n      paymentStatus\n    }\n    id\n  }\n  ...editPrivateBooking_query\n  ...editPrivateBooking_organizationMembers_query\n  ...editPrivateBooking_customerTeams_query\n  ...editPrivateBooking_availableResources_query\n  ...editMarketplaceBooking_query\n  ...editMarketplaceBooking_booking_query\n  ...editMarketplaceBooking_organizationMembers_query\n  ...editMarketplaceBooking_customerTeams_query\n  ...payMarketplaceBooking_booking_query\n}\n\nfragment editMarketplaceBooking_booking_query on Query {\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    type {\n      type\n    }\n    involvedCustomers {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    involvedOrganizations {\n      uniqueId\n      name\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    involvedTeams {\n      uniqueId\n      name\n    }\n    resources {\n      uniqueId\n      name\n      color\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n    bookingCheckoutSession {\n      checkoutUrl\n      paymentStatus\n      amountTotalToDisplay\n    }\n    bookingCheckoutSessionExpiry\n  }\n}\n\nfragment editMarketplaceBooking_customerTeams_query on Query {\n  customerTeams(where: {organizationId: $organizationId, customerId: $customerId}, orderBy: $teamsSortingValues) @include(if: $customerExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment editMarketplaceBooking_organizationMembers_query on Query {\n  organizationMembers(where: {organizationId: $organizationId, nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment editMarketplaceBooking_query on Query {\n  openingHoursMinutesStep\n  ...singleChoiceMarketplaceBookingType_query\n}\n\nfragment editPrivateBooking_availableResources_query on Query {\n  availableResources(where: {organizationId: $organizationId, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment editPrivateBooking_customerTeams_query on Query {\n  customerTeams(where: {organizationId: $organizationId, customerId: $customerId}, orderBy: $teamsSortingValues) @include(if: $customerExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment editPrivateBooking_organizationMembers_query on Query {\n  organizationMembers(where: {organizationId: $organizationId, nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment editPrivateBooking_query on Query {\n  locations(where: {organizationId: $organizationId}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    type {\n      type\n    }\n    involvedCustomers {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    involvedOrganizations {\n      uniqueId\n      name\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    involvedTeams {\n      uniqueId\n      name\n    }\n    resources {\n      uniqueId\n      name\n      color\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n  }\n  openingHoursMinutesStep\n  ...singleChoiceBookingType_query\n}\n\nfragment payMarketplaceBooking_booking_query on Query {\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    type {\n      type\n    }\n    involvedCustomers {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    involvedOrganizations {\n      uniqueId\n      name\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    involvedTeams {\n      uniqueId\n      name\n    }\n    resources {\n      uniqueId\n      name\n      color\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n    bookingCheckoutSession {\n      checkoutUrl\n      paymentStatus\n      amountTotalToDisplay\n    }\n    bookingCheckoutSessionExpiry\n    lineItems {\n      quantity\n      productVersion {\n        uniqueId\n        name\n        priceToDisplay\n      }\n    }\n  }\n}\n\nfragment singleChoiceBookingType_query on Query {\n  bookingTypes {\n    type\n    name\n  }\n}\n\nfragment singleChoiceMarketplaceBookingType_query on Query {\n  marketplaceBookingTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "ab1899e54c76fc214c147d1ae847d3ab";

export default node;
