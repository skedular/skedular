/**
 * @generated SignedSource<<6fe02136a28a1b838702277f0510c66d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationDeskOccupancyInsightRoot_rootQuery$variables = {
  from: any;
  locationExists: boolean;
  locationId: string;
  to: any;
};
export type locationDeskOccupancyInsightRoot_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationDeskOccupancyInsight_locationAnalytics_query" | "locationDeskOccupancyInsight_query">;
};
export type locationDeskOccupancyInsightRoot_rootQuery = {
  response: locationDeskOccupancyInsightRoot_rootQuery$data;
  variables: locationDeskOccupancyInsightRoot_rootQuery$variables;
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
    "name": "locationDeskOccupancyInsightRoot_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationDeskOccupancyInsight_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationDeskOccupancyInsight_locationAnalytics_query"
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
    "name": "locationDeskOccupancyInsightRoot_rootQuery",
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
      },
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
                "concreteType": "LocationDesksOccupancyPercentage",
                "kind": "LinkedField",
                "name": "desksOccupancyPercentage",
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
                    "name": "percentage",
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
    "cacheID": "bf71d0bd2991fbe1902d0b8cabe1ae96",
    "id": null,
    "metadata": {},
    "name": "locationDeskOccupancyInsightRoot_rootQuery",
    "operationKind": "query",
    "text": "query locationDeskOccupancyInsightRoot_rootQuery(\n  $locationId: String!\n  $locationExists: Boolean!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...locationDeskOccupancyInsight_query\n  ...locationDeskOccupancyInsight_locationAnalytics_query\n}\n\nfragment locationDeskOccupancyInsight_locationAnalytics_query on Query {\n  locationAnalytics(locationId: $locationId, from: $from, until: $to) @include(if: $locationExists) {\n    desksOccupancyPercentage {\n      date\n      percentage\n    }\n  }\n}\n\nfragment locationDeskOccupancyInsight_query on Query {\n  location(id: $locationId) {\n    name\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "72d02e652e392a801f4c668b97d604ea";

export default node;
