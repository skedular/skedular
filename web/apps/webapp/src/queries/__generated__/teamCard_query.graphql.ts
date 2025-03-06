/**
 * @generated SignedSource<<a5750d782a0cf9c55c306b0cf2a051fa>>
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
    readonly id: string;
    readonly preferredTeams: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
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
          "name": "preferredTeams",
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

(node as any).hash = "e770a28ae90a2a46a43128474f9b6278";

export default node;
