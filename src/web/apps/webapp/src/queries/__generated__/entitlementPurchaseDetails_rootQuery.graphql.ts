/**
 * @generated SignedSource<<ca3af977d692c0eb56b2380b3645106f>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type entitlementPurchaseDetails_rootQuery$variables = {
  purchaseId: string;
};
export type entitlementPurchaseDetails_rootQuery$data = {
  readonly entitlementPurchase: {
    readonly amount: any;
    readonly creditQuantity: number;
    readonly currency: string;
    readonly entitlement: {
      readonly autoRenew: boolean;
      readonly availableQuantity: number;
      readonly cancelAtPeriodEnd: boolean;
      readonly id: string;
      readonly nextRenewalAt: any | null | undefined;
      readonly renewalFailureReason: string | null | undefined;
      readonly status: EntitlementStatus;
    } | null | undefined;
    readonly id: string;
    readonly invoiceNumber: string | null | undefined;
    readonly invoiceUrl: string | null | undefined;
    readonly lifecycleState: string;
    readonly linkedBookings: {
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly bookingResources: ReadonlyArray<{
            readonly resource: {
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
    readonly paymentAction: string | null | undefined;
    readonly paymentExpiry: any;
    readonly paymentMethod: string;
    readonly paymentStatus: string;
    readonly pricingId: string;
    readonly productVersion: {
      readonly featureImages: ReadonlyArray<{
        readonly original: {
          readonly url: string;
        } | null | undefined;
      }>;
      readonly listingMetadata: {
        readonly about: string | null | undefined;
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly pricingOptions: ReadonlyArray<{
        readonly id: string;
        readonly listingMetadata: {
          readonly title: string | null | undefined;
        };
      }>;
    };
    readonly serviceStartAt: any;
    readonly validityDays: number;
  } | null | undefined;
};
export type entitlementPurchaseDetails_rootQuery = {
  response: entitlementPurchaseDetails_rootQuery$data;
  variables: entitlementPurchaseDetails_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "purchaseId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "purchaseId",
    "variableName": "purchaseId"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentStatus",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lifecycleState",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentMethod",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentExpiry",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "serviceStartAt",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "pricingId",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v10 = {
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
      "name": "about",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "includedFeatures",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "subTitle",
      "storageKey": null
    },
    (v9/*:: as any*/)
  ],
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "CdnImageFile",
  "kind": "LinkedField",
  "name": "featureImages",
  "plural": true,
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CdnFile",
      "kind": "LinkedField",
      "name": "original",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "url",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "concreteType": "ProductPricing",
  "kind": "LinkedField",
  "name": "pricingOptions",
  "plural": true,
  "selections": [
    (v2/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "ListingMetadata",
      "kind": "LinkedField",
      "name": "listingMetadata",
      "plural": false,
      "selections": [
        (v9/*:: as any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "amount",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currency",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "creditQuantity",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "validityDays",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceNumber",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentAction",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "concreteType": "EntitlementDetails",
  "kind": "LinkedField",
  "name": "entitlement",
  "plural": false,
  "selections": [
    (v2/*:: as any*/),
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
      "kind": "ScalarField",
      "name": "status",
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
      "name": "renewalFailureReason",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v21 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 10
  }
],
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v26 = [
  (v25/*:: as any*/)
],
v27 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_LocationDetails",
  "kind": "LinkedField",
  "name": "involvedLocations",
  "plural": true,
  "selections": (v26/*:: as any*/),
  "storageKey": null
},
v28 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v29 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": [
    (v25/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "type",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "entitlementPurchaseDetails_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "EntitlementPurchaseDetails",
        "kind": "LinkedField",
        "name": "entitlementPurchase",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "ProductVersionDetails",
            "kind": "LinkedField",
            "name": "productVersion",
            "plural": false,
            "selections": [
              (v10/*:: as any*/),
              (v11/*:: as any*/),
              (v12/*:: as any*/)
            ],
            "storageKey": null
          },
          (v13/*:: as any*/),
          (v14/*:: as any*/),
          (v15/*:: as any*/),
          (v16/*:: as any*/),
          (v17/*:: as any*/),
          (v18/*:: as any*/),
          (v19/*:: as any*/),
          (v20/*:: as any*/),
          {
            "alias": null,
            "args": (v21/*:: as any*/),
            "concreteType": "ConnectionOfBookingEdge",
            "kind": "LinkedField",
            "name": "linkedBookings",
            "plural": false,
            "selections": [
              (v22/*:: as any*/),
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
                      (v2/*:: as any*/),
                      (v23/*:: as any*/),
                      (v24/*:: as any*/),
                      (v27/*:: as any*/),
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
                            "selections": (v26/*:: as any*/),
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
                          (v28/*:: as any*/),
                          (v29/*:: as any*/)
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
            "storageKey": "linkedBookings(first:10)"
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "entitlementPurchaseDetails_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "EntitlementPurchaseDetails",
        "kind": "LinkedField",
        "name": "entitlementPurchase",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "ProductVersionDetails",
            "kind": "LinkedField",
            "name": "productVersion",
            "plural": false,
            "selections": [
              (v10/*:: as any*/),
              (v11/*:: as any*/),
              (v12/*:: as any*/),
              (v2/*:: as any*/)
            ],
            "storageKey": null
          },
          (v13/*:: as any*/),
          (v14/*:: as any*/),
          (v15/*:: as any*/),
          (v16/*:: as any*/),
          (v17/*:: as any*/),
          (v18/*:: as any*/),
          (v19/*:: as any*/),
          (v20/*:: as any*/),
          {
            "alias": null,
            "args": (v21/*:: as any*/),
            "concreteType": "ConnectionOfBookingEdge",
            "kind": "LinkedField",
            "name": "linkedBookings",
            "plural": false,
            "selections": [
              (v22/*:: as any*/),
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
                      (v2/*:: as any*/),
                      (v23/*:: as any*/),
                      (v24/*:: as any*/),
                      (v27/*:: as any*/),
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
                              (v25/*:: as any*/),
                              (v2/*:: as any*/)
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
                          (v28/*:: as any*/),
                          (v29/*:: as any*/),
                          (v2/*:: as any*/)
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
            "storageKey": "linkedBookings(first:10)"
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "356a3fa180c3ce0da875c1f1a59dfaae",
    "id": null,
    "metadata": {},
    "name": "entitlementPurchaseDetails_rootQuery",
    "operationKind": "query",
    "text": "query entitlementPurchaseDetails_rootQuery(\n  $purchaseId: String!\n) {\n  entitlementPurchase(purchaseId: $purchaseId) {\n    id\n    paymentStatus\n    lifecycleState\n    paymentMethod\n    paymentExpiry\n    serviceStartAt\n    pricingId\n    productVersion {\n      listingMetadata {\n        about\n        includedFeatures\n        subTitle\n        title\n      }\n      featureImages {\n        original {\n          url\n        }\n      }\n      pricingOptions {\n        id\n        listingMetadata {\n          title\n        }\n      }\n      id\n    }\n    amount\n    currency\n    creditQuantity\n    validityDays\n    invoiceNumber\n    invoiceUrl\n    paymentAction\n    entitlement {\n      id\n      availableQuantity\n      autoRenew\n      cancelAtPeriodEnd\n      status\n      nextRenewalAt\n      renewalFailureReason\n    }\n    linkedBookings(first: 10) {\n      totalCount\n      edges {\n        node {\n          id\n          from\n          until\n          involvedLocations {\n            name\n          }\n          bookingResources {\n            resource {\n              name\n              id\n            }\n          }\n          marketplaceBooking {\n            quantity\n            paymentStatus {\n              name\n              type\n            }\n            id\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8e68a8d3613e12567736d2afeecf4008";

export default node;
