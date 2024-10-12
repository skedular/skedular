/**
 * @generated SignedSource<<6715bf8878cd0fc385bac0e6ac84034b>>
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
  readonly " $fragmentSpreads": FragmentRefs<"bulkNewDeskDialog_query" | "deskCard_query" | "deskMultipleChoicesZones_query" | "newDeskDialog_query">;
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
      "name": "deskMultipleChoicesZones_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "newDeskDialog_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bulkNewDeskDialog_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "d6c98a354b64795f8f46095d069ce95e";

export default node;
