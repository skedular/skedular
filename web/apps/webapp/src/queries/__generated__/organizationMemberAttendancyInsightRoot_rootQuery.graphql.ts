/**
 * @generated SignedSource<<8a459a0eb65c68cbe24c1abb0808414e>>
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
  organizationId: string;
  to: any;
};
export type organizationMemberAttendancyInsightRoot_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationMemberAttendancyInsight_query">;
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
    "name": "organizationMemberAttendancyInsightRoot_rootQuery",
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
    "cacheID": "b97f83f0f035dcd4b91007e2a2dabcb4",
    "id": null,
    "metadata": {},
    "name": "organizationMemberAttendancyInsightRoot_rootQuery",
    "operationKind": "query",
    "text": "query organizationMemberAttendancyInsightRoot_rootQuery(\n  $organizationId: String!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...organizationMemberAttendancyInsight_query\n}\n\nfragment organizationMemberAttendancyInsight_query on Query {\n  organizationAnalytics(organizationId: $organizationId, from: $from, until: $to) {\n    memberAttendancePercentage {\n      date\n      percentage\n    }\n  }\n  organization(id: $organizationId) {\n    name\n    logoUrl\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "408c54f14ef1293cb908a0cfa7227b14";

export default node;
