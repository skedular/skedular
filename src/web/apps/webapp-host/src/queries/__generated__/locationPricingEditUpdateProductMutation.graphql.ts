/**
 * @generated SignedSource<<6f53b2f7fe24da3c0fb04cb5c6116676>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type DurationDisplayUnit = "HOURS" | "MINUTES" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type ProductPatchField = "CURRENCY" | "FEATURE_IMAGES" | "LISTING_METADATA" | "PRICING_OPTIONS" | "TAGS" | "TYPE" | "%future added value";
export type ProductPricingBillingMode = "IN_ARREARS" | "NOT_SET" | "UPFRONT" | "%future added value";
export type ProductPricingCadence = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "HALF_DAY" | "MONTHLY" | "NOT_SET" | "ONE_TIME" | "PER15_MINUTES" | "PER30_MINUTES" | "PER_HOUR" | "PER_MINUTE" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
export type ProductPricingCancellationPolicyType = "FULL_REFUND_BEFORE_CUTOFF" | "NOT_SET" | "NO_CANCELLATION" | "TIERED_REFUND" | "%future added value";
export type ProductPricingFulfillmentType = "ENTITLEMENT" | "RESERVATION" | "%future added value";
export type ProductType = "EVENT" | "RESOURCE" | "%future added value";
export type UpdateProductInput = {
  clientMutationId?: string | null | undefined;
  currency: Currency;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  fieldsToUpdate: ReadonlyArray<ProductPatchField>;
  id: string;
  listingMetadata?: ListingMetadataInput | null | undefined;
  pricingOptions: ReadonlyArray<ProductPricingInput>;
  tagIds: ReadonlyArray<string>;
  type: ProductType;
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
  availableDays?: ReadonlyArray<DayOfWeek> | null | undefined;
  billingMode: ProductPricingBillingMode;
  bookingCadence: ProductPricingCadence;
  cancellationPolicyType: ProductPricingCancellationPolicyType;
  cancellationRefundRules: ReadonlyArray<ProductPricingCancellationRefundRuleInput>;
  entitlementCreditQuantity?: number | null | undefined;
  entitlementValidityDays?: number | null | undefined;
  fulfillmentType?: ProductPricingFulfillmentType;
  id: string;
  index: number;
  isTaxInclusive: boolean;
  listingMetadata: ListingMetadataInput;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit?: DurationDisplayUnit | null | undefined;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxAllowedResourcesLockTimePaidViaCardDisplayUnit?: DurationDisplayUnit | null | undefined;
  maxDurationDisplayUnit?: DurationDisplayUnit | null | undefined;
  maxDurationMinutes?: number | null | undefined;
  minDurationDisplayUnit?: DurationDisplayUnit | null | undefined;
  minDurationMinutes?: number | null | undefined;
  numberOfResourcesToBook: number;
  price: any;
  purchaseCadence: ProductPricingCadence;
  requiredDaysPerWeek?: number | null | undefined;
  supportsSubscriptionAutoRenewal: boolean;
};
export type ProductPricingCancellationRefundRuleInput = {
  displayUnit?: DurationDisplayUnit | null | undefined;
  minutesBefore: number;
  refundPercentage: number;
};
export type locationPricingEditUpdateProductMutation$variables = {
  input: UpdateProductInput;
};
export type locationPricingEditUpdateProductMutation$data = {
  readonly updateProduct: {
    readonly product: {
      readonly id: string;
    };
  };
};
export type locationPricingEditUpdateProductMutation = {
  response: locationPricingEditUpdateProductMutation$data;
  variables: locationPricingEditUpdateProductMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "locationPricingEditUpdateProductMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "locationPricingEditUpdateProductMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "83e4b565ebbbea9d6ccd865cf3604d61",
    "id": null,
    "metadata": {},
    "name": "locationPricingEditUpdateProductMutation",
    "operationKind": "mutation",
    "text": "mutation locationPricingEditUpdateProductMutation(\n  $input: UpdateProductInput!\n) {\n  updateProduct(input: $input) {\n    product {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e9aa86aa496d8cedba4e3aa36a4d2def";

export default node;
