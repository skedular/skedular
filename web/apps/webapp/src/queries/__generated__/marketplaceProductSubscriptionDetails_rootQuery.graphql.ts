/**
 * @generated SignedSource<<337b2ae94c24030c00e0a3f95516b32c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type marketplaceProductSubscriptionDetails_rootQuery$variables = {
  subscriptionId: string;
};
export type marketplaceProductSubscriptionDetails_rootQuery$data = {
  readonly marketplaceBookingSubscription: {
    readonly autoRenew: boolean;
    readonly cancelAtPeriodEnd: boolean;
    readonly id: string;
    readonly involvedCustomers: ReadonlyArray<{
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly id: string;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
    }>;
    readonly nextRenewalAt: any | null | undefined;
    readonly recurringBookings: ReadonlyArray<{
      readonly endDate: any | null | undefined;
      readonly id: string;
      readonly marketplaceBooking: {
        readonly bookingCheckoutSession: {
          readonly checkoutUrl: string;
        } | null | undefined;
        readonly id: string;
        readonly invoiceUrl: string | null | undefined;
        readonly isPaymentRequired: boolean;
        readonly paymentExpiry: any;
        readonly paymentMethod: {
          readonly name: string;
          readonly type: PaymentMethod;
        };
        readonly paymentStatus: {
          readonly name: string;
          readonly type: PaymentStatus;
        };
        readonly productVersion: {
          readonly featureImages: ReadonlyArray<{
            readonly original: {
              readonly url: string;
            } | null | undefined;
          }>;
          readonly id: string;
          readonly listingMetadata: {
            readonly about: string | null | undefined;
            readonly includedFeatures: ReadonlyArray<string> | null | undefined;
            readonly subTitle: string | null | undefined;
            readonly title: string | null | undefined;
          };
          readonly organization: {
            readonly customerFacingTermsAndConditionsUrl: string | null | undefined;
          };
        };
        readonly quantity: number;
      } | null | undefined;
      readonly startDate: any;
    }>;
    readonly startedAt: any;
    readonly status: {
      readonly name: string;
      readonly type: MarketplaceBookingSubscriptionStatus;
    };
  } | null | undefined;
};
export type marketplaceProductSubscriptionDetails_rootQuery = {
  response: marketplaceProductSubscriptionDetails_rootQuery$data;
  variables: marketplaceProductSubscriptionDetails_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "subscriptionId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "subscriptionId"
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
  "name": "startedAt",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "nextRenewalAt",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "autoRenew",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cancelAtPeriodEnd",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v8 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v7/*: any*/)
],
v9 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v8/*: any*/),
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "involvedCustomers",
  "plural": true,
  "selections": [
    (v2/*: any*/),
    (v7/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "givenName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "middleName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "familyName",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "startDate",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "endDate",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isPaymentRequired",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentExpiry",
  "storageKey": null
},
v17 = {
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
    },
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
    }
  ],
  "storageKey": null
},
v18 = {
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
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "customerFacingTermsAndConditionsUrl",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingCheckoutSessionDetails",
  "kind": "LinkedField",
  "name": "bookingCheckoutSession",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "checkoutUrl",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v8/*: any*/),
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v8/*: any*/),
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceProductSubscriptionDetails_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "MarketplaceBookingSubscriptionDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscription",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/),
          (v5/*: any*/),
          (v6/*: any*/),
          (v9/*: any*/),
          (v10/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "RecurringBookingDetails",
            "kind": "LinkedField",
            "name": "recurringBookings",
            "plural": true,
            "selections": [
              (v2/*: any*/),
              (v11/*: any*/),
              (v12/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingDetails",
                "kind": "LinkedField",
                "name": "marketplaceBooking",
                "plural": false,
                "selections": [
                  (v2/*: any*/),
                  (v13/*: any*/),
                  (v14/*: any*/),
                  (v15/*: any*/),
                  (v16/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ProductVersionDetails",
                    "kind": "LinkedField",
                    "name": "productVersion",
                    "plural": false,
                    "selections": [
                      (v2/*: any*/),
                      (v17/*: any*/),
                      (v18/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationDetails",
                        "kind": "LinkedField",
                        "name": "organization",
                        "plural": false,
                        "selections": [
                          (v19/*: any*/)
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  (v20/*: any*/),
                  (v21/*: any*/),
                  (v22/*: any*/)
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
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "marketplaceProductSubscriptionDetails_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "MarketplaceBookingSubscriptionDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscription",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/),
          (v5/*: any*/),
          (v6/*: any*/),
          (v9/*: any*/),
          (v10/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "RecurringBookingDetails",
            "kind": "LinkedField",
            "name": "recurringBookings",
            "plural": true,
            "selections": [
              (v2/*: any*/),
              (v11/*: any*/),
              (v12/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingDetails",
                "kind": "LinkedField",
                "name": "marketplaceBooking",
                "plural": false,
                "selections": [
                  (v2/*: any*/),
                  (v13/*: any*/),
                  (v14/*: any*/),
                  (v15/*: any*/),
                  (v16/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ProductVersionDetails",
                    "kind": "LinkedField",
                    "name": "productVersion",
                    "plural": false,
                    "selections": [
                      (v2/*: any*/),
                      (v17/*: any*/),
                      (v18/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationDetails",
                        "kind": "LinkedField",
                        "name": "organization",
                        "plural": false,
                        "selections": [
                          (v19/*: any*/),
                          (v2/*: any*/)
                        ],
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  (v20/*: any*/),
                  (v21/*: any*/),
                  (v22/*: any*/)
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
  },
  "params": {
    "cacheID": "69527777f3b0386f01f840446c30a4c5",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductSubscriptionDetails_rootQuery",
    "operationKind": "query",
    "text": "query marketplaceProductSubscriptionDetails_rootQuery(\n  $subscriptionId: String!\n) {\n  marketplaceBookingSubscription(id: $subscriptionId) {\n    id\n    startedAt\n    nextRenewalAt\n    autoRenew\n    cancelAtPeriodEnd\n    status {\n      type\n      name\n    }\n    involvedCustomers {\n      id\n      name\n      givenName\n      middleName\n      familyName\n    }\n    recurringBookings {\n      id\n      startDate\n      endDate\n      marketplaceBooking {\n        id\n        quantity\n        invoiceUrl\n        isPaymentRequired\n        paymentExpiry\n        productVersion {\n          id\n          listingMetadata {\n            title\n            subTitle\n            about\n            includedFeatures\n          }\n          featureImages {\n            original {\n              url\n            }\n          }\n          organization {\n            customerFacingTermsAndConditionsUrl\n            id\n          }\n        }\n        bookingCheckoutSession {\n          checkoutUrl\n        }\n        paymentMethod {\n          type\n          name\n        }\n        paymentStatus {\n          type\n          name\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1d1b71abce7c0a594a6be379a3a6f965";

export default node;
