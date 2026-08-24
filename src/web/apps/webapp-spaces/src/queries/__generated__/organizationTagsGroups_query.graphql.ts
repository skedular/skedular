/**
 * @generated SignedSource<<9e412d0990101cd742f6036caae5bccf>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationTagsGroups_query$data = {
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentType": "organizationTagsGroups_query";
};
export type organizationTagsGroups_query$key = {
  readonly " $data"?: organizationTagsGroups_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationTagsGroups_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationTagsGroups_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "customDomain",
          "variableName": "organizationCustomDomain"
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

(node as any).hash = "e357170e5065c86d964e6656a0fe337e";

export default node;
