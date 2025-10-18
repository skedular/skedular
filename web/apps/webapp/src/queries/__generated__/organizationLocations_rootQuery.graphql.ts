/**
 * @generated SignedSource<<a90ef629c23624fcc8f09041de0361f2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "ABOUT" | "NAME" | "TIMEZONE" | "TYPE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationMemberOrderField = "FAMILY_NAME" | "GIVEN_NAME" | "MIDDLE_NAME" | "NAME" | "PHONE_NUMBER" | "ROLE" | "STATUS" | "%future added value";
export type OrganizationTagOrderField = "DESCRIPTION" | "NAME" | "TYPE" | "%future added value";
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type organizationLocations_rootQuery$variables = {
  customTagIds?: ReadonlyArray<string> | null | undefined;
  customTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  fromTodayDate: any;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  organizationUniqueAlphanumericName: string;
  untilTodayDate: any;
  zoneIds?: ReadonlyArray<string> | null | undefined;
  zonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
};
export type organizationLocations_rootQuery$data = {
  readonly me: {
    readonly id: string;
    readonly preferredLocations: ReadonlyArray<{
      readonly id: string;
    }>;
  };
  readonly organization: {
    readonly canModify: boolean;
    readonly members: {
      readonly __id: string;
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly customer: {
            readonly familyName: string | null | undefined;
            readonly givenName: string | null | undefined;
            readonly id: string;
            readonly middleName: string | null | undefined;
            readonly name: string | null | undefined;
            readonly photoUrl: string | null | undefined;
          };
          readonly id: string;
        };
      }>;
      readonly totalCount: number;
    };
    readonly uniqueAlphanumericName: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"customTagSelector_allCustomTags_query" | "locationCard_query" | "newLocationButton_query" | "organizationLocations_locations_availableOrganizationResources_query" | "zoneSelector_allZones_query">;
};
export type organizationLocations_rootQuery = {
  response: organizationLocations_rootQuery$data;
  variables: organizationLocations_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagIds"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagsSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "fromTodayDate"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "untilTodayDate"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneIds"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zonesSortingValues"
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v10 = [
  (v9/*: any*/)
],
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v9/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "LocationDetails",
      "kind": "LinkedField",
      "name": "preferredLocations",
      "plural": true,
      "selections": (v10/*: any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v12 = [
  {
    "kind": "Variable",
    "name": "uniqueAlphanumericName",
    "variableName": "organizationUniqueAlphanumericName"
  }
],
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v15 = {
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
v16 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "orderBy",
      "variableName": "organizationMembersSortingValues"
    }
  ],
  "concreteType": "ConnectionOfOrganizationMemberEdge",
  "kind": "LinkedField",
  "name": "members",
  "plural": false,
  "selections": [
    (v13/*: any*/),
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
              "concreteType": "CustomerDetails",
              "kind": "LinkedField",
              "name": "customer",
              "plural": false,
              "selections": [
                (v9/*: any*/),
                (v14/*: any*/),
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
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    (v15/*: any*/)
  ],
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueAlphanumericName",
  "storageKey": null
},
v19 = [
  (v9/*: any*/),
  (v14/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
],
v20 = [
  (v13/*: any*/),
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
        "selections": (v19/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  (v15/*: any*/)
],
v21 = {
  "kind": "Variable",
  "name": "customTagIds",
  "variableName": "customTagIds"
},
v22 = {
  "kind": "Variable",
  "name": "organizationUniqueAlphanumericName",
  "variableName": "organizationUniqueAlphanumericName"
},
v23 = {
  "kind": "Variable",
  "name": "zoneIds",
  "variableName": "zoneIds"
},
v24 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationsSortingValues"
  },
  {
    "fields": [
      (v21/*: any*/),
      (v22/*: any*/),
      (v23/*: any*/)
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
    "name": "organizationLocations_rootQuery",
    "selections": [
      (v11/*: any*/),
      {
        "alias": null,
        "args": (v12/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v16/*: any*/),
          (v17/*: any*/),
          (v18/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newLocationButton_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationCard_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "customTagSelector_allCustomTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "zoneSelector_allZones_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationLocations_locations_availableOrganizationResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v5/*: any*/),
      (v3/*: any*/),
      (v8/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v6/*: any*/),
      (v4/*: any*/),
      (v7/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "organizationLocations_rootQuery",
    "selections": [
      (v11/*: any*/),
      {
        "alias": null,
        "args": (v12/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v16/*: any*/),
          (v17/*: any*/),
          (v18/*: any*/),
          (v9/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTypeDetails",
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
                "variableName": "customTagsSortingValues"
              }
            ],
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "customTags",
            "plural": false,
            "selections": (v20/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "orderBy",
                "variableName": "zonesSortingValues"
              }
            ],
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "zones",
            "plural": false,
            "selections": (v20/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v24/*: any*/),
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v13/*: any*/),
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
                  (v9/*: any*/),
                  (v14/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "customTags",
                    "plural": true,
                    "selections": (v19/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "zones",
                    "plural": true,
                    "selections": (v19/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ConnectionOfResourceEdge",
                    "kind": "LinkedField",
                    "name": "resources",
                    "plural": false,
                    "selections": [
                      (v13/*: any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationPhysicalAddressDetails",
                    "kind": "LinkedField",
                    "name": "physicalAddress",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "formattedAddress",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "longitude",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "latitude",
                        "storageKey": null
                      },
                      (v9/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "multilinesFormattedAddress",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "hasFutureBooking",
                    "storageKey": null
                  },
                  (v17/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "canDelete",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": [
                      (v18/*: any*/),
                      (v9/*: any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationExtraMetadata",
                    "kind": "LinkedField",
                    "name": "extraMetadata",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ContactDetails",
                        "kind": "LinkedField",
                        "name": "contactDetails",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "contactPhones",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "contactPeople",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "contactEmails",
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
                    "concreteType": "CdnImageFile",
                    "kind": "LinkedField",
                    "name": "primaryFeatureImage",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CdnFile",
                        "kind": "LinkedField",
                        "name": "thumbnail",
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
                    "kind": "ScalarField",
                    "name": "uniqueClaimCode",
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
          (v15/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v24/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationLocations_locations",
        "kind": "LinkedHandle",
        "name": "locations"
      },
      {
        "alias": null,
        "args": [
          {
            "fields": [
              (v21/*: any*/),
              {
                "kind": "Variable",
                "name": "from",
                "variableName": "fromTodayDate"
              },
              (v22/*: any*/),
              {
                "kind": "Variable",
                "name": "until",
                "variableName": "untilTodayDate"
              },
              (v23/*: any*/)
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
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": (v10/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "21d7adb0131af443ccc2650931c2e881",
    "id": null,
    "metadata": {},
    "name": "organizationLocations_rootQuery",
    "operationKind": "query",
    "text": "query organizationLocations_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $locationsSortingValues: [LocationOrderInput!]\n  $zonesSortingValues: [OrganizationTagOrderInput!]\n  $customTagsSortingValues: [OrganizationTagOrderInput!]\n  $fromTodayDate: DateTime!\n  $untilTodayDate: DateTime!\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $zoneIds: [String!]\n  $customTagIds: [String!]\n) {\n  me {\n    id\n    preferredLocations {\n      id\n    }\n  }\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    members(orderBy: $organizationMembersSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          customer {\n            id\n            name\n            givenName\n            middleName\n            familyName\n            photoUrl\n          }\n        }\n      }\n    }\n    canModify\n    uniqueAlphanumericName\n    id\n  }\n  ...newLocationButton_query\n  ...locationCard_query\n  ...customTagSelector_allCustomTags_query\n  ...zoneSelector_allZones_query\n  ...organizationLocations_locations_availableOrganizationResources_query\n}\n\nfragment customTagSelector_allCustomTags_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    customTags(orderBy: $customTagsSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n        }\n      }\n    }\n    id\n  }\n}\n\nfragment locationCard_LocationDetails on LocationDetails {\n  id\n  name\n  customTags {\n    id\n    name\n    color\n  }\n  zones {\n    id\n    name\n    color\n  }\n  resources {\n    totalCount\n  }\n  physicalAddress {\n    multilinesFormattedAddress\n    latitude\n    longitude\n    id\n  }\n  primaryFeatureImage {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n  hasFutureBooking\n  canModify\n  canDelete\n  organization {\n    uniqueAlphanumericName\n    id\n  }\n  extraMetadata {\n    contactDetails {\n      contactPeople\n      contactEmails\n      contactPhones\n    }\n  }\n  uniqueClaimCode\n}\n\nfragment locationCard_query on Query {\n  me {\n    id\n    preferredLocations {\n      id\n    }\n  }\n}\n\nfragment newLocationButton_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    type {\n      type\n    }\n    id\n  }\n}\n\nfragment organizationLocations_locations_availableOrganizationResources_query on Query {\n  locations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, zoneIds: $zoneIds, customTagIds: $customTagIds}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        customTags {\n          id\n          name\n          color\n        }\n        zones {\n          id\n          name\n          color\n        }\n        resources {\n          totalCount\n        }\n        physicalAddress {\n          formattedAddress\n          longitude\n          latitude\n          id\n        }\n        hasFutureBooking\n        canModify\n        canDelete\n        organization {\n          uniqueAlphanumericName\n          id\n        }\n        extraMetadata {\n          contactDetails {\n            contactPhones\n          }\n        }\n        ...locationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  availableResources(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, from: $fromTodayDate, until: $untilTodayDate, zoneIds: $zoneIds, customTagIds: $customTagIds}) {\n    location {\n      id\n    }\n  }\n}\n\nfragment zoneSelector_allZones_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    zones(orderBy: $zonesSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n        }\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "be31e58a6a1508b2d286644017c9f467";

export default node;
