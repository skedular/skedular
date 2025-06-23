/**
 * @generated SignedSource<<9054d446fbae70f1a3e461f573ccf92a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type ResourceOrderField = "NAME" | "%future added value";
export type ResourceOrderInput = {
  direction: OrderDirection;
  field: ResourceOrderField;
};
export type floorPlans_floorPlan_refetchableFragment$variables = {
  floorPlanExists: boolean;
  floorPlanId: string;
  locationId: string;
  resourcesSortingValues?: ReadonlyArray<ResourceOrderInput> | null | undefined;
};
export type floorPlans_floorPlan_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"floorPlans_floorPlan_query">;
};
export type floorPlans_floorPlan_refetchableFragment = {
  response: floorPlans_floorPlan_refetchableFragment$data;
  variables: floorPlans_floorPlan_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "floorPlanExists"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "floorPlanId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "resourcesSortingValues"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v5 = [
  (v3/*: any*/),
  (v2/*: any*/),
  (v4/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "floorPlans_floorPlan_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "floorPlans_floorPlan_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "floorPlans_floorPlan_refetchableFragment",
    "selections": [
      {
        "condition": "floorPlanExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "id",
                "variableName": "floorPlanId"
              }
            ],
            "concreteType": "FloorPlanDetails",
            "kind": "LinkedField",
            "name": "floorPlan",
            "plural": false,
            "selections": [
              (v1/*: any*/),
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnImageFile",
                "kind": "LinkedField",
                "name": "image",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "CdnFile",
                    "kind": "LinkedField",
                    "name": "original",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "url",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "height",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "width",
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
                "args": null,
                "concreteType": "ResourcePositionDetails",
                "kind": "LinkedField",
                "name": "resourcePositions",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "x",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "y",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ResourceDetails",
                    "kind": "LinkedField",
                    "name": "resource",
                    "plural": false,
                    "selections": [
                      (v1/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v1/*: any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
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
                    "name": "floorPlanId",
                    "variableName": "floorPlanId"
                  },
                  {
                    "kind": "Variable",
                    "name": "locationId",
                    "variableName": "locationId"
                  }
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "ResourceConnection",
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
                      (v1/*: any*/),
                      (v2/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Location_OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "resourceType",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "tagType",
                            "storageKey": null
                          },
                          (v3/*: any*/),
                          (v2/*: any*/),
                          (v4/*: any*/)
                        ],
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "inactive",
                        "storageKey": null
                      },
                      (v4/*: any*/),
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
                        "concreteType": "Location_OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "customTags",
                        "plural": true,
                        "selections": (v5/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Location_OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "zones",
                        "plural": true,
                        "selections": (v5/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Location_OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "productTags",
                        "plural": true,
                        "selections": (v5/*: any*/),
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "6bcb334140481b1792a9cbb4553a4a07",
    "id": null,
    "metadata": {},
    "name": "floorPlans_floorPlan_refetchableFragment",
    "operationKind": "query",
    "text": "query floorPlans_floorPlan_refetchableFragment(\n  $floorPlanExists: Boolean!\n  $floorPlanId: String!\n  $locationId: String!\n  $resourcesSortingValues: [ResourceOrderInput!]\n) {\n  ...floorPlans_floorPlan_query\n}\n\nfragment floorPlans_floorPlan_query on Query {\n  floorPlan(id: $floorPlanId) @include(if: $floorPlanExists) {\n    id\n    name\n    image {\n      original {\n        url\n        height\n        width\n      }\n    }\n    resourcePositions {\n      x\n      y\n      resource {\n        id\n      }\n      id\n    }\n  }\n  resources(where: {locationId: $locationId, floorPlanId: $floorPlanId}, orderBy: $resourcesSortingValues) @include(if: $floorPlanExists) {\n    edges {\n      node {\n        id\n        name\n        resourceType {\n          tagType\n        }\n        ...resourceCard_ResourceDetails\n      }\n    }\n  }\n}\n\nfragment resourceCard_ResourceDetails on ResourceDetails {\n  id\n  name\n  inactive\n  color\n  capacity\n  customTags {\n    uniqueId\n    name\n    color\n  }\n  zones {\n    uniqueId\n    name\n    color\n  }\n  productTags {\n    uniqueId\n    name\n    color\n  }\n  resourceType {\n    uniqueId\n    name\n    color\n    tagType\n  }\n}\n"
  }
};
})();

(node as any).hash = "f012ebe5fecfa93d07f94b2c21ada196";

export default node;
