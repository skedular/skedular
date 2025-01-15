/**
 * @generated SignedSource<<e05d452981cfcb7232f93e124cdbdde3>>
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
export type organizationCustomTagsTab_rootQuery$variables = {
  customTagNameSearchText?: string | null | undefined;
  customTagSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationId: string;
};
export type organizationCustomTagsTab_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationCustomTagsTab_customTags_query" | "organizationCustomTagsTab_query">;
};
export type organizationCustomTagsTab_rootQuery = {
  response: organizationCustomTagsTab_rootQuery$data;
  variables: organizationCustomTagsTab_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagNameSearchText"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v4 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
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
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationCustomTagsTab_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationCustomTagsTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationCustomTagsTab_customTags_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Operation",
    "name": "organizationCustomTagsTab_rootQuery",
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
          (v3/*: any*/)
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
          (v3/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerOrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredCustomTags",
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
        "args": (v4/*: any*/),
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
                  (v3/*: any*/),
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
        "args": (v4/*: any*/),
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
    "cacheID": "1f0df8b3f3bbbfbf337cfe2244194ab7",
    "id": null,
    "metadata": {},
    "name": "organizationCustomTagsTab_rootQuery",
    "operationKind": "query",
    "text": "query organizationCustomTagsTab_rootQuery(\n  $organizationId: String!\n  $customTagNameSearchText: String\n  $customTagSortingValues: [OrganizationTagOrderInput!]\n) {\n  ...organizationCustomTagsTab_query\n  ...organizationCustomTagsTab_customTags_query\n}\n\nfragment customTagCard_OrganizationTagDetails on OrganizationTagDetails {\n  id\n  name\n  description\n  color\n}\n\nfragment customTagCard_Query on Query {\n  me {\n    id\n    preferredCustomTags {\n      uniqueId\n    }\n  }\n  organization(id: $organizationId) {\n    canModify\n    id\n  }\n}\n\nfragment organizationCustomTagsTab_customTags_query on Query {\n  customTags(first: 50, where: {organizationId: $organizationId, nameContains: $customTagNameSearchText}, orderBy: $customTagSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...customTagCard_OrganizationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationCustomTagsTab_query on Query {\n  organization(id: $organizationId) {\n    canModify\n    id\n  }\n  ...customTagCard_Query\n}\n"
  }
};
})();

(node as any).hash = "c548364f56407ed6406ea5dd57e3bc6c";

export default node;
