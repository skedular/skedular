/**
 * @generated SignedSource<<7f7a550add086e42536d9fad507185b8>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationTermsOfUse_query$data = {
  readonly activeOrganizationTermsOfUse: {
    readonly id: string;
    readonly terms: string;
  };
  readonly " $fragmentType": "organizationTermsOfUse_query";
};
export type organizationTermsOfUse_query$key = {
  readonly " $data"?: organizationTermsOfUse_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationTermsOfUse_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationTermsOfUse_query",
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
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "terms",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "7fdda46c011b2800c36f85814f0a8951";

export default node;
