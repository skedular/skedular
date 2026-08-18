/**
 * @generated SignedSource<<21407181ff63503e160838e9de353ba1>>
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
export type operatorMarketplaceBookingDialog_addMarketplaceBookingMutation$variables = {
  input: AddMarketplaceBookingInput;
};
export type operatorMarketplaceBookingDialog_addMarketplaceBookingMutation$data = {
  readonly addMarketplaceBooking: {
    readonly accessError: {
      readonly message: string;
    } | null | undefined;
    readonly booking: {
      readonly id: string;
    } | null | undefined;
  };
};
export type operatorMarketplaceBookingDialog_addMarketplaceBookingMutation = {
  response: operatorMarketplaceBookingDialog_addMarketplaceBookingMutation$data;
  variables: operatorMarketplaceBookingDialog_addMarketplaceBookingMutation$variables;
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
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "SpacesAccessErrorDetails",
        "kind": "LinkedField",
        "name": "accessError",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "message",
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
    "name": "operatorMarketplaceBookingDialog_addMarketplaceBookingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "operatorMarketplaceBookingDialog_addMarketplaceBookingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "714000d86eb046eb3d9b8dca812e7120",
    "id": null,
    "metadata": {},
    "name": "operatorMarketplaceBookingDialog_addMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation operatorMarketplaceBookingDialog_addMarketplaceBookingMutation(\n  $input: AddMarketplaceBookingInput!\n) {\n  addMarketplaceBooking(input: $input) {\n    booking {\n      id\n    }\n    accessError {\n      message\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "639dca28b5455c9eeeb7c558ffc31998";

export default node;
