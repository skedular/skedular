/**
 * @generated SignedSource<<aef7c9185c582d3b0adfc8777c3f4c92>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationTeamsTab_query$data = {
  readonly organization: {
    readonly canModify: boolean;
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentType": "organizationTeamsTab_query";
};
export type organizationTeamsTab_query$key = {
  readonly " $data"?: organizationTeamsTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationTeamsTab_query">;
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
  "name": "organizationTeamsTab_query",
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
          "name": "canModify",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "0a3d95f8490272b6da6e6e0840d89d4b";

export default node;
