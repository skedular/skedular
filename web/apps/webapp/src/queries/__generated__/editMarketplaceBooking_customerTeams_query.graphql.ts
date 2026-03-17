/**
 * @generated SignedSource<<5221b518e6baf3b0e533f9abc2524e4e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editMarketplaceBooking_customerTeams_query$data = {
  readonly customerTeams?: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentType": "editMarketplaceBooking_customerTeams_query";
};
export type editMarketplaceBooking_customerTeams_query$key = {
  readonly " $data"?: editMarketplaceBooking_customerTeams_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editMarketplaceBooking_customerTeams_query">;
};

import editMarketplaceBooking_customerTeams_refetchableFragment_graphql from './editMarketplaceBooking_customerTeams_refetchableFragment.graphql';

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
      "operation": editMarketplaceBooking_customerTeams_refetchableFragment_graphql
    }
  },
  "name": "editMarketplaceBooking_customerTeams_query",
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
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "462b9f17f4769ed3a3dcc3fb4ed88dda";

export default node;
