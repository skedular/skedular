/**
 * @generated SignedSource<<e199475de7cf5ac6ebe704155962a685>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMemberAttendancyInsight_organizationAnalytics$variables = {
  from: any;
  organizationId: string;
  to: any;
};
export type organizationMemberAttendancyInsight_organizationAnalytics$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationMemberAttendancyInsight_query">;
};
export type organizationMemberAttendancyInsight_organizationAnalytics = {
  response: organizationMemberAttendancyInsight_organizationAnalytics$data;
  variables: organizationMemberAttendancyInsight_organizationAnalytics$variables;
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
    "name": "organizationMemberAttendancyInsight_organizationAnalytics",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMemberAttendancyInsight_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMemberAttendancyInsight_organizationAnalytics",
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
    "cacheID": "f0a2683b30dbdf9b4fc0e9b9fa265471",
    "id": null,
    "metadata": {},
    "name": "organizationMemberAttendancyInsight_organizationAnalytics",
    "operationKind": "query",
    "text": "query organizationMemberAttendancyInsight_organizationAnalytics(\n  $from: DateTime!\n  $organizationId: String!\n  $to: DateTime!\n) {\n  ...organizationMemberAttendancyInsight_query\n}\n\nfragment organizationMemberAttendancyInsight_query on Query {\n  organizationAnalytics(organizationId: $organizationId, from: $from, until: $to) {\n    memberAttendancePercentage {\n      date\n      percentage\n    }\n  }\n  organization(id: $organizationId) {\n    name\n    logoUrl\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "c9995957b38909124e4f224564f9b0d9";

export default node;
