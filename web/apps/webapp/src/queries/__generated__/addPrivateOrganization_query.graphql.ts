/**
 * @generated SignedSource<<3ca136d25195d6741a216eac222ae587>>
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
  readonly emailsToShowLatestCapabilities: ReadonlyArray<string>;
  readonly me: {
    readonly emails: ReadonlyArray<string>;
  };
  readonly " $fragmentSpreads": FragmentRefs<"organizationTermsOfUse_query">;
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
      "kind": "ScalarField",
      "name": "emailsToShowLatestCapabilities",
      "storageKey": null
    },
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

(node as any).hash = "51d94daa96bec19e59fdb2f9d290c963";

export default node;
