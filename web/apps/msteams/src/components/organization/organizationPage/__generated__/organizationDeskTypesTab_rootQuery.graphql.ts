/**
 * @generated SignedSource<<5aa92a20cfc06e5dd5776c6ffa5d13a2>>
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
export type organizationDeskTypesTab_rootQuery$variables = {
  deskTypeNameSearchText?: string | null | undefined;
  deskTypeSortingValues: ReadonlyArray<OrganizationTagOrderInput>;
  deskTypeTagType: string;
  organizationId: string;
};
export type organizationDeskTypesTab_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationDeskTypesTab_organizationTags_query" | "organizationDeskTypesTab_query">;
};
export type organizationDeskTypesTab_rootQuery = {
  response: organizationDeskTypesTab_rootQuery$data;
  variables: organizationDeskTypesTab_rootQuery$variables;
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
  "name": "deskTypeSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "deskTypeTagType"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v5 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
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
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationDeskTypesTab_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationDeskTypesTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationDeskTypesTab_organizationTags_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v3/*: any*/),
      (v2/*: any*/),
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Operation",
    "name": "organizationDeskTypesTab_rootQuery",
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
          (v4/*: any*/)
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
          (v4/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerOrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredDeskTypes",
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
        "args": (v5/*: any*/),
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
                  (v4/*: any*/),
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
        "args": (v5/*: any*/),
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
    "cacheID": "f63a5d100aee376ff4ac0156b7a9fae2",
    "id": null,
    "metadata": {},
    "name": "organizationDeskTypesTab_rootQuery",
    "operationKind": "query",
    "text": "query organizationDeskTypesTab_rootQuery(\n  $organizationId: String!\n  $deskTypeTagType: String!\n  $deskTypeNameSearchText: String\n  $deskTypeSortingValues: [OrganizationTagOrderInput!]!\n) {\n  ...organizationDeskTypesTab_query\n  ...organizationDeskTypesTab_organizationTags_query\n}\n\nfragment deskTypeCard_OrganizationTagDetails on OrganizationTagDetails {\n  id\n  name\n}\n\nfragment deskTypeCard_Query on Query {\n  me {\n    id\n    preferredDeskTypes {\n      uniqueId\n    }\n  }\n  organization(id: $organizationId) {\n    canModify\n    id\n  }\n}\n\nfragment organizationDeskTypesTab_organizationTags_query on Query {\n  organizationTags(first: 50, where: {organizationId: $organizationId, tagType: $deskTypeTagType, nameContains: $deskTypeNameSearchText}, orderBy: $deskTypeSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...deskTypeCard_OrganizationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationDeskTypesTab_query on Query {\n  organization(id: $organizationId) {\n    canModify\n    id\n  }\n  ...deskTypeCard_Query\n}\n"
  }
};
})();

(node as any).hash = "909d37e26f55df62d3473b915d950b58";

export default node;
