/**
 * @generated SignedSource<<d111720819328f7082a2715a581ba739>>
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
export type organizationDeskTypesTab_deskTypes_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  deskTypeNameSearchText?: string | null | undefined;
  deskTypeSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationId: string;
};
export type organizationDeskTypesTab_deskTypes_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationDeskTypesTab_deskTypes_query">;
};
export type organizationDeskTypesTab_deskTypes_refetchableFragment = {
  response: organizationDeskTypesTab_deskTypes_refetchableFragment$data;
  variables: organizationDeskTypesTab_deskTypes_refetchableFragment$variables;
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
    "name": "organizationDeskTypesTab_deskTypes_refetchableFragment",
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
        "name": "organizationDeskTypesTab_deskTypes_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationDeskTypesTab_deskTypes_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "deskTypes",
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
        "key": "organizationDeskTypesTab_deskTypes",
        "kind": "LinkedHandle",
        "name": "deskTypes"
      }
    ]
  },
  "params": {
    "cacheID": "4b71926c3ffa55bcb14229f6d1973dfc",
    "id": null,
    "metadata": {},
    "name": "organizationDeskTypesTab_deskTypes_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationDeskTypesTab_deskTypes_refetchableFragment(\n  $count: Int = 50\n  $cursor: String\n  $deskTypeNameSearchText: String\n  $deskTypeSortingValues: [OrganizationTagOrderInput!]\n  $organizationId: String!\n) {\n  ...organizationDeskTypesTab_deskTypes_query_1G22uz\n}\n\nfragment deskTypeCard_OrganizationTagDetails on OrganizationTagDetails {\n  id\n  name\n}\n\nfragment organizationDeskTypesTab_deskTypes_query_1G22uz on Query {\n  deskTypes(first: $count, after: $cursor, where: {organizationId: $organizationId, nameContains: $deskTypeNameSearchText}, orderBy: $deskTypeSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...deskTypeCard_OrganizationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d96b9a29970af12be2f25f68a5941c46";

export default node;
