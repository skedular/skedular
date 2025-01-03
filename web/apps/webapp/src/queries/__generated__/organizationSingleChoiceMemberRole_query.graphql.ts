/**
 * @generated SignedSource<<8e7e1b0f18e171b889ca932877b24d61>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationMemberRole = "Administrator" | "Member" | "Owner" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationSingleChoiceMemberRole_query$data = {
  readonly organizationMemberRoles: ReadonlyArray<OrganizationMemberRole>;
  readonly " $fragmentType": "organizationSingleChoiceMemberRole_query";
};
export type organizationSingleChoiceMemberRole_query$key = {
  readonly " $data"?: organizationSingleChoiceMemberRole_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationSingleChoiceMemberRole_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationSingleChoiceMemberRole_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "organizationMemberRoles",
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "78245e0aae18afda39e6a7c20f88bb00";

export default node;
