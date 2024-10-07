/**
 * @generated SignedSource<<7ef1936e485c3573fd197b91dc7fb0cd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationBookingInsightRoot_rootQuery$variables = {
  from: any;
  locationExists: boolean;
  locationId: string;
  to: any;
};
export type locationBookingInsightRoot_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationBookingInsight_query">;
};
export type locationBookingInsightRoot_rootQuery = {
  response: locationBookingInsightRoot_rootQuery$data;
  variables: locationBookingInsightRoot_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "from"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationExists"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "to"
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "locationBookingInsightRoot_rootQuery",
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
    "argumentDefinitions": [
      (v2/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Operation",
    "name": "locationBookingInsightRoot_rootQuery",
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
    "cacheID": "ce0ff10dd8974f03ad58b2cb8642a4ca",
    "id": null,
    "metadata": {},
    "name": "locationBookingInsightRoot_rootQuery",
    "operationKind": "query",
    "text": "query locationBookingInsightRoot_rootQuery(\n  $locationId: String!\n  $locationExists: Boolean!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...locationBookingInsight_query\n}\n\nfragment locationBookingInsight_query on Query {\n  locationAnalytics(locationId: $locationId, from: $from, until: $to) @include(if: $locationExists) {\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n  location(id: $locationId) {\n    name\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "f748f4f91267604a3cbe61dd14c06379";

export default node;
