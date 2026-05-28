/**
 * @generated SignedSource<<ebe975ab057ea62d4c3694cde15e5abb>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type AvailabilityDashboard_data$data = {
  readonly subscriptionKey: string;
  readonly " $fragmentSpreads": FragmentRefs<"ResourceDayViewList_result">;
  readonly " $fragmentType": "AvailabilityDashboard_data";
};
export type AvailabilityDashboard_data$key = {
  readonly " $data"?: AvailabilityDashboard_data$data;
  readonly " $fragmentSpreads": FragmentRefs<"AvailabilityDashboard_data">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "AvailabilityDashboard_data",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "subscriptionKey",
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "ResourceDayViewList_result"
    }
  ],
  "type": "ResourceDayViewConnection",
  "abstractKey": null
};

(node as any).hash = "f0646015340dfd42a125bf1fbfa96177";

export default node;
