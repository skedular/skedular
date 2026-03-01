/**
 * @generated SignedSource<<0bcbbd3be3b24de645fc24497f8c9f43>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PriceUnit = "PER_HOUR" | "PER_MINUTE" | "PER_USE" | "%future added value";
export type ProductVersionPricingCadence = "DAILY_V1" | "MONTHLY_V1" | "ONE_TIME_V1" | "PER_MINUTE_V1" | "WEEKLY_V1" | "%future added value";
export type UpdateProductInput = {
  acceptedBookingPaymentMethods: ReadonlyArray<PaymentMethod>;
  bookAllLocationResources: boolean;
  clientMutationId?: string | null | undefined;
  currency: Currency;
  description?: string | null | undefined;
  featureImages: ReadonlyArray<CdnImageFileInput>;
  id: string;
  isPriceTaxInclusive: boolean;
  locationTagIds: ReadonlyArray<string>;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  name: string;
  numberOfResourcesToBook: number;
  price: string;
  priceUnit: PriceUnit;
  pricingOptions: ReadonlyArray<ProductVersionPricingOptionsInput>;
  productTagIds: ReadonlyArray<string>;
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
export type ProductVersionPricingOptionsInput = {
  cadence: ProductVersionPricingCadence;
  dailyV1?: ProductVersionDailyPricingV1Input | null | undefined;
  monthlyV1?: ProductVersionMonthlyPricingV1Input | null | undefined;
  oneTimeV1?: ProductVersionOneTimePricingV1Input | null | undefined;
  perMinuteV1?: ProductVersionPerMinutePricingV1Input | null | undefined;
  weeklyV1?: ProductVersionWeeklyPricingV1Input | null | undefined;
};
export type ProductVersionDailyPricingV1Input = {
  acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
  currency: Currency;
  description: string;
  index: number;
  isTaxInclusive: boolean;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  name: string;
  price: any;
};
export type ProductVersionMonthlyPricingV1Input = {
  acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
  currency: Currency;
  description: string;
  index: number;
  isTaxInclusive: boolean;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  name: string;
  price: any;
};
export type ProductVersionOneTimePricingV1Input = {
  acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
  currency: Currency;
  description: string;
  index: number;
  isTaxInclusive: boolean;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  name: string;
  price: any;
};
export type ProductVersionPerMinutePricingV1Input = {
  acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
  currency: Currency;
  description: string;
  index: number;
  isTaxInclusive: boolean;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  name: string;
  price: any;
};
export type ProductVersionWeeklyPricingV1Input = {
  acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
  currency: Currency;
  description: string;
  index: number;
  isTaxInclusive: boolean;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  name: string;
  price: any;
};
export type editProduct_updateProductMutation$variables = {
  input: UpdateProductInput;
};
export type editProduct_updateProductMutation$data = {
  readonly updateProduct: {
    readonly product: {
      readonly acceptedBookingPaymentMethods: ReadonlyArray<{
        readonly type: PaymentMethod;
      }>;
      readonly bookAllLocationResources: boolean;
      readonly currency: {
        readonly name: string;
        readonly type: Currency;
      };
      readonly description: string | null | undefined;
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
      readonly isPriceTaxInclusive: boolean;
      readonly locationTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly maxAllowedResourcesLockTimePaidViaBankTransfer: number;
      readonly maxAllowedResourcesLockTimePaidViaCard: number;
      readonly maxDurationMinutes: number | null | undefined;
      readonly minDurationMinutes: number | null | undefined;
      readonly name: string;
      readonly numberOfResourcesToBook: number;
      readonly price: string;
      readonly priceUnit: {
        readonly name: string;
        readonly type: PriceUnit;
      };
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
    };
  };
};
export type editProduct_updateProductMutation$rawResponse = {
  readonly updateProduct: {
    readonly product: {
      readonly acceptedBookingPaymentMethods: ReadonlyArray<{
        readonly type: PaymentMethod;
      }>;
      readonly bookAllLocationResources: boolean;
      readonly currency: {
        readonly name: string;
        readonly type: Currency;
      };
      readonly description: string | null | undefined;
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
      readonly isPriceTaxInclusive: boolean;
      readonly locationTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly maxAllowedResourcesLockTimePaidViaBankTransfer: number;
      readonly maxAllowedResourcesLockTimePaidViaCard: number;
      readonly maxDurationMinutes: number | null | undefined;
      readonly minDurationMinutes: number | null | undefined;
      readonly name: string;
      readonly numberOfResourcesToBook: number;
      readonly price: string;
      readonly priceUnit: {
        readonly name: string;
        readonly type: PriceUnit;
      };
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
    };
  };
};
export type editProduct_updateProductMutation = {
  rawResponse: editProduct_updateProductMutation$rawResponse;
  response: editProduct_updateProductMutation$data;
  variables: editProduct_updateProductMutation$variables;
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
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v4 = [
  (v3/*: any*/),
  (v2/*: any*/)
],
v5 = [
  (v1/*: any*/),
  (v2/*: any*/),
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
    "name": "updateProduct",
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
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "description",
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
            "concreteType": "PriceUnitDetails",
            "kind": "LinkedField",
            "name": "priceUnit",
            "plural": false,
            "selections": (v4/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CurrencyDetails",
            "kind": "LinkedField",
            "name": "currency",
            "plural": false,
            "selections": (v4/*: any*/),
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
            "name": "bookAllLocationResources",
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
            "name": "locationTags",
            "plural": true,
            "selections": (v5/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Marketplace_PaymentMethodTypeDetails",
            "kind": "LinkedField",
            "name": "acceptedBookingPaymentMethods",
            "plural": true,
            "selections": [
              (v3/*: any*/)
            ],
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
            "kind": "ScalarField",
            "name": "isPriceTaxInclusive",
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
    "name": "editProduct_updateProductMutation",
    "selections": (v7/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editProduct_updateProductMutation",
    "selections": (v7/*: any*/)
  },
  "params": {
    "cacheID": "351b95f06adf580ebb0fe1b73cb6b67a",
    "id": null,
    "metadata": {},
    "name": "editProduct_updateProductMutation",
    "operationKind": "mutation",
    "text": "mutation editProduct_updateProductMutation(\n  $input: UpdateProductInput!\n) {\n  updateProduct(input: $input) {\n    product {\n      id\n      inactive\n      name\n      description\n      price\n      priceUnit {\n        type\n        name\n      }\n      currency {\n        type\n        name\n      }\n      numberOfResourcesToBook\n      minDurationMinutes\n      maxDurationMinutes\n      bookAllLocationResources\n      productTags {\n        id\n        name\n        color\n      }\n      locationTags {\n        id\n        name\n        color\n      }\n      acceptedBookingPaymentMethods {\n        type\n      }\n      maxAllowedResourcesLockTimePaidViaCard\n      maxAllowedResourcesLockTimePaidViaBankTransfer\n      featureImages {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n      isPriceTaxInclusive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d7c1b9ed8553d5e73d19d6883ac7e5be";

export default node;
