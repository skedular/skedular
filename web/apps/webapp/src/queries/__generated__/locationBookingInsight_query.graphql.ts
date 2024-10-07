/**
 * @generated SignedSource<<6f6d6028ca2e30ed6a0452ff44f2ed6f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationBookingInsight_query$data = {
  readonly location: {
    readonly name: string;
  } | null | undefined;
  readonly locationAnalytics?: {
    readonly dailyBookingsTotals: ReadonlyArray<{
      readonly date: any;
      readonly total: number;
    }>;
  } | null | undefined;
  readonly " $fragmentType": "locationBookingInsight_query";
};
export type locationBookingInsight_query$key = {
  readonly " $data"?: locationBookingInsight_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationBookingInsight_query">;
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
      "operation": require('./locationBookingInsight_organizationAnalytics.graphql')
    }
  },
  "name": "locationBookingInsight_query",
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

(node as any).hash = "948373ee0a2f820b1cfacdeef37cfcda";

export default node;
