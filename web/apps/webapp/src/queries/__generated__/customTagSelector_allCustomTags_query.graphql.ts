/**
 * @generated SignedSource<<3d132c2b5d6c7aa291293d8d791d7538>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type customTagSelector_allCustomTags_query$data = {
  readonly organization: {
    readonly customTags: {
      readonly __id: string;
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly color: string | null | undefined;
          readonly id: string;
          readonly name: string;
        };
      }>;
      readonly totalCount: number;
    };
  } | null | undefined;
  readonly " $fragmentType": "customTagSelector_allCustomTags_query";
};
export type customTagSelector_allCustomTags_query$key = {
  readonly " $data"?: customTagSelector_allCustomTags_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"customTagSelector_allCustomTags_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "customTagsSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "customTagSelector_allCustomTags_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "uniqueAlphanumericName",
          "variableName": "organizationUniqueAlphanumericName"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": [
            {
              "kind": "Variable",
              "name": "orderBy",
              "variableName": "customTagsSortingValues"
            }
          ],
          "concreteType": "ConnectionOfOrganizationTagEdge",
          "kind": "LinkedField",
          "name": "customTags",
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
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b49fcfc18b93870ca57c4e622dd245a3";

export default node;
