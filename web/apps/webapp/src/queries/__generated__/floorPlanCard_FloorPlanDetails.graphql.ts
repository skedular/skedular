/**
 * @generated SignedSource<<36613ae358b10ee4039777939677bd73>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type floorPlanCard_FloorPlanDetails$data = {
  readonly id: string;
  readonly image: {
    readonly thumbnail: {
      readonly height: number | null | undefined;
      readonly url: string;
      readonly width: number | null | undefined;
    } | null | undefined;
  };
  readonly name: string;
  readonly resourceCount: number;
  readonly " $fragmentType": "floorPlanCard_FloorPlanDetails";
};
export type floorPlanCard_FloorPlanDetails$key = {
  readonly " $data"?: floorPlanCard_FloorPlanDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"floorPlanCard_FloorPlanDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "floorPlanCard_FloorPlanDetails",
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
          "name": "thumbnail",
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
      "kind": "ScalarField",
      "name": "resourceCount",
      "storageKey": null
    }
  ],
  "type": "FloorPlanDetails",
  "abstractKey": null
};

(node as any).hash = "8a67c3bf6ea8b5fb6bd4e9610d2aa3c9";

export default node;
