/**
 * @generated SignedSource<<0239df2afd61c20147a90672d8dad0e4>>
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
  readonly locationAnalytics?: {
    readonly desksOccupancyPercentage: ReadonlyArray<{
      readonly date: any;
      readonly percentage: number;
    }>;
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
      "name": "from"
    },
    {
      "kind": "RootArgument",
      "name": "locationExists"
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
      "operation": require('./locationDeskOccupancyInsight_organizationAnalytics.graphql')
    }
  },
  "name": "locationDeskOccupancyInsight_query",
  "selections": [
    {
      "condition": "locationExists",
      "kind": "Condition",
      "passingValue": true,
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
      ]
    },
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

(node as any).hash = "a1cd322cb01675138ab490b2a246ec9b";

export default node;
