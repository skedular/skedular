/**
 * @generated SignedSource<<f5bfb82cf83cc09092af39a6537e0780>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationBookingInsight_organizationAnalytics$variables = {
  from: any;
  locationExists: boolean;
  locationId: string;
  to: any;
};
export type locationBookingInsight_organizationAnalytics$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationBookingInsight_query">;
};
export type locationBookingInsight_organizationAnalytics = {
  response: locationBookingInsight_organizationAnalytics$data;
  variables: locationBookingInsight_organizationAnalytics$variables;
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
    "name": "locationBookingInsight_organizationAnalytics",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationBookingInsight_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationBookingInsight_organizationAnalytics",
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
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "17a9142ffa2569536793bd07785e171b",
    "id": null,
    "metadata": {},
    "name": "locationBookingInsight_organizationAnalytics",
    "operationKind": "query",
    "text": "query locationBookingInsight_organizationAnalytics(\n  $from: DateTime!\n  $locationExists: Boolean!\n  $locationId: String!\n  $to: DateTime!\n) {\n  ...locationBookingInsight_query\n}\n\nfragment locationBookingInsight_query on Query {\n  locationAnalytics(locationId: $locationId, from: $from, until: $to) @include(if: $locationExists) {\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n  location(id: $locationId) {\n    name\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "948373ee0a2f820b1cfacdeef37cfcda";

export default node;
