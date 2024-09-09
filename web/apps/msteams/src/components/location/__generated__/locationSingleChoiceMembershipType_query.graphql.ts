/**
 * @generated SignedSource<<10630c9aad40b04735a157a660d5f45f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type LocationMemberMembershipType = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type locationSingleChoiceMembershipType_query$data = {
  readonly locationMemberMembershipTypes: ReadonlyArray<LocationMemberMembershipType>;
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
      "name": "locationMemberMembershipTypes",
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b00b52649779116118aea1936b49f20a";

export default node;
