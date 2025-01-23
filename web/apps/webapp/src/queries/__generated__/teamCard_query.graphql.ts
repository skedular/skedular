/**
 * @generated SignedSource<<b74a7fd6ea0541c9b4c034af763b8494>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamCard_query$data = {
  readonly me: {
    readonly defaultTeams: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentType": "teamCard_query";
};
export type teamCard_query$key = {
  readonly " $data"?: teamCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamCard_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "teamCard_query",
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "0f0786724d6cd8b909870321bbe9de02";

export default node;
