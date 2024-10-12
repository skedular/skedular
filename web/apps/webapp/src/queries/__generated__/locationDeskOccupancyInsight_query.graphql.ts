/**
 * @generated SignedSource<<ca4091ac3ff58c53f98dddb4aaa1d4d9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationDeskOccupancyInsight_query$data = {
  readonly location: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentType": "locationDeskOccupancyInsight_query";
};
export type locationDeskOccupancyInsight_query$key = {
  readonly " $data"?: locationDeskOccupancyInsight_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationDeskOccupancyInsight_query">;
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
  "name": "locationDeskOccupancyInsight_query",
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
          "name": "name",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "a3fef42f518d0c2769cc38221bd2bef2";

export default node;
