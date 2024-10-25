/**
 * @generated SignedSource<<a5b2d984e9ac6d646705fae56f81f90b>>
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
  readonly " $fragmentSpreads": FragmentRefs<"organizationSingleChoiceMembershipType_query">;
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
      "name": "organizationSingleChoiceMembershipType_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "a669a1d59b2e28759dd88271fae5ddd7";

export default node;
