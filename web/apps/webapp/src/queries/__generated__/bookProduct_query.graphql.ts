/**
 * @generated SignedSource<<e21a32068c0952d2d7fc5fbe17175708>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type ProductPricingCadence = "DAILY_V1" | "MONTHLY_V1" | "NOT_SET" | "ONE_TIME_V1" | "PER_HOUR_V1" | "PER_MINUTE_V1" | "WEEKLY_V1" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type bookProduct_query$data = {
  readonly bookingSlotSizeInMinutes: number;
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly me: {
    readonly emails: ReadonlyArray<string>;
    readonly id: string;
  };
  readonly organization: {
    readonly taxDetails: {
      readonly taxId: string;
      readonly taxRatePercentage: any;
    } | null | undefined;
  } | null | undefined;
  readonly product: {
    readonly currency: {
      readonly name: string;
      readonly type: Currency;
    };
    readonly description: string | null | undefined;
    readonly id: string;
    readonly latestProductVersionId: string;
    readonly name: string;
    readonly pricingOptions: ReadonlyArray<{
      readonly acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
      readonly cadence: ProductPricingCadence;
      readonly description: string;
      readonly id: string;
      readonly index: number;
      readonly isTaxInclusive: boolean;
      readonly maxAllowedResourcesLockTimePaidViaBankTransfer: number;
      readonly maxAllowedResourcesLockTimePaidViaCard: number;
      readonly maxDurationMinutes: number | null | undefined;
      readonly minDurationMinutes: number | null | undefined;
      readonly name: string;
      readonly numberOfResourcesToBook: number;
      readonly price: any;
    }>;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesUserEmails_query" | "singleChoiceBookingPaymentMethodType_query" | "singleChoiceMarketplaceBookingCategory_query">;
  readonly " $fragmentType": "bookProduct_query";
};
export type bookProduct_query$key = {
  readonly " $data"?: bookProduct_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookProduct_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
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
  "name": "description",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v1/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    },
    {
      "kind": "RootArgument",
      "name": "productId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "bookProduct_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "emails",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "uniqueAlphanumericName",
          "variableName": "organizationUniqueAlphanumericName"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTaxDetails",
          "kind": "LinkedField",
          "name": "taxDetails",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "taxId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "taxRatePercentage",
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
        (v0/*: any*/),
        (v1/*: any*/),
        (v2/*: any*/),
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
          "name": "latestProductVersionId",
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
            (v0/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "index",
              "storageKey": null
            },
            (v1/*: any*/),
            (v2/*: any*/),
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
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currencies",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "bookingSlotSizeInMinutes",
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceMarketplaceBookingCategory_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceBookingPaymentMethodType_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesUserEmails_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "0e2b746ab2000b90a1e0d276091d0929";

export default node;
