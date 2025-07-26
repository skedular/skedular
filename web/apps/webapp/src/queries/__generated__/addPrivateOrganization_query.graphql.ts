/**
 * @generated SignedSource<<be574fbaed392bff64cc546bc083266c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type addPrivateOrganization_query$data = {
  readonly activeOrganizationTermsOfUse: {
    readonly id: string;
  };
  readonly " $fragmentSpreads": FragmentRefs<"organizationTermsOfUse_query" | "singleChoiceOrganizationMemberVisibilityPolicyquery">;
  readonly " $fragmentType": "addPrivateOrganization_query";
};
export type addPrivateOrganization_query$key = {
  readonly " $data"?: addPrivateOrganization_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"addPrivateOrganization_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "addPrivateOrganization_query",
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

(node as any).hash = "2caecf12527d80b2e6ea97c7a7adf78d";

export default node;
