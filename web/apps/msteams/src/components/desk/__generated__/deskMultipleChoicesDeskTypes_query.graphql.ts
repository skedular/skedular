/**
 * @generated SignedSource<<9c550c7c50d8ca972216b5f751d9bbfd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type deskMultipleChoicesDeskTypes_query$data = {
  readonly organizationTags: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "deskMultipleChoicesDeskTypes_query";
};
export type deskMultipleChoicesDeskTypes_query$key = {
  readonly " $data"?: deskMultipleChoicesDeskTypes_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"deskMultipleChoicesDeskTypes_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "deskMultipleChoicesDeskTypesSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "deskTypeTagType"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "deskMultipleChoicesDeskTypes_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "deskMultipleChoicesDeskTypesSortingValues"
        },
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "organizationId",
              "variableName": "organizationId"
            },
            {
              "kind": "Variable",
              "name": "tagType",
              "variableName": "deskTypeTagType"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "OrganizationTagConnection",
      "kind": "LinkedField",
      "name": "organizationTags",
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

(node as any).hash = "02c6985efedc64a7b76050a4ff6e627d";

export default node;
