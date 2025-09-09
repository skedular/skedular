/**
 * @generated SignedSource<<a4f75d6f55fa400c34bffbd8cae9dd34>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationBookingInsight_organizationAnalytics_refetchableFragment$variables = {
  from: any;
  organizationUniqueAlphanumericName?: string | null | undefined;
  to: any;
};
export type organizationBookingInsight_organizationAnalytics_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationBookingInsight_organizationAnalytics_query">;
};
export type organizationBookingInsight_organizationAnalytics_refetchableFragment = {
  response: organizationBookingInsight_organizationAnalytics_refetchableFragment$data;
  variables: organizationBookingInsight_organizationAnalytics_refetchableFragment$variables;
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
    "name": "organizationUniqueAlphanumericName"
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
    "name": "organizationBookingInsight_organizationAnalytics_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationBookingInsight_organizationAnalytics_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationBookingInsight_organizationAnalytics_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "uniqueAlphanumericName",
            "variableName": "organizationUniqueAlphanumericName"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
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
            "concreteType": "OrganizationAnalytics",
            "kind": "LinkedField",
            "name": "analytics",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationDailyBookingsTotal",
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
    "cacheID": "1573e50c4f010d4bd02c9889f69de458",
    "id": null,
    "metadata": {},
    "name": "organizationBookingInsight_organizationAnalytics_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationBookingInsight_organizationAnalytics_refetchableFragment(\n  $from: DateTime!\n  $organizationUniqueAlphanumericName: String\n  $to: DateTime!\n) {\n  ...organizationBookingInsight_organizationAnalytics_query\n}\n\nfragment organizationBookingInsight_organizationAnalytics_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    analytics(from: $from, until: $to) {\n      dailyBookingsTotals {\n        date\n        total\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "a0c6555f8b8a587e9ac9656458787b7c";

export default node;
