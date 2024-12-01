/**
 * @generated SignedSource<<ade75dc01e71b592725645bf3f9342db>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type TeamOrderField = "About" | "Name" | "%future added value";
export type TeamOrderInput = {
  direction: OrderDirection;
  field: TeamOrderField;
};
export type organizationTeamsTab_rootQuery$variables = {
  organizationId: string;
  organizationTeamsSortingValues: ReadonlyArray<TeamOrderInput>;
  teamNameSearchText?: string | null | undefined;
};
export type organizationTeamsTab_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationTeamsTab_teams_query">;
};
export type organizationTeamsTab_rootQuery = {
  response: organizationTeamsTab_rootQuery$data;
  variables: organizationTeamsTab_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationTeamsSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "teamNameSearchText"
  }
],
v1 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationTeamsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "teamNameSearchText"
      },
      {
        "kind": "Variable",
        "name": "organizationId",
        "variableName": "organizationId"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationTeamsTab_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationTeamsTab_teams_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeamsTab_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamConnection",
        "kind": "LinkedField",
        "name": "teams",
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
                  (v2/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "uniqueId",
                        "storageKey": null
                      },
                      (v2/*: any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "__typename",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "cursor",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "PageInfo",
            "kind": "LinkedField",
            "name": "pageInfo",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "endCursor",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "hasNextPage",
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
      },
      {
        "alias": null,
        "args": (v1/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationTeamsTab_teams",
        "kind": "LinkedHandle",
        "name": "teams"
      }
    ]
  },
  "params": {
    "cacheID": "fe99995301a5fea3a0e7d4ba76f1a38c",
    "id": null,
    "metadata": {},
    "name": "organizationTeamsTab_rootQuery",
    "operationKind": "query",
    "text": "query organizationTeamsTab_rootQuery(\n  $organizationId: String!\n  $organizationTeamsSortingValues: [TeamOrderInput!]!\n  $teamNameSearchText: String\n) {\n  ...organizationTeamsTab_teams_query\n}\n\nfragment organizationTeamsTab_teams_query on Query {\n  teams(first: 50, where: {organizationId: $organizationId, nameContains: $teamNameSearchText}, orderBy: $organizationTeamsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        organization {\n          uniqueId\n          name\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1dd154ec91d4f435eec36624497c492f";

export default node;
