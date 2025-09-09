/**
 * @generated SignedSource<<296373b10b3648c278c9bdd65bb17cbd>>
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
export type UpdateProductInput = {
  acceptedBookingPaymentMethods: ReadonlyArray<PaymentMethod>;
  bookAllLocationResources: boolean;
  clientMutationId?: string | null | undefined;
  currency: Currency;
  description?: string | null | undefined;
  id: string;
  isPriceTaxInclusive: boolean;
  locationTagIds: ReadonlyArray<string>;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxBookingSpreadDays?: number | null | undefined;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  name: string;
  numberOfResourcesToBook: number;
  price: string;
  priceUnit: PriceUnit;
  primaryFeatureImage?: CdnImageFileInput | null | undefined;
  productTagIds: ReadonlyArray<string>;
  recurrenceWindowDays: number;
  requireConsecutiveDays: boolean;
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
      readonly maxBookingSpreadDays: number | null | undefined;
      readonly maxDurationMinutes: number | null | undefined;
      readonly minDurationMinutes: number | null | undefined;
      readonly name: string;
      readonly numberOfResourcesToBook: number;
      readonly price: string;
      readonly priceUnit: {
        readonly name: string;
        readonly type: PriceUnit;
      };
      readonly primaryFeatureImage: {
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
      } | null | undefined;
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly recurrenceWindowDays: number;
      readonly requireConsecutiveDays: boolean;
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
      readonly maxBookingSpreadDays: number | null | undefined;
      readonly maxDurationMinutes: number | null | undefined;
      readonly minDurationMinutes: number | null | undefined;
      readonly name: string;
      readonly numberOfResourcesToBook: number;
      readonly price: string;
      readonly priceUnit: {
        readonly name: string;
        readonly type: PriceUnit;
      };
      readonly primaryFeatureImage: {
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
      } | null | undefined;
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly recurrenceWindowDays: number;
      readonly requireConsecutiveDays: boolean;
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
            "kind": "ScalarField",
            "name": "recurrenceWindowDays",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "requireConsecutiveDays",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "maxBookingSpreadDays",
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
            "name": "primaryFeatureImage",
            "plural": false,
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
    "cacheID": "3294d2dfe60ae18ba3d3ea1385c69ca3",
    "id": null,
    "metadata": {},
    "name": "editProduct_updateProductMutation",
    "operationKind": "mutation",
    "text": "mutation editProduct_updateProductMutation(\n  $input: UpdateProductInput!\n) {\n  updateProduct(input: $input) {\n    product {\n      id\n      inactive\n      name\n      description\n      price\n      priceUnit {\n        type\n        name\n      }\n      currency {\n        type\n        name\n      }\n      numberOfResourcesToBook\n      minDurationMinutes\n      maxDurationMinutes\n      bookAllLocationResources\n      recurrenceWindowDays\n      requireConsecutiveDays\n      maxBookingSpreadDays\n      productTags {\n        id\n        name\n        color\n      }\n      locationTags {\n        id\n        name\n        color\n      }\n      acceptedBookingPaymentMethods {\n        type\n      }\n      maxAllowedResourcesLockTimePaidViaCard\n      maxAllowedResourcesLockTimePaidViaBankTransfer\n      primaryFeatureImage {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n      isPriceTaxInclusive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "35fed2c6f396ffd3ec89251e9f84f9ca";

export default node;
