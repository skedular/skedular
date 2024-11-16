/**
 * @generated SignedSource<<41807ad68e88765d14b99a12f363058a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type leftSideNavigationMenu_query$data = {
  readonly organization: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly " $fragmentType": "leftSideNavigationMenu_query";
};
export type leftSideNavigationMenu_query$key = {
  readonly " $data"?: leftSideNavigationMenu_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"leftSideNavigationMenu_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "leftSideNavigationMenu_query",
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
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "a2d8f4958fcc417cbc3bc6c9f7385516";

export default node;
