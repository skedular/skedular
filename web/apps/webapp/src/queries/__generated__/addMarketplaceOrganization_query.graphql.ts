/**
 * @generated SignedSource<<c12eaacc7d7b7d3f35746fde88556267>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type addMarketplaceOrganization_query$data = {
  readonly activeOrganizationTermsOfUse: {
    readonly id: string;
  };
  readonly me: {
    readonly emails: ReadonlyArray<string>;
  };
  readonly " $fragmentSpreads": FragmentRefs<"organizationTermsOfUse_query" | "singleChoiceOrganizationMemberVisibilityPolicyquery">;
  readonly " $fragmentType": "addMarketplaceOrganization_query";
};
export type addMarketplaceOrganization_query$key = {
  readonly " $data"?: addMarketplaceOrganization_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"addMarketplaceOrganization_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "addMarketplaceOrganization_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "emails",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
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
      "name": "organizationTermsOfUse_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceOrganizationMemberVisibilityPolicyquery"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b9224f12f9a1fe1977b7d7fd7479d8d0";

export default node;
