/**
 * @generated SignedSource<<8bcc46e07e85fd8a733bdb1ac1848b43>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationDeskOccupancyInsight_locationAnalytics_query$data = {
  readonly locationAnalytics: {
    readonly desksOccupancyPercentage: ReadonlyArray<{
      readonly date: any;
      readonly percentage: number;
    }>;
  } | null | undefined;
  readonly " $fragmentType": "locationDeskOccupancyInsight_locationAnalytics_query";
};
export type locationDeskOccupancyInsight_locationAnalytics_query$key = {
  readonly " $data"?: locationDeskOccupancyInsight_locationAnalytics_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationDeskOccupancyInsight_locationAnalytics_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "from"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "to"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./locationDeskOccupancyInsight_locationAnalytics_refetchableFragment.graphql')
    }
  },
  "name": "locationDeskOccupancyInsight_locationAnalytics_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "from",
          "variableName": "from"
        },
        {
          "kind": "Variable",
          "name": "locationId",
          "variableName": "locationId"
        },
        {
          "kind": "Variable",
          "name": "until",
          "variableName": "to"
        }
      ],
      "concreteType": "LocationAnalytics",
      "kind": "LinkedField",
      "name": "locationAnalytics",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationDesksOccupancyPercentage",
          "kind": "LinkedField",
          "name": "desksOccupancyPercentage",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "date",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "percentage",
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

(node as any).hash = "ec5590930e3eb17f96cd8094253dab87";

export default node;
