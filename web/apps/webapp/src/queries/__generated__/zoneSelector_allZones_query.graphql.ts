/**
 * @generated SignedSource<<e0324830b161326c9a61e6bf0bfbfe3a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type zoneSelector_allZones_query$data = {
  readonly zones: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly " $fragmentType": "zoneSelector_allZones_query";
};
export type zoneSelector_allZones_query$key = {
  readonly " $data"?: zoneSelector_allZones_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"zoneSelector_allZones_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    },
    {
      "kind": "RootArgument",
      "name": "zonesSortingValues"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "zoneSelector_allZones_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "zonesSortingValues"
        },
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
      "concreteType": "ConnectionOfOrganizationTagEdge",
      "kind": "LinkedField",
      "name": "zones",
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
          "concreteType": "OrganizationTagEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
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
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "color",
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

(node as any).hash = "825f102effb56efaaeb7a7b9c472f935";

export default node;
