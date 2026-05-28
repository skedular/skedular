/**
 * @generated SignedSource<<7c8f35a903d44ca572381bcfff07d4ea>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type LocationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceLocationType_query$data = {
  readonly locationTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: LocationType;
  }>;
  readonly " $fragmentType": "singleChoiceLocationType_query";
};
export type singleChoiceLocationType_query$key = {
  readonly " $data"?: singleChoiceLocationType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceLocationType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceLocationType_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "LocationTypeDetails",
      "kind": "LinkedField",
      "name": "locationTypes",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "name",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "c6c4dc0664680bee1b64f8c7c69e3cac";

export default node;
