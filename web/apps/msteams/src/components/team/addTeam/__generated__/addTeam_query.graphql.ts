/**
 * @generated SignedSource<<50aa422d9f119cf46e07085db0327096>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type addTeam_query$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMemberSelector_query">;
  readonly " $fragmentType": "addTeam_query";
};
export type addTeam_query$key = {
  readonly " $data"?: addTeam_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"addTeam_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "addTeam_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationMemberSelector_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "6c7222f2ce081b14ce858084cf7868fb";

export default node;
