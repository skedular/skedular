/**
 * @generated SignedSource<<f15dba15ff0b3e2f539d754076f95958>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment$variables = {
  from: any;
  organizationCustomDomain?: string | null | undefined;
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
    "name": "organizationCustomDomain"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "to"
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "customDomain",
            "variableName": "organizationCustomDomain"
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
    "cacheID": "d551cdb6cfc44613dc0d8603c51b5d36",
    "id": null,
    "metadata": {},
    "name": "organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment(\n  $from: DateTime!\n  $organizationCustomDomain: String\n  $to: DateTime!\n) {\n  ...organizationMemberAttendancyInsight_organizationAnalytics_query\n}\n\nfragment organizationMemberAttendancyInsight_organizationAnalytics_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    analytics(from: $from, until: $to) {\n      memberAttendancePercentage {\n        date\n        percentage\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "5f8d20b871e5221360f00ceee613b273";

export default node;
