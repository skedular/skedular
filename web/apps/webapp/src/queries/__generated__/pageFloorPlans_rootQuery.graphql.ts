/**
 * @generated SignedSource<<70307003cdb271b530c80a4102f81a4f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type FloorPlanOrderField = "NAME" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationMemberOrderField = "FAMILY_NAME" | "GIVEN_NAME" | "MIDDLE_NAME" | "NAME" | "PHONE_NUMBER" | "ROLE" | "STATUS" | "%future added value";
export type OrganizationTagOrderField = "DESCRIPTION" | "NAME" | "TYPE" | "%future added value";
export type ResourceOrderField = "NAME" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type FloorPlanOrderInput = {
  direction: OrderDirection;
  field: FloorPlanOrderField;
};
export type ResourceOrderInput = {
  direction: OrderDirection;
  field: ResourceOrderField;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type pageFloorPlans_rootQuery$variables = {
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaTo: any;
  customTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  floorPlanExists: boolean;
  floorPlanId: string;
  floorPlansSortingValues?: ReadonlyArray<FloorPlanOrderInput> | null | undefined;
  locationId: string;
  organizationId: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
  resourcesSortingValues?: ReadonlyArray<ResourceOrderInput> | null | undefined;
  zonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
};
export type pageFloorPlans_rootQuery$data = {
  readonly location: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"floorPlans_bookings_query" | "floorPlans_floorPlan_query" | "floorPlans_query">;
};
export type pageFloorPlans_rootQuery = {
  response: pageFloorPlans_rootQuery$data;
  variables: pageFloorPlans_rootQuery$variables;
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
  "name": "customTagsSortingValues"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "floorPlanExists"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "floorPlanId"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "floorPlansSortingValues"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
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
  "name": "resourcesSortingValues"
},
v11 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zonesSortingValues"
},
v12 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "locationId"
  }
],
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
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
  "name": "givenName",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v19 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v20 = {
  "fields": [
    (v19/*: any*/)
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v23 = {
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
v24 = [
  (v21/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "OrganizationTagEdge",
    "kind": "LinkedField",
    "name": "edges",
    "plural": true,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTagDetails",
        "kind": "LinkedField",
        "name": "node",
        "plural": false,
        "selections": [
          (v14/*: any*/),
          (v13/*: any*/),
          (v22/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  (v23/*: any*/)
],
v25 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v26 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v27 = [
  (v26/*: any*/),
  (v13/*: any*/),
  (v15/*: any*/),
  (v16/*: any*/),
  (v17/*: any*/),
  (v18/*: any*/)
],
v28 = [
  (v26/*: any*/),
  (v13/*: any*/),
  (v22/*: any*/)
],
v29 = [
  {
    "fields": [
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
        "items": [
          {
            "kind": "Variable",
            "name": "locationIds.0",
            "variableName": "locationId"
          }
        ],
        "kind": "ListValue",
        "name": "locationIds"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v30 = [
  (v26/*: any*/),
  (v13/*: any*/)
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
      (v11/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageFloorPlans_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v12/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v13/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "floorPlans_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "floorPlans_floorPlan_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "floorPlans_bookings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v7/*: any*/),
      (v6/*: any*/),
      (v4/*: any*/),
      (v3/*: any*/),
      (v11/*: any*/),
      (v2/*: any*/),
      (v5/*: any*/),
      (v10/*: any*/),
      (v9/*: any*/),
      (v8/*: any*/),
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageFloorPlans_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v12/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v13/*: any*/),
          (v14/*: any*/)
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
          (v14/*: any*/),
          (v13/*: any*/),
          (v15/*: any*/),
          (v16/*: any*/),
          (v17/*: any*/),
          (v18/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "deskResourceType",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "roomResourceType",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "parkingResourceType",
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "customTagsSortingValues"
          },
          (v20/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "customTags",
        "plural": false,
        "selections": (v24/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "zonesSortingValues"
          },
          (v20/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v24/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "floorPlansSortingValues"
          },
          {
            "fields": [
              (v25/*: any*/)
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "FloorPlanConnection",
        "kind": "LinkedField",
        "name": "floorPlans",
        "plural": false,
        "selections": [
          (v21/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "FloorPlanEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "FloorPlanDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v14/*: any*/),
                  (v13/*: any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v23/*: any*/)
        ],
        "storageKey": null
      },
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
              (v19/*: any*/)
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "organizationMembers",
        "plural": false,
        "selections": [
          (v21/*: any*/),
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
                    "concreteType": "Organization_CustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": (v27/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v23/*: any*/)
        ],
        "storageKey": null
      },
      {
        "condition": "floorPlanExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "id",
                "variableName": "floorPlanId"
              }
            ],
            "concreteType": "FloorPlanDetails",
            "kind": "LinkedField",
            "name": "floorPlan",
            "plural": false,
            "selections": [
              (v14/*: any*/),
              (v13/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnImageFile",
                "kind": "LinkedField",
                "name": "image",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CdnFile",
                    "kind": "LinkedField",
                    "name": "original",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "url",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "height",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "width",
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
                "args": null,
                "concreteType": "ResourcePositionDetails",
                "kind": "LinkedField",
                "name": "resourcePositions",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "x",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "y",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ResourceDetails",
                    "kind": "LinkedField",
                    "name": "resource",
                    "plural": false,
                    "selections": [
                      (v14/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v14/*: any*/)
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
                "variableName": "resourcesSortingValues"
              },
              {
                "fields": [
                  {
                    "kind": "Variable",
                    "name": "floorPlanId",
                    "variableName": "floorPlanId"
                  },
                  (v25/*: any*/)
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "ResourceConnection",
            "kind": "LinkedField",
            "name": "resources",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "ResourceEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ResourceDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v14/*: any*/),
                      (v13/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Location_OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "resourceType",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "tagType",
                            "storageKey": null
                          },
                          (v26/*: any*/),
                          (v13/*: any*/),
                          (v22/*: any*/)
                        ],
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "inactive",
                        "storageKey": null
                      },
                      (v22/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "capacity",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Location_OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "customTags",
                        "plural": true,
                        "selections": (v28/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Location_OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "zones",
                        "plural": true,
                        "selections": (v28/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Location_OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "productTags",
                        "plural": true,
                        "selections": (v28/*: any*/),
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
          }
        ]
      },
      {
        "alias": null,
        "args": (v29/*: any*/),
        "concreteType": "BookingConnection",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v21/*: any*/),
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
                    "concreteType": "Booking_CustomerDetails",
                    "kind": "LinkedField",
                    "name": "involvedCustomers",
                    "plural": true,
                    "selections": (v27/*: any*/),
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
                      (v26/*: any*/),
                      (v13/*: any*/),
                      (v22/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Booking_OrganizationCustomTagDetails",
                        "kind": "LinkedField",
                        "name": "customTags",
                        "plural": true,
                        "selections": (v28/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Booking_OrganizationZoneDetails",
                        "kind": "LinkedField",
                        "name": "zones",
                        "plural": true,
                        "selections": (v28/*: any*/),
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
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
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "type",
                        "storageKey": null
                      },
                      (v13/*: any*/)
                    ],
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
                      (v26/*: any*/)
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
                    "selections": (v30/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Booking_TeamDetails",
                    "kind": "LinkedField",
                    "name": "involvedTeams",
                    "plural": true,
                    "selections": (v30/*: any*/),
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
          (v23/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v29/*: any*/),
        "filters": [
          "where"
        ],
        "handle": "connection",
        "key": "floorPlans_bookings",
        "kind": "LinkedHandle",
        "name": "bookings"
      }
    ]
  },
  "params": {
    "cacheID": "40d38584f279529f44969f233222ea70",
    "id": null,
    "metadata": {},
    "name": "pageFloorPlans_rootQuery",
    "operationKind": "query",
    "text": "query pageFloorPlans_rootQuery(\n  $organizationId: String!\n  $locationId: String!\n  $floorPlanId: String!\n  $floorPlanExists: Boolean!\n  $zonesSortingValues: [OrganizationTagOrderInput!]\n  $customTagsSortingValues: [OrganizationTagOrderInput!]\n  $floorPlansSortingValues: [FloorPlanOrderInput!]\n  $resourcesSortingValues: [ResourceOrderInput!]\n  $peopleNameSearchText: String\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaTo: DateTime!\n) {\n  location(id: $locationId) {\n    name\n    id\n  }\n  ...floorPlans_query\n  ...floorPlans_floorPlan_query\n  ...floorPlans_bookings_query\n}\n\nfragment bookingCard_BookingDetails on BookingDetails {\n  id\n  from\n  until\n  notes\n  type {\n    type\n    name\n  }\n  involvedCustomers {\n    uniqueId\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  involvedOrganizations {\n    uniqueId\n  }\n  involvedLocations {\n    uniqueId\n    name\n  }\n  involvedTeams {\n    uniqueId\n    name\n  }\n  resources {\n    uniqueId\n    name\n    color\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n\nfragment bookingCard_query on Query {\n  me {\n    id\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n}\n\nfragment customTagSelector_allCustomTags_query on Query {\n  customTags(where: {organizationId: $organizationId}, orderBy: $customTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n\nfragment floorPlanSelector_allFloorPlans_query on Query {\n  floorPlans(where: {locationId: $locationId}, orderBy: $floorPlansSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment floorPlans_bookings_query on Query {\n  bookings(where: {locationIds: [$locationId], fromGte: $bookingsSearchCriteriaFrom, fromLte: $bookingsSearchCriteriaTo}) {\n    totalCount\n    edges {\n      node {\n        id\n        involvedCustomers {\n          uniqueId\n        }\n        resources {\n          uniqueId\n        }\n        ...bookingCard_BookingDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment floorPlans_floorPlan_query on Query {\n  floorPlan(id: $floorPlanId) @include(if: $floorPlanExists) {\n    id\n    name\n    image {\n      original {\n        url\n        height\n        width\n      }\n    }\n    resourcePositions {\n      x\n      y\n      resource {\n        id\n      }\n      id\n    }\n  }\n  resources(where: {locationId: $locationId, floorPlanId: $floorPlanId}, orderBy: $resourcesSortingValues) @include(if: $floorPlanExists) {\n    edges {\n      node {\n        id\n        name\n        resourceType {\n          tagType\n        }\n        ...resourceCard_ResourceDetails\n      }\n    }\n  }\n}\n\nfragment floorPlans_query on Query {\n  me {\n    id\n  }\n  deskResourceType\n  roomResourceType\n  parkingResourceType\n  ...customTagSelector_allCustomTags_query\n  ...zoneSelector_allZones_query\n  ...floorPlanSelector_allFloorPlans_query\n  ...organizationUserSelector_organizationMembers_query\n  ...bookingCard_query\n  ...resourceCard_query\n}\n\nfragment organizationUserSelector_organizationMembers_query on Query {\n  organizationMembers(where: {organizationId: $organizationId, nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n      }\n    }\n  }\n}\n\nfragment resourceCard_ResourceDetails on ResourceDetails {\n  id\n  name\n  inactive\n  color\n  capacity\n  customTags {\n    uniqueId\n    name\n    color\n  }\n  zones {\n    uniqueId\n    name\n    color\n  }\n  productTags {\n    uniqueId\n    name\n    color\n  }\n  resourceType {\n    uniqueId\n    name\n    color\n    tagType\n  }\n}\n\nfragment resourceCard_query on Query {\n  deskResourceType\n  roomResourceType\n  parkingResourceType\n}\n\nfragment zoneSelector_allZones_query on Query {\n  zones(where: {organizationId: $organizationId}, orderBy: $zonesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "78c9ca7cf0f55dd050bc0585edfc4500";

export default node;
