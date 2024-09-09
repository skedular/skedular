/**
 * @generated SignedSource<<036092862c2585b3b426e201760802e0>>
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
export type newBookingDialog_addBookingMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddBookingInput;
};
export type newBookingDialog_addBookingMutation$data = {
  readonly addBooking: {
    readonly booking: {
      readonly customer: {
        readonly uniqueId: string;
      };
      readonly from: any;
      readonly id: string;
      readonly notes: string | null | undefined;
      readonly to: any;
    };
  } | null | undefined;
};
export type newBookingDialog_addBookingMutation$rawResponse = {
  readonly addBooking: {
    readonly booking: {
      readonly customer: {
        readonly uniqueId: string;
      };
      readonly from: any;
      readonly id: string;
      readonly notes: string | null | undefined;
      readonly to: any;
    };
  } | null | undefined;
};
export type newBookingDialog_addBookingMutation = {
  rawResponse: newBookingDialog_addBookingMutation$rawResponse;
  response: newBookingDialog_addBookingMutation$data;
  variables: newBookingDialog_addBookingMutation$variables;
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
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "uniqueId",
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
    "name": "newBookingDialog_addBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "addBooking",
        "plural": false,
        "selections": [
          (v2/*: any*/)
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
    "name": "newBookingDialog_addBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "addBooking",
        "plural": false,
        "selections": [
          (v2/*: any*/),
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
    "cacheID": "9faeebb504065ca8cb5ed3f240e67dca",
    "id": null,
    "metadata": {},
    "name": "newBookingDialog_addBookingMutation",
    "operationKind": "mutation",
    "text": "mutation newBookingDialog_addBookingMutation(\n  $input: AddBookingInput!\n) {\n  addBooking(input: $input) {\n    booking {\n      id\n      from\n      to\n      notes\n      customer {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6ccd19902e2cfc17af608cb299a8d799";

export default node;
