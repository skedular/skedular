/**
 * @generated SignedSource<<26e6c77ee81c6340eb33cfc59876758c>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarketplaceBookingModificationActorKind = "CUSTOMER" | "ORGANIZATION_OPERATOR" | "%future added value";
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
export type editMarketplaceBooking_modifyMarketplaceBookingMutation$variables = {
  input: ModifyMarketplaceBookingInput;
};
export type editMarketplaceBooking_modifyMarketplaceBookingMutation$data = {
  readonly modifyMarketplaceBooking: {
    readonly accessError: {
      readonly message: string;
    } | null | undefined;
    readonly availabilityError: {
      readonly message: string;
    } | null | undefined;
    readonly booking: {
      readonly id: string;
    } | null | undefined;
    readonly conflictError: {
      readonly message: string;
    } | null | undefined;
    readonly eligibilityError: {
      readonly message: string;
    } | null | undefined;
    readonly modification: {
      readonly id: string;
    } | null | undefined;
  };
};
export type editMarketplaceBooking_modifyMarketplaceBookingMutation$rawResponse = {
  readonly modifyMarketplaceBooking: {
    readonly accessError: {
      readonly message: string;
    } | null | undefined;
    readonly availabilityError: {
      readonly message: string;
    } | null | undefined;
    readonly booking: {
      readonly id: string;
    } | null | undefined;
    readonly conflictError: {
      readonly message: string;
    } | null | undefined;
    readonly eligibilityError: {
      readonly message: string;
    } | null | undefined;
    readonly modification: {
      readonly id: string;
    } | null | undefined;
  };
};
export type editMarketplaceBooking_modifyMarketplaceBookingMutation = {
  rawResponse: editMarketplaceBooking_modifyMarketplaceBookingMutation$rawResponse;
  response: editMarketplaceBooking_modifyMarketplaceBookingMutation$data;
  variables: editMarketplaceBooking_modifyMarketplaceBookingMutation$variables;
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
    "args": null,
    "kind": "ScalarField",
    "name": "id",
    "storageKey": null
  }
],
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "message",
    "storageKey": null
  }
],
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
        "selections": (v1/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceBookingModificationDetails",
        "kind": "LinkedField",
        "name": "modification",
        "plural": false,
        "selections": (v1/*:: as any*/),
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
        "selections": (v2/*:: as any*/),
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
        "selections": (v2/*:: as any*/),
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
    "name": "editMarketplaceBooking_modifyMarketplaceBookingMutation",
    "selections": (v3/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editMarketplaceBooking_modifyMarketplaceBookingMutation",
    "selections": (v3/*:: as any*/)
  },
  "params": {
    "cacheID": "1c5126a5719cd49efdb143958d921abb",
    "id": null,
    "metadata": {},
    "name": "editMarketplaceBooking_modifyMarketplaceBookingMutation",
    "operationKind": "mutation",
    "text": "mutation editMarketplaceBooking_modifyMarketplaceBookingMutation(\n  $input: ModifyMarketplaceBookingInput!\n) {\n  modifyMarketplaceBooking(input: $input) {\n    booking {\n      id\n    }\n    modification {\n      id\n    }\n    eligibilityError {\n      message\n    }\n    availabilityError {\n      message\n    }\n    conflictError {\n      message\n    }\n    accessError {\n      message\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "12d1ca8078ea44595e5b733284583148";

export default node;
