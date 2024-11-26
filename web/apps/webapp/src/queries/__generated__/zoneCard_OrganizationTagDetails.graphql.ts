/**
 * @generated SignedSource<<c9ce4ada9e7c072c1d1852a46e52bf9d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type zoneCard_OrganizationTagDetails$data = {
  readonly id: string;
  readonly name: string;
  readonly " $fragmentType": "zoneCard_OrganizationTagDetails";
};
export type zoneCard_OrganizationTagDetails$key = {
  readonly " $data"?: zoneCard_OrganizationTagDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"zoneCard_OrganizationTagDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "zoneCard_OrganizationTagDetails",
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
  "type": "OrganizationTagDetails",
  "abstractKey": null
};

(node as any).hash = "40a54602bec512aad41a25c9ff70e848";

export default node;
