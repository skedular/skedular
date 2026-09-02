/**
 * @generated SignedSource<<12c9f297314bcc6126267ffa586479dd>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type ProductPricingBillingMode = "IN_ARREARS" | "NOT_SET" | "UPFRONT" | "%future added value";
export type ProductPricingCadence = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "MONTHLY" | "NOT_SET" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
export type ProductPricingCancellationPolicyType = "FULL_REFUND_BEFORE_CUTOFF" | "NOT_SET" | "NO_CANCELLATION" | "TIERED_REFUND" | "%future added value";
export type hostListingQuery$variables = {
  locationId: string;
};
export type hostListingQuery$data = {
  readonly location: {
    readonly extraMetadata: {
      readonly peopleCapacity: {
        readonly from: string;
        readonly to: string;
      } | null | undefined;
    } | null | undefined;
    readonly id: string;
    readonly name: string;
    readonly physicalAddress: {
      readonly multilinesFormattedAddress: string | null | undefined;
    } | null | undefined;
    readonly products: ReadonlyArray<{
      readonly currency: {
        readonly name: string;
        readonly type: Currency;
      };
      readonly id: string;
      readonly inactive: boolean;
      readonly listingMetadata: {
        readonly about: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly pricingOptions: ReadonlyArray<{
        readonly billingMode: ProductPricingBillingMode;
        readonly cancellationPolicyType: ProductPricingCancellationPolicyType;
        readonly id: string;
        readonly maxDurationMinutes: number | null | undefined;
        readonly minDurationMinutes: number | null | undefined;
        readonly price: any;
        readonly purchaseCadence: ProductPricingCadence;
      }>;
    }>;
    readonly timezone: string | null | undefined;
  } | null | undefined;
};
export type hostListingQuery = {
  response: hostListingQuery$data;
  variables: hostListingQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "locationId"
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
  "name": "name",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "timezone",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "multilinesFormattedAddress",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "LocationExtraMetadata",
  "kind": "LinkedField",
  "name": "extraMetadata",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "PeopleCapacity",
      "kind": "LinkedField",
      "name": "peopleCapacity",
      "plural": false,
      "selections": [
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
          "name": "to",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "ProductDetails",
  "kind": "LinkedField",
  "name": "products",
  "plural": true,
  "selections": [
    (v2/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "inactive",
      "storageKey": null
    },
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
          "name": "about",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
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
          "kind": "ScalarField",
          "name": "price",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "purchaseCadence",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "billingMode",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "cancellationPolicyType",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "minDurationMinutes",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "maxDurationMinutes",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currency",
      "plural": false,
      "selections": [
        (v3/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
          "storageKey": null
        }
      ],
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
    "name": "hostListingQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationPhysicalAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              (v5/*:: as any*/)
            ],
            "storageKey": null
          },
          (v6/*:: as any*/),
          (v7/*:: as any*/)
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
    "name": "hostListingQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationPhysicalAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              (v5/*:: as any*/),
              (v2/*:: as any*/)
            ],
            "storageKey": null
          },
          (v6/*:: as any*/),
          (v7/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "2507c4c760b1b127dd363e49359238c9",
    "id": null,
    "metadata": {},
    "name": "hostListingQuery",
    "operationKind": "query",
    "text": "query hostListingQuery(\n  $locationId: String!\n) {\n  location(id: $locationId) {\n    id\n    name\n    timezone\n    physicalAddress {\n      multilinesFormattedAddress\n      id\n    }\n    extraMetadata {\n      peopleCapacity {\n        from\n        to\n      }\n    }\n    products {\n      id\n      inactive\n      listingMetadata {\n        title\n        about\n      }\n      pricingOptions {\n        id\n        price\n        purchaseCadence\n        billingMode\n        cancellationPolicyType\n        minDurationMinutes\n        maxDurationMinutes\n      }\n      currency {\n        name\n        type\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "69969b50e42b89d0a0a5f42908a7e1b7";

export default node;
