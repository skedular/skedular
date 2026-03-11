/**
 * @generated SignedSource<<9212cced58ab9c97cdea823070e60d20>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMarketplaceSetup_query$data = {
  readonly organization: {
    readonly id: string;
    readonly marketplaceListingMetadata: {
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"existingStripeConnectAccountButton_query">;
  readonly " $fragmentType": "organizationMarketplaceSetup_query";
};
export type organizationMarketplaceSetup_query$key = {
  readonly " $data"?: organizationMarketplaceSetup_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationMarketplaceSetup_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "uniqueAlphanumericName",
          "variableName": "organizationUniqueAlphanumericName"
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
          "name": "id",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "name",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "ListingMetadata",
          "kind": "LinkedField",
          "name": "marketplaceListingMetadata",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "title",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "subTitle",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "existingStripeConnectAccountButton_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "035c0c53743ab2edc47afec8db0b1abe";

export default node;
