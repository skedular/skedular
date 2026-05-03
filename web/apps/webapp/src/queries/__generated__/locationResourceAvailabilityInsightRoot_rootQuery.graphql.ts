/**
 * @generated SignedSource<<9ee26d8e8cdcaec0b1639d5a36f3f0fe>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationResourceAvailabilityInsightRoot_rootQuery$variables = {
  from: any;
  locationId: string;
  to: any;
};
export type locationResourceAvailabilityInsightRoot_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationResourceAvailabilityInsight_locationAnalytics_query">;
};
export type locationResourceAvailabilityInsightRoot_rootQuery = {
  response: locationResourceAvailabilityInsightRoot_rootQuery$data;
  variables: locationResourceAvailabilityInsightRoot_rootQuery$variables;
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
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "locationResourceAvailabilityInsightRoot_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationResourceAvailabilityInsight_locationAnalytics_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "locationResourceAvailabilityInsightRoot_rootQuery",
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
                "concreteType": "ResourceAvailabilityDailySnapshot",
                "kind": "LinkedField",
                "name": "resourceAvailabilitySnapshots",
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
                    "name": "resourceType",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "availableCount",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "unavailableCount",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "bookedCount",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
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
    "cacheID": "2991a668646acf3f68933eb50c2d41d9",
    "id": null,
    "metadata": {},
    "name": "locationResourceAvailabilityInsightRoot_rootQuery",
    "operationKind": "query",
    "text": "query locationResourceAvailabilityInsightRoot_rootQuery(\n  $locationId: String!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...locationResourceAvailabilityInsight_locationAnalytics_query\n}\n\nfragment locationResourceAvailabilityInsight_locationAnalytics_query on Query {\n  location(id: $locationId) {\n    analytics(from: $from, until: $to) {\n      resourceAvailabilitySnapshots {\n        date\n        resourceType\n        availableCount\n        unavailableCount\n        bookedCount\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "83a623c4f133049f0312649dffc8a2fe";

export default node;
