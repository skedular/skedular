/**
 * @generated SignedSource<<d8ea1079191a4a6e419e0c03034932ea>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationTagOrderField = "Description" | "Name" | "TagType" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type pageOrganizationLocation_rootQuery$variables = {
  customTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  deskCustomTagIds?: ReadonlyArray<string> | null | undefined;
  deskNameSearchText?: string | null | undefined;
  deskZoneIds?: ReadonlyArray<string> | null | undefined;
  locationId: string;
  organizationId: string;
  roomCustomTagIds?: ReadonlyArray<string> | null | undefined;
  roomNameSearchText?: string | null | undefined;
  roomZoneIds?: ReadonlyArray<string> | null | undefined;
  zonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
};
export type pageOrganizationLocation_rootQuery$data = {
  readonly location: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationLocation_desks_query" | "organizationLocation_query" | "organizationLocation_rooms_query">;
};
export type pageOrganizationLocation_rootQuery = {
  response: pageOrganizationLocation_rootQuery$data;
  variables: pageOrganizationLocation_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskCustomTagIds"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskNameSearchText"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskZoneIds"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "roomCustomTagIds"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "roomNameSearchText"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "roomZoneIds"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zonesSortingValues"
},
v10 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "locationId"
  }
],
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v14 = [
  (v13/*: any*/)
],
v15 = {
  "fields": [
    {
      "kind": "Variable",
      "name": "organizationId",
      "variableName": "organizationId"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
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
  (v16/*: any*/),
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
          (v12/*: any*/),
          (v11/*: any*/),
          (v17/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  (v18/*: any*/)
],
v20 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v21 = [
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "customTagIds",
        "variableName": "deskCustomTagIds"
      },
      (v20/*: any*/),
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "deskNameSearchText"
      },
      {
        "kind": "Variable",
        "name": "zoneIds",
        "variableName": "deskZoneIds"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v22 = [
  (v13/*: any*/),
  (v11/*: any*/),
  (v17/*: any*/)
],
v23 = [
  (v12/*: any*/),
  (v11/*: any*/),
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
  (v17/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "Organization_OrganizationTagDetails",
    "kind": "LinkedField",
    "name": "customTags",
    "plural": true,
    "selections": (v22/*: any*/),
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "Organization_OrganizationTagDetails",
    "kind": "LinkedField",
    "name": "zones",
    "plural": true,
    "selections": (v22/*: any*/),
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
v26 = [
  "where"
],
v27 = [
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "customTagIds",
        "variableName": "roomCustomTagIds"
      },
      (v20/*: any*/),
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "roomNameSearchText"
      },
      {
        "kind": "Variable",
        "name": "zoneIds",
        "variableName": "roomZoneIds"
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
      (v9/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationLocation_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v10/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v11/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationLocation_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationLocation_desks_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationLocation_rooms_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v5/*: any*/),
      (v4/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v1/*: any*/),
      (v9/*: any*/),
      (v0/*: any*/),
      (v7/*: any*/),
      (v8/*: any*/),
      (v6/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationLocation_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v10/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v11/*: any*/),
          (v12/*: any*/),
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
            "concreteType": "LocationAddressDetails",
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
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v12/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDeskDetails",
            "kind": "LinkedField",
            "name": "preferredDesks",
            "plural": true,
            "selections": (v14/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerRoomDetails",
            "kind": "LinkedField",
            "name": "preferredRooms",
            "plural": true,
            "selections": (v14/*: any*/),
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
          },
          (v15/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "customTags",
        "plural": false,
        "selections": (v19/*: any*/),
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
          (v15/*: any*/)
        ],
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v19/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v21/*: any*/),
        "concreteType": "DeskConnection",
        "kind": "LinkedField",
        "name": "desks",
        "plural": false,
        "selections": [
          (v16/*: any*/),
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
                "selections": (v23/*: any*/),
                "storageKey": null
              },
              (v24/*: any*/)
            ],
            "storageKey": null
          },
          (v25/*: any*/),
          (v18/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v21/*: any*/),
        "filters": (v26/*: any*/),
        "handle": "connection",
        "key": "organizationLocation_desks",
        "kind": "LinkedHandle",
        "name": "desks"
      },
      {
        "alias": null,
        "args": (v27/*: any*/),
        "concreteType": "RoomConnection",
        "kind": "LinkedField",
        "name": "rooms",
        "plural": false,
        "selections": [
          (v16/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "RoomEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "RoomDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": (v23/*: any*/),
                "storageKey": null
              },
              (v24/*: any*/)
            ],
            "storageKey": null
          },
          (v25/*: any*/),
          (v18/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v27/*: any*/),
        "filters": (v26/*: any*/),
        "handle": "connection",
        "key": "organizationLocation_rooms",
        "kind": "LinkedHandle",
        "name": "rooms"
      }
    ]
  },
  "params": {
    "cacheID": "82a2c804729044694b2aab531c879c22",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationLocation_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationLocation_rootQuery(\n  $organizationId: String!\n  $locationId: String!\n  $deskNameSearchText: String\n  $deskZoneIds: [String!]\n  $deskCustomTagIds: [String!]\n  $zonesSortingValues: [OrganizationTagOrderInput!]\n  $customTagsSortingValues: [OrganizationTagOrderInput!]\n  $roomNameSearchText: String\n  $roomZoneIds: [String!]\n  $roomCustomTagIds: [String!]\n) {\n  location(id: $locationId) {\n    name\n    id\n  }\n  ...organizationLocation_query\n  ...organizationLocation_desks_query\n  ...organizationLocation_rooms_query\n}\n\nfragment customTagSelector_allCustomTags_query on Query {\n  customTags(where: {organizationId: $organizationId}, orderBy: $customTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n\nfragment organizationLocation_desks_query on Query {\n  desks(where: {locationId: $locationId, nameContains: $deskNameSearchText, customTagIds: $deskCustomTagIds, zoneIds: $deskZoneIds}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        deactivated\n        requireBookingApproval\n        color\n        customTags {\n          uniqueId\n          name\n          color\n        }\n        zones {\n          uniqueId\n          name\n          color\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationLocation_query on Query {\n  me {\n    id\n    preferredDesks {\n      uniqueId\n    }\n    preferredRooms {\n      uniqueId\n    }\n  }\n  location(id: $locationId) {\n    id\n    name\n    about\n    timezone\n    physicalAddress {\n      formattedAddress\n    }\n  }\n  ...customTagSelector_allCustomTags_query\n  ...zoneSelector_allZones_query\n}\n\nfragment organizationLocation_rooms_query on Query {\n  rooms(where: {locationId: $locationId, nameContains: $roomNameSearchText, customTagIds: $roomCustomTagIds, zoneIds: $roomZoneIds}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        deactivated\n        requireBookingApproval\n        color\n        customTags {\n          uniqueId\n          name\n          color\n        }\n        zones {\n          uniqueId\n          name\n          color\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment zoneSelector_allZones_query on Query {\n  zones(where: {organizationId: $organizationId}, orderBy: $zonesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2c707293e6dc97058aad2607be14e47a";

export default node;
