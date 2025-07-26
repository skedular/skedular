/**
 * @generated SignedSource<<3e82322a78e8a2166040582680f2db73>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import {FragmentRefs, ReaderFragment} from 'relay-runtime';

export type organizationMarketplaceSetup_query$data = {
    readonly " $fragmentSpreads": FragmentRefs<"existingStripeConnectAccountButton_query">;
    readonly " $fragmentType": "organizationMarketplaceSetup_query";
};
export type organizationMarketplaceSetup_query$key = {
    readonly " $data"?: organizationMarketplaceSetup_query$data;
    readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_query">;
};

const node: ReaderFragment = {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMarketplaceSetup_query",
    "selections": [
        {
            "args": null,
            "kind": "FragmentSpread",
            "name": "existingStripeConnectAccountButton_query"
        }
    ],
    "type": "Query",
    "abstractKey": null
};

(node as any).hash = "c850d157b96f27cc0e21a1c3bfd26627";

export default node;
