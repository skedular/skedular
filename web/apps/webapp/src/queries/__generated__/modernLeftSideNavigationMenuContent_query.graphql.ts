/**
 * @generated SignedSource<<e004b39eadc3659f87fa10cc71091992>>
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
    readonly activeOffering: {
      readonly earlyBird: boolean;
      readonly free: boolean;
    };
    readonly canModify: boolean;
    readonly canViewAnalytics: boolean;
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
              "kind": "ScalarField",
              "name": "canViewAnalytics",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationActiveOfferingDetails",
              "kind": "LinkedField",
              "name": "activeOffering",
              "plural": false,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "free",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "earlyBird",
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

(node as any).hash = "023f69ee516533dccff9b865b2b5c7ac";

export default node;
