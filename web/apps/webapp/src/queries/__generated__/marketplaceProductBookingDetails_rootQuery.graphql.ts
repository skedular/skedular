/**
 * @generated SignedSource<<4bcbc685ec3723eb8f767098888d1e88>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type marketplaceProductBookingDetails_rootQuery$variables = {
  bookingId: string;
};
export type marketplaceProductBookingDetails_rootQuery$data = {
  readonly booking: {
    readonly bookingResources: ReadonlyArray<{
      readonly resource: {
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly from: any;
    readonly id: string;
    readonly involvedCustomers: ReadonlyArray<{
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly id: string;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
    }>;
    readonly involvedLocations: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
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
        readonly listingMetadata: {
          readonly about: string | null | undefined;
          readonly includedFeatures: ReadonlyArray<string> | null | undefined;
          readonly subTitle: string | null | undefined;
          readonly title: string | null | undefined;
        };
      };
      readonly quantity: number;
    } | null | undefined;
    readonly until: any;
  } | null | undefined;
};
export type marketplaceProductBookingDetails_rootQuery = {
  response: marketplaceProductBookingDetails_rootQuery$data;
  variables: marketplaceProductBookingDetails_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "bookingId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "bookingId"
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
  "name": "from",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "involvedCustomers",
  "plural": true,
  "selections": [
    (v2/*: any*/),
    (v5/*: any*/),
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
v7 = [
  (v2/*: any*/),
  (v5/*: any*/)
],
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "LocationDetails",
  "kind": "LinkedField",
  "name": "involvedLocations",
  "plural": true,
  "selections": (v7/*: any*/),
  "storageKey": null
},
v9 = {
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
      "selections": (v7/*: any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "quantity",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isPaymentRequired",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "paymentExpiry",
  "storageKey": null
},
v14 = {
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
v15 = {
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
v16 = {
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
v17 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v5/*: any*/)
],
v18 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethod",
  "plural": false,
  "selections": (v17/*: any*/),
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v17/*: any*/),
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceProductBookingDetails_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/),
          (v6/*: any*/),
          (v8/*: any*/),
          (v9/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              (v10/*: any*/),
              (v11/*: any*/),
              (v12/*: any*/),
              (v13/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductVersionDetails",
                "kind": "LinkedField",
                "name": "productVersion",
                "plural": false,
                "selections": [
                  (v14/*: any*/),
                  (v15/*: any*/)
                ],
                "storageKey": null
              },
              (v16/*: any*/),
              (v18/*: any*/),
              (v19/*: any*/)
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
    "name": "marketplaceProductBookingDetails_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/),
          (v6/*: any*/),
          (v8/*: any*/),
          (v9/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              (v10/*: any*/),
              (v11/*: any*/),
              (v12/*: any*/),
              (v13/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductVersionDetails",
                "kind": "LinkedField",
                "name": "productVersion",
                "plural": false,
                "selections": [
                  (v14/*: any*/),
                  (v15/*: any*/),
                  (v2/*: any*/)
                ],
                "storageKey": null
              },
              (v16/*: any*/),
              (v18/*: any*/),
              (v19/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "cc464439719f8ddc3b6bb628b24d75b9",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductBookingDetails_rootQuery",
    "operationKind": "query",
    "text": "query marketplaceProductBookingDetails_rootQuery(\n  $bookingId: String!\n) {\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    involvedCustomers {\n      id\n      name\n      givenName\n      middleName\n      familyName\n    }\n    involvedLocations {\n      id\n      name\n    }\n    bookingResources {\n      resource {\n        id\n        name\n      }\n    }\n    marketplaceBooking {\n      id\n      quantity\n      invoiceUrl\n      isPaymentRequired\n      paymentExpiry\n      productVersion {\n        listingMetadata {\n          title\n          subTitle\n          about\n          includedFeatures\n        }\n        featureImages {\n          original {\n            url\n          }\n        }\n        id\n      }\n      bookingCheckoutSession {\n        checkoutUrl\n      }\n      paymentMethod {\n        type\n        name\n      }\n      paymentStatus {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "734a2c2765b3b20dcbe5cfd5f30af053";

export default node;
