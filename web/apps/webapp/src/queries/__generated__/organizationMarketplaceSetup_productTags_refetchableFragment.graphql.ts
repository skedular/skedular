/**
 * @generated SignedSource<<5c9e0a45de4ebcc6e0deb05456a84295>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMarketplaceSetup_productTags_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  organizationId: string;
  productTagNameSearchText?: string | null | undefined;
};
export type organizationMarketplaceSetup_productTags_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_productTags_query">;
};
export type organizationMarketplaceSetup_productTags_refetchableFragment = {
  response: organizationMarketplaceSetup_productTags_refetchableFragment$data;
  variables: organizationMarketplaceSetup_productTags_refetchableFragment$variables;
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
    "name": "organizationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "productTagNameSearchText"
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
        "variableName": "productTagNameSearchText"
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
    "name": "organizationMarketplaceSetup_productTags_refetchableFragment",
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
        "name": "organizationMarketplaceSetup_productTags_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_productTags_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "productTags",
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
        "key": "organizationMarketplaceSetup_productTags",
        "kind": "LinkedHandle",
        "name": "productTags"
      }
    ]
  },
  "params": {
    "cacheID": "356d97b6ce14cb2929934ab84558b38d",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_productTags_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationMarketplaceSetup_productTags_refetchableFragment(\n  $count: Int = null\n  $cursor: String\n  $organizationId: String!\n  $productTagNameSearchText: String\n) {\n  ...organizationMarketplaceSetup_productTags_query_1G22uz\n}\n\nfragment organizationMarketplaceSetup_productTags_query_1G22uz on Query {\n  productTags(first: $count, after: $cursor, where: {organizationId: $organizationId, nameContains: $productTagNameSearchText}, orderBy: [{direction: Ascending, field: Name}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f31c0f2f2c96251a84712725290b9acb";

export default node;
