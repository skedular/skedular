/**
 * @generated SignedSource<<566965d3f27ce6392dfffabbc8bb880d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontActiveSubscriptionsStrip_query$data = {
  readonly marketplaceBookingSubscriptions?: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly autoRenew: boolean;
        readonly cancelAtPeriodEnd: boolean;
        readonly id: string;
        readonly marketplaceBooking: {
          readonly paymentStatus: {
            readonly name: string;
            readonly type: PaymentStatus;
          };
          readonly productVersion: {
            readonly listingMetadata: {
              readonly subTitle: string | null | undefined;
              readonly title: string | null | undefined;
            };
          };
          readonly quantity: number;
        };
        readonly nextRenewalAt: any | null | undefined;
        readonly recurringBookings: ReadonlyArray<{
          readonly endDate: any | null | undefined;
          readonly id: string;
          readonly startDate: any;
        }>;
        readonly startedAt: any;
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentType": "guestStoreFrontActiveSubscriptionsStrip_query";
};
export type guestStoreFrontActiveSubscriptionsStrip_query$key = {
  readonly " $data"?: guestStoreFrontActiveSubscriptionsStrip_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontActiveSubscriptionsStrip_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "defaultValue": false,
      "kind": "LocalArgument",
      "name": "includeActiveSubscriptions"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "guestStoreFrontActiveSubscriptionsStrip_query",
  "selections": [
    {
      "condition": "includeActiveSubscriptions",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": null,
          "args": [
            {
              "kind": "Literal",
              "name": "first",
              "value": 3
            },
            {
              "kind": "Literal",
              "name": "orderBy",
              "value": [
                {
                  "direction": "ASCENDING",
                  "field": "NEXT_RENEWAL_AT"
                }
              ]
            },
            {
              "fields": [
                {
                  "kind": "Literal",
                  "name": "includeMineOnly",
                  "value": true
                },
                {
                  "kind": "Variable",
                  "name": "organizationCustomDomain",
                  "variableName": "organizationCustomDomain"
                },
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
                      "kind": "ScalarField",
                      "name": "startedAt",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "nextRenewalAt",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "autoRenew",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "cancelAtPeriodEnd",
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
                          "args": null,
                          "concreteType": "ProductVersionDetails",
                          "kind": "LinkedField",
                          "name": "productVersion",
                          "plural": false,
                          "selections": [
                            {
                              "alias": null,
                              "args": null,
                              "concreteType": "ListingMetadata",
                              "kind": "LinkedField",
                              "name": "listingMetadata",
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
                        }
                      ],
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "RecurringBookingDetails",
                      "kind": "LinkedField",
                      "name": "recurringBookings",
                      "plural": true,
                      "selections": [
                        (v0/*:: as any*/),
                        {
                          "alias": null,
                          "args": null,
                          "kind": "ScalarField",
                          "name": "startDate",
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "kind": "ScalarField",
                          "name": "endDate",
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

(node as any).hash = "27a54b37958f3efa339aeb3d94e270a1";

export default node;
