/**
 * @generated SignedSource<<822639b17e436c250604335a5e0b0f2a>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ResolveMarketplaceExternalRefundReconciliationInput = {
  clientMutationId?: string | null | undefined;
  externalRefundId: string;
  organizationId: string;
  provider: string;
  reason: string;
  status: string;
};
export type pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation$variables = {
  input: ResolveMarketplaceExternalRefundReconciliationInput;
};
export type pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation$data = {
  readonly resolveMarketplaceExternalRefundReconciliation: {
    readonly reconciliation: {
      readonly externalRefundId: string;
      readonly provider: string;
      readonly resolutionReason: string | null | undefined;
      readonly status: string;
    };
  };
};
export type pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation = {
  response: pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation$data;
  variables: pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation$variables;
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
    "concreteType": "MarketplaceExternalRefundReconciliationPayload",
    "kind": "LinkedField",
    "name": "resolveMarketplaceExternalRefundReconciliation",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceExternalRefundReconciliationDetails",
        "kind": "LinkedField",
        "name": "reconciliation",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "provider",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "externalRefundId",
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
            "name": "resolutionReason",
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
    "name": "pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "a442a460d5b7f88ee64b532a2c0545ed",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation(\n  $input: ResolveMarketplaceExternalRefundReconciliationInput!\n) {\n  resolveMarketplaceExternalRefundReconciliation(input: $input) {\n    reconciliation {\n      provider\n      externalRefundId\n      status\n      resolutionReason\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "bce1840565c533ef955d0d5a41464aba";

export default node;
