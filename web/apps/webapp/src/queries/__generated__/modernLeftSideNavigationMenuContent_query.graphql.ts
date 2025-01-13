/**
 * @generated SignedSource<<a0d5c425018b29368095238e14badc1c>>
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
    readonly canModify: boolean;
    readonly id: string;
  } | null | undefined;
  readonly organizationDesksAvailability?: {
    readonly availableDesksCount: number;
    readonly desksCount: number;
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
    },
    {
      "kind": "RootArgument",
      "name": "todayDate"
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
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": [
            {
              "fields": [
                {
                  "kind": "Variable",
                  "name": "date",
                  "variableName": "todayDate"
                },
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
          "concreteType": "OrganizationAvailableDesks",
          "kind": "LinkedField",
          "name": "organizationDesksAvailability",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "desksCount",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "availableDesksCount",
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

(node as any).hash = "9d39d89f66cf66433a2176f58910478f";

export default node;
