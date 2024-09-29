/**
 * @generated SignedSource<<960b23d426a44c544dc3965e142a707c>>
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
  organizationId: string;
  to: any;
};
export type organizationBookingInsightRoot_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationBookingInsight_query">;
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
  "name": "organizationId"
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
        "name": "organizationBookingInsight_query"
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
    "cacheID": "e64104fea2b2cba256a4babcacd42aea",
    "id": null,
    "metadata": {},
    "name": "organizationBookingInsightRoot_rootQuery",
    "operationKind": "query",
    "text": "query organizationBookingInsightRoot_rootQuery(\n  $organizationId: String!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...organizationBookingInsight_query\n}\n\nfragment organizationBookingInsight_query on Query {\n  organizationAnalytics(organizationId: $organizationId, from: $from, until: $to) {\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n  organization(id: $organizationId) {\n    name\n    logoUrl\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "1d5d0517dbd9798ff99e340bde868855";

export default node;
