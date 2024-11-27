/**
 * @generated SignedSource<<fe064855fef93179b77b762f6ab465b2>>
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
  readonly " $fragmentSpreads": FragmentRefs<"bulkNewDeskDialog_query" | "deskCard_query" | "deskMultipleChoicesZones_query" | "multipleChoicesDeskTypes_query" | "multipleChoicesZones_query" | "newDeskDialog_query">;
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
      "name": "multipleChoicesDeskTypes_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesZones_query"
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

(node as any).hash = "94e20f807640ca7739950ebd0f0fe726";

export default node;
