/**
 * @generated SignedSource<<7b5165e9398c9c48a1ab9cd809f5b2f8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationZonesTab_query$data = {
  readonly organization: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"zoneCard_Query">;
  readonly " $fragmentType": "organizationZonesTab_query";
};
export type organizationZonesTab_query$key = {
  readonly " $data"?: organizationZonesTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationZonesTab_query">;
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
  "name": "organizationZonesTab_query",
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
          "name": "canModify",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "zoneCard_Query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "22e02bf7fa198f6ff453b68c7a3b81bb";

export default node;
