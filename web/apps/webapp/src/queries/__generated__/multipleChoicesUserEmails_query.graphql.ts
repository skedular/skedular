/**
 * @generated SignedSource<<9e74414ce3a851ee96e30de6a648dd0d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type multipleChoicesUserEmails_query$data = {
  readonly me: {
    readonly emails: ReadonlyArray<string>;
  };
  readonly " $fragmentType": "multipleChoicesUserEmails_query";
};
export type multipleChoicesUserEmails_query$key = {
  readonly " $data"?: multipleChoicesUserEmails_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesUserEmails_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "multipleChoicesUserEmails_query",
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
          "name": "emails",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "0158ca8fba8fbb04a5056b1a90830694";

export default node;
