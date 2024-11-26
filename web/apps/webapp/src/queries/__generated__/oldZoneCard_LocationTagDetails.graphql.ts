/**
 * @generated SignedSource<<060f487f031db483eb76f167e7ab59e4>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type oldZoneCard_LocationTagDetails$data = {
  readonly id: string;
  readonly name: string;
  readonly " $fragmentType": "oldZoneCard_LocationTagDetails";
};
export type oldZoneCard_LocationTagDetails$key = {
  readonly " $data"?: oldZoneCard_LocationTagDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"oldZoneCard_LocationTagDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "oldZoneCard_LocationTagDetails",
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
    }
  ],
  "type": "LocationTagDetails",
  "abstractKey": null
};

(node as any).hash = "93de1a4fe753cdd243e488557aa4684b";

export default node;
