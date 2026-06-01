/**
 * @generated SignedSource<<9219b37201f58bede8bb7ddc56ed9a41>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontActivitySummary_query$data = {
  readonly bookings?: {
    readonly totalCount: number;
  };
  readonly marketplaceBookingSubscriptions?: {
    readonly totalCount: number;
  };
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "d1408baabadd294a60da7d5c89b13c1b";

export default node;
