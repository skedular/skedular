/**
 * @generated SignedSource<<db2e508207a90187d517b4a81183b83f>>
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
export type pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation$variables = {
  input: SetEntitlementRenewalPolicyInput;
};
export type pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation$data = {
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
export type pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation = {
  response: pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation$data;
  variables: pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation$variables;
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
    "name": "pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "be09996c2c601bdad019052bde852f03",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation(\n  $input: SetEntitlementRenewalPolicyInput!\n) {\n  setEntitlementRenewalPolicy(input: $input) {\n    entitlement {\n      id\n      autoRenew\n      cancelAtPeriodEnd\n      status\n      nextRenewalAt\n      renewalFailureReason\n    }\n    error\n  }\n}\n"
  }
};
})();

(node as any).hash = "bd983810099aec97faf9228e990ad55e";

export default node;
