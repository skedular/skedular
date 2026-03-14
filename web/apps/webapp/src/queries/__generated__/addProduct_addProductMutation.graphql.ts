/**
 * @generated SignedSource<<05a7899887885cfefcfa106bf74d7a6c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type ProductPricingCadence = "DAILY_V1" | "FIVE_MONTHS_V1" | "FOUR_MONTHS_V1" | "HALF_DAY_V1" | "MONTHLY_V1" | "NOT_SET" | "ONE_TIME_V1" | "PER15_MINUTE_V1" | "PER30_MINUTE_V1" | "PER_HOUR_V1" | "PER_MINUTE_V1" | "QUARTERLY_V1" | "SIX_MONTHS_V1" | "TWO_MONTHS_V1" | "WEEKLY_V1" | "YEARLY_V1" | "%future added value";
export type AddProductInput = {
  clientMutationId?: string | null | undefined;
  currency: Currency;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  id?: string | null | undefined;
  listingMetadata?: ListingMetadataInput | null | undefined;
  organizationId?: string | null | undefined;
  organizationUniqueAlphanumericName?: string | null | undefined;
  pricingOptions: ReadonlyArray<ProductPricingInput>;
  tagIds: ReadonlyArray<string>;
};
export type CdnImageFileInput = {
  original?: CdnFileInput | null | undefined;
  thumbnail?: CdnFileInput | null | undefined;
};
export type CdnFileInput = {
  height?: number | null | undefined;
  url: string;
  width?: number | null | undefined;
};
export type ListingMetadataInput = {
  about?: string | null | undefined;
  includedFeatures?: ReadonlyArray<string> | null | undefined;
  subTitle?: string | null | undefined;
  title?: string | null | undefined;
};
export type ProductPricingInput = {
  acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
  cadence: ProductPricingCadence;
  id: string;
  index: number;
  isTaxInclusive: boolean;
  listingMetadata: ListingMetadataInput;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  numberOfResourcesToBook: number;
  price: any;
};
export type addProduct_addProductMutation$variables = {
  input: AddProductInput;
};
export type addProduct_addProductMutation$data = {
  readonly addProduct: {
    readonly product: {
      readonly amenities: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly currency: {
        readonly name: string;
        readonly type: Currency;
      };
      readonly featureImages: ReadonlyArray<{
        readonly original: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
        readonly thumbnail: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
      }>;
      readonly id: string;
      readonly inactive: boolean;
      readonly listingMetadata: {
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly pricingOptions: ReadonlyArray<{
        readonly acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
        readonly cadence: ProductPricingCadence;
        readonly index: number;
        readonly isTaxInclusive: boolean;
        readonly listingMetadata: {
          readonly subTitle: string | null | undefined;
          readonly title: string | null | undefined;
        };
        readonly maxAllowedResourcesLockTimePaidViaBankTransfer: number;
        readonly maxAllowedResourcesLockTimePaidViaCard: number;
        readonly maxDurationMinutes: number | null | undefined;
        readonly minDurationMinutes: number | null | undefined;
        readonly numberOfResourcesToBook: number;
        readonly price: any;
      }>;
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
    };
  };
};
export type addProduct_addProductMutation$rawResponse = {
  readonly addProduct: {
    readonly product: {
      readonly amenities: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly currency: {
        readonly name: string;
        readonly type: Currency;
      };
      readonly featureImages: ReadonlyArray<{
        readonly original: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
        readonly thumbnail: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
      }>;
      readonly id: string;
      readonly inactive: boolean;
      readonly listingMetadata: {
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly pricingOptions: ReadonlyArray<{
        readonly acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
        readonly cadence: ProductPricingCadence;
        readonly index: number;
        readonly isTaxInclusive: boolean;
        readonly listingMetadata: {
          readonly subTitle: string | null | undefined;
          readonly title: string | null | undefined;
        };
        readonly maxAllowedResourcesLockTimePaidViaBankTransfer: number;
        readonly maxAllowedResourcesLockTimePaidViaCard: number;
        readonly maxDurationMinutes: number | null | undefined;
        readonly minDurationMinutes: number | null | undefined;
        readonly numberOfResourcesToBook: number;
        readonly price: any;
      }>;
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
    };
  };
};
export type addProduct_addProductMutation = {
  rawResponse: addProduct_addProductMutation$rawResponse;
  response: addProduct_addProductMutation$data;
  variables: addProduct_addProductMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = [
  (v1/*: any*/),
  (v4/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
],
v6 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "url",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "height",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "width",
    "storageKey": null
  }
],
v7 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "ProductPayload",
    "kind": "LinkedField",
    "name": "addProduct",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          (v1/*: any*/),
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
              (v2/*: any*/),
              (v3/*: any*/),
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
          {
            "alias": null,
            "args": null,
            "concreteType": "CurrencyDetails",
            "kind": "LinkedField",
            "name": "currency",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "type",
                "storageKey": null
              },
              (v4/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": true,
            "selections": (v5/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "amenities",
            "plural": true,
            "selections": (v5/*: any*/),
            "storageKey": null
          },
          {
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
                "selections": (v6/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v6/*: any*/),
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
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "index",
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
                  (v2/*: any*/),
                  (v3/*: any*/)
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "cadence",
                "storageKey": null
              },
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
                "name": "numberOfResourcesToBook",
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
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "isTaxInclusive",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "maxAllowedResourcesLockTimePaidViaCard",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "maxAllowedResourcesLockTimePaidViaBankTransfer",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "acceptedPaymentMethods",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addProduct_addProductMutation",
    "selections": (v7/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addProduct_addProductMutation",
    "selections": (v7/*: any*/)
  },
  "params": {
    "cacheID": "3dfbdf4c66c5428dcaaed398762af662",
    "id": null,
    "metadata": {},
    "name": "addProduct_addProductMutation",
    "operationKind": "mutation",
    "text": "mutation addProduct_addProductMutation(\n  $input: AddProductInput!\n) {\n  addProduct(input: $input) {\n    product {\n      id\n      inactive\n      listingMetadata {\n        title\n        subTitle\n        includedFeatures\n      }\n      currency {\n        type\n        name\n      }\n      productTags {\n        id\n        name\n        color\n      }\n      amenities {\n        id\n        name\n        color\n      }\n      featureImages {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n      pricingOptions {\n        index\n        listingMetadata {\n          title\n          subTitle\n        }\n        cadence\n        price\n        numberOfResourcesToBook\n        minDurationMinutes\n        maxDurationMinutes\n        isTaxInclusive\n        maxAllowedResourcesLockTimePaidViaCard\n        maxAllowedResourcesLockTimePaidViaBankTransfer\n        acceptedPaymentMethods\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5df2af737947465819cb440e5a4d489b";

export default node;
