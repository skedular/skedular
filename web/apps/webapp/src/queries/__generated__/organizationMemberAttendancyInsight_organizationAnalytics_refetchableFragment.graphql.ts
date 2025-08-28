/**
 * @generated SignedSource<<b913bf03c6eae6bcea395144b49e9db1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment$variables = {
  from: any;
  organizationUniqueAlphanumericName?: string | null | undefined;
  to: any;
};
export type organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationMemberAttendancyInsight_organizationAnalytics_query">;
};
export type organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment = {
  response: organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment$data;
  variables: organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment$variables;
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
    "name": "organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment",
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
    "cacheID": "8b6db719e344a990f3f5d1ff30376902",
    "id": null,
    "metadata": {},
    "name": "organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment(\n  $from: DateTime!\n  $organizationUniqueAlphanumericName: String\n  $to: DateTime!\n) {\n  ...organizationMemberAttendancyInsight_organizationAnalytics_query\n}\n\nfragment organizationMemberAttendancyInsight_organizationAnalytics_query on Query {\n  organizationAnalytics(uniqueAlphanumericName: $organizationUniqueAlphanumericName, from: $from, until: $to) {\n    memberAttendancePercentage {\n      date\n      percentage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9c5f3bbefaa574927946ad36a42a2213";

export default node;
