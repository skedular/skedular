/**
 * @generated SignedSource<<1bb96b794e868f663824e08f9fbcc2e1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type addOrganization_query$data = {
  readonly activeOrganizationTermsOfUse: {
    readonly id: string;
  };
  readonly " $fragmentSpreads": FragmentRefs<"organizationMultipleChoicesIndustries_query" | "organizationTermsOfUse_query">;
  readonly " $fragmentType": "addOrganization_query";
};
export type addOrganization_query$key = {
  readonly " $data"?: addOrganization_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"addOrganization_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "addOrganization_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTermsOfUse",
      "kind": "LinkedField",
      "name": "activeOrganizationTermsOfUse",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationMultipleChoicesIndustries_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationTermsOfUse_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "6daef64ee00d2cbb2b0cb7889b54cd02";

export default node;
