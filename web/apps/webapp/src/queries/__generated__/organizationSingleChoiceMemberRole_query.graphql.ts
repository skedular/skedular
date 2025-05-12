/**
 * @generated SignedSource<<cb509d24800e3d779184f24fdc2f3153>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
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
