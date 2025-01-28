/**
 * @generated SignedSource<<fcb03f34ead2f8ff395d9ab32b4eb56d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationLocation_rooms_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  locationId: string;
  roomCustomTagIds?: ReadonlyArray<string> | null | undefined;
  roomNameSearchText?: string | null | undefined;
  roomZoneIds?: ReadonlyArray<string> | null | undefined;
};
export type organizationLocation_rooms_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationLocation_rooms_query">;
};
export type organizationLocation_rooms_refetchableFragment = {
  response: organizationLocation_rooms_refetchableFragment$data;
  variables: organizationLocation_rooms_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "count"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "cursor"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "roomCustomTagIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "roomNameSearchText"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "roomZoneIds"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "after",
    "variableName": "cursor"
  },
  {
    "kind": "Variable",
    "name": "first",
    "variableName": "count"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "customTagIds",
        "variableName": "roomCustomTagIds"
      },
      {
        "kind": "Variable",
        "name": "locationId",
        "variableName": "locationId"
      },
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
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v4 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v2/*: any*/),
  (v3/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocation_rooms_refetchableFragment",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "count",
            "variableName": "count"
          },
          {
            "kind": "Variable",
            "name": "cursor",
            "variableName": "cursor"
          }
        ],
        "kind": "FragmentSpread",
        "name": "organizationLocation_rooms_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_rooms_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "RoomConnection",
        "kind": "LinkedField",
        "name": "rooms",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "totalCount",
            "storageKey": null
          },
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
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "id",
                    "storageKey": null
                  },
                  (v2/*: any*/),
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
                  (v3/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "customTags",
                    "plural": true,
                    "selections": (v4/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "zones",
                    "plural": true,
                    "selections": (v4/*: any*/),
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
          {
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
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v1/*: any*/),
        "filters": [
          "where"
        ],
        "handle": "connection",
        "key": "organizationLocation_rooms",
        "kind": "LinkedHandle",
        "name": "rooms"
      }
    ]
  },
  "params": {
    "cacheID": "b83b190c33089191b4dc1e8d5c588c4c",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_rooms_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationLocation_rooms_refetchableFragment(\n  $count: Int = null\n  $cursor: String\n  $locationId: String!\n  $roomCustomTagIds: [String!]\n  $roomNameSearchText: String\n  $roomZoneIds: [String!]\n) {\n  ...organizationLocation_rooms_query_1G22uz\n}\n\nfragment organizationLocation_rooms_query_1G22uz on Query {\n  rooms(first: $count, after: $cursor, where: {locationId: $locationId, nameContains: $roomNameSearchText, customTagIds: $roomCustomTagIds, zoneIds: $roomZoneIds}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        deactivated\n        requireBookingApproval\n        color\n        customTags {\n          uniqueId\n          name\n          color\n        }\n        zones {\n          uniqueId\n          name\n          color\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b9378c26bdd92be329ead4f0e996c547";

export default node;
