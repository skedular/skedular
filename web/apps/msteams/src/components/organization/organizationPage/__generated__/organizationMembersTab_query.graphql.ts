/**
 * @generated SignedSource<<11e61691dc34e2143aede9ccda3658ca>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMembersTab_query$data = {
  readonly organization: {
    readonly canInvitePeople: boolean;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationSingleChoiceMemberRole_query">;
  readonly " $fragmentType": "organizationMembersTab_query";
};
export type organizationMembersTab_query$key = {
  readonly " $data"?: organizationMembersTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMembersTab_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationMembersTab_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
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
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canInvitePeople",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationSingleChoiceMemberRole_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "90eb4e7020f05854f5df50aeca26c28c";

export default node;
