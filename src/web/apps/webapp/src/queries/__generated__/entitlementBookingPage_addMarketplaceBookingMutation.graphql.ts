/**
 * @generated SignedSource<<02ba267e18823b4af856f6aef3e5a65a>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type AddMarketplaceBookingInput = {
  category?: BookingCategory | null | undefined;
  checkoutReturnUrl?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  entitlementId?: string | null | undefined;
  from: any;
  id?: string | null | undefined;
  invoiceEmailList?: ReadonlyArray<string> | null | undefined;
  notes?: string | null | undefined;
  organizationCustomDomains?: ReadonlyArray<string> | null | undefined;
  organizationIds?: ReadonlyArray<string> | null | undefined;
  paymentMethod: PaymentMethod;
  pricingId: string;
  productVersionId: string;
  quantity: number;
  resourceIds?: ReadonlyArray<string> | null | undefined;
  teamIds?: ReadonlyArray<string> | null | undefined;
  until: any;
};
export type entitlementBookingPage_addMarketplaceBookingMutation$variables = {
  input: AddMarketplaceBookingInput;
};
export type entitlementBookingPage_addMarketplaceBookingMutation$data = {
  readonly addMarketplaceBooking: {
    readonly booking: {
      readonly id: string;
    } | null | undefined;
  };
};
export type entitlementBookingPage_addMarketplaceBookingMutation = {
  response: entitlementBookingPage_addMarketplaceBookingMutation$data;
  variables: entitlementBookingPage_addMarketplaceBookingMutation$variables;
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
    "concreteType": "BookingPayload",
    "kind": "LinkedField",
    "name": "addMarketplaceBooking",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
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
    "name": "entitlementBookingPage_addMarketplaceBookingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "entitlementBookingPage_addMarketplaceBookingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "97e43e73ee7347d3fd4443b7bd366eac",
    "id": null,
    "metadata": {},
    "name": "entitlementBookingPage_addMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation entitlementBookingPage_addMarketplaceBookingMutation(\n  $input: AddMarketplaceBookingInput!\n) {\n  addMarketplaceBooking(input: $input) {\n    booking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "666294f0e373c90d20a312a9d49cc2b3";

export default node;
