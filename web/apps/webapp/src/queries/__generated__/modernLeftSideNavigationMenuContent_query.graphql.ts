/**
 * @generated SignedSource<<a59b42673c4845528c0a6ede761d79e1>>
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

(node as any).hash = "94e4fbd6d402f75e96ebc0bb145b21bb";

export default node;
