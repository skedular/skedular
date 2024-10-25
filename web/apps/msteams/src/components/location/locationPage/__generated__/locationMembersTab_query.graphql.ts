/**
 * @generated SignedSource<<4930435ab1d4994b857f302825ba2171>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationMembersTab_query$data = {
  readonly location: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"locationSingleChoiceMembershipType_query">;
  readonly " $fragmentType": "locationMembersTab_query";
};
export type locationMembersTab_query$key = {
  readonly " $data"?: locationMembersTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationMembersTab_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "locationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationMembersTab_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "locationId"
        }
      ],
      "concreteType": "LocationDetails",
      "kind": "LinkedField",
      "name": "location",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
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
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "locationSingleChoiceMembershipType_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "e7758bf8658f4461b256aedea6bade54";

export default node;
