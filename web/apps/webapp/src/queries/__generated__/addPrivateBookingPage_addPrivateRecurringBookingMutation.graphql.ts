/**
 * @generated SignedSource<<0e20b0b822ef880147419f6df0011c0a>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type BookingFrequency = "DAILY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type RecurringBookingEndType = "AFTER_OCCURRENCES" | "NEVER" | "UNTIL_DATE" | "%future added value";
export type AddPrivateRecurringBookingInput = {
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
  occurrenceCount?: number | null | undefined;
  organizationCustomDomains?: ReadonlyArray<string> | null | undefined;
  organizationIds?: ReadonlyArray<string> | null | undefined;
  requestedResourceIds?: ReadonlyArray<string> | null | undefined;
  skippedDates?: ReadonlyArray<any> | null | undefined;
  startDate: any;
  teamIds?: ReadonlyArray<string> | null | undefined;
  until: any;
};
export type addPrivateBookingPage_addPrivateRecurringBookingMutation$variables = {
  input: AddPrivateRecurringBookingInput;
};
export type addPrivateBookingPage_addPrivateRecurringBookingMutation$data = {
  readonly addPrivateRecurringBooking: {
    readonly recurringBooking: {
      readonly endDate: any | null | undefined;
      readonly frequency: {
        readonly frequency: BookingFrequency;
        readonly name: string;
      };
      readonly id: string;
      readonly involvedCustomers: ReadonlyArray<{
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly id: string;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      }>;
      readonly startDate: any;
    };
  };
};
export type addPrivateBookingPage_addPrivateRecurringBookingMutation = {
  response: addPrivateBookingPage_addPrivateRecurringBookingMutation$data;
  variables: addPrivateBookingPage_addPrivateRecurringBookingMutation$variables;
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
v3 = [
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
    "name": "addPrivateRecurringBooking",
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
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "startDate",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "endDate",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingFrequencyDetails",
            "kind": "LinkedField",
            "name": "frequency",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "frequency",
                "storageKey": null
              },
              (v2/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDetails",
            "kind": "LinkedField",
            "name": "involvedCustomers",
            "plural": true,
            "selections": [
              (v1/*:: as any*/),
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "givenName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "middleName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "familyName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "photoUrl",
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
    "name": "addPrivateBookingPage_addPrivateRecurringBookingMutation",
    "selections": (v3/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "addPrivateBookingPage_addPrivateRecurringBookingMutation",
    "selections": (v3/*:: as any*/)
  },
  "params": {
    "cacheID": "78abcf983e6fc000e955b1985a0cb2fd",
    "id": null,
    "metadata": {},
    "name": "addPrivateBookingPage_addPrivateRecurringBookingMutation",
    "operationKind": "mutation",
    "text": "mutation addPrivateBookingPage_addPrivateRecurringBookingMutation(\n  $input: AddPrivateRecurringBookingInput!\n) {\n  addPrivateRecurringBooking(input: $input) {\n    recurringBooking {\n      id\n      startDate\n      endDate\n      frequency {\n        frequency\n        name\n      }\n      involvedCustomers {\n        id\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a7d031a9ba7d816fa285a03ffae095d1";

export default node;
