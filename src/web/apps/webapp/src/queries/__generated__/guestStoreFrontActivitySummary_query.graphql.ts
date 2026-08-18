/**
 * @generated SignedSource<<cdec568f23bd2ab1077e747ce45ac159>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontActivitySummary_query$data = {
  readonly bookings?: {
    readonly totalCount: number;
  };
  readonly marketplaceBookingSubscriptions?: {
    readonly totalCount: number;
  };
  readonly myEntitlements: ReadonlyArray<{
    readonly availableQuantity: number;
    readonly id: string;
    readonly status: EntitlementStatus;
  }>;
  readonly " $fragmentType": "guestStoreFrontActivitySummary_query";
};
export type guestStoreFrontActivitySummary_query$key = {
  readonly " $data"?: guestStoreFrontActivitySummary_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontActivitySummary_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "kind": "Literal",
  "name": "first",
  "value": 0
},
v1 = {
  "kind": "Literal",
  "name": "includeMineOnly",
  "value": true
},
v2 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v3 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "totalCount",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "bookingsSearchCriteriaFrom"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "bookingsSearchCriteriaTo"
    },
    {
      "defaultValue": false,
      "kind": "LocalArgument",
      "name": "includeActiveSubscriptions"
    },
    {
      "defaultValue": false,
      "kind": "LocalArgument",
      "name": "includeUpcomingBookings"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "guestStoreFrontActivitySummary_query",
  "selections": [
    {
      "condition": "includeUpcomingBookings",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": null,
          "args": [
            (v0/*:: as any*/),
            {
              "fields": [
                {
                  "kind": "Literal",
                  "name": "channel",
                  "value": "MARKETPLACE"
                },
                {
                  "kind": "Variable",
                  "name": "fromGte",
                  "variableName": "bookingsSearchCriteriaFrom"
                },
                {
                  "kind": "Variable",
                  "name": "fromLte",
                  "variableName": "bookingsSearchCriteriaTo"
                },
                (v1/*:: as any*/),
                (v2/*:: as any*/)
              ],
              "kind": "ObjectValue",
              "name": "where"
            }
          ],
          "concreteType": "ConnectionOfBookingEdge",
          "kind": "LinkedField",
          "name": "bookings",
          "plural": false,
          "selections": (v3/*:: as any*/),
          "storageKey": null
        }
      ]
    },
    {
      "condition": "includeActiveSubscriptions",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": null,
          "args": [
            (v0/*:: as any*/),
            {
              "fields": [
                (v1/*:: as any*/),
                (v2/*:: as any*/),
                {
                  "kind": "Literal",
                  "name": "status",
                  "value": "ACTIVE"
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
          "selections": (v3/*:: as any*/),
          "storageKey": null
        }
      ]
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "EntitlementDetails",
      "kind": "LinkedField",
      "name": "myEntitlements",
      "plural": true,
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
          "name": "availableQuantity",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "status",
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

(node as any).hash = "298cc9136e97572f664fc657e51562b7";

export default node;
