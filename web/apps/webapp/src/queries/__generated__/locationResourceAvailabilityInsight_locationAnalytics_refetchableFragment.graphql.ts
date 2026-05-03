/**
 * @generated SignedSource<<b6db0e8fd4d0fa0eb003fef68440901c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment$variables = {
  from: any;
  locationId: string;
  to: any;
};
export type locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationResourceAvailabilityInsight_locationAnalytics_query">;
};
export type locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment = {
  response: locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment$data;
  variables: locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment$variables;
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
    "name": "locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment",
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
    "cacheID": "bc1983cae34de9afde6e1c550a70184f",
    "id": null,
    "metadata": {},
    "name": "locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment",
    "operationKind": "query",
    "text": "query locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment(\n  $from: DateTime!\n  $locationId: String!\n  $to: DateTime!\n) {\n  ...locationResourceAvailabilityInsight_locationAnalytics_query\n}\n\nfragment locationResourceAvailabilityInsight_locationAnalytics_query on Query {\n  location(id: $locationId) {\n    analytics(from: $from, until: $to) {\n      resourceAvailabilitySnapshots {\n        date\n        resourceType\n        availableCount\n        unavailableCount\n        bookedCount\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "3ecf2a2efbb9c8ec24c4eadbda1f0472";

export default node;
