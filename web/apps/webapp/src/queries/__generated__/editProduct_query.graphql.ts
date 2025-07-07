/**
 * @generated SignedSource<<4ec62d4570c48b6c489f97f9fd06fd46>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingPaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type Currency = "NZD" | "USD" | "%future added value";
export type PriceUnit = "PER_HOUR" | "PER_MINUTE" | "PER_USE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type editProduct_query$data = {
  readonly defaultMaxAllowedResourcesLockTimePaidViaBankTransfer: number;
  readonly defaultMaxAllowedResourcesLockTimePaidViaCard: number;
  readonly openingHoursMinutesStep: number;
  readonly product: {
    readonly acceptedBookingPaymentMethods: ReadonlyArray<{
      readonly type: BookingPaymentMethod;
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
    readonly organization: {
      readonly uniqueId: string;
    };
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
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesBookingPaymentMethodTypes_query" | "multipleChoicesLocationTags_query" | "multipleChoicesProductTags_query" | "singleChoiceCurrency_query" | "singleChoicePriceUnit_query">;
  readonly " $fragmentType": "editProduct_query";
};
export type editProduct_query$key = {
  readonly " $data"?: editProduct_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editProduct_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v2 = [
  (v1/*: any*/),
  (v0/*: any*/)
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v4 = [
  (v3/*: any*/),
  (v0/*: any*/),
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
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "productId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "editProduct_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "productId"
        }
      ],
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
        (v0/*: any*/),
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
          "concreteType": "Marketplace_OrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
          "plural": false,
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
          "concreteType": "Marketplace_BookingPaymentMethodTypeDetails",
          "kind": "LinkedField",
          "name": "acceptedBookingPaymentMethods",
          "plural": true,
          "selections": [
            (v1/*: any*/)
          ],
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
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "openingHoursMinutesStep",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "defaultMaxAllowedResourcesLockTimePaidViaCard",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "defaultMaxAllowedResourcesLockTimePaidViaBankTransfer",
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesProductTags_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesLocationTags_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoicePriceUnit_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceCurrency_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesBookingPaymentMethodTypes_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "11557ab380b6da04f8a5ef2757d16684";

export default node;
