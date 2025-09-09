/**
 * @generated SignedSource<<e32dc7cdd98fa33204a18f361aa2538f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "ABOUT" | "NAME" | "TIMEZONE" | "TYPE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagOrderField = "DESCRIPTION" | "NAME" | "TYPE" | "%future added value";
export type OrganizationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type addResourceDialog_rootQuery$variables = {
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  multipleChoicesCustomTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  multipleChoicesProductTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  multipleChoicesZonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationUniqueAlphanumericName: string;
};
export type addResourceDialog_rootQuery$data = {
  readonly locations: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly totalCount: number;
  };
  readonly organization: {
    readonly type: {
      readonly type: OrganizationType;
    };
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesCustomTags_query" | "multipleChoicesProductTags_query" | "multipleChoicesZones_query" | "singleChoiceResourceType_query">;
};
export type addResourceDialog_rootQuery = {
  response: addResourceDialog_rootQuery$data;
  variables: addResourceDialog_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesCustomTagsSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesProductTagsSortingValues"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesZonesSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v5 = [
  {
    "kind": "Variable",
    "name": "uniqueAlphanumericName",
    "variableName": "organizationUniqueAlphanumericName"
  }
],
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTypeDetails",
  "kind": "LinkedField",
  "name": "type",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "type",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v10 = {
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
},
v11 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "orderBy",
      "variableName": "locationsSortingValues"
    },
    {
      "fields": [
        {
          "kind": "Variable",
          "name": "organizationUniqueAlphanumericName",
          "variableName": "organizationUniqueAlphanumericName"
        }
      ],
      "kind": "ObjectValue",
      "name": "where"
    }
  ],
  "concreteType": "ConnectionOfLocationEdge",
  "kind": "LinkedField",
  "name": "locations",
  "plural": false,
  "selections": [
    (v7/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "LocationEdge",
      "kind": "LinkedField",
      "name": "edges",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationDetails",
          "kind": "LinkedField",
          "name": "node",
          "plural": false,
          "selections": [
            (v8/*: any*/),
            (v9/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    (v10/*: any*/)
  ],
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v13 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesCustomTagsSortingValues"
  }
],
v14 = [
  (v7/*: any*/),
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
          (v8/*: any*/),
          (v9/*: any*/),
          (v12/*: any*/),
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
  (v10/*: any*/)
],
v15 = [
  "orderBy"
],
v16 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesZonesSortingValues"
  }
],
v17 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesProductTagsSortingValues"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "addResourceDialog_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v5/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v6/*: any*/)
        ],
        "storageKey": null
      },
      (v11/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceResourceType_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesCustomTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesZones_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesProductTags_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v4/*: any*/),
      (v1/*: any*/),
      (v3/*: any*/),
      (v2/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "addResourceDialog_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v5/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v6/*: any*/),
          (v8/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "resourceTypes",
            "plural": true,
            "selections": [
              (v8/*: any*/),
              (v9/*: any*/),
              (v12/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v13/*: any*/),
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "customTags",
            "plural": false,
            "selections": (v14/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v13/*: any*/),
            "filters": (v15/*: any*/),
            "handle": "connection",
            "key": "multipleChoicesCustomTags_customTags",
            "kind": "LinkedHandle",
            "name": "customTags"
          },
          {
            "alias": null,
            "args": (v16/*: any*/),
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "zones",
            "plural": false,
            "selections": (v14/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v16/*: any*/),
            "filters": (v15/*: any*/),
            "handle": "connection",
            "key": "multipleChoicesZones_zones",
            "kind": "LinkedHandle",
            "name": "zones"
          },
          {
            "alias": null,
            "args": (v17/*: any*/),
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": false,
            "selections": (v14/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v17/*: any*/),
            "filters": (v15/*: any*/),
            "handle": "connection",
            "key": "multipleChoicesProductTags_productTags",
            "kind": "LinkedHandle",
            "name": "productTags"
          }
        ],
        "storageKey": null
      },
      (v11/*: any*/)
    ]
  },
  "params": {
    "cacheID": "3f910f965a96fe28c50f6ef628ac69f0",
    "id": null,
    "metadata": {},
    "name": "addResourceDialog_rootQuery",
    "operationKind": "query",
    "text": "query addResourceDialog_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    type {\n      type\n    }\n    id\n  }\n  locations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  ...singleChoiceResourceType_query\n  ...multipleChoicesCustomTags_query\n  ...multipleChoicesZones_query\n  ...multipleChoicesProductTags_query\n}\n\nfragment multipleChoicesCustomTags_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    customTags(orderBy: $multipleChoicesCustomTagsSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment multipleChoicesProductTags_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    productTags(orderBy: $multipleChoicesProductTagsSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment multipleChoicesZones_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    zones(orderBy: $multipleChoicesZonesSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment singleChoiceResourceType_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    resourceTypes {\n      id\n      name\n      color\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "df1b7ddbaef4773c00e8339e0f37a455";

export default node;
