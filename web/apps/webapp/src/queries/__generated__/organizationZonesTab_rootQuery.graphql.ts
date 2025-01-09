/**
 * @generated SignedSource<<076eef357054e7f39b754d0e1aea5c17>>
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
export type organizationZonesTab_rootQuery$variables = {
  organizationId: string;
  zoneNameSearchText?: string | null | undefined;
  zoneSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
};
export type organizationZonesTab_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationZonesTab_query" | "organizationZonesTab_zones_query">;
};
export type organizationZonesTab_rootQuery = {
  response: organizationZonesTab_rootQuery$data;
  variables: organizationZonesTab_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
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
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
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
    "name": "organizationZonesTab_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationZonesTab_query"
      },
      {
        "args": null,
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
    "name": "organizationZonesTab_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "organizationId"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canModify",
            "storageKey": null
          },
          (v1/*: any*/)
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerOrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredZones",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
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
        "args": (v2/*: any*/),
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
                  (v1/*: any*/),
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
        "args": (v2/*: any*/),
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
    "cacheID": "05d1c68c6335fd689782f4c96220953f",
    "id": null,
    "metadata": {},
    "name": "organizationZonesTab_rootQuery",
    "operationKind": "query",
    "text": "query organizationZonesTab_rootQuery(\n  $organizationId: String!\n  $zoneNameSearchText: String\n  $zoneSortingValues: [OrganizationTagOrderInput!]\n) {\n  ...organizationZonesTab_query\n  ...organizationZonesTab_zones_query\n}\n\nfragment organizationZonesTab_query on Query {\n  organization(id: $organizationId) {\n    canModify\n    id\n  }\n  ...zoneCard_Query\n}\n\nfragment organizationZonesTab_zones_query on Query {\n  zones(first: 50, where: {organizationId: $organizationId, nameContains: $zoneNameSearchText}, orderBy: $zoneSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...zoneCard_OrganizationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment zoneCard_OrganizationTagDetails on OrganizationTagDetails {\n  id\n  name\n}\n\nfragment zoneCard_Query on Query {\n  me {\n    id\n    preferredZones {\n      uniqueId\n    }\n  }\n  organization(id: $organizationId) {\n    canModify\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "ea3bee83a5b086e617821b7d83507b8a";

export default node;
