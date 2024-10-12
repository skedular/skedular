/**
 * @generated SignedSource<<873dbe2558f159c431e148e026459be7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationDeskOccupancyInsight_locationAnalytics_refetchableFragment$variables = {
  from: any;
  locationExists: boolean;
  locationId: string;
  to: any;
};
export type locationDeskOccupancyInsight_locationAnalytics_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationDeskOccupancyInsight_locationAnalytics_query">;
};
export type locationDeskOccupancyInsight_locationAnalytics_refetchableFragment = {
  response: locationDeskOccupancyInsight_locationAnalytics_refetchableFragment$data;
  variables: locationDeskOccupancyInsight_locationAnalytics_refetchableFragment$variables;
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
    "name": "locationDeskOccupancyInsight_locationAnalytics_refetchableFragment",
    "selections": [
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationDeskOccupancyInsight_locationAnalytics_refetchableFragment",
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
      }
    ]
  },
  "params": {
    "cacheID": "5b65fec03993a49d43cd9894645c3108",
    "id": null,
    "metadata": {},
    "name": "locationDeskOccupancyInsight_locationAnalytics_refetchableFragment",
    "operationKind": "query",
    "text": "query locationDeskOccupancyInsight_locationAnalytics_refetchableFragment(\n  $from: DateTime!\n  $locationExists: Boolean!\n  $locationId: String!\n  $to: DateTime!\n) {\n  ...locationDeskOccupancyInsight_locationAnalytics_query\n}\n\nfragment locationDeskOccupancyInsight_locationAnalytics_query on Query {\n  locationAnalytics(locationId: $locationId, from: $from, until: $to) @include(if: $locationExists) {\n    desksOccupancyPercentage {\n      date\n      percentage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2d5210f0bc90e9268b2dcd5ac5feb420";

export default node;
