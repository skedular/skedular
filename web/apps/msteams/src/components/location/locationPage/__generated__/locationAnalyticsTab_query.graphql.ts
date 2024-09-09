/**
 * @generated SignedSource<<2b99d2b7300f75bde677291421661264>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationAnalyticsTab_query$data = {
  readonly locationAnalytics: {
    readonly dailyBookingsTotals: ReadonlyArray<{
      readonly date: any;
      readonly total: number;
    }>;
    readonly desksOccupancyPercentage: ReadonlyArray<{
      readonly date: any;
      readonly percentage: number;
    }>;
  };
  readonly " $fragmentType": "locationAnalyticsTab_query";
};
export type locationAnalyticsTab_query$key = {
  readonly " $data"?: locationAnalyticsTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationAnalyticsTab_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "date",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "locationAnalyticsFrom"
    },
    {
      "kind": "RootArgument",
      "name": "locationAnalyticsUntil"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./locationAnalyticsPaginationQuery.graphql')
    }
  },
  "name": "locationAnalyticsTab_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "from",
          "variableName": "locationAnalyticsFrom"
        },
        {
          "kind": "Variable",
          "name": "locationId",
          "variableName": "locationId"
        },
        {
          "kind": "Variable",
          "name": "until",
          "variableName": "locationAnalyticsUntil"
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
            (v0/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "percentage",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationDailyBookingsTotal",
          "kind": "LinkedField",
          "name": "dailyBookingsTotals",
          "plural": true,
          "selections": [
            (v0/*: any*/),
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
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "116f48eaba42df413a809570d470e272";

export default node;
