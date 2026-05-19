/**
 * @generated SignedSource<<2a5b568ac6a405079d6338f1e2e0d8b3>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type AddMarketplaceBookingInput = {
  category?: BookingCategory | null | undefined;
  checkoutReturnUrl?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
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
export type marketplaceProductBookingForm_addMarketplaceBookingMutation$variables = {
  input: AddMarketplaceBookingInput;
};
export type marketplaceProductBookingForm_addMarketplaceBookingMutation$data = {
  readonly addMarketplaceBooking: {
    readonly booking: {
      readonly from: any;
      readonly id: string;
      readonly marketplaceBooking: {
        readonly bookingCheckoutSession: {
          readonly checkoutUrl: string;
        } | null | undefined;
        readonly id: string;
        readonly invoiceNumber: string | null | undefined;
        readonly invoiceUrl: string | null | undefined;
        readonly isPaymentRequired: boolean;
        readonly paymentExpiry: any;
        readonly paymentMethod: {
          readonly name: string;
          readonly type: PaymentMethod;
        };
        readonly paymentStatus: {
          readonly name: string;
          readonly type: PaymentStatus;
        };
        readonly totalAmountToDisplay: string;
      } | null | undefined;
      readonly until: any;
    };
  };
};
export type marketplaceProductBookingForm_addMarketplaceBookingMutation = {
  response: marketplaceProductBookingForm_addMarketplaceBookingMutation$data;
  variables: marketplaceProductBookingForm_addMarketplaceBookingMutation$variables;
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
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "from",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "until",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "isPaymentRequired",
                "storageKey": null
              },
              (v1/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "paymentExpiry",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "invoiceUrl",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "invoiceNumber",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "totalAmountToDisplay",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingCheckoutSessionDetails",
                "kind": "LinkedField",
                "name": "bookingCheckoutSession",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "checkoutUrl",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PaymentMethodTypeDetails",
                "kind": "LinkedField",
                "name": "paymentMethod",
                "plural": false,
                "selections": [
                  (v2/*:: as any*/),
                  (v3/*:: as any*/)
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PaymentStatusDetails",
                "kind": "LinkedField",
                "name": "paymentStatus",
                "plural": false,
                "selections": [
                  (v3/*:: as any*/),
                  (v2/*:: as any*/)
                ],
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceProductBookingForm_addMarketplaceBookingMutation",
    "selections": (v4/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductBookingForm_addMarketplaceBookingMutation",
    "selections": (v4/*:: as any*/)
  },
  "params": {
    "cacheID": "faef7b90286017d48ccc900b7cc7ad08",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductBookingForm_addMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceProductBookingForm_addMarketplaceBookingMutation(\n  $input: AddMarketplaceBookingInput!\n) {\n  addMarketplaceBooking(input: $input) {\n    booking {\n      id\n      from\n      until\n      marketplaceBooking {\n        isPaymentRequired\n        id\n        paymentExpiry\n        invoiceUrl\n        invoiceNumber\n        totalAmountToDisplay\n        bookingCheckoutSession {\n          checkoutUrl\n        }\n        paymentMethod {\n          name\n          type\n        }\n        paymentStatus {\n          type\n          name\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4e17c2fa177958f33cb41036f939d47b";

export default node;
