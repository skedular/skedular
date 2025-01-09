/**
 * @generated SignedSource<<ee9c6a87cf9cb74d4f46319fd0c5f2a8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationManageAssets_rootQuery$variables = {
  deskTypeNameSearchText?: string | null | undefined;
  organizationId: string;
  zoneNameSearchText?: string | null | undefined;
};
export type organizationManageAssets_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationManageAssets_deskTypes_query" | "organizationManageAssets_zones_query">;
};
export type organizationManageAssets_rootQuery = {
  response: organizationManageAssets_rootQuery$data;
  variables: organizationManageAssets_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskTypeNameSearchText"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zoneNameSearchText"
},
v3 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v4 = [
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "zoneNameSearchText"
      },
      (v3/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v5 = [
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
v6 = [
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "Ascending",
        "field": "Name"
      }
    ]
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "deskTypeNameSearchText"
      },
      (v3/*: any*/)
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
      (v2/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationManageAssets_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationManageAssets_zones_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationManageAssets_deskTypes_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v2/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "organizationManageAssets_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v5/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v4/*: any*/),
        "filters": [
          "where"
        ],
        "handle": "connection",
        "key": "organizationManageAssets_zones",
        "kind": "LinkedHandle",
        "name": "zones"
      },
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "deskTypes",
        "plural": false,
        "selections": (v5/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v6/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationManageAssets_deskTypes",
        "kind": "LinkedHandle",
        "name": "deskTypes"
      }
    ]
  },
  "params": {
    "cacheID": "cc855eef57cb3aa58e1a00f658288849",
    "id": null,
    "metadata": {},
    "name": "organizationManageAssets_rootQuery",
    "operationKind": "query",
    "text": "query organizationManageAssets_rootQuery(\n  $organizationId: String!\n  $zoneNameSearchText: String\n  $deskTypeNameSearchText: String\n) {\n  ...organizationManageAssets_zones_query\n  ...organizationManageAssets_deskTypes_query\n}\n\nfragment organizationManageAssets_deskTypes_query on Query {\n  deskTypes(where: {organizationId: $organizationId, nameContains: $deskTypeNameSearchText}, orderBy: [{direction: Ascending, field: Name}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationManageAssets_zones_query on Query {\n  zones(where: {organizationId: $organizationId, nameContains: $zoneNameSearchText}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "23f5fb0d0e0e3c45777172949418dfe2";

export default node;
