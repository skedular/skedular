/**
 * @generated SignedSource<<b4f6fee587aa8f424be38c1210106fd8>>
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
  readonly " $fragmentSpreads": FragmentRefs<"organizationTermsOfUse_query">;
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "1b81b7cd1ddeed1c4d9312b806aeb57e";

export default node;
