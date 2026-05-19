/**
 * @generated SignedSource<<f1d7bc020883845b0de7ae43cec63f87>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationBookingInsightRoot_rootQuery$variables = {
  from: any;
  locationId: string;
  to: any;
};
export type locationBookingInsightRoot_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationBookingInsight_locationAnalytics_query" | "locationBookingInsight_query">;
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
  "name": "locationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "to"
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "locationBookingInsightRoot_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationBookingInsight_query"
      },
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
    "argumentDefinitions": [
      (v1/*:: as any*/),
      (v0/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "locationBookingInsightRoot_rootQuery",
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
          (v3/*:: as any*/),
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
            "concreteType": "OrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              (v3/*:: as any*/)
            ],
            "storageKey": null
          },
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
    ]
  },
  "params": {
    "cacheID": "47649fb3a9b16a1545f4b4b58c45fc7c",
    "id": null,
    "metadata": {},
    "name": "locationBookingInsightRoot_rootQuery",
    "operationKind": "query",
    "text": "query locationBookingInsightRoot_rootQuery(\n  $locationId: String!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...locationBookingInsight_query\n  ...locationBookingInsight_locationAnalytics_query\n}\n\nfragment locationBookingInsight_locationAnalytics_query on Query {\n  location(id: $locationId) {\n    analytics(from: $from, until: $to) {\n      dailyBookingsTotals {\n        date\n        total\n      }\n    }\n    id\n  }\n}\n\nfragment locationBookingInsight_query on Query {\n  location(id: $locationId) {\n    id\n    name\n    organization {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "192dce347a057b79742eb68aa9d1d92f";

export default node;
