/**
 * @generated SignedSource<<c160a5dc84d387955115b4b78a262fe7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddBookingInput = {
  clientMutationId?: string | null | undefined;
  customerId: string;
  deskIds: ReadonlyArray<string>;
  from: any;
  id?: string | null | undefined;
  locationId?: string | null | undefined;
  notes?: string | null | undefined;
  organizationId?: string | null | undefined;
  teamId?: string | null | undefined;
  to: any;
};
export type smallMonthlyViewCalendarDay_addBookingMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddBookingInput;
};
export type smallMonthlyViewCalendarDay_addBookingMutation$data = {
  readonly addBooking: {
    readonly booking: {
      readonly customer: {
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
        readonly uniqueId: string;
      };
      readonly desks: ReadonlyArray<{
        readonly deskTypes: ReadonlyArray<{
          readonly name: string;
          readonly uniqueId: string;
        }>;
        readonly name: string;
        readonly uniqueId: string;
        readonly zones: ReadonlyArray<{
          readonly name: string;
          readonly uniqueId: string;
        }>;
      }>;
      readonly from: any;
      readonly id: string;
      readonly location: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly notes: string | null | undefined;
      readonly organization: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly team: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly to: any;
    };
  } | null | undefined;
};
export type smallMonthlyViewCalendarDay_addBookingMutation$rawResponse = {
  readonly addBooking: {
    readonly booking: {
      readonly customer: {
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
        readonly uniqueId: string;
      };
      readonly desks: ReadonlyArray<{
        readonly deskTypes: ReadonlyArray<{
          readonly name: string;
          readonly uniqueId: string;
        }>;
        readonly name: string;
        readonly uniqueId: string;
        readonly zones: ReadonlyArray<{
          readonly name: string;
          readonly uniqueId: string;
        }>;
      }>;
      readonly from: any;
      readonly id: string;
      readonly location: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly notes: string | null | undefined;
      readonly organization: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly team: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly to: any;
    };
  } | null | undefined;
};
export type smallMonthlyViewCalendarDay_addBookingMutation = {
  rawResponse: smallMonthlyViewCalendarDay_addBookingMutation$rawResponse;
  response: smallMonthlyViewCalendarDay_addBookingMutation$data;
  variables: smallMonthlyViewCalendarDay_addBookingMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = [
  (v2/*: any*/),
  (v3/*: any*/)
],
v5 = {
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
    },
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
      "name": "to",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "notes",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingCustomerDetails",
      "kind": "LinkedField",
      "name": "customer",
      "plural": false,
      "selections": [
        (v2/*: any*/),
        (v3/*: any*/),
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
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingOrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": (v4/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingLocationDetails",
      "kind": "LinkedField",
      "name": "location",
      "plural": false,
      "selections": (v4/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingTeamDetails",
      "kind": "LinkedField",
      "name": "team",
      "plural": false,
      "selections": (v4/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingDeskDetails",
      "kind": "LinkedField",
      "name": "desks",
      "plural": true,
      "selections": [
        (v2/*: any*/),
        (v3/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingOrganizationDeskTypeDetails",
          "kind": "LinkedField",
          "name": "deskTypes",
          "plural": true,
          "selections": (v4/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingOrganizationZoneDetails",
          "kind": "LinkedField",
          "name": "zones",
          "plural": true,
          "selections": (v4/*: any*/),
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "smallMonthlyViewCalendarDay_addBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "addBooking",
        "plural": false,
        "selections": [
          (v5/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "smallMonthlyViewCalendarDay_addBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "addBooking",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          {
            "alias": null,
            "args": null,
            "filters": null,
            "handle": "appendNode",
            "key": "",
            "kind": "LinkedHandle",
            "name": "booking",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "BookingDetails"
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "f4810c87063fbafd3d39934cd0e5c5af",
    "id": null,
    "metadata": {},
    "name": "smallMonthlyViewCalendarDay_addBookingMutation",
    "operationKind": "mutation",
    "text": "mutation smallMonthlyViewCalendarDay_addBookingMutation(\n  $input: AddBookingInput!\n) {\n  addBooking(input: $input) {\n    booking {\n      id\n      from\n      to\n      notes\n      customer {\n        uniqueId\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n      organization {\n        uniqueId\n        name\n      }\n      location {\n        uniqueId\n        name\n      }\n      team {\n        uniqueId\n        name\n      }\n      desks {\n        uniqueId\n        name\n        deskTypes {\n          uniqueId\n          name\n        }\n        zones {\n          uniqueId\n          name\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "07be451eb8b10726576332fa77ba14ae";

export default node;
