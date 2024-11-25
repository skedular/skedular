/**
 * @generated SignedSource<<3239753616e9fbf425dfed81d36270a3>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type oldLeftSideNavigationMenu_query$data = {
  readonly organization?: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly " $fragmentType": "oldLeftSideNavigationMenu_query";
};
export type oldLeftSideNavigationMenu_query$key = {
  readonly " $data"?: oldLeftSideNavigationMenu_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"oldLeftSideNavigationMenu_query">;
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
  "name": "oldLeftSideNavigationMenu_query",
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

(node as any).hash = "72faecb97278833f9ff73198968662e3";

export default node;
