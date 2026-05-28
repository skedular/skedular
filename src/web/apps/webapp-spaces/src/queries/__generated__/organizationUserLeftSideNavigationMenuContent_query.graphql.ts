/**
 * @generated SignedSource<<fe5db1bf81518e7b81c3fceb004d94a2>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationUserLeftSideNavigationMenuContent_query$data = {
  readonly me: {
    readonly id: string;
  };
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
