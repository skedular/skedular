/**
 * @generated SignedSource<<bef53c4fdaa43ddfbad101ef4b2c2c9b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationCustomTagsTab_query$data = {
  readonly organization: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"customTagCard_Query">;
  readonly " $fragmentType": "organizationCustomTagsTab_query";
};
export type organizationCustomTagsTab_query$key = {
  readonly " $data"?: organizationCustomTagsTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationCustomTagsTab_query">;
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
  "name": "organizationCustomTagsTab_query",
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
      "name": "customTagCard_Query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "0ba4b09dab0c8b69f82bc5c486a29cfa";

export default node;
