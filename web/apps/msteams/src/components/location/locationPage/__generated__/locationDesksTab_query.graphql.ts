/**
 * @generated SignedSource<<b32e7133ece4e0c16cec90173ac6ec10>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationDesksTab_query$data = {
  readonly location: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"deskCard_query" | "multipleChoicesCustomTags_query" | "multipleChoicesZones_query">;
  readonly " $fragmentType": "locationDesksTab_query";
};
export type locationDesksTab_query$key = {
  readonly " $data"?: locationDesksTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationDesksTab_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "locationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationDesksTab_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "locationId"
        }
      ],
      "concreteType": "LocationDetails",
      "kind": "LinkedField",
      "name": "location",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canModify",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "deskCard_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesCustomTags_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesZones_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "c7a5a0ccce3282316f3c6d30c974f7f3";

export default node;
