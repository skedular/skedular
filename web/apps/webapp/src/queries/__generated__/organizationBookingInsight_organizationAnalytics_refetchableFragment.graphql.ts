/**
 * @generated SignedSource<<d66847536c14867241dcf8955bd2538a>>
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
            "name": "from",
            "variableName": "from"
          },
          {
            "kind": "Variable",
            "name": "uniqueAlphanumericName",
            "variableName": "organizationUniqueAlphanumericName"
          },
          {
            "kind": "Variable",
            "name": "until",
            "variableName": "to"
          }
        ],
        "concreteType": "OrganizationAnalytics",
        "kind": "LinkedField",
        "name": "organizationAnalytics",
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
      }
    ]
  },
  "params": {
    "cacheID": "dd8cee476478b5374f98ae7a1c10ee8b",
    "id": null,
    "metadata": {},
    "name": "organizationBookingInsight_organizationAnalytics_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationBookingInsight_organizationAnalytics_refetchableFragment(\n  $from: DateTime!\n  $organizationUniqueAlphanumericName: String\n  $to: DateTime!\n) {\n  ...organizationBookingInsight_organizationAnalytics_query\n}\n\nfragment organizationBookingInsight_organizationAnalytics_query on Query {\n  organizationAnalytics(uniqueAlphanumericName: $organizationUniqueAlphanumericName, from: $from, until: $to) {\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b24c4a49ea23e76a5046a31237324d07";

export default node;
