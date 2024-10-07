/**
 * @generated SignedSource<<417d3bd655240855794feb6d4290bd2e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationDeskOccupancyInsight_organizationAnalytics$variables = {
  from: any;
  locationExists: boolean;
  locationId: string;
  to: any;
};
export type locationDeskOccupancyInsight_organizationAnalytics$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationDeskOccupancyInsight_query">;
};
export type locationDeskOccupancyInsight_organizationAnalytics = {
  response: locationDeskOccupancyInsight_organizationAnalytics$data;
  variables: locationDeskOccupancyInsight_organizationAnalytics$variables;
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
    "name": "locationDeskOccupancyInsight_organizationAnalytics",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationDeskOccupancyInsight_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationDeskOccupancyInsight_organizationAnalytics",
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
    "cacheID": "333f6db8b9422f992b3406626cf9925a",
    "id": null,
    "metadata": {},
    "name": "locationDeskOccupancyInsight_organizationAnalytics",
    "operationKind": "query",
    "text": "query locationDeskOccupancyInsight_organizationAnalytics(\n  $from: DateTime!\n  $locationExists: Boolean!\n  $locationId: String!\n  $to: DateTime!\n) {\n  ...locationDeskOccupancyInsight_query\n}\n\nfragment locationDeskOccupancyInsight_query on Query {\n  locationAnalytics(locationId: $locationId, from: $from, until: $to) @include(if: $locationExists) {\n    desksOccupancyPercentage {\n      date\n      percentage\n    }\n  }\n  location(id: $locationId) {\n    name\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "a1cd322cb01675138ab490b2a246ec9b";

export default node;
