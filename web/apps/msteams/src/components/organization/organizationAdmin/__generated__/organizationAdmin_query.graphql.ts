/**
 * @generated SignedSource<<43ab57db0f1c895195a275e12ab8f401>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationAdmin_query$data = {
  readonly organization: {
    readonly about: string | null | undefined;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentType": "organizationAdmin_query";
};
export type organizationAdmin_query$key = {
  readonly " $data"?: organizationAdmin_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAdmin_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationAdmin_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "5c51c0bfa8e139bf075f8164f72ff07d";

export default node;
