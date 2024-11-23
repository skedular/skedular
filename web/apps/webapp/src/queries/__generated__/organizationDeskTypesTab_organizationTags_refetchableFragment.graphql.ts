/**
 * @generated SignedSource<<cc92b8808be8bd770ef1a443a3fa1033>>
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
export type organizationDeskTypesTab_organizationTags_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  deskTypeNameSearchText?: string | null | undefined;
  deskTypeSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  deskTypeTagType?: string | null | undefined;
  organizationId: string;
};
export type organizationDeskTypesTab_organizationTags_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationDeskTypesTab_organizationTags_query">;
};
export type organizationDeskTypesTab_organizationTags_refetchableFragment = {
  response: organizationDeskTypesTab_organizationTags_refetchableFragment$data;
  variables: organizationDeskTypesTab_organizationTags_refetchableFragment$variables;
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
    "name": "deskTypeNameSearchText"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "deskTypeSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "deskTypeTagType"
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
    "variableName": "deskTypeSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "deskTypeNameSearchText"
      },
      {
        "kind": "Variable",
        "name": "organizationId",
        "variableName": "organizationId"
      },
      {
        "kind": "Variable",
        "name": "tagType",
        "variableName": "deskTypeTagType"
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
    "name": "organizationDeskTypesTab_organizationTags_refetchableFragment",
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
        "name": "organizationDeskTypesTab_organizationTags_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationDeskTypesTab_organizationTags_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "organizationTags",
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
        "key": "organizationDeskTypesTab_organizationTags",
        "kind": "LinkedHandle",
        "name": "organizationTags"
      }
    ]
  },
  "params": {
    "cacheID": "829c8a841d6d8f7f27aed129e4a0cd8e",
    "id": null,
    "metadata": {},
    "name": "organizationDeskTypesTab_organizationTags_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationDeskTypesTab_organizationTags_refetchableFragment(\n  $count: Int = 50\n  $cursor: String\n  $deskTypeNameSearchText: String\n  $deskTypeSortingValues: [OrganizationTagOrderInput!]\n  $deskTypeTagType: String\n  $organizationId: String!\n) {\n  ...organizationDeskTypesTab_organizationTags_query_1G22uz\n}\n\nfragment deskTypeCard_OrganizationTagDetails on OrganizationTagDetails {\n  id\n  name\n}\n\nfragment organizationDeskTypesTab_organizationTags_query_1G22uz on Query {\n  organizationTags(first: $count, after: $cursor, where: {organizationId: $organizationId, tagType: $deskTypeTagType, nameContains: $deskTypeNameSearchText}, orderBy: $deskTypeSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...deskTypeCard_OrganizationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f74df9728c61bfb15380eb9e68b5f6c0";

export default node;
