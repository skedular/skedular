/**
 * @generated SignedSource<<4074adfb72d199bb3760c0a7ca815f94>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myBookings_query$data = {
  readonly me: {
    readonly id: string;
  };
  readonly " $fragmentType": "myBookings_query";
};
export type myBookings_query$key = {
  readonly " $data"?: myBookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myBookings_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "myBookings_query",
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

(node as any).hash = "c664223bbd164fa6e688b2b30710dc2f";

export default node;
