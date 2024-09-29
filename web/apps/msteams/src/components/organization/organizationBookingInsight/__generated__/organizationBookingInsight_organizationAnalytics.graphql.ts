/**
 * @generated SignedSource<<60d9217257a22150931fc32167cea1e8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationBookingInsight_organizationAnalytics$variables = {
  from: any;
  organizationId: string;
  to: any;
};
export type organizationBookingInsight_organizationAnalytics$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationBookingInsight_query">;
};
export type organizationBookingInsight_organizationAnalytics = {
  response: organizationBookingInsight_organizationAnalytics$data;
  variables: organizationBookingInsight_organizationAnalytics$variables;
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
    "name": "organizationId"
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
    "name": "organizationBookingInsight_organizationAnalytics",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationBookingInsight_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationBookingInsight_organizationAnalytics",
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
            "name": "organizationId",
            "variableName": "organizationId"
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
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "organizationId"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
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
            "name": "logoUrl",
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
    "cacheID": "46de8c599bbaa26a621b2f815fbc23e1",
    "id": null,
    "metadata": {},
    "name": "organizationBookingInsight_organizationAnalytics",
    "operationKind": "query",
    "text": "query organizationBookingInsight_organizationAnalytics(\n  $from: DateTime!\n  $organizationId: String!\n  $to: DateTime!\n) {\n  ...organizationBookingInsight_query\n}\n\nfragment organizationBookingInsight_query on Query {\n  organizationAnalytics(organizationId: $organizationId, from: $from, until: $to) {\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n  organization(id: $organizationId) {\n    name\n    logoUrl\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "94b88194e3b5185712c1ea22a327415c";

export default node;
