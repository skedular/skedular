/**
 * @generated SignedSource<<4d801889b1aa1e9bdc3508c012f4eb0c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type zoneCard_LocationTagDetails$data = {
  readonly id: string;
  readonly name: string;
  readonly " $fragmentType": "zoneCard_LocationTagDetails";
};
export type zoneCard_LocationTagDetails$key = {
  readonly " $data"?: zoneCard_LocationTagDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"zoneCard_LocationTagDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "zoneCard_LocationTagDetails",
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

(node as any).hash = "fff997dfedc2d46e0a247a8a8dead657";

export default node;
