/**
 * @generated SignedSource<<84eb8e4f3c16e0d34951e4ee3a65c7e0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationUserLeftSideNavigationMenuContent_query$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentType": "organizationUserLeftSideNavigationMenuContent_query";
};
export type organizationUserLeftSideNavigationMenuContent_query$key = {
  readonly " $data"?: organizationUserLeftSideNavigationMenuContent_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationUserLeftSideNavigationMenuContent_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationUserLeftSideNavigationMenuContent_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "9fd03c1080d10da981aacc3051677bcd";

export default node;
