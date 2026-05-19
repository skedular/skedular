/**
 * @generated SignedSource<<d4fbc9a6198b1f9d0ef60f73cf9902cc>>
 * @lightSyntaxTransform
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
export type addPrivateBookingPage_customerTeams_refetchableFragment$variables = {
  customerExists: boolean;
  customerId: string;
  organizationCustomDomain?: string | null | undefined;
  teamsSortingValues?: ReadonlyArray<TeamOrderInput> | null | undefined;
};
export type addPrivateBookingPage_customerTeams_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"addPrivateBookingPage_customerTeams_query">;
};
export type addPrivateBookingPage_customerTeams_refetchableFragment = {
  response: addPrivateBookingPage_customerTeams_refetchableFragment$data;
  variables: addPrivateBookingPage_customerTeams_refetchableFragment$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addPrivateBookingPage_customerTeams_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "addPrivateBookingPage_customerTeams_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "addPrivateBookingPage_customerTeams_refetchableFragment",
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
              }
            ],
            "storageKey": null
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "bbe8426d94f6ddb8e86995381378cc04",
    "id": null,
    "metadata": {},
    "name": "addPrivateBookingPage_customerTeams_refetchableFragment",
    "operationKind": "query",
    "text": "query addPrivateBookingPage_customerTeams_refetchableFragment(\n  $customerExists: Boolean!\n  $customerId: String!\n  $organizationCustomDomain: String\n  $teamsSortingValues: [TeamOrderInput!]\n) {\n  ...addPrivateBookingPage_customerTeams_query\n}\n\nfragment addPrivateBookingPage_customerTeams_query on Query {\n  customerTeams(where: {organizationCustomDomain: $organizationCustomDomain, customerId: $customerId}, orderBy: $teamsSortingValues) @include(if: $customerExists) {\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "788d37e38babf02028772189f4a1999b";

export default node;
