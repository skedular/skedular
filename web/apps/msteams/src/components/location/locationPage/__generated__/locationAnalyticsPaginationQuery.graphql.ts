/**
 * @generated SignedSource<<b3a924ae4ebb5e77a67c4f7c204136d5>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationAnalyticsPaginationQuery$variables = {
  locationAnalyticsFrom: any;
  locationAnalyticsUntil: any;
  locationId: string;
};
export type locationAnalyticsPaginationQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationAnalyticsTab_query">;
};
export type locationAnalyticsPaginationQuery = {
  response: locationAnalyticsPaginationQuery$data;
  variables: locationAnalyticsPaginationQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationAnalyticsFrom"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationAnalyticsUntil"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "date",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "locationAnalyticsPaginationQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationAnalyticsTab_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationAnalyticsPaginationQuery",
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
              (v1/*: any*/),
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
              (v1/*: any*/),
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
  "params": {
    "cacheID": "94237fc05205aad8c876b5f4f33258e2",
    "id": null,
    "metadata": {},
    "name": "locationAnalyticsPaginationQuery",
    "operationKind": "query",
    "text": "query locationAnalyticsPaginationQuery(\n  $locationAnalyticsFrom: DateTime!\n  $locationAnalyticsUntil: DateTime!\n  $locationId: String!\n) {\n  ...locationAnalyticsTab_query\n}\n\nfragment locationAnalyticsTab_query on Query {\n  locationAnalytics(locationId: $locationId, from: $locationAnalyticsFrom, until: $locationAnalyticsUntil) {\n    desksOccupancyPercentage {\n      date\n      percentage\n    }\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "116f48eaba42df413a809570d470e272";

export default node;
