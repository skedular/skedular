/**
 * @generated SignedSource<<dd552294bc9723ef061b0d169ef3fc3e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingType = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type AddBookingInput = {
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  from: any;
  id?: string | null | undefined;
  lineItems: ReadonlyArray<LineItemInput>;
  notes?: string | null | undefined;
  organizationIds: ReadonlyArray<string>;
  resourceIds: ReadonlyArray<string>;
  teamIds: ReadonlyArray<string>;
  type: BookingType;
  until: any;
};
export type LineItemInput = {
  productVersionId: string;
  quantity: number;
};
export type bookProduct_addBookingMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddBookingInput;
};
export type bookProduct_addBookingMutation$data = {
  readonly addBooking: {
    readonly booking: {
      readonly from: any;
      readonly id: string;
      readonly involvedCustomers: ReadonlyArray<{
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly involvedOrganizations: ReadonlyArray<{
        readonly name: string;
        readonly uniqueId: string;
      }>;
      readonly resources: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly customTags: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly name: string;
        readonly uniqueId: string;
        readonly zones: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
      }>;
      readonly type: {
        readonly name: string;
        readonly type: BookingType;
      };
      readonly until: any;
    };
  };
};
export type bookProduct_addBookingMutation$rawResponse = {
  readonly addBooking: {
    readonly booking: {
      readonly from: any;
      readonly id: string;
      readonly involvedCustomers: ReadonlyArray<{
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly involvedOrganizations: ReadonlyArray<{
        readonly name: string;
        readonly uniqueId: string;
      }>;
      readonly resources: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly customTags: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly name: string;
        readonly uniqueId: string;
        readonly zones: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
      }>;
      readonly type: {
        readonly name: string;
        readonly type: BookingType;
      };
      readonly until: any;
    };
  };
};
export type bookProduct_addBookingMutation = {
  rawResponse: bookProduct_addBookingMutation$rawResponse;
  response: bookProduct_addBookingMutation$data;
  variables: bookProduct_addBookingMutation$variables;
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
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v5 = [
  (v3/*: any*/),
  (v2/*: any*/),
  (v4/*: any*/)
],
v6 = {
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
      "name": "until",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingTypeDetails",
      "kind": "LinkedField",
      "name": "type",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
          "storageKey": null
        },
        (v2/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "Booking_CustomerDetails",
      "kind": "LinkedField",
      "name": "involvedCustomers",
      "plural": true,
      "selections": [
        (v3/*: any*/),
        (v2/*: any*/),
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
      "concreteType": "Booking_OrganizationDetails",
      "kind": "LinkedField",
      "name": "involvedOrganizations",
      "plural": true,
      "selections": [
        (v3/*: any*/),
        (v2/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingResourceDetails",
      "kind": "LinkedField",
      "name": "resources",
      "plural": true,
      "selections": [
        (v3/*: any*/),
        (v2/*: any*/),
        (v4/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "Booking_OrganizationCustomTagDetails",
          "kind": "LinkedField",
          "name": "customTags",
          "plural": true,
          "selections": (v5/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Booking_OrganizationZoneDetails",
          "kind": "LinkedField",
          "name": "zones",
          "plural": true,
          "selections": (v5/*: any*/),
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
    "name": "bookProduct_addBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "addBooking",
        "plural": false,
        "selections": [
          (v6/*: any*/)
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
    "name": "bookProduct_addBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "addBooking",
        "plural": false,
        "selections": [
          (v6/*: any*/),
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
    "cacheID": "de6761518cf94230661c49f74b0711ee",
    "id": null,
    "metadata": {},
    "name": "bookProduct_addBookingMutation",
    "operationKind": "mutation",
    "text": "mutation bookProduct_addBookingMutation(\n  $input: AddBookingInput!\n) {\n  addBooking(input: $input) {\n    booking {\n      id\n      from\n      until\n      type {\n        type\n        name\n      }\n      involvedCustomers {\n        uniqueId\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n      involvedOrganizations {\n        uniqueId\n        name\n      }\n      resources {\n        uniqueId\n        name\n        color\n        customTags {\n          uniqueId\n          name\n          color\n        }\n        zones {\n          uniqueId\n          name\n          color\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a0e44a19f87e8f4d769460f8e90bf3f6";

export default node;
