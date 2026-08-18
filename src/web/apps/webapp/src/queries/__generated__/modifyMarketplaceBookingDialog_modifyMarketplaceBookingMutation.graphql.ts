/**
 * @generated SignedSource<<18f9ed1d773bf3c0e6dc06697f12bc29>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarketplaceBookingModificationActorKind = "CUSTOMER" | "ORGANIZATION_OPERATOR" | "%future added value";
export type MarketplaceBookingModificationErrorCode = "INVALID_INPUT" | "INVALID_RESOURCE_SELECTION" | "NOT_ELIGIBLE" | "OPERATOR_REASON_REQUIRED" | "OUTSIDE_SUBSCRIPTION_CYCLE" | "STALE_VERSION" | "UNAUTHORIZED" | "UNAVAILABLE" | "%future added value";
export type ModifyMarketplaceBookingInput = {
  actorKind: MarketplaceBookingModificationActorKind;
  bookingId: string;
  clientMutationId?: string | null | undefined;
  entitlementId?: string | null | undefined;
  expectedVersion: number;
  from: any;
  reason: string;
  resourceIds?: ReadonlyArray<string> | null | undefined;
  until: any;
};
export type modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation$variables = {
  input: ModifyMarketplaceBookingInput;
};
export type modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation$data = {
  readonly modifyMarketplaceBooking: {
    readonly accessError: {
      readonly message: string;
    } | null | undefined;
    readonly availabilityError: {
      readonly message: string;
    } | null | undefined;
    readonly booking: {
      readonly from: any;
      readonly id: string;
      readonly until: any;
    } | null | undefined;
    readonly conflictError: {
      readonly code: MarketplaceBookingModificationErrorCode;
      readonly message: string;
    } | null | undefined;
    readonly eligibilityError: {
      readonly code: MarketplaceBookingModificationErrorCode;
      readonly message: string;
    } | null | undefined;
  };
};
export type modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation = {
  response: modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation$data;
  variables: modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation$variables;
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
  "name": "message",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "code",
    "storageKey": null
  },
  (v1/*:: as any*/)
],
v3 = [
  (v1/*:: as any*/)
],
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
    "concreteType": "ModifyMarketplaceBookingPayload",
    "kind": "LinkedField",
    "name": "modifyMarketplaceBooking",
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
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceBookingModificationEligibilityErrorDetails",
        "kind": "LinkedField",
        "name": "eligibilityError",
        "plural": false,
        "selections": (v2/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingAvailabilityErrorDetails",
        "kind": "LinkedField",
        "name": "availabilityError",
        "plural": false,
        "selections": (v3/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceBookingModificationConflictErrorDetails",
        "kind": "LinkedField",
        "name": "conflictError",
        "plural": false,
        "selections": (v2/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "SpacesAccessErrorDetails",
        "kind": "LinkedField",
        "name": "accessError",
        "plural": false,
        "selections": (v3/*:: as any*/),
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
    "name": "modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation",
    "selections": (v4/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation",
    "selections": (v4/*:: as any*/)
  },
  "params": {
    "cacheID": "a054a9e3cf1fe4f3fd10204b30713cc5",
    "id": null,
    "metadata": {},
    "name": "modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation modifyMarketplaceBookingDialog_modifyMarketplaceBookingMutation(\n  $input: ModifyMarketplaceBookingInput!\n) {\n  modifyMarketplaceBooking(input: $input) {\n    booking {\n      id\n      from\n      until\n    }\n    eligibilityError {\n      code\n      message\n    }\n    availabilityError {\n      message\n    }\n    conflictError {\n      code\n      message\n    }\n    accessError {\n      message\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "fb94974c1dcc0b50b7d7bd4862f43275";

export default node;
