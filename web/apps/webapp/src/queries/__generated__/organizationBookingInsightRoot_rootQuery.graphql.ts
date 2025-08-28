/**
 * @generated SignedSource<<a3faed8dededa7c68a0b4d0f364557af>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationBookingInsightRoot_rootQuery$variables = {
  from: any;
  organizationUniqueAlphanumericName: string;
  to: any;
};
export type organizationBookingInsightRoot_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationBookingInsight_organizationAnalytics_query">;
};
export type organizationBookingInsightRoot_rootQuery = {
  response: organizationBookingInsightRoot_rootQuery$data;
  variables: organizationBookingInsightRoot_rootQuery$variables;
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
  "name": "organizationUniqueAlphanumericName"
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
    "name": "organizationBookingInsightRoot_rootQuery",
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
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "organizationBookingInsightRoot_rootQuery",
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
    "cacheID": "d8a99a1a235d5b8ed1886d315eb9d11c",
    "id": null,
    "metadata": {},
    "name": "organizationBookingInsightRoot_rootQuery",
    "operationKind": "query",
    "text": "query organizationBookingInsightRoot_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...organizationBookingInsight_organizationAnalytics_query\n}\n\nfragment organizationBookingInsight_organizationAnalytics_query on Query {\n  organizationAnalytics(uniqueAlphanumericName: $organizationUniqueAlphanumericName, from: $from, until: $to) {\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ed1dfdc8cc66329d8d17577a03a636e2";

export default node;
