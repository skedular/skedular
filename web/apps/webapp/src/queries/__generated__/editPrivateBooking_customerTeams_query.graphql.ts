/**
 * @generated SignedSource<<3b916b6b13833e8fa1b7233fe726580c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editPrivateBooking_customerTeams_query$data = {
  readonly customerTeams?: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly " $fragmentType": "editPrivateBooking_customerTeams_query";
};
export type editPrivateBooking_customerTeams_query$key = {
  readonly " $data"?: editPrivateBooking_customerTeams_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editPrivateBooking_customerTeams_query">;
};

import editPrivateBooking_customerTeams_refetchableFragment_graphql from './editPrivateBooking_customerTeams_refetchableFragment.graphql';

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "customerExists"
    },
    {
      "kind": "RootArgument",
      "name": "customerId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    },
    {
      "kind": "RootArgument",
      "name": "teamsSortingValues"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": editPrivateBooking_customerTeams_refetchableFragment_graphql
    }
  },
  "name": "editPrivateBooking_customerTeams_query",
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
                  "name": "organizationId",
                  "variableName": "organizationId"
                }
              ],
              "kind": "ObjectValue",
              "name": "where"
            }
          ],
          "concreteType": "TeamConnection",
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
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "34f614f85a794aff3a398e819ab7a007";

export default node;
