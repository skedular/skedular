/**
 * @generated SignedSource<<d6ba73fc4c5ef04bb7c1b34c72e738a2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationLocation_desks_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  deskDeskTypeIds?: ReadonlyArray<string> | null | undefined;
  deskNameSearchText?: string | null | undefined;
  deskZoneIds?: ReadonlyArray<string> | null | undefined;
  locationId: string;
};
export type organizationLocation_desks_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationLocation_desks_query">;
};
export type organizationLocation_desks_refetchableFragment = {
  response: organizationLocation_desks_refetchableFragment$data;
  variables: organizationLocation_desks_refetchableFragment$variables;
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
    "name": "deskDeskTypeIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "deskNameSearchText"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "deskZoneIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
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
        "name": "deskTypeIds",
        "variableName": "deskDeskTypeIds"
      },
      {
        "kind": "Variable",
        "name": "locationId",
        "variableName": "locationId"
      },
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
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v2/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocation_desks_refetchableFragment",
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
        "name": "organizationLocation_desks_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocation_desks_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "DeskConnection",
        "kind": "LinkedField",
        "name": "locationDesks",
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
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "deskTypes",
                    "plural": true,
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "zones",
                    "plural": true,
                    "selections": (v3/*: any*/),
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
        "key": "organizationLocation_locationDesks",
        "kind": "LinkedHandle",
        "name": "locationDesks"
      }
    ]
  },
  "params": {
    "cacheID": "9057b5543be72fc920a04ba3f331bce6",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_desks_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationLocation_desks_refetchableFragment(\n  $count: Int = null\n  $cursor: String\n  $deskDeskTypeIds: [String!]\n  $deskNameSearchText: String\n  $deskZoneIds: [String!]\n  $locationId: String!\n) {\n  ...organizationLocation_desks_query_1G22uz\n}\n\nfragment organizationLocation_desks_query_1G22uz on Query {\n  locationDesks(first: $count, after: $cursor, where: {locationId: $locationId, nameContains: $deskNameSearchText, deskTypeIds: $deskDeskTypeIds, zoneIds: $deskZoneIds}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        deactivated\n        requireBookingApproval\n        deskTypes {\n          uniqueId\n          name\n        }\n        zones {\n          uniqueId\n          name\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7ac03f4d8f99525723afaf039969346c";

export default node;
