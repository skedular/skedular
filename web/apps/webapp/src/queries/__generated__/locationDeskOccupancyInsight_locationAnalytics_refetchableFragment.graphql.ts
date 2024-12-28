/**
 * @generated SignedSource<<da0997d603e8f2d9ea131e367e94fdc5>>
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
  "params": {
    "cacheID": "a8f2a01a1bfd94971fb55130bdfd9f2b",
    "id": null,
    "metadata": {},
    "name": "locationDeskOccupancyInsight_locationAnalytics_refetchableFragment",
    "operationKind": "query",
    "text": "query locationDeskOccupancyInsight_locationAnalytics_refetchableFragment(\n  $from: DateTime!\n  $locationId: String!\n  $to: DateTime!\n) {\n  ...locationDeskOccupancyInsight_locationAnalytics_query\n}\n\nfragment locationDeskOccupancyInsight_locationAnalytics_query on Query {\n  locationAnalytics(locationId: $locationId, from: $from, until: $to) {\n    desksOccupancyPercentage {\n      date\n      percentage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ec5590930e3eb17f96cd8094253dab87";

export default node;
