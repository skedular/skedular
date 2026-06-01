/**
 * @generated SignedSource<<1870f37efb35ac15f2023b186a0d4746>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type editMarketplaceBooking_query$data = {
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
  readonly organizationBookingPermissions: {
    readonly canModifyPaymentMethod: boolean;
  };
  readonly paymentStatuses: ReadonlyArray<{
    readonly name: string;
    readonly type: PaymentStatus;
  }>;
  readonly " $fragmentType": "editMarketplaceBooking_query";
};
export type editMarketplaceBooking_query$key = {
  readonly " $data"?: editMarketplaceBooking_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editMarketplaceBooking_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  {
    "kind": "Variable",
    "name": "organizationCustomDomain",
    "variableName": "organizationCustomDomain"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "editMarketplaceBooking_query",
  "selections": [
    {
      "alias": null,
      "args": (v0/*:: as any*/),
      "concreteType": "OrganizationBookingPermissions",
      "kind": "LinkedField",
      "name": "organizationBookingPermissions",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canModifyPaymentMethod",
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
          "fields": (v0/*:: as any*/),
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
                (v1/*:: as any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "RecurringBookingDetails",
                  "kind": "LinkedField",
                  "name": "recurringBookings",
                  "plural": true,
                  "selections": [
                    (v1/*:: as any*/)
                  ],
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
      "alias": null,
      "args": null,
      "concreteType": "PaymentStatusDetails",
      "kind": "LinkedField",
      "name": "paymentStatuses",
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
})();

(node as any).hash = "20a4751f09a93652c47497f1435778e2";

export default node;
