/**
 * @generated SignedSource<<c30f1a18ac224310e78fbdab76048e75>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "ABOUT" | "NAME" | "TIMEZONE" | "%future added value";
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
    readonly totalCount: number | null | undefined;
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
  "fields": [
    {
      "kind": "Variable",
      "name": "organizationUniqueAlphanumericName",
      "variableName": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v11 = {
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
v12 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "orderBy",
      "variableName": "locationsSortingValues"
    },
    (v7/*: any*/)
  ],
  "concreteType": "ConnectionOfLocationEdge",
  "kind": "LinkedField",
  "name": "locations",
  "plural": false,
  "selections": [
    (v8/*: any*/),
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
            (v9/*: any*/),
            (v10/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    (v11/*: any*/)
  ],
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v14 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesCustomTagsSortingValues"
  },
  (v7/*: any*/)
],
v15 = [
  (v8/*: any*/),
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
          (v9/*: any*/),
          (v10/*: any*/),
          (v13/*: any*/),
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
  (v11/*: any*/)
],
v16 = [
  "where",
  "orderBy"
],
v17 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesZonesSortingValues"
  },
  (v7/*: any*/)
],
v18 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesProductTagsSortingValues"
  },
  (v7/*: any*/)
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
      (v12/*: any*/),
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
          (v9/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "resourceTypes",
            "plural": true,
            "selections": [
              (v9/*: any*/),
              (v10/*: any*/),
              (v13/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      (v12/*: any*/),
      {
        "alias": null,
        "args": (v14/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "customTags",
        "plural": false,
        "selections": (v15/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v14/*: any*/),
        "filters": (v16/*: any*/),
        "handle": "connection",
        "key": "multipleChoicesCustomTags_customTags",
        "kind": "LinkedHandle",
        "name": "customTags"
      },
      {
        "alias": null,
        "args": (v17/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "zones",
        "plural": false,
        "selections": (v15/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v17/*: any*/),
        "filters": (v16/*: any*/),
        "handle": "connection",
        "key": "multipleChoicesZones_zones",
        "kind": "LinkedHandle",
        "name": "zones"
      },
      {
        "alias": null,
        "args": (v18/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "productTags",
        "plural": false,
        "selections": (v15/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v18/*: any*/),
        "filters": (v16/*: any*/),
        "handle": "connection",
        "key": "multipleChoicesProductTags_productTags",
        "kind": "LinkedHandle",
        "name": "productTags"
      }
    ]
  },
  "params": {
    "cacheID": "38573596cad70a49b316584bf7fde2d8",
    "id": null,
    "metadata": {},
    "name": "addResourceDialog_rootQuery",
    "operationKind": "query",
    "text": "query addResourceDialog_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    type {\n      type\n    }\n    id\n  }\n  locations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  ...singleChoiceResourceType_query\n  ...multipleChoicesCustomTags_query\n  ...multipleChoicesZones_query\n  ...multipleChoicesProductTags_query\n}\n\nfragment multipleChoicesCustomTags_query on Query {\n  customTags(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}, orderBy: $multipleChoicesCustomTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment multipleChoicesProductTags_query on Query {\n  productTags(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}, orderBy: $multipleChoicesProductTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment multipleChoicesZones_query on Query {\n  zones(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}, orderBy: $multipleChoicesZonesSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment singleChoiceResourceType_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    resourceTypes {\n      id\n      name\n      color\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "df1b7ddbaef4773c00e8339e0f37a455";

export default node;
