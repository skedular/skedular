/**
 * @generated SignedSource<<4ddb2115305e1f27e1fd4a9cbb7c719a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myLocations_locations_query$data = {
  readonly locations: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "myLocations_locations_query";
};
export type myLocations_locations_query$key = {
  readonly " $data"?: myLocations_locations_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myLocations_locations_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "locationsSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./myLocations_locations_refetchableFragment.graphql')
    }
  },
  "name": "myLocations_locations_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "locationsSortingValues"
        },
        {
          "fields": [
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
      "concreteType": "LocationConnection",
      "kind": "LinkedField",
      "name": "locations",
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
          "concreteType": "LocationEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "LocationDetails",
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

(node as any).hash = "c8ef07a8a724793d5c9cf092e8c58780";

export default node;
