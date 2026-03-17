/**
 * @generated SignedSource<<f3103495b81d6539c0b291b0d7335aa2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type TeamOrderField = "ABOUT" | "NAME" | "%future added value";
export type TeamOrderInput = {
  direction: OrderDirection;
  field: TeamOrderField;
};
export type newBookingDialog_customerTeams_refetchableFragment$variables = {
  customerExists: boolean;
  customerId: string;
  organizationCustomDomain?: string | null | undefined;
  teamsSortingValues?: ReadonlyArray<TeamOrderInput> | null | undefined;
};
export type newBookingDialog_customerTeams_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"newBookingDialog_customerTeams_query">;
};
export type newBookingDialog_customerTeams_refetchableFragment = {
  response: newBookingDialog_customerTeams_refetchableFragment$data;
  variables: newBookingDialog_customerTeams_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "customerExists"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "customerId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "teamsSortingValues"
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "newBookingDialog_customerTeams_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newBookingDialog_customerTeams_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "newBookingDialog_customerTeams_refetchableFragment",
    "selections": [
      {
        "condition": "customerExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "kind": "Variable",
                "name": "orderBy",
                "variableName": "teamsSortingValues"
              },
              {
                "fields": [
                  {
                    "kind": "Variable",
                    "name": "customerId",
                    "variableName": "customerId"
                  },
                  {
                    "kind": "Variable",
                    "name": "organizationCustomDomain",
                    "variableName": "organizationCustomDomain"
                  }
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "ConnectionOfTeamEdge",
            "kind": "LinkedField",
            "name": "customerTeams",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "totalCount",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "id",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "name",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "kind": "ClientExtension",
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "__id",
                    "storageKey": null
                  }
                ]
              }
            ],
            "storageKey": null
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "caad89d309ff1c3c9c8ac9d78b800615",
    "id": null,
    "metadata": {},
    "name": "newBookingDialog_customerTeams_refetchableFragment",
    "operationKind": "query",
    "text": "query newBookingDialog_customerTeams_refetchableFragment(\n  $customerExists: Boolean!\n  $customerId: String!\n  $organizationCustomDomain: String\n  $teamsSortingValues: [TeamOrderInput!]\n) {\n  ...newBookingDialog_customerTeams_query\n}\n\nfragment newBookingDialog_customerTeams_query on Query {\n  customerTeams(where: {organizationCustomDomain: $organizationCustomDomain, customerId: $customerId}, orderBy: $teamsSortingValues) @include(if: $customerExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9cb3ca5a0c2e392368adbfae0cce88ee";

export default node;
