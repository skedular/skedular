/**
 * @generated SignedSource<<f2d65a1690960ce08f116f6a19762e3c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationAdmin_rootQuery$variables = {
  deskTypeNameSearchText?: string | null | undefined;
  organizationId: string;
  zoneNameSearchText?: string | null | undefined;
};
export type pageOrganizationAdmin_rootQuery$data = {
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAdmin_deskTypes_query" | "organizationAdmin_query" | "organizationAdmin_zones_query">;
};
export type pageOrganizationAdmin_rootQuery = {
  response: pageOrganizationAdmin_rootQuery$data;
  variables: pageOrganizationAdmin_rootQuery$variables;
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
v3 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationId"
  }
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v6 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v7 = [
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "zoneNameSearchText"
      },
      (v6/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v8 = [
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
          (v5/*: any*/),
          (v4/*: any*/),
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
v9 = [
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
      (v6/*: any*/)
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
    "name": "pageOrganizationAdmin_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v4/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAdmin_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAdmin_zones_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAdmin_deskTypes_query"
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
    "name": "pageOrganizationAdmin_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v4/*: any*/),
          (v5/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "about",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v7/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v8/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v7/*: any*/),
        "filters": [
          "where"
        ],
        "handle": "connection",
        "key": "organizationAdmin_zones",
        "kind": "LinkedHandle",
        "name": "zones"
      },
      {
        "alias": null,
        "args": (v9/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "deskTypes",
        "plural": false,
        "selections": (v8/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v9/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationAdmin_deskTypes",
        "kind": "LinkedHandle",
        "name": "deskTypes"
      }
    ]
  },
  "params": {
    "cacheID": "43f21b7e3113acbcd2af18125647953f",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationAdmin_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationAdmin_rootQuery(\n  $organizationId: String!\n  $zoneNameSearchText: String\n  $deskTypeNameSearchText: String\n) {\n  organization(id: $organizationId) {\n    name\n    id\n  }\n  ...organizationAdmin_query\n  ...organizationAdmin_zones_query\n  ...organizationAdmin_deskTypes_query\n}\n\nfragment organizationAdmin_deskTypes_query on Query {\n  deskTypes(where: {organizationId: $organizationId, nameContains: $deskTypeNameSearchText}, orderBy: [{direction: Ascending, field: Name}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationAdmin_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    about\n  }\n}\n\nfragment organizationAdmin_zones_query on Query {\n  zones(where: {organizationId: $organizationId, nameContains: $zoneNameSearchText}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f572cb0b3289c7c0556f56184729b31a";

export default node;
