/**
 * @generated SignedSource<<82c1b4e29ed25e82ed4eebaf99bfb349>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagType = "CUSTOM" | "LOCATION_SPACE_TYPE_CAR_PARK_SPACE" | "LOCATION_SPACE_TYPE_COMMERCIAL_KITCHEN" | "LOCATION_SPACE_TYPE_EVENT_SPACE" | "LOCATION_SPACE_TYPE_MEETING_SPACE" | "LOCATION_SPACE_TYPE_OFFICE_SPACE" | "LOCATION_SPACE_TYPE_OTHERS" | "LOCATION_SPACE_TYPE_RETAIL_SPACE" | "LOCATION_SPACE_TYPE_SHOOT_LOCATION" | "LOCATION_SPACE_TYPE_STORAGE_SPACE" | "LOCATION_SPACE_TYPE_STUDIO_SPACE" | "PRODUCT" | "RESOURCE_DESK" | "RESOURCE_OTHERS" | "RESOURCE_PARKING" | "RESOURCE_ROOM" | "ZONE" | "%future added value";
export type ResourceOrderField = "NAME" | "%future added value";
export type ResourceOrderInput = {
  direction: OrderDirection;
  field: ResourceOrderField;
};
export type addFloorPlan_rootQuery$variables = {
  floorPlanId: string;
  locationId: string;
  resourcesSortingValues?: ReadonlyArray<ResourceOrderInput> | null | undefined;
};
export type addFloorPlan_rootQuery$data = {
  readonly deskResourceType: OrganizationTagType;
  readonly parkingResourceType: OrganizationTagType;
  readonly roomResourceType: OrganizationTagType;
  readonly " $fragmentSpreads": FragmentRefs<"addFloorPlan_resources_query">;
};
export type addFloorPlan_rootQuery = {
  response: addFloorPlan_rootQuery$data;
  variables: addFloorPlan_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "floorPlanId"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourcesSortingValues"
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "deskResourceType",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "roomResourceType",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "parkingResourceType",
  "storageKey": null
},
v6 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "resourcesSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "floorPlanId",
        "variableName": "floorPlanId"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v10 = [
  (v7/*: any*/),
  (v8/*: any*/),
  (v9/*: any*/)
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
    "name": "addFloorPlan_rootQuery",
    "selections": [
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "addFloorPlan_resources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "addFloorPlan_rootQuery",
    "selections": [
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "locationId"
          }
        ],
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": (v6/*: any*/),
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
                      (v7/*: any*/),
                      (v8/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "inactive",
                        "storageKey": null
                      },
                      (v9/*: any*/),
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
                        "selections": (v10/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "zones",
                        "plural": true,
                        "selections": (v10/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "productTags",
                        "plural": true,
                        "selections": (v10/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "resourceType",
                        "plural": false,
                        "selections": [
                          (v7/*: any*/),
                          (v8/*: any*/),
                          (v9/*: any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "tagType",
                            "storageKey": null
                          }
                        ],
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
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v6/*: any*/),
            "filters": [
              "where",
              "orderBy"
            ],
            "handle": "connection",
            "key": "addFloorPlanResourcesQuery_resources",
            "kind": "LinkedHandle",
            "name": "resources"
          },
          (v7/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "8afddcaa79c483f3e162f53ecbba688d",
    "id": null,
    "metadata": {},
    "name": "addFloorPlan_rootQuery",
    "operationKind": "query",
    "text": "query addFloorPlan_rootQuery(\n  $locationId: String!\n  $floorPlanId: String!\n  $resourcesSortingValues: [ResourceOrderInput!]\n) {\n  deskResourceType\n  roomResourceType\n  parkingResourceType\n  ...addFloorPlan_resources_query\n}\n\nfragment addFloorPlan_resources_query on Query {\n  location(id: $locationId) {\n    resources(where: {floorPlanId: $floorPlanId}, orderBy: $resourcesSortingValues) {\n      edges {\n        node {\n          id\n          name\n          inactive\n          color\n          capacity\n          customTags {\n            id\n            name\n            color\n          }\n          zones {\n            id\n            name\n            color\n          }\n          productTags {\n            id\n            name\n            color\n          }\n          resourceType {\n            id\n            name\n            color\n            tagType\n          }\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "e00b0dae00169f4b0a30a5cea25f3594";

export default node;
