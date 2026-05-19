/**
 * @generated SignedSource<<f7f90b59f813b0ac1d4ac47b2982b3a3>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type addPrivateBookingPage_customerTeams_query$data = {
  readonly customerTeams?: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
      };
    }>;
  };
  readonly " $fragmentType": "addPrivateBookingPage_customerTeams_query";
};
export type addPrivateBookingPage_customerTeams_query$key = {
  readonly " $data"?: addPrivateBookingPage_customerTeams_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"addPrivateBookingPage_customerTeams_query">;
};

import addPrivateBookingPage_customerTeams_refetchableFragment_graphql from './addPrivateBookingPage_customerTeams_refetchableFragment.graphql';

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
      "name": "organizationCustomDomain"
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
      "operation": addPrivateBookingPage_customerTeams_refetchableFragment_graphql
    }
  },
  "name": "addPrivateBookingPage_customerTeams_query",
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
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "788d37e38babf02028772189f4a1999b";

export default node;
