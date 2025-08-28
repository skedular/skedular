/**
 * @generated SignedSource<<23afedb7dccb3900f846bc11b92d737a>>
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
export type editPrivateBooking_customerTeams_refetchableFragment$variables = {
  customerExists: boolean;
  customerId: string;
  organizationUniqueAlphanumericName?: string | null | undefined;
  teamsSortingValues?: ReadonlyArray<TeamOrderInput> | null | undefined;
};
export type editPrivateBooking_customerTeams_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"editPrivateBooking_customerTeams_query">;
};
export type editPrivateBooking_customerTeams_refetchableFragment = {
  response: editPrivateBooking_customerTeams_refetchableFragment$data;
  variables: editPrivateBooking_customerTeams_refetchableFragment$variables;
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
    "name": "organizationUniqueAlphanumericName"
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
    "name": "editPrivateBooking_customerTeams_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editPrivateBooking_customerTeams_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editPrivateBooking_customerTeams_refetchableFragment",
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
                    "name": "organizationUniqueAlphanumericName",
                    "variableName": "organizationUniqueAlphanumericName"
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
    "cacheID": "0fa52acd6b9f99879ef7b888d484062e",
    "id": null,
    "metadata": {},
    "name": "editPrivateBooking_customerTeams_refetchableFragment",
    "operationKind": "query",
    "text": "query editPrivateBooking_customerTeams_refetchableFragment(\n  $customerExists: Boolean!\n  $customerId: String!\n  $organizationUniqueAlphanumericName: String\n  $teamsSortingValues: [TeamOrderInput!]\n) {\n  ...editPrivateBooking_customerTeams_query\n}\n\nfragment editPrivateBooking_customerTeams_query on Query {\n  customerTeams(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, customerId: $customerId}, orderBy: $teamsSortingValues) @include(if: $customerExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2143f626ee6718100496ab52c6eb9cb5";

export default node;
