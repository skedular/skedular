/**
 * @generated SignedSource<<ee72fc8a737777287b0cb2abe80b2e08>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type BookingFrequency = "DAILY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type RecurringBookingEndType = "AFTER_OCCURRENCES" | "NEVER" | "UNTIL_DATE" | "%future added value";
export type AddMarketplaceRecurringBookingInput = {
  byMonthDay?: number | null | undefined;
  bySetPosition?: number | null | undefined;
  byWeekDays: ReadonlyArray<DayOfWeek>;
  category?: BookingCategory | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  endDate?: any | null | undefined;
  endType: RecurringBookingEndType;
  frequency: BookingFrequency;
  from: any;
  id?: string | null | undefined;
  interval: number;
  invoiceEmailList?: ReadonlyArray<string> | null | undefined;
  occurrenceCount?: number | null | undefined;
  organizationCustomDomains?: ReadonlyArray<string> | null | undefined;
  organizationIds?: ReadonlyArray<string> | null | undefined;
  paymentMethod: PaymentMethod;
  pricingId: string;
  productVersionId: string;
  quantity: number;
  skippedDates?: ReadonlyArray<any> | null | undefined;
  startDate: any;
  teamIds?: ReadonlyArray<string> | null | undefined;
  until: any;
};
export type bookProduct_addMarketplaceRecurringBookingMutation$variables = {
  input: AddMarketplaceRecurringBookingInput;
};
export type bookProduct_addMarketplaceRecurringBookingMutation$data = {
  readonly addMarketplaceRecurringBooking: {
    readonly recurringBooking: {
      readonly id: string;
    };
  };
};
export type bookProduct_addMarketplaceRecurringBookingMutation$rawResponse = {
  readonly addMarketplaceRecurringBooking: {
    readonly recurringBooking: {
      readonly id: string;
    };
  };
};
export type bookProduct_addMarketplaceRecurringBookingMutation = {
  rawResponse: bookProduct_addMarketplaceRecurringBookingMutation$rawResponse;
  response: bookProduct_addMarketplaceRecurringBookingMutation$data;
  variables: bookProduct_addMarketplaceRecurringBookingMutation$variables;
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
    "concreteType": "RecurringBookingPayload",
    "kind": "LinkedField",
    "name": "addMarketplaceRecurringBooking",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "RecurringBookingDetails",
        "kind": "LinkedField",
        "name": "recurringBooking",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "bookProduct_addMarketplaceRecurringBookingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookProduct_addMarketplaceRecurringBookingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "123d8676ed507fba9a95154b65bbdaaf",
    "id": null,
    "metadata": {},
    "name": "bookProduct_addMarketplaceRecurringBookingMutation",
    "operationKind": "mutation",
    "text": "mutation bookProduct_addMarketplaceRecurringBookingMutation(\n  $input: AddMarketplaceRecurringBookingInput!\n) {\n  addMarketplaceRecurringBooking(input: $input) {\n    recurringBooking {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "362cccc4a9aaed5b21e9a6095a8b367f";

export default node;
