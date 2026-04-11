/**
 * @generated SignedSource<<452ba5a4dcf4506b2e568f57d97649e1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationCard_query$data = {
  readonly me: {
    readonly preferredLocations: ReadonlyArray<{
      readonly id: string;
    }>;
  };
  readonly " $fragmentType": "locationCard_query";
};
export type locationCard_query$key = {
  readonly " $data"?: locationCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationCard_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationCard_query",
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
          "name": "preferredLocations",
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
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b45eeed0f54a53c440e8875500b183ef";

export default node;
