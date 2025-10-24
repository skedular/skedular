/**
 * @generated SignedSource<<afd542dd1a80585d0e4c8cdef1c00771>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type observability_query$data = {
  readonly emailsToIgnoreObservability: ReadonlyArray<string>;
  readonly me: {
    readonly emails: ReadonlyArray<string>;
  };
  readonly " $fragmentSpreads": FragmentRefs<"logrocket_query">;
  readonly " $fragmentType": "observability_query";
};
export type observability_query$key = {
  readonly " $data"?: observability_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"observability_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "observability_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "emailsToIgnoreObservability",
      "storageKey": null
    },
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
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "logrocket_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "627ad27e798618ff5943166be7f56667";

export default node;
