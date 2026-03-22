/**
 * @generated SignedSource<<ac9db5f70fba7dba84b4988c47fee2a1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontUpcomingBookingsStrip_query$data = {
  readonly bookings?: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly bookingResources: ReadonlyArray<{
          readonly resource: {
            readonly id: string;
            readonly name: string;
          };
        }>;
        readonly from: any;
        readonly id: string;
        readonly involvedLocations: ReadonlyArray<{
          readonly name: string;
        }>;
        readonly marketplaceBooking: {
          readonly paymentStatus: {
            readonly name: string;
            readonly type: PaymentStatus;
          };
          readonly quantity: number;
        } | null | undefined;
        readonly until: any;
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentType": "guestStoreFrontUpcomingBookingsStrip_query";
};
export type guestStoreFrontUpcomingBookingsStrip_query$key = {
  readonly " $data"?: guestStoreFrontUpcomingBookingsStrip_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontUpcomingBookingsStrip_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
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
  "name": "guestStoreFrontUpcomingBookingsStrip_query",
  "selections": [
    {
      "condition": "includeUpcomingBookings",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": null,
          "args": [
            {
              "kind": "Literal",
              "name": "first",
              "value": 6
            },
            {
              "kind": "Literal",
              "name": "orderBy",
              "value": [
                {
                  "direction": "ASCENDING",
                  "field": "FROM"
                }
              ]
            },
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
                {
                  "kind": "Literal",
                  "name": "includeMineOnly",
                  "value": true
                },
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
          "concreteType": "ConnectionOfBookingEdge",
          "kind": "LinkedField",
          "name": "bookings",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "totalCount",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "BookingEdge",
              "kind": "LinkedField",
              "name": "edges",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "BookingDetails",
                  "kind": "LinkedField",
                  "name": "node",
                  "plural": false,
                  "selections": [
                    (v0/*: any*/),
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "from",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "until",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "Booking_LocationDetails",
                      "kind": "LinkedField",
                      "name": "involvedLocations",
                      "plural": true,
                      "selections": [
                        (v1/*: any*/)
                      ],
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "BookingResourceDetails",
                      "kind": "LinkedField",
                      "name": "bookingResources",
                      "plural": true,
                      "selections": [
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "ResourceDetails",
                          "kind": "LinkedField",
                          "name": "resource",
                          "plural": false,
                          "selections": [
                            (v0/*: any*/),
                            (v1/*: any*/)
                          ],
                          "storageKey": null
                        }
                      ],
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "MarketplaceBookingDetails",
                      "kind": "LinkedField",
                      "name": "marketplaceBooking",
                      "plural": false,
                      "selections": [
                        {
                          "alias": null,
                          "args": null,
                          "kind": "ScalarField",
                          "name": "quantity",
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "PaymentStatusDetails",
                          "kind": "LinkedField",
                          "name": "paymentStatus",
                          "plural": false,
                          "selections": [
                            {
                              "alias": null,
                              "args": null,
                              "kind": "ScalarField",
                              "name": "type",
                              "storageKey": null
                            },
                            (v1/*: any*/)
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
            }
          ],
          "storageKey": null
        }
      ]
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "6b56f2f2567e3c8a083a592a7119c662";

export default node;
