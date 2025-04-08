/**
 * @generated SignedSource<<cdb34e899e61501749c9fa418934b5b4>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "Nzd" | "Usd" | "%future added value";
export type PriceUnit = "PerHour" | "PerMinute" | "PerUse" | "%future added value";
export type UpdateProductInput = {
  bookAllLocationResources: boolean;
  clientMutationId?: string | null | undefined;
  currency: Currency;
  description?: string | null | undefined;
  id: string;
  locationTagIds: ReadonlyArray<string>;
  maxBookingSpreadDays?: number | null | undefined;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  name: string;
  numberOfResourcesToBook: number;
  organizationId: string;
  price: string;
  priceUnit: PriceUnit;
  productTagIds: ReadonlyArray<string>;
  recurrenceWindowDays: number;
  requireConsecutiveDays: boolean;
};
export type editProduct_updateProductMutation$variables = {
  input: UpdateProductInput;
};
export type editProduct_updateProductMutation$data = {
  readonly updateProduct: {
    readonly product: {
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
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly recurrenceWindowDays: number;
      readonly requireConsecutiveDays: boolean;
    };
  } | null | undefined;
};
export type editProduct_updateProductMutation$rawResponse = {
  readonly updateProduct: {
    readonly product: {
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
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly recurrenceWindowDays: number;
      readonly requireConsecutiveDays: boolean;
    };
  } | null | undefined;
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
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v1/*: any*/)
],
v3 = [
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
v4 = [
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
            "selections": (v2/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CurrencyDetails",
            "kind": "LinkedField",
            "name": "currency",
            "plural": false,
            "selections": (v2/*: any*/),
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
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Marketplace_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "locationTags",
            "plural": true,
            "selections": (v3/*: any*/),
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
    "selections": (v4/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editProduct_updateProductMutation",
    "selections": (v4/*: any*/)
  },
  "params": {
    "cacheID": "c64a8f45259f5eb94af1b81c5fa643a1",
    "id": null,
    "metadata": {},
    "name": "editProduct_updateProductMutation",
    "operationKind": "mutation",
    "text": "mutation editProduct_updateProductMutation(\n  $input: UpdateProductInput!\n) {\n  updateProduct(input: $input) {\n    product {\n      id\n      inactive\n      name\n      description\n      price\n      priceUnit {\n        type\n        name\n      }\n      currency {\n        type\n        name\n      }\n      numberOfResourcesToBook\n      minDurationMinutes\n      maxDurationMinutes\n      bookAllLocationResources\n      recurrenceWindowDays\n      requireConsecutiveDays\n      maxBookingSpreadDays\n      productTags {\n        uniqueId\n        name\n        color\n      }\n      locationTags {\n        uniqueId\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0852dde3cf5db088b9561150386be4f5";

export default node;
