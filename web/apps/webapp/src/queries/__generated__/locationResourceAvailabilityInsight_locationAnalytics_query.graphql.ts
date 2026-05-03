/**
 * @generated SignedSource<<98e0818e0b9949b532f240a4861c3e59>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationResourceAvailabilityInsight_locationAnalytics_query$data = {
  readonly location: {
    readonly analytics: {
      readonly resourceAvailabilitySnapshots: ReadonlyArray<{
        readonly availableCount: number;
        readonly bookedCount: number;
        readonly date: any;
        readonly resourceType: string;
        readonly unavailableCount: number;
      }>;
    };
  } | null | undefined;
  readonly " $fragmentType": "locationResourceAvailabilityInsight_locationAnalytics_query";
};
export type locationResourceAvailabilityInsight_locationAnalytics_query$key = {
  readonly " $data"?: locationResourceAvailabilityInsight_locationAnalytics_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationResourceAvailabilityInsight_locationAnalytics_query">;
};

import locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment_graphql from './locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment.graphql';

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
      "operation": locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment_graphql
    }
  },
  "name": "locationResourceAvailabilityInsight_locationAnalytics_query",
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
              "concreteType": "ResourceAvailabilityDailySnapshot",
              "kind": "LinkedField",
              "name": "resourceAvailabilitySnapshots",
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
                  "name": "resourceType",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "availableCount",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "unavailableCount",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "bookedCount",
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

(node as any).hash = "3ecf2a2efbb9c8ec24c4eadbda1f0472";

export default node;
