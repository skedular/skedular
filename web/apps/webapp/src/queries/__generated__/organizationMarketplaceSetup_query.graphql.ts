/**
 * @generated SignedSource<<45c11fab715b7c7b1fa55403fb9d680d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationMarketplaceSetup_query$data = {
  readonly organization: {
    readonly billingCycle: {
      readonly name: string;
      readonly type: OrganizationBillingCycle;
    };
    readonly id: string;
    readonly marketplaceListingMetadata: {
      readonly about: string | null | undefined;
      readonly includedFeatures: ReadonlyArray<string> | null | undefined;
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"existingStripeConnectAccountButton_query" | "singleChoiceOrganizationBillingCycle_query">;
  readonly " $fragmentType": "organizationMarketplaceSetup_query";
};
export type organizationMarketplaceSetup_query$key = {
  readonly " $data"?: organizationMarketplaceSetup_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
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
        (v0/*: any*/),
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
              "name": "about",
              "storageKey": null
            },
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
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "includedFeatures",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationBillingCycleDetails",
          "kind": "LinkedField",
          "name": "billingCycle",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "type",
              "storageKey": null
            },
            (v0/*: any*/)
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
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceOrganizationBillingCycle_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "ce2cdcabff01dc34bc23192b7b019737";

export default node;
