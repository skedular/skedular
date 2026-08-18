/**
 * @generated SignedSource<<31d1a585e5010b109041b3e9c9469c56>>
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
export type entitlementBalanceCard_setRenewalPolicyMutation$variables = {
  input: SetEntitlementRenewalPolicyInput;
};
export type entitlementBalanceCard_setRenewalPolicyMutation$data = {
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
export type entitlementBalanceCard_setRenewalPolicyMutation = {
  response: entitlementBalanceCard_setRenewalPolicyMutation$data;
  variables: entitlementBalanceCard_setRenewalPolicyMutation$variables;
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
    "name": "entitlementBalanceCard_setRenewalPolicyMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "entitlementBalanceCard_setRenewalPolicyMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "47f292abc54f879027ccd6dc87203e27",
    "id": null,
    "metadata": {},
    "name": "entitlementBalanceCard_setRenewalPolicyMutation",
    "operationKind": "mutation",
    "text": "mutation entitlementBalanceCard_setRenewalPolicyMutation(\n  $input: SetEntitlementRenewalPolicyInput!\n) {\n  setEntitlementRenewalPolicy(input: $input) {\n    entitlement {\n      id\n      autoRenew\n      cancelAtPeriodEnd\n      status\n      nextRenewalAt\n      renewalFailureReason\n    }\n    error\n  }\n}\n"
  }
};
})();

(node as any).hash = "3c03c116017ff7e47591f7ed2bada3af";

export default node;
