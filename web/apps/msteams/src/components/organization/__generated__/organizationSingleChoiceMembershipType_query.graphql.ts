/**
 * @generated SignedSource<<dace581f4b0c4a11fcc8fe9dad45d627>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationMemberMembershipType = "Administrator" | "Member" | "Owner" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationSingleChoiceMembershipType_query$data = {
  readonly organizationMemberMembershipTypes: ReadonlyArray<OrganizationMemberMembershipType>;
  readonly " $fragmentType": "organizationSingleChoiceMembershipType_query";
};
export type organizationSingleChoiceMembershipType_query$key = {
  readonly " $data"?: organizationSingleChoiceMembershipType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationSingleChoiceMembershipType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationSingleChoiceMembershipType_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "organizationMemberMembershipTypes",
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "179cc423db8c3bb4ee6d7ca5d03d4654";

export default node;
