/**
 * @generated SignedSource<<e371211bf1d25f9c3a9da23ca5afc731>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceLocationCard_query$data = {
  readonly me?: {
    readonly favouriteLocations: ReadonlyArray<{
      readonly id: string;
    }>;
  };
  readonly " $fragmentType": "marketplaceLocationCard_query";
};
export type marketplaceLocationCard_query$key = {
  readonly " $data"?: marketplaceLocationCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocationCard_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "userSignedIn"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "marketplaceLocationCard_query",
  "selections": [
    {
      "condition": "userSignedIn",
      "kind": "Condition",
      "passingValue": true,
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
              "concreteType": "LocationDetails",
              "kind": "LinkedField",
              "name": "favouriteLocations",
              "plural": true,
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
          "storageKey": null
        }
      ]
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "7d5821f7b6ca496f1bfab2ff909bf0d5";

export default node;
