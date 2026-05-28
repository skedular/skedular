/**
 * @generated SignedSource<<96b61682f273bb13b99c33bd751659c1>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type ResourceDayViewList_result$data = {
  readonly items: ReadonlyArray<{
    readonly resourceId: string;
    readonly " $fragmentSpreads": FragmentRefs<"ResourceDayViewCard_resourceDayView">;
  }>;
  readonly subscriptionKey: string;
  readonly " $fragmentType": "ResourceDayViewList_result";
};
export type ResourceDayViewList_result$key = {
  readonly " $data"?: ResourceDayViewList_result$data;
  readonly " $fragmentSpreads": FragmentRefs<"ResourceDayViewList_result">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "ResourceDayViewList_result",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "subscriptionKey",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "ResourceDayViewDetails",
      "kind": "LinkedField",
      "name": "items",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "resourceId",
          "storageKey": null
        },
        {
          "args": null,
          "kind": "FragmentSpread",
          "name": "ResourceDayViewCard_resourceDayView"
        }
      ],
      "storageKey": null
    }
  ],
  "type": "ResourceDayViewConnection",
  "abstractKey": null
};

(node as any).hash = "0135b7452501ff2069336308741f654a";

export default node;
