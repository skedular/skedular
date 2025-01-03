/**
 * @generated SignedSource<<460a2c0328a65c5d768e343e7c42a2ff>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type LocationMemberRole = "Administrator" | "Member" | "Owner" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type locationSingleChoiceMemberRole_query$data = {
  readonly locationMemberRoles: ReadonlyArray<LocationMemberRole>;
  readonly " $fragmentType": "locationSingleChoiceMemberRole_query";
};
export type locationSingleChoiceMemberRole_query$key = {
  readonly " $data"?: locationSingleChoiceMemberRole_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationSingleChoiceMemberRole_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationSingleChoiceMemberRole_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "locationMemberRoles",
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "583206443931108b2d02c4fb6096f09a";

export default node;
