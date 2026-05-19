/**
 * @generated SignedSource<<8403ea14a765afd6abc3c45de4fe51f6>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type bookings_query$data = {
  readonly marketplaceBookingSubscriptionCancellationModes: ReadonlyArray<{
    readonly name: string;
    readonly type: MarketplaceBookingSubscriptionCancellationMode;
  }>;
  readonly marketplaceBookingSubscriptions: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly recurringBookings: ReadonlyArray<{
          readonly id: string;
        }>;
      };
    }>;
  };
  readonly me: {
    readonly id: string;
  };
  readonly " $fragmentSpreads": FragmentRefs<"bookingCard_query">;
  readonly " $fragmentType": "bookings_query";
};
export type bookings_query$key = {
  readonly " $data"?: bookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookings_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = [
  (v0/*:: as any*/)
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "bookings_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": (v1/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingSubscriptionCancellationModeDetails",
      "kind": "LinkedField",
      "name": "marketplaceBookingSubscriptionCancellationModes",
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
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Literal",
          "name": "first",
          "value": 100
        },
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "organizationCustomDomain",
              "variableName": "organizationCustomDomain"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
      "kind": "LinkedField",
      "name": "marketplaceBookingSubscriptions",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceBookingSubscriptionEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "MarketplaceBookingSubscriptionDetails",
              "kind": "LinkedField",
              "name": "node",
              "plural": false,
              "selections": [
                (v0/*:: as any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "RecurringBookingDetails",
                  "kind": "LinkedField",
                  "name": "recurringBookings",
                  "plural": true,
                  "selections": (v1/*:: as any*/),
                  "storageKey": null
                }
              ],
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
      "name": "bookingCard_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "6463bc46c58ace4daa6054c0cf986750";

export default node;
