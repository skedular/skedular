/**
 * @generated SignedSource<<e4849676bf6644ccdcd4d729e2cce18f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myTeamCard__query$data = {
  readonly me: {
    readonly defaultTeams: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentType": "myTeamCard__query";
};
export type myTeamCard__query$key = {
  readonly " $data"?: myTeamCard__query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myTeamCard__query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "myTeamCard__query",
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

(node as any).hash = "c728aa11bfb815c9e2e8afc22dbc645d";

export default node;
