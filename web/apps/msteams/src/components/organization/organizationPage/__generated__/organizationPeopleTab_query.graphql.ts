/**
 * @generated SignedSource<<e3804aeae1637d684e98c359be45bd1f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationPeopleTab_query$data = {
  readonly organization: {
    readonly canInvitePeople: boolean;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationSingleChoiceMembershipType_query">;
  readonly " $fragmentType": "organizationPeopleTab_query";
};
export type organizationPeopleTab_query$key = {
  readonly " $data"?: organizationPeopleTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationPeopleTab_query">;
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
  "name": "organizationPeopleTab_query",
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

(node as any).hash = "b49c3aa321f191e67e69460df5f9b1fa";

export default node;
