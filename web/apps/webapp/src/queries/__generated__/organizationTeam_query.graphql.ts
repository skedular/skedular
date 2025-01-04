/**
 * @generated SignedSource<<67528a52830b72c611e2aa2547d530c8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationTeam_query$data = {
  readonly team: {
    readonly about: string | null | undefined;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceLocation_locations_query">;
  readonly " $fragmentType": "organizationTeam_query";
};
export type organizationTeam_query$key = {
  readonly " $data"?: organizationTeam_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationTeam_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "teamId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationTeam_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "teamId"
        }
      ],
      "concreteType": "TeamDetails",
      "kind": "LinkedField",
      "name": "team",
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
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "about",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceLocation_locations_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "a2062ab3d99bcd0595baee1a2a9c7edb";

export default node;
