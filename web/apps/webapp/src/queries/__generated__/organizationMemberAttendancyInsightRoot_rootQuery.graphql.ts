/**
 * @generated SignedSource<<56638ef3455d72a6d19b14c403dcd7b0>>
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
    "cacheID": "b92b41bf7b8b92353f16e7f0155aae1e",
    "id": null,
    "metadata": {},
    "name": "organizationMemberAttendancyInsightRoot_rootQuery",
    "operationKind": "query",
    "text": "query organizationMemberAttendancyInsightRoot_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...organizationMemberAttendancyInsight_organizationAnalytics_query\n}\n\nfragment organizationMemberAttendancyInsight_organizationAnalytics_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    analytics(from: $from, until: $to) {\n      memberAttendancePercentage {\n        date\n        percentage\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "b002c44f2887f80f746558e5e45238e5";

export default node;
