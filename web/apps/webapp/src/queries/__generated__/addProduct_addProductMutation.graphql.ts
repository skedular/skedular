/**
 * @generated SignedSource<<dd301e2a32c9650c67763b17b2125eaa>>
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
export type AddProductInput = {
  acceptedBookingPaymentMethods: ReadonlyArray<PaymentMethod>;
  bookAllLocationResources: boolean;
  clientMutationId?: string | null | undefined;
  currency: Currency;
  description?: string | null | undefined;
  id?: string | null | undefined;
  locationTagIds: ReadonlyArray<string>;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxBookingSpreadDays?: number | null | undefined;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  name: string;
  numberOfResourcesToBook: number;
  organizationId: string;
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
export type addProduct_addProductMutation$variables = {
  input: AddProductInput;
};
export type addProduct_addProductMutation$data = {
  readonly addProduct: {
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
      readonly locationTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
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
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly recurrenceWindowDays: number;
      readonly requireConsecutiveDays: boolean;
    };
  };
};
export type addProduct_addProductMutation$rawResponse = {
  readonly addProduct: {
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
      readonly locationTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
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
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly recurrenceWindowDays: number;
      readonly requireConsecutiveDays: boolean;
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
  "name": "name",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v3 = [
  (v2/*: any*/),
  (v1/*: any*/)
],
v4 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v1/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
],
v5 = [
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
v6 = [
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "inactive",
            "storageKey": null
          },
          (v1/*: any*/),
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
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CurrencyDetails",
            "kind": "LinkedField",
            "name": "currency",
            "plural": false,
            "selections": (v3/*: any*/),
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
            "concreteType": "Marketplace_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": true,
            "selections": (v4/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Marketplace_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "locationTags",
            "plural": true,
            "selections": (v4/*: any*/),
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
              (v2/*: any*/)
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
                "selections": (v5/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v5/*: any*/),
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
    "selections": (v6/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addProduct_addProductMutation",
    "selections": (v6/*: any*/)
  },
  "params": {
    "cacheID": "8314a22cbcd0bd4f68d254374abb6ead",
    "id": null,
    "metadata": {},
    "name": "addProduct_addProductMutation",
    "operationKind": "mutation",
    "text": "mutation addProduct_addProductMutation(\n  $input: AddProductInput!\n) {\n  addProduct(input: $input) {\n    product {\n      id\n      inactive\n      name\n      description\n      price\n      priceUnit {\n        type\n        name\n      }\n      currency {\n        type\n        name\n      }\n      numberOfResourcesToBook\n      minDurationMinutes\n      maxDurationMinutes\n      bookAllLocationResources\n      recurrenceWindowDays\n      requireConsecutiveDays\n      maxBookingSpreadDays\n      productTags {\n        uniqueId\n        name\n        color\n      }\n      locationTags {\n        uniqueId\n        name\n        color\n      }\n      acceptedBookingPaymentMethods {\n        type\n      }\n      maxAllowedResourcesLockTimePaidViaCard\n      maxAllowedResourcesLockTimePaidViaBankTransfer\n      primaryFeatureImage {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "bec3ecfe9c626b62c58624b81076e62a";

export default node;
