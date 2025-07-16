/**
 * @generated SignedSource<<2b083608c56685209491407aaa19eea7>>
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
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type TeamOrderField = "ABOUT" | "NAME" | "%future added value";
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
    readonly from: any;
    readonly isPaymentRequired: boolean;
    readonly paymentStatus: {
      readonly type: PaymentStatus;
    };
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
  "name": "type",
  "storageKey": null
},
v16 = [
  (v15/*: any*/)
],
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v18 = [
  (v15/*: any*/),
  (v17/*: any*/)
],
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
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
  (v20/*: any*/),
  (v17/*: any*/),
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
v22 = [
  (v20/*: any*/),
  (v17/*: any*/)
],
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v24 = [
  (v20/*: any*/),
  (v17/*: any*/),
  (v23/*: any*/)
],
v25 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_OrganizationCustomTagDetails",
  "kind": "LinkedField",
  "name": "customTags",
  "plural": true,
  "selections": (v24/*: any*/),
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_OrganizationZoneDetails",
  "kind": "LinkedField",
  "name": "zones",
  "plural": true,
  "selections": (v24/*: any*/),
  "storageKey": null
},
v27 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v28 = [
  (v27/*: any*/)
],
v29 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v30 = [
  (v19/*: any*/),
  (v17/*: any*/)
],
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
      (v27/*: any*/)
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
            "concreteType": "PaymentStatusDetails",
            "kind": "LinkedField",
            "name": "paymentStatus",
            "plural": false,
            "selections": (v16/*: any*/),
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
            "concreteType": "PaymentStatusDetails",
            "kind": "LinkedField",
            "name": "paymentStatus",
            "plural": false,
            "selections": (v18/*: any*/),
            "storageKey": null
          },
          (v19/*: any*/),
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
            "selections": (v16/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_CustomerDetails",
            "kind": "LinkedField",
            "name": "involvedCustomers",
            "plural": true,
            "selections": (v21/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_OrganizationDetails",
            "kind": "LinkedField",
            "name": "involvedOrganizations",
            "plural": true,
            "selections": (v22/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "involvedLocations",
            "plural": true,
            "selections": (v22/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_TeamDetails",
            "kind": "LinkedField",
            "name": "involvedTeams",
            "plural": true,
            "selections": (v22/*: any*/),
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
              (v20/*: any*/),
              (v17/*: any*/),
              (v23/*: any*/),
              (v25/*: any*/),
              (v26/*: any*/)
            ],
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
            "name": "totalAmountToDisplay",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "PaymentMethodTypeDetails",
            "kind": "LinkedField",
            "name": "paymentMethod",
            "plural": false,
            "selections": (v16/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCheckoutSessionDetails",
            "kind": "LinkedField",
            "name": "bookingCheckoutSession",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "checkoutUrl",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "paymentExpiry",
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
                  (v20/*: any*/),
                  (v17/*: any*/),
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
            "fields": (v28/*: any*/),
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v29/*: any*/),
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
                "selections": (v30/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v31/*: any*/)
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
        "selections": (v18/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v32/*: any*/),
        "concreteType": "ConnectionOfOrganizationMemberEdge",
        "kind": "LinkedField",
        "name": "organizationMembers",
        "plural": false,
        "selections": [
          (v29/*: any*/),
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
                  (v19/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_CustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": (v21/*: any*/),
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
          (v31/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v32/*: any*/),
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
                  (v27/*: any*/)
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "ConnectionOfTeamEdge",
            "kind": "LinkedField",
            "name": "customerTeams",
            "plural": false,
            "selections": [
              (v29/*: any*/),
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
                    "selections": (v30/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v31/*: any*/)
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
              (v27/*: any*/),
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
          (v20/*: any*/),
          (v17/*: any*/),
          (v25/*: any*/),
          (v26/*: any*/)
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
        "selections": (v18/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v28/*: any*/),
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
        "selections": (v18/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "af5475114c66480c1f60755f2675f24f",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationBooking_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationBooking_rootQuery(\n  $organizationId: String!\n  $bookingId: String!\n  $peopleNameSearchText: String\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $locationId: String!\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $customerId: String!\n  $customerExists: Boolean!\n  $teamsSortingValues: [TeamOrderInput!]\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  booking(id: $bookingId) {\n    from\n    bookedOnMarketplace\n    isPaymentRequired\n    paymentStatus {\n      type\n    }\n    id\n  }\n  ...editPrivateBooking_query\n  ...editPrivateBooking_organizationMembers_query\n  ...editPrivateBooking_customerTeams_query\n  ...editPrivateBooking_availableResources_query\n  ...editMarketplaceBooking_query\n  ...editMarketplaceBooking_booking_query\n  ...editMarketplaceBooking_organizationMembers_query\n  ...editMarketplaceBooking_customerTeams_query\n  ...payMarketplaceBooking_booking_query\n}\n\nfragment editMarketplaceBooking_booking_query on Query {\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    type {\n      type\n    }\n    involvedCustomers {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    involvedOrganizations {\n      uniqueId\n      name\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    involvedTeams {\n      uniqueId\n      name\n    }\n    resources {\n      uniqueId\n      name\n      color\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n    isPaymentRequired\n    paymentStatus {\n      type\n      name\n    }\n    invoiceUrl\n  }\n}\n\nfragment editMarketplaceBooking_customerTeams_query on Query {\n  customerTeams(where: {organizationId: $organizationId, customerId: $customerId}, orderBy: $teamsSortingValues) @include(if: $customerExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment editMarketplaceBooking_organizationMembers_query on Query {\n  organizationMembers(where: {organizationId: $organizationId, nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment editMarketplaceBooking_query on Query {\n  openingHoursMinutesStep\n  ...singleChoiceMarketplaceBookingType_query\n}\n\nfragment editPrivateBooking_availableResources_query on Query {\n  availableResources(where: {organizationId: $organizationId, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment editPrivateBooking_customerTeams_query on Query {\n  customerTeams(where: {organizationId: $organizationId, customerId: $customerId}, orderBy: $teamsSortingValues) @include(if: $customerExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment editPrivateBooking_organizationMembers_query on Query {\n  organizationMembers(where: {organizationId: $organizationId, nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment editPrivateBooking_query on Query {\n  locations(where: {organizationId: $organizationId}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    type {\n      type\n    }\n    involvedCustomers {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    involvedOrganizations {\n      uniqueId\n      name\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    involvedTeams {\n      uniqueId\n      name\n    }\n    resources {\n      uniqueId\n      name\n      color\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n  }\n  openingHoursMinutesStep\n  ...singleChoiceBookingType_query\n}\n\nfragment payMarketplaceBooking_booking_query on Query {\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    type {\n      type\n    }\n    involvedCustomers {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    involvedOrganizations {\n      uniqueId\n      name\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    involvedTeams {\n      uniqueId\n      name\n    }\n    resources {\n      uniqueId\n      name\n      color\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n    totalAmountToDisplay\n    paymentMethod {\n      type\n    }\n    bookingCheckoutSession {\n      checkoutUrl\n    }\n    paymentExpiry\n    invoiceUrl\n    lineItems {\n      quantity\n      productVersion {\n        uniqueId\n        name\n        priceToDisplay\n      }\n    }\n    isPaymentRequired\n    paymentStatus {\n      type\n      name\n    }\n  }\n  organizationBookingPermissions(organizationId: $organizationId) {\n    canModifyPaymentMethod\n  }\n  paymentStatuses {\n    type\n    name\n  }\n}\n\nfragment singleChoiceBookingType_query on Query {\n  bookingTypes {\n    type\n    name\n  }\n}\n\nfragment singleChoiceMarketplaceBookingType_query on Query {\n  marketplaceBookingTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "8efc9114f47ff7668c3ec9ae7b327856";

export default node;
