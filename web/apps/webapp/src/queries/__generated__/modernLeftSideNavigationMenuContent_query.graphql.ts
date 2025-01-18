/**
 * @generated SignedSource<<1cb523de9da7330eb0d9d35770a4d9b0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type modernLeftSideNavigationMenuContent_query$data = {
  readonly organization?: {
    readonly availableOfferings: ReadonlyArray<{
      readonly code: string;
    }>;
    readonly canModify: boolean;
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentType": "modernLeftSideNavigationMenuContent_query";
};
export type modernLeftSideNavigationMenuContent_query$key = {
  readonly " $data"?: modernLeftSideNavigationMenuContent_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"modernLeftSideNavigationMenuContent_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationExists"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "modernLeftSideNavigationMenuContent_query",
  "selections": [
    {
      "condition": "organizationExists",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": null,
          "args": [
            {
              "kind": "Variable",
              "name": "id",
              "variableName": "organizationId"
            }
          ],
          "concreteType": "OrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
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
              "name": "canModify",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationOfferingDetails",
              "kind": "LinkedField",
              "name": "availableOfferings",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "code",
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

(node as any).hash = "b156e3e23c209c10b6e4f3018e1e6df7";

export default node;
