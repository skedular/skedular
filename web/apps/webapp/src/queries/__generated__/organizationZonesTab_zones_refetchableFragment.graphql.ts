/**
 * @generated SignedSource<<fb2fa2f48c4c637c9f79eec27ec2be28>>
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
export type organizationZonesTab_zones_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  organizationId: string;
  zoneNameSearchText?: string | null | undefined;
  zoneSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
};
export type organizationZonesTab_zones_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationZonesTab_zones_query">;
};
export type organizationZonesTab_zones_refetchableFragment = {
  response: organizationZonesTab_zones_refetchableFragment$data;
  variables: organizationZonesTab_zones_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": 50,
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
    "name": "organizationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "zoneNameSearchText"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "zoneSortingValues"
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
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "zoneSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "zoneNameSearchText"
      },
      {
        "kind": "Variable",
        "name": "organizationId",
        "variableName": "organizationId"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationZonesTab_zones_refetchableFragment",
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
        "name": "organizationZonesTab_zones_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationZonesTab_zones_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "zones",
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
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "id",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "name",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "description",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "color",
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
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationZonesTab_zones",
        "kind": "LinkedHandle",
        "name": "zones"
      }
    ]
  },
  "params": {
    "cacheID": "412b833e28052ce210302e93725a7163",
    "id": null,
    "metadata": {},
    "name": "organizationZonesTab_zones_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationZonesTab_zones_refetchableFragment(\n  $count: Int = 50\n  $cursor: String\n  $organizationId: String!\n  $zoneNameSearchText: String\n  $zoneSortingValues: [OrganizationTagOrderInput!]\n) {\n  ...organizationZonesTab_zones_query_1G22uz\n}\n\nfragment organizationZonesTab_zones_query_1G22uz on Query {\n  zones(first: $count, after: $cursor, where: {organizationId: $organizationId, nameContains: $zoneNameSearchText}, orderBy: $zoneSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...zoneCard_OrganizationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment zoneCard_OrganizationTagDetails on OrganizationTagDetails {\n  id\n  name\n  description\n  color\n}\n"
  }
};
})();

(node as any).hash = "8c32a8b4de7aafdb21189d35d1435b3e";

export default node;
