/**
 * @generated SignedSource<<04a9c0fa8f73e5bbff54d94166dc35f5>>
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
export type organizationCustomTagsTab_customTags_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  customTagNameSearchText?: string | null | undefined;
  customTagSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationId: string;
};
export type organizationCustomTagsTab_customTags_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationCustomTagsTab_customTags_query">;
};
export type organizationCustomTagsTab_customTags_refetchableFragment = {
  response: organizationCustomTagsTab_customTags_refetchableFragment$data;
  variables: organizationCustomTagsTab_customTags_refetchableFragment$variables;
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
    "name": "customTagNameSearchText"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "customTagSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
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
    "variableName": "customTagSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "customTagNameSearchText"
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
    "name": "organizationCustomTagsTab_customTags_refetchableFragment",
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
        "name": "organizationCustomTagsTab_customTags_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationCustomTagsTab_customTags_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "customTags",
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
        "key": "organizationCustomTagsTab_customTags",
        "kind": "LinkedHandle",
        "name": "customTags"
      }
    ]
  },
  "params": {
    "cacheID": "235e21bee79c567fdf614d0c40d9b990",
    "id": null,
    "metadata": {},
    "name": "organizationCustomTagsTab_customTags_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationCustomTagsTab_customTags_refetchableFragment(\n  $count: Int = 50\n  $cursor: String\n  $customTagNameSearchText: String\n  $customTagSortingValues: [OrganizationTagOrderInput!]\n  $organizationId: String!\n) {\n  ...organizationCustomTagsTab_customTags_query_1G22uz\n}\n\nfragment customTagCard_OrganizationTagDetails on OrganizationTagDetails {\n  id\n  name\n}\n\nfragment organizationCustomTagsTab_customTags_query_1G22uz on Query {\n  customTags(first: $count, after: $cursor, where: {organizationId: $organizationId, nameContains: $customTagNameSearchText}, orderBy: $customTagSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...customTagCard_OrganizationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "bad5c42787aebac97a985b4c94f0e9a1";

export default node;
