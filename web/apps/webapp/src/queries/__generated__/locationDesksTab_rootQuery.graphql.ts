/**
 * @generated SignedSource<<58b0e484ac9c49ec1c67f272e6fc6ac1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type DeskOrderField = "Name" | "%future added value";
export type LocationTagOrderField = "Description" | "Name" | "TagType" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationTagOrderField = "Description" | "Name" | "TagType" | "%future added value";
export type DeskOrderInput = {
  direction: OrderDirection;
  field: DeskOrderField;
};
export type LocationTagOrderInput = {
  direction: OrderDirection;
  field: LocationTagOrderField;
};
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type locationDesksTab_rootQuery$variables = {
  deskMultipleChoicesDeskTypesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  deskMultipleChoicesZonesSortingValues?: ReadonlyArray<LocationTagOrderInput> | null | undefined;
  deskNameSearchText?: string | null | undefined;
  deskSortingValues: ReadonlyArray<DeskOrderInput>;
  deskTypeTagType: string;
  fromToGetBookings?: any | null | undefined;
  locationId: string;
  organizationExists: boolean;
  organizationId: string;
  toToGetBookings?: any | null | undefined;
  zoneTagType: string;
};
export type locationDesksTab_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationDesksTab_allBookings_query" | "locationDesksTab_locationDesks_query" | "locationDesksTab_query">;
};
export type locationDesksTab_rootQuery = {
  response: locationDesksTab_rootQuery$data;
  variables: locationDesksTab_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskMultipleChoicesDeskTypesSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskMultipleChoicesZonesSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskNameSearchText"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskTypeTagType"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "fromToGetBookings"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "toToGetBookings"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneTagType"
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
  "name": "uniqueId",
  "storageKey": null
},
v13 = [
  (v12/*: any*/)
],
v14 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v17 = [
  (v11/*: any*/),
  (v16/*: any*/)
],
v18 = {
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
v19 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "deskSortingValues"
  },
  {
    "fields": [
      (v14/*: any*/),
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "deskNameSearchText"
      }
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
    "name": "locationDesksTab_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationDesksTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationDesksTab_locationDesks_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationDesksTab_allBookings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v8/*: any*/),
      (v6/*: any*/),
      (v7/*: any*/),
      (v10/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      (v9/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "locationDesksTab_rootQuery",
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
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canModify",
            "storageKey": null
          },
          (v11/*: any*/)
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
          (v11/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v13/*: any*/),
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
            "variableName": "deskMultipleChoicesZonesSortingValues"
          },
          {
            "fields": [
              (v14/*: any*/),
              {
                "kind": "Variable",
                "name": "tagType",
                "variableName": "zoneTagType"
              }
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "LocationTagConnection",
        "kind": "LinkedField",
        "name": "locationTags",
        "plural": false,
        "selections": [
          (v15/*: any*/),
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
                "selections": (v17/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v18/*: any*/)
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
                "name": "orderBy",
                "variableName": "deskMultipleChoicesDeskTypesSortingValues"
              },
              {
                "fields": [
                  {
                    "kind": "Variable",
                    "name": "organizationId",
                    "variableName": "organizationId"
                  },
                  {
                    "kind": "Variable",
                    "name": "tagType",
                    "variableName": "deskTypeTagType"
                  }
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "OrganizationTagConnection",
            "kind": "LinkedField",
            "name": "organizationTags",
            "plural": false,
            "selections": [
              (v15/*: any*/),
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
                    "selections": (v17/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v18/*: any*/)
            ],
            "storageKey": null
          }
        ]
      },
      {
        "alias": null,
        "args": (v19/*: any*/),
        "concreteType": "DeskConnection",
        "kind": "LinkedField",
        "name": "locationDesks",
        "plural": false,
        "selections": [
          (v15/*: any*/),
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
                  (v11/*: any*/),
                  (v16/*: any*/),
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
                    "selections": (v17/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "organizationTags",
                    "plural": true,
                    "selections": [
                      (v12/*: any*/),
                      (v16/*: any*/)
                    ],
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
          (v18/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "locationDesksTab_locationDesks",
        "kind": "LinkedHandle",
        "name": "locationDesks"
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
          (v11/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": [
              (v12/*: any*/),
              (v16/*: any*/),
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
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDeskDetails",
            "kind": "LinkedField",
            "name": "desks",
            "plural": true,
            "selections": (v13/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "0d8272bc03edcc9c18a9511dd8aa0684",
    "id": null,
    "metadata": {},
    "name": "locationDesksTab_rootQuery",
    "operationKind": "query",
    "text": "query locationDesksTab_rootQuery(\n  $organizationId: String!\n  $locationId: String!\n  $organizationExists: Boolean!\n  $zoneTagType: String!\n  $deskTypeTagType: String!\n  $fromToGetBookings: DateTime\n  $toToGetBookings: DateTime\n  $deskNameSearchText: String\n  $deskSortingValues: [DeskOrderInput!]!\n  $deskMultipleChoicesZonesSortingValues: [LocationTagOrderInput!]\n  $deskMultipleChoicesDeskTypesSortingValues: [OrganizationTagOrderInput!]\n) {\n  ...locationDesksTab_query\n  ...locationDesksTab_locationDesks_query\n  ...locationDesksTab_allBookings_query\n}\n\nfragment bulkNewDeskDialog_query on Query {\n  ...deskMultipleChoicesZones_query\n  ...deskMultipleChoicesDeskTypes_query\n}\n\nfragment deskCard_DeskDetails on DeskDetails {\n  id\n  name\n  deactivated\n  requireBookingApproval\n  locationTags {\n    id\n    name\n  }\n  organizationTags {\n    uniqueId\n    name\n  }\n}\n\nfragment deskCard_query on Query {\n  me {\n    id\n    preferredDesks {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    canModify\n    id\n  }\n}\n\nfragment deskMultipleChoicesDeskTypes_query on Query {\n  organizationTags(where: {organizationId: $organizationId, tagType: $deskTypeTagType}, orderBy: $deskMultipleChoicesDeskTypesSortingValues) @include(if: $organizationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment deskMultipleChoicesZones_query on Query {\n  locationTags(where: {locationId: $locationId, tagType: $zoneTagType}, orderBy: $deskMultipleChoicesZonesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment locationDesksTab_allBookings_query on Query {\n  allBookings(where: {locationIds: [$locationId], fromGTE: $fromToGetBookings, toLTE: $toToGetBookings}) {\n    id\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    desks {\n      uniqueId\n    }\n  }\n}\n\nfragment locationDesksTab_locationDesks_query on Query {\n  locationDesks(first: 50, where: {locationId: $locationId, nameContains: $deskNameSearchText}, orderBy: $deskSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...deskCard_DeskDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationDesksTab_query on Query {\n  location(id: $locationId) {\n    canModify\n    id\n  }\n  ...deskCard_query\n  ...deskMultipleChoicesZones_query\n  ...deskMultipleChoicesDeskTypes_query\n  ...newDeskDialog_query\n  ...bulkNewDeskDialog_query\n}\n\nfragment newDeskDialog_query on Query {\n  ...deskMultipleChoicesZones_query\n  ...deskMultipleChoicesDeskTypes_query\n}\n"
  }
};
})();

(node as any).hash = "a80cd5f443d24efecc2c14b4e36d21db";

export default node;
