/**
 * @generated SignedSource<<388e09097a448ac6f57412765ae854b7>>
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
  readonly organizationMemberRoles: ReadonlyArray<{
    readonly name: string;
    readonly type: OrganizationMemberRole;
  }>;
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
      "concreteType": "OrganizationMemberRoleDetails",
      "kind": "LinkedField",
      "name": "organizationMemberRoles",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
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
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "c206dac27e239b1d890fe339e84c3665";

export default node;
