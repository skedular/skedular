/**
 * @generated SignedSource<<132aff8edf526a7129fe2aec39bcc308>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationBookingInsight_locationAnalytics_query$data = {
  readonly location: {
    readonly analytics: {
      readonly dailyBookingsTotals: ReadonlyArray<{
        readonly date: any;
        readonly total: number;
      }>;
    };
  } | null | undefined;
  readonly " $fragmentType": "locationBookingInsight_locationAnalytics_query";
};
export type locationBookingInsight_locationAnalytics_query$key = {
  readonly " $data"?: locationBookingInsight_locationAnalytics_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationBookingInsight_locationAnalytics_query">;
};

import locationBookingInsight_locationAnalytics_refetchableFragment_graphql from './locationBookingInsight_locationAnalytics_refetchableFragment.graphql';

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
      "operation": locationBookingInsight_locationAnalytics_refetchableFragment_graphql
    }
  },
  "name": "locationBookingInsight_locationAnalytics_query",
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
              "concreteType": "LocationDailyBookingsTotal",
              "kind": "LinkedField",
              "name": "dailyBookingsTotals",
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
                  "name": "total",
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

(node as any).hash = "4430d77b8f91b4b831e5febf5b80bafd";

export default node;
