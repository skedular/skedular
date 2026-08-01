/**
 * @generated SignedSource<<fc3d7ad7c434eb0ba4be824a1ae3cfc2>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type ResolveRefundReconciliationRequiredInput = {
  clientMutationId?: string | null | undefined;
  completed: boolean;
  id: string;
  providerReference?: string | null | undefined;
  reason: string;
};
export type marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation$variables = {
  input: ResolveRefundReconciliationRequiredInput;
};
export type marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation$data = {
  readonly resolveRefundReconciliationRequired: {
    readonly marketplaceRefund: {
      readonly externalRefundNumber: string | null | undefined;
      readonly id: string;
      readonly lastError: string | null | undefined;
      readonly status: {
        readonly name: string;
        readonly type: MarketplaceRefundStatus;
      };
    };
  };
};
export type marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation = {
  response: marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation$data;
  variables: marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation$variables;
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
    "concreteType": "MarketplaceRefundPayload",
    "kind": "LinkedField",
    "name": "resolveRefundReconciliationRequired",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceRefundDetails",
        "kind": "LinkedField",
        "name": "marketplaceRefund",
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
            "concreteType": "MarketplaceRefundStatusDetails",
            "kind": "LinkedField",
            "name": "status",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "type",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "name",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "lastError",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "externalRefundNumber",
            "storageKey": null
          }
        ],
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
    "name": "marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "ee96b973f4c1f191bdc26058c9377ed4",
    "id": null,
    "metadata": {},
    "name": "marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceRefundAdminPanel_resolveRefundReconciliationRequiredMutation(\n  $input: ResolveRefundReconciliationRequiredInput!\n) {\n  resolveRefundReconciliationRequired(input: $input) {\n    marketplaceRefund {\n      id\n      status {\n        type\n        name\n      }\n      lastError\n      externalRefundNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cd111ee41f2433ce41a20a120c010038";

export default node;
