/**
 * @generated SignedSource<<322e666aecbd7f299c7bba1eb2f33081>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationBookingInsight_locationAnalytics_refetchableFragment$variables = {
  from: any;
  locationId: string;
  to: any;
};
export type locationBookingInsight_locationAnalytics_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationBookingInsight_locationAnalytics_query">;
};
export type locationBookingInsight_locationAnalytics_refetchableFragment = {
  response: locationBookingInsight_locationAnalytics_refetchableFragment$data;
  variables: locationBookingInsight_locationAnalytics_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "from"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "to"
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "locationBookingInsight_locationAnalytics_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationBookingInsight_locationAnalytics_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationBookingInsight_locationAnalytics_refetchableFragment",
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
  "params": {
    "cacheID": "4097ce04625f6a9830ae35a29e79fa3d",
    "id": null,
    "metadata": {},
    "name": "locationBookingInsight_locationAnalytics_refetchableFragment",
    "operationKind": "query",
    "text": "query locationBookingInsight_locationAnalytics_refetchableFragment(\n  $from: DateTime!\n  $locationId: String!\n  $to: DateTime!\n) {\n  ...locationBookingInsight_locationAnalytics_query\n}\n\nfragment locationBookingInsight_locationAnalytics_query on Query {\n  locationAnalytics(locationId: $locationId, from: $from, until: $to) {\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1a8a6a355221a1c718ea558a3ff49f4f";

export default node;
