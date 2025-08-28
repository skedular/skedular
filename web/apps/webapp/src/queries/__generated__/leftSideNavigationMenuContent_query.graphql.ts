/**
 * @generated SignedSource<<efa4822e243aa600ca7901c09bcfa44b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type leftSideNavigationMenuContent_query$data = {
  readonly organization?: {
    readonly activeOffering: {
      readonly earlyBird: boolean;
      readonly free: boolean;
    };
    readonly canModify: boolean;
    readonly canViewAnalytics: boolean;
    readonly id: string;
    readonly type: {
      readonly type: OrganizationType;
    };
    readonly uniqueAlphanumericName: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "leftSideNavigationMenuContent_query";
};
export type leftSideNavigationMenuContent_query$key = {
  readonly " $data"?: leftSideNavigationMenuContent_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"leftSideNavigationMenuContent_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationExists"
    },
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "leftSideNavigationMenuContent_query",
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
              "args": null,
              "kind": "ScalarField",
              "name": "id",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "uniqueAlphanumericName",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTypeDetails",
              "kind": "LinkedField",
              "name": "type",
              "plural": false,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "type",
                  "storageKey": null
                }
              ],
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

(node as any).hash = "1558732f3c7033ab50d39cc7fb3881e5";

export default node;
