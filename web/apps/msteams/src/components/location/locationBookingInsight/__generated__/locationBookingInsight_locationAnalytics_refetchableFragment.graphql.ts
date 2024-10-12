/**
 * @generated SignedSource<<3d0e3358d542016120d87e75e2dea6d5>>
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
  locationExists: boolean;
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
    "name": "locationExists"
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
      }
    ]
  },
  "params": {
    "cacheID": "1164c7fe92bf756240e7a1786555f04c",
    "id": null,
    "metadata": {},
    "name": "locationBookingInsight_locationAnalytics_refetchableFragment",
    "operationKind": "query",
    "text": "query locationBookingInsight_locationAnalytics_refetchableFragment(\n  $from: DateTime!\n  $locationExists: Boolean!\n  $locationId: String!\n  $to: DateTime!\n) {\n  ...locationBookingInsight_locationAnalytics_query\n}\n\nfragment locationBookingInsight_locationAnalytics_query on Query {\n  locationAnalytics(locationId: $locationId, from: $from, until: $to) @include(if: $locationExists) {\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "02ef4aab5cda85d2e02617bd450be88f";

export default node;
