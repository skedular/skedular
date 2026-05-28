/**
 * @generated SignedSource<<5fce99c9d58d5ca2e5657a5d9a30fe2f>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type BookingFrequency = "DAILY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type PrivateRecurringBookingPatchField = "CATEGORY" | "PARTICIPANTS" | "RECURRENCE" | "REQUESTED_RESOURCES" | "SCHEDULE" | "SKIPPED_DATES" | "%future added value";
export type RecurringBookingEndType = "AFTER_OCCURRENCES" | "NEVER" | "UNTIL_DATE" | "%future added value";
export type UpdatePrivateRecurringBookingInput = {
  byMonthDay?: number | null | undefined;
  bySetPosition?: number | null | undefined;
  byWeekDays: ReadonlyArray<DayOfWeek>;
  category?: BookingCategory | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  endDate?: any | null | undefined;
  endType: RecurringBookingEndType;
  fieldsToUpdate: ReadonlyArray<PrivateRecurringBookingPatchField>;
  frequency: BookingFrequency;
  from: any;
  id: string;
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
export type editPrivateRecurringBooking_updatePrivateRecurringBookingMutation$variables = {
  input: UpdatePrivateRecurringBookingInput;
};
export type editPrivateRecurringBooking_updatePrivateRecurringBookingMutation$data = {
  readonly updatePrivateRecurringBooking: {
    readonly recurringBooking: {
      readonly endDate: any | null | undefined;
      readonly frequency: {
        readonly frequency: BookingFrequency;
        readonly name: string;
      };
      readonly id: string;
      readonly startDate: any;
    };
  };
};
export type editPrivateRecurringBooking_updatePrivateRecurringBookingMutation = {
  response: editPrivateRecurringBooking_updatePrivateRecurringBookingMutation$data;
  variables: editPrivateRecurringBooking_updatePrivateRecurringBookingMutation$variables;
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
    "name": "updatePrivateRecurringBooking",
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
          },
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
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "name",
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
    "name": "editPrivateRecurringBooking_updatePrivateRecurringBookingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editPrivateRecurringBooking_updatePrivateRecurringBookingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "b20346aeb98da31005c154d8410c6598",
    "id": null,
    "metadata": {},
    "name": "editPrivateRecurringBooking_updatePrivateRecurringBookingMutation",
    "operationKind": "mutation",
    "text": "mutation editPrivateRecurringBooking_updatePrivateRecurringBookingMutation(\n  $input: UpdatePrivateRecurringBookingInput!\n) {\n  updatePrivateRecurringBooking(input: $input) {\n    recurringBooking {\n      id\n      startDate\n      endDate\n      frequency {\n        frequency\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5a3eff3f88b2b12686fe81208ad2fe0d";

export default node;
