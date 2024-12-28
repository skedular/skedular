/**
 * @generated SignedSource<<5688dc705271a3d66580bcbd9b3d0f05>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type LocationMembershipType = "Administrator" | "Member" | "Owner" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type locationSingleChoiceMembershipType_query$data = {
  readonly locationMembershipTypes: ReadonlyArray<LocationMembershipType>;
  readonly " $fragmentType": "locationSingleChoiceMembershipType_query";
};
export type locationSingleChoiceMembershipType_query$key = {
  readonly " $data"?: locationSingleChoiceMembershipType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationSingleChoiceMembershipType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationSingleChoiceMembershipType_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "locationMembershipTypes",
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "6fd3e375b4d447775aee1977e28524a6";

export default node;
