/**
 * @generated SignedSource<<66337887de58c1b06ddb823a1885314f>>
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

(node as any).hash = "89ec249576ab0788ce06e8b66d9f5bab";

export default node;
