/**
 * @generated SignedSource<<037ac46488c93cb11fd80aa7fe78c7d9>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type SetEntitlementRenewalPolicyInput = {
  autoRenew: boolean;
  cancelAtPeriodEnd: boolean;
  clientMutationId?: string | null | undefined;
  entitlementId: string;
};
export type entitlementPurchaseDetails_setRenewalPolicyMutation$variables = {
  input: SetEntitlementRenewalPolicyInput;
};
export type entitlementPurchaseDetails_setRenewalPolicyMutation$data = {
  readonly setEntitlementRenewalPolicy: {
    readonly entitlement: {
      readonly autoRenew: boolean;
      readonly cancelAtPeriodEnd: boolean;
      readonly id: string;
      readonly nextRenewalAt: any | null | undefined;
      readonly renewalFailureReason: string | null | undefined;
      readonly status: EntitlementStatus;
    } | null | undefined;
    readonly error: string | null | undefined;
  };
};
export type entitlementPurchaseDetails_setRenewalPolicyMutation = {
  response: entitlementPurchaseDetails_setRenewalPolicyMutation$data;
  variables: entitlementPurchaseDetails_setRenewalPolicyMutation$variables;
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
    "concreteType": "SetEntitlementRenewalPolicyPayload",
    "kind": "LinkedField",
    "name": "setEntitlementRenewalPolicy",
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
            "name": "autoRenew",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cancelAtPeriodEnd",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "status",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "nextRenewalAt",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "renewalFailureReason",
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
    "name": "entitlementPurchaseDetails_setRenewalPolicyMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "entitlementPurchaseDetails_setRenewalPolicyMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "58751c08439dc112b5eeb0555a9c0152",
    "id": null,
    "metadata": {},
    "name": "entitlementPurchaseDetails_setRenewalPolicyMutation",
    "operationKind": "mutation",
    "text": "mutation entitlementPurchaseDetails_setRenewalPolicyMutation(\n  $input: SetEntitlementRenewalPolicyInput!\n) {\n  setEntitlementRenewalPolicy(input: $input) {\n    entitlement {\n      id\n      autoRenew\n      cancelAtPeriodEnd\n      status\n      nextRenewalAt\n      renewalFailureReason\n    }\n    error\n  }\n}\n"
  }
};
})();

(node as any).hash = "2663e3d8086e5c7c79277a894b81caa5";

export default node;
