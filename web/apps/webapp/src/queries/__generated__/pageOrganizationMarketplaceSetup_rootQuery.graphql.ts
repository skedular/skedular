/**
 * @generated SignedSource<<040b1cc856ed4ca4e22d19675eb7b5b6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationMarketplaceSetup_rootQuery$variables = {
  locationTagNameSearchText?: string | null | undefined;
  organizationId: string;
  productTagNameSearchText?: string | null | undefined;
};
export type pageOrganizationMarketplaceSetup_rootQuery$data = {
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_locationTags_query" | "organizationMarketplaceSetup_productTags_query">;
};
export type pageOrganizationMarketplaceSetup_rootQuery = {
  response: pageOrganizationMarketplaceSetup_rootQuery$data;
  variables: pageOrganizationMarketplaceSetup_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationTagNameSearchText"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productTagNameSearchText"
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
  "kind": "Literal",
  "name": "orderBy",
  "value": [
    {
      "direction": "Ascending",
      "field": "Name"
    }
  ]
},
v7 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v8 = [
  (v6/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "productTagNameSearchText"
      },
      (v7/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v9 = [
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
v10 = [
  "where",
  "orderBy"
],
v11 = [
  (v6/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "locationTagNameSearchText"
      },
      (v7/*: any*/)
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
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
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
        "name": "organizationMarketplaceSetup_productTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMarketplaceSetup_locationTags_query"
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
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
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
          (v5/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v8/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "productTags",
        "plural": false,
        "selections": (v9/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v8/*: any*/),
        "filters": (v10/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_productTags",
        "kind": "LinkedHandle",
        "name": "productTags"
      },
      {
        "alias": null,
        "args": (v11/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "locationTags",
        "plural": false,
        "selections": (v9/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v11/*: any*/),
        "filters": (v10/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_locationTags",
        "kind": "LinkedHandle",
        "name": "locationTags"
      }
    ]
  },
  "params": {
    "cacheID": "fa5e75b54af7aca100a02c7e644b4849",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationMarketplaceSetup_rootQuery(\n  $organizationId: String!\n  $productTagNameSearchText: String\n  $locationTagNameSearchText: String\n) {\n  organization(id: $organizationId) {\n    name\n    id\n  }\n  ...organizationMarketplaceSetup_productTags_query\n  ...organizationMarketplaceSetup_locationTags_query\n}\n\nfragment organizationMarketplaceSetup_locationTags_query on Query {\n  locationTags(where: {organizationId: $organizationId, nameContains: $locationTagNameSearchText}, orderBy: [{direction: Ascending, field: Name}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_productTags_query on Query {\n  productTags(where: {organizationId: $organizationId, nameContains: $productTagNameSearchText}, orderBy: [{direction: Ascending, field: Name}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5e67d4967318767b24ec8a1179cfca7d";

export default node;
