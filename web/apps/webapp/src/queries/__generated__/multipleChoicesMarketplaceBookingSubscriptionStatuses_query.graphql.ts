/**
 * @generated SignedSource<<4db49aef48a6d685532e76140d952da3>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type multipleChoicesMarketplaceBookingSubscriptionStatuses_query$data = {
  readonly marketplaceBookingSubscriptionStatuses: ReadonlyArray<{
    readonly name: string;
    readonly type: MarketplaceBookingSubscriptionStatus;
  }>;
  readonly " $fragmentType": "multipleChoicesMarketplaceBookingSubscriptionStatuses_query";
};
export type multipleChoicesMarketplaceBookingSubscriptionStatuses_query$key = {
  readonly " $data"?: multipleChoicesMarketplaceBookingSubscriptionStatuses_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesMarketplaceBookingSubscriptionStatuses_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "multipleChoicesMarketplaceBookingSubscriptionStatuses_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
      "kind": "LinkedField",
      "name": "marketplaceBookingSubscriptionStatuses",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "name",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "73ddef2d1ffcf9a5832c99b6fcce5597";

export default node;
