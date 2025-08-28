/**
 * @generated SignedSource<<7fbc360c06b06412b5f991d35d5dc9b2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamSelector_allTeams_query$data = {
  readonly teams: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly " $fragmentType": "teamSelector_allTeams_query";
};
export type teamSelector_allTeams_query$key = {
  readonly " $data"?: teamSelector_allTeams_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamSelector_allTeams_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "teamSelector_allTeams_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "fields": [
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
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b81a2dc569d9fc9bcec90524aa3cd704";

export default node;
