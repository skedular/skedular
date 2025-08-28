/**
 * @generated SignedSource<<41524884b00f439a361f311f6df56b3c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMemberAttendancyInsightRoot_rootQuery$variables = {
  from: any;
  organizationUniqueAlphanumericName: string;
  to: any;
};
export type organizationMemberAttendancyInsightRoot_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationMemberAttendancyInsight_organizationAnalytics_query">;
};
export type organizationMemberAttendancyInsightRoot_rootQuery = {
  response: organizationMemberAttendancyInsightRoot_rootQuery$data;
  variables: organizationMemberAttendancyInsightRoot_rootQuery$variables;
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
    "name": "organizationMemberAttendancyInsightRoot_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMemberAttendancyInsight_organizationAnalytics_query"
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
    "name": "organizationMemberAttendancyInsightRoot_rootQuery",
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
            "concreteType": "OrganizationMemberAttendancePercentage",
            "kind": "LinkedField",
            "name": "memberAttendancePercentage",
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
    "cacheID": "1537a474b0e145729423afd236e21d08",
    "id": null,
    "metadata": {},
    "name": "organizationMemberAttendancyInsightRoot_rootQuery",
    "operationKind": "query",
    "text": "query organizationMemberAttendancyInsightRoot_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...organizationMemberAttendancyInsight_organizationAnalytics_query\n}\n\nfragment organizationMemberAttendancyInsight_organizationAnalytics_query on Query {\n  organizationAnalytics(uniqueAlphanumericName: $organizationUniqueAlphanumericName, from: $from, until: $to) {\n    memberAttendancePercentage {\n      date\n      percentage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b002c44f2887f80f746558e5e45238e5";

export default node;
