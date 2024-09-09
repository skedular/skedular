/**
 * @generated SignedSource<<85a7304981ad563ddfbe296e3b5a36fb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamPage_query$data = {
  readonly team: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"teamAboutTab_query" | "teamBookingsTab_query" | "teamPeopleTab_query">;
  readonly " $fragmentType": "teamPage_query";
};
export type teamPage_query$key = {
  readonly " $data"?: teamPage_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamPage_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "teamId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "teamPage_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "teamId"
        }
      ],
      "concreteType": "TeamDetails",
      "kind": "LinkedField",
      "name": "team",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "name",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "teamBookingsTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "teamAboutTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "teamPeopleTab_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "d8f429749c31d6afeacba7c106fa73d5";

export default node;
