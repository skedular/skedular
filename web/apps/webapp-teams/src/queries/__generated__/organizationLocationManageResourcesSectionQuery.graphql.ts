/**
 * @generated SignedSource<<dac9b8e80b519092bdb12552e2354bb3>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagOrderField = "DESCRIPTION" | "NAME" | "TYPE" | "%future added value";
export type ResourceOrderField = "NAME" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type ResourceOrderInput = {
  direction: OrderDirection;
  field: ResourceOrderField;
};
export type organizationLocationManageResourcesSectionQuery$variables = {
  customTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  locationId: string;
  organizationCustomDomain: string;
  resourceCustomTagIds?: ReadonlyArray<string> | null | undefined;
  resourceNameSearchText?: string | null | undefined;
  resourceZoneIds?: ReadonlyArray<string> | null | undefined;
  resourcesSortingValues?: ReadonlyArray<ResourceOrderInput> | null | undefined;
  zonesSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
};
export type organizationLocationManageResourcesSectionQuery$data = {
  readonly location: {
    readonly resources: {
      readonly __id: string;
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly capacity: number;
          readonly color: string | null | undefined;
          readonly customTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly id: string;
          readonly inactive: boolean;
          readonly name: string;
          readonly productTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly requireBookingApproval: boolean;
          readonly resourceType: {
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          };
          readonly zones: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
        };
      }>;
    };
  } | null | undefined;
  readonly me: {
    readonly id: string;
    readonly preferredResources: ReadonlyArray<{
      readonly id: string;
    }>;
  };
  readonly " $fragmentSpreads": FragmentRefs<"customTagSelector_allCustomTags_query" | "zoneSelector_allZones_query">;
};
export type organizationLocationManageResourcesSectionQuery = {
  response: organizationLocationManageResourcesSectionQuery$data;
  variables: organizationLocationManageResourcesSectionQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "customTagsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourceCustomTagIds"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourceNameSearchText"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourceZoneIds"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourcesSortingValues"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "zonesSortingValues"
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
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v8/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "ResourceDetails",
      "kind": "LinkedField",
      "name": "preferredResources",
      "plural": true,
      "selections": [
        (v8/*:: as any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v10 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "locationId"
  }
],
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
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
  (v8/*:: as any*/),
  (v11/*:: as any*/),
  (v12/*:: as any*/)
],
v14 = {
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
v15 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "orderBy",
      "variableName": "resourcesSortingValues"
    },
    {
      "fields": [
        {
          "kind": "Variable",
          "name": "customTagIds",
          "variableName": "resourceCustomTagIds"
        },
        {
          "kind": "Variable",
          "name": "nameContains",
          "variableName": "resourceNameSearchText"
        },
        {
          "kind": "Variable",
          "name": "zoneIds",
          "variableName": "resourceZoneIds"
        }
      ],
      "kind": "ObjectValue",
      "name": "where"
    }
  ],
  "concreteType": "ConnectionOfResourceEdge",
  "kind": "LinkedField",
  "name": "resources",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ResourceEdge",
      "kind": "LinkedField",
      "name": "edges",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "ResourceDetails",
          "kind": "LinkedField",
          "name": "node",
          "plural": false,
          "selections": [
            (v8/*:: as any*/),
            (v11/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "inactive",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "requireBookingApproval",
              "storageKey": null
            },
            (v12/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "capacity",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "customTags",
              "plural": true,
              "selections": (v13/*:: as any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "zones",
              "plural": true,
              "selections": (v13/*:: as any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "productTags",
              "plural": true,
              "selections": (v13/*:: as any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "resourceType",
              "plural": false,
              "selections": (v13/*:: as any*/),
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    (v14/*:: as any*/)
  ],
  "storageKey": null
},
v16 = [
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
        "selections": (v13/*:: as any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  (v14/*:: as any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      (v6/*:: as any*/),
      (v7/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocationManageResourcesSectionQuery",
    "selections": [
      (v9/*:: as any*/),
      {
        "alias": null,
        "args": (v10/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v15/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "customTagSelector_allCustomTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "zoneSelector_allZones_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*:: as any*/),
      (v1/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      (v3/*:: as any*/),
      (v7/*:: as any*/),
      (v0/*:: as any*/),
      (v6/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "organizationLocationManageResourcesSectionQuery",
    "selections": [
      (v9/*:: as any*/),
      {
        "alias": null,
        "args": (v10/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v15/*:: as any*/),
          (v8/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "customDomain",
            "variableName": "organizationCustomDomain"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "orderBy",
                "variableName": "customTagsSortingValues"
              }
            ],
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "customTags",
            "plural": false,
            "selections": (v16/*:: as any*/),
            "storageKey": null
          },
          (v8/*:: as any*/),
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "orderBy",
                "variableName": "zonesSortingValues"
              }
            ],
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "zones",
            "plural": false,
            "selections": (v16/*:: as any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "51368b95718931f90e3aee75bcde0291",
    "id": null,
    "metadata": {},
    "name": "organizationLocationManageResourcesSectionQuery",
    "operationKind": "query",
    "text": "query organizationLocationManageResourcesSectionQuery(\n  $organizationCustomDomain: String!\n  $locationId: String!\n  $resourceNameSearchText: String\n  $resourceZoneIds: [String!]\n  $resourceCustomTagIds: [String!]\n  $zonesSortingValues: [OrganizationTagOrderInput!]\n  $customTagsSortingValues: [OrganizationTagOrderInput!]\n  $resourcesSortingValues: [ResourceOrderInput!]\n) {\n  me {\n    id\n    preferredResources {\n      id\n    }\n  }\n  location(id: $locationId) {\n    resources(where: {nameContains: $resourceNameSearchText, customTagIds: $resourceCustomTagIds, zoneIds: $resourceZoneIds}, orderBy: $resourcesSortingValues) {\n      edges {\n        node {\n          id\n          name\n          inactive\n          requireBookingApproval\n          color\n          capacity\n          customTags {\n            id\n            name\n            color\n          }\n          zones {\n            id\n            name\n            color\n          }\n          productTags {\n            id\n            name\n            color\n          }\n          resourceType {\n            id\n            name\n            color\n          }\n        }\n      }\n    }\n    id\n  }\n  ...customTagSelector_allCustomTags_query\n  ...zoneSelector_allZones_query\n}\n\nfragment customTagSelector_allCustomTags_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    customTags(orderBy: $customTagsSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n        }\n      }\n    }\n    id\n  }\n}\n\nfragment zoneSelector_allZones_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    zones(orderBy: $zonesSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n        }\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "8afc95bbc48858f510afa761b41bbf5d";

export default node;
