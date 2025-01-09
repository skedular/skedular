/**
 * @generated SignedSource<<4d16b28cd098254dc7fa036af9c1b564>>
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
  deskTypeSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationId: string;
};
export type organizationDeskTypesTab_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationDeskTypesTab_deskTypes_query" | "organizationDeskTypesTab_query">;
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
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
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
        "name": "organizationDeskTypesTab_deskTypes_query"
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
        "args": (v4/*: any*/),
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
        "key": "organizationDeskTypesTab_deskTypes",
        "kind": "LinkedHandle",
        "name": "deskTypes"
      }
    ]
  },
  "params": {
    "cacheID": "46d99e1d706db75c97ebe8752d2d5183",
    "id": null,
    "metadata": {},
    "name": "organizationDeskTypesTab_rootQuery",
    "operationKind": "query",
    "text": "query organizationDeskTypesTab_rootQuery(\n  $organizationId: String!\n  $deskTypeNameSearchText: String\n  $deskTypeSortingValues: [OrganizationTagOrderInput!]\n) {\n  ...organizationDeskTypesTab_query\n  ...organizationDeskTypesTab_deskTypes_query\n}\n\nfragment deskTypeCard_OrganizationTagDetails on OrganizationTagDetails {\n  id\n  name\n}\n\nfragment deskTypeCard_Query on Query {\n  me {\n    id\n    preferredDeskTypes {\n      uniqueId\n    }\n  }\n  organization(id: $organizationId) {\n    canModify\n    id\n  }\n}\n\nfragment organizationDeskTypesTab_deskTypes_query on Query {\n  deskTypes(first: 50, where: {organizationId: $organizationId, nameContains: $deskTypeNameSearchText}, orderBy: $deskTypeSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...deskTypeCard_OrganizationTagDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationDeskTypesTab_query on Query {\n  organization(id: $organizationId) {\n    canModify\n    id\n  }\n  ...deskTypeCard_Query\n}\n"
  }
};
})();

(node as any).hash = "e5e32d3118a269e3fe8721a1d0cc10b8";

export default node;
