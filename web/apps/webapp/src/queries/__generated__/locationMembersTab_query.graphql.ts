/**
 * @generated SignedSource<<62af0166fd3a80399a67eaf075c2a823>>
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
  readonly " $fragmentSpreads": FragmentRefs<"locationSingleChoiceMemberRole_query">;
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
      "name": "locationSingleChoiceMemberRole_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "3e69e064f99080384b012b7900be4fa2";

export default node;
