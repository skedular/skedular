/**
 * @generated SignedSource<<b6eeb715c1302467b1f0f760a5309add>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type ResourceAvailabilityClassification = "AVAILABLE" | "BLOCKED" | "FULLY_BOOKED" | "OCCUPIED" | "PARTIALLY_BOOKED" | "UNAVAILABLE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type AvailabilityFilterBar_statuses$data = ReadonlyArray<{
  readonly name: string;
  readonly type: ResourceAvailabilityClassification;
  readonly " $fragmentType": "AvailabilityFilterBar_statuses";
}>;
export type AvailabilityFilterBar_statuses$key = ReadonlyArray<{
  readonly " $data"?: AvailabilityFilterBar_statuses$data;
  readonly " $fragmentSpreads": FragmentRefs<"AvailabilityFilterBar_statuses">;
}>;

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": {
    "plural": true
  },
  "name": "AvailabilityFilterBar_statuses",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "type",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "name",
      "storageKey": null
    }
  ],
  "type": "ResourceAvailabilityClassificationDetails",
  "abstractKey": null
};

(node as any).hash = "0c8be98cd0e78b011648c6d98349ed4b";

export default node;
