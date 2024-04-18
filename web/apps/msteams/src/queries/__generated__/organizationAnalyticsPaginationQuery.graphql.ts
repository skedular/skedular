/**
 * @generated SignedSource<<18670a68c65c50d41ccef10842b09f78>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Query } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationAnalyticsPaginationQuery$variables = {
  organizationAnalyticsFrom: any;
  organizationAnalyticsUntil: any;
  organizationId: string;
};
export type organizationAnalyticsPaginationQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationAnalyticsTab_query">;
};
export type organizationAnalyticsPaginationQuery = {
  response: organizationAnalyticsPaginationQuery$data;
  variables: organizationAnalyticsPaginationQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationAnalyticsFrom"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationAnalyticsUntil"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "date",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAnalyticsPaginationQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAnalyticsTab_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAnalyticsPaginationQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "from",
            "variableName": "organizationAnalyticsFrom"
          },
          {
            "kind": "Variable",
            "name": "organizationId",
            "variableName": "organizationId"
          },
          {
            "kind": "Variable",
            "name": "until",
            "variableName": "organizationAnalyticsUntil"
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
            "concreteType": "OrganizationMemberAttendancePercentage",
            "kind": "LinkedField",
            "name": "memberAttendancePercentage",
            "plural": true,
            "selections": [
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "percentage",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationDailyBookingsTotal",
            "kind": "LinkedField",
            "name": "dailyBookingsTotals",
            "plural": true,
            "selections": [
              (v1/*: any*/),
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
    "cacheID": "4db100687665553ff84afbbc3e971bf6",
    "id": null,
    "metadata": {},
    "name": "organizationAnalyticsPaginationQuery",
    "operationKind": "query",
    "text": "query organizationAnalyticsPaginationQuery(\n  $organizationAnalyticsFrom: DateTime!\n  $organizationAnalyticsUntil: DateTime!\n  $organizationId: String!\n) {\n  ...organizationAnalyticsTab_query\n}\n\nfragment organizationAnalyticsTab_query on Query {\n  organizationAnalytics(organizationId: $organizationId, from: $organizationAnalyticsFrom, until: $organizationAnalyticsUntil) {\n    memberAttendancePercentage {\n      date\n      percentage\n    }\n    dailyBookingsTotals {\n      date\n      total\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e3be4c7a5acfb28d20d245e6f248fa49";

export default node;
