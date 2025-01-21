/**
 * @generated SignedSource<<d1fc00f3afec4b98ad1b3c7eae937f86>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myTeams_query$data = {
  readonly me: {
    readonly defaultTeams: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"myTeamCard__query">;
  readonly " $fragmentType": "myTeams_query";
};
export type myTeams_query$key = {
  readonly " $data"?: myTeams_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myTeams_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "myTeams_query",
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
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "CustomerTeamDetails",
          "kind": "LinkedField",
          "name": "defaultTeams",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "uniqueId",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "myTeamCard__query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "1f8369d6d7ac1dc4747c70281c861539";

export default node;
