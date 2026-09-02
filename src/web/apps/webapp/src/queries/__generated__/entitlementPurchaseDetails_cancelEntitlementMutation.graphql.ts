/**
 * @generated SignedSource<<6157d3bd9d3075dd23d110ecd1d5ee1c>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type CancelEntitlementInput = {
  clientMutationId?: string | null | undefined;
  entitlementId: string;
  reason: string;
};
export type entitlementPurchaseDetails_cancelEntitlementMutation$variables = {
  input: CancelEntitlementInput;
};
export type entitlementPurchaseDetails_cancelEntitlementMutation$data = {
  readonly cancelEntitlement: {
    readonly entitlement: {
      readonly id: string;
      readonly status: EntitlementStatus;
    } | null | undefined;
    readonly error: string | null | undefined;
  };
};
export type entitlementPurchaseDetails_cancelEntitlementMutation = {
  response: entitlementPurchaseDetails_cancelEntitlementMutation$data;
  variables: entitlementPurchaseDetails_cancelEntitlementMutation$variables;
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
    "concreteType": "CancelEntitlementPayload",
    "kind": "LinkedField",
    "name": "cancelEntitlement",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "EntitlementDetails",
        "kind": "LinkedField",
        "name": "entitlement",
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
            "name": "status",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "error",
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
    "name": "entitlementPurchaseDetails_cancelEntitlementMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "entitlementPurchaseDetails_cancelEntitlementMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "64dba8c0b604b023017fa8448a55b9e8",
    "id": null,
    "metadata": {},
    "name": "entitlementPurchaseDetails_cancelEntitlementMutation",
    "operationKind": "mutation",
    "text": "mutation entitlementPurchaseDetails_cancelEntitlementMutation(\n  $input: CancelEntitlementInput!\n) {\n  cancelEntitlement(input: $input) {\n    entitlement {\n      id\n      status\n    }\n    error\n  }\n}\n"
  }
};
})();

(node as any).hash = "110f94842aa1d8b845f8ac931b4038b3";

export default node;
