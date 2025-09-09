/**
 * @generated SignedSource<<cfb75800f76238b808bc4f5d4306cf5f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationDeskOccupancyInsight_locationAnalytics_query$data = {
  readonly location: {
    readonly analytics: {
      readonly desksOccupancyPercentage: ReadonlyArray<{
        readonly date: any;
        readonly percentage: number;
      }>;
    };
  } | null | undefined;
  readonly " $fragmentType": "locationDeskOccupancyInsight_locationAnalytics_query";
};
export type locationDeskOccupancyInsight_locationAnalytics_query$key = {
  readonly " $data"?: locationDeskOccupancyInsight_locationAnalytics_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationDeskOccupancyInsight_locationAnalytics_query">;
};

import locationDeskOccupancyInsight_locationAnalytics_refetchableFragment_graphql from './locationDeskOccupancyInsight_locationAnalytics_refetchableFragment.graphql';

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
      "operation": locationDeskOccupancyInsight_locationAnalytics_refetchableFragment_graphql
    }
  },
  "name": "locationDeskOccupancyInsight_locationAnalytics_query",
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
          "args": [
            {
              "kind": "Variable",
              "name": "from",
              "variableName": "from"
            },
            {
              "kind": "Variable",
              "name": "until",
              "variableName": "to"
            }
          ],
          "concreteType": "LocationAnalytics",
          "kind": "LinkedField",
          "name": "analytics",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "DesksOccupancyPercentage",
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
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "0fbcfcd225e53c4a4a346290e4bb67bc";

export default node;
