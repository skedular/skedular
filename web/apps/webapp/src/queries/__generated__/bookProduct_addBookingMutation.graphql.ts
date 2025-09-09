/**
 * @generated SignedSource<<673a632b89d5ad6dfe00c9263aff221c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingType = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type AddBookingInput = {
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  from: any;
  id?: string | null | undefined;
  invoiceEmailList?: ReadonlyArray<string> | null | undefined;
  lineItems: ReadonlyArray<LineItemInput>;
  notes?: string | null | undefined;
  organizationIds?: ReadonlyArray<string> | null | undefined;
  organizationUniqueAlphanumericNames?: ReadonlyArray<string> | null | undefined;
  paymentMethod?: PaymentMethod | null | undefined;
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
      readonly bookingResources: ReadonlyArray<{
        readonly resource: {
          readonly color: string | null | undefined;
          readonly customTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly id: string;
          readonly name: string;
          readonly zones: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
        };
      }>;
      readonly from: any;
      readonly id: string;
      readonly invoiceEmailList: ReadonlyArray<string> | null | undefined;
      readonly involvedCustomers: ReadonlyArray<{
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly id: string;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      }>;
      readonly involvedOrganizations: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly notes: string | null | undefined;
      readonly paymentMethod: {
        readonly name: string;
        readonly type: PaymentMethod;
      } | null | undefined;
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
      readonly bookingResources: ReadonlyArray<{
        readonly resource: {
          readonly color: string | null | undefined;
          readonly customTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly id: string;
          readonly name: string;
          readonly zones: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
        };
      }>;
      readonly from: any;
      readonly id: string;
      readonly invoiceEmailList: ReadonlyArray<string> | null | undefined;
      readonly involvedCustomers: ReadonlyArray<{
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly id: string;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      }>;
      readonly involvedOrganizations: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly notes: string | null | undefined;
      readonly paymentMethod: {
        readonly name: string;
        readonly type: PaymentMethod;
      } | null | undefined;
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
  "name": "id",
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
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v3/*: any*/)
],
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v6 = [
  (v2/*: any*/),
  (v3/*: any*/),
  (v5/*: any*/)
],
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingDetails",
  "kind": "LinkedField",
  "name": "booking",
  "plural": false,
  "selections": [
    (v2/*: any*/),
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
      "name": "notes",
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
      "selections": (v4/*: any*/),
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
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "involvedOrganizations",
      "plural": true,
      "selections": [
        (v2/*: any*/),
        (v3/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingResourceDetails",
      "kind": "LinkedField",
      "name": "bookingResources",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "ResourceDetails",
          "kind": "LinkedField",
          "name": "resource",
          "plural": false,
          "selections": [
            (v2/*: any*/),
            (v3/*: any*/),
            (v5/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "customTags",
              "plural": true,
              "selections": (v6/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "zones",
              "plural": true,
              "selections": (v6/*: any*/),
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
      "concreteType": "PaymentMethodTypeDetails",
      "kind": "LinkedField",
      "name": "paymentMethod",
      "plural": false,
      "selections": (v4/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "invoiceEmailList",
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
          (v7/*: any*/)
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
          (v7/*: any*/),
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
    "cacheID": "129a8fe54d9973ee73fba41f30de2fec",
    "id": null,
    "metadata": {},
    "name": "bookProduct_addBookingMutation",
    "operationKind": "mutation",
    "text": "mutation bookProduct_addBookingMutation(\n  $input: AddBookingInput!\n) {\n  addBooking(input: $input) {\n    booking {\n      id\n      from\n      notes\n      until\n      type {\n        type\n        name\n      }\n      involvedCustomers {\n        id\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n      involvedOrganizations {\n        id\n        name\n      }\n      bookingResources {\n        resource {\n          id\n          name\n          color\n          customTags {\n            id\n            name\n            color\n          }\n          zones {\n            id\n            name\n            color\n          }\n        }\n      }\n      paymentMethod {\n        type\n        name\n      }\n      invoiceEmailList\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8f444d4fc539867816f4d88a2bf9c7e0";

export default node;
