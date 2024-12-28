/**
 * @generated SignedSource<<294633f64db94c97624be2ac7cd1570e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationMembershipType = "Administrator" | "Member" | "Owner" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationSingleChoiceMembershipType_query$data = {
  readonly organizationMembershipTypes: ReadonlyArray<OrganizationMembershipType>;
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
      "name": "organizationMembershipTypes",
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "f1abfa0a3721cb69389f51aaf7fdb72c";

export default node;
