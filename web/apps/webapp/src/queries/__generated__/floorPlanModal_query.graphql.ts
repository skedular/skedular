/**
 * @generated SignedSource<<590b69a5b776e9e79926348b0901321c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type floorPlanModal_query$data = {
  readonly floorPlansByLocation: ReadonlyArray<{
    readonly floorLevel: number;
    readonly floorName: string | null | undefined;
    readonly height: number;
    readonly id: string;
    readonly imagePath: string;
    readonly isActive: boolean;
    readonly name: string;
    readonly resourcePositions: ReadonlyArray<{
      readonly height: number;
      readonly id: string;
      readonly metadata: string | null | undefined;
      readonly resource: {
        readonly id: string;
        readonly name: string;
      };
      readonly shape: string | null | undefined;
      readonly width: number;
      readonly x: number;
      readonly y: number;
    }>;
    readonly thumbnailPath: string | null | undefined;
    readonly width: number;
  }>;
  readonly location: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly resources: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly capacity: number;
        readonly color: string | null | undefined;
        readonly customTags: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly id: string;
        readonly inactive: boolean;
        readonly name: string;
        readonly requireBookingApproval: boolean;
        readonly resourceType: {
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly tagType: string | null | undefined;
          readonly uniqueId: string;
        };
        readonly zones: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
      };
    }>;
  };
  readonly " $fragmentSpreads": FragmentRefs<"customerFloorPlanView_availableResources_query">;
  readonly " $fragmentType": "floorPlanModal_query";
};
export type floorPlanModal_query$key = {
  readonly " $data"?: floorPlanModal_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"floorPlanModal_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  (v0/*: any*/),
  (v1/*: any*/)
],
v3 = [
  {
    "kind": "Variable",
    "name": "locationId",
    "variableName": "locationId"
  }
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "width",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "height",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v8 = [
  (v7/*: any*/),
  (v1/*: any*/),
  (v6/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "locationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "floorPlanModal_query",
  "selections": [
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
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": (v3/*: any*/),
      "concreteType": "FloorPlanDetails",
      "kind": "LinkedField",
      "name": "floorPlansByLocation",
      "plural": true,
      "selections": [
        (v0/*: any*/),
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "floorLevel",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "floorName",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "imagePath",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "thumbnailPath",
          "storageKey": null
        },
        (v4/*: any*/),
        (v5/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "isActive",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "ResourcePosition",
          "kind": "LinkedField",
          "name": "resourcePositions",
          "plural": true,
          "selections": [
            (v0/*: any*/),
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
            (v4/*: any*/),
            (v5/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "shape",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "metadata",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "ResourceDetails",
              "kind": "LinkedField",
              "name": "resource",
              "plural": false,
              "selections": (v2/*: any*/),
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
      "args": [
        {
          "fields": (v3/*: any*/),
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
                (v0/*: any*/),
                (v1/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "inactive",
                  "storageKey": null
                },
                (v6/*: any*/),
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
                  "kind": "ScalarField",
                  "name": "requireBookingApproval",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Location_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "resourceType",
                  "plural": false,
                  "selections": [
                    (v7/*: any*/),
                    (v1/*: any*/),
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "tagType",
                      "storageKey": null
                    },
                    (v6/*: any*/)
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Location_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "customTags",
                  "plural": true,
                  "selections": (v8/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Location_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "zones",
                  "plural": true,
                  "selections": (v8/*: any*/),
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
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "customerFloorPlanView_availableResources_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "9268a13b010f0cfcd0cd83d9a7ff73b0";

export default node;
