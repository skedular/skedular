/**
 * @generated SignedSource<<77d47fff81fd04abca75624254221147>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationXeroBillingMode = "DISABLED" | "ENABLED" | "REPEATING_INVOICES" | "%future added value";
export type UpdateOrganizationXeroConnectionInput = {
  autoReconcilePayments: boolean;
  billingMode: OrganizationXeroBillingMode;
  clientMutationId?: string | null | undefined;
  defaultBrandingThemeId?: string | null | undefined;
  defaultReceivablesAccountCode?: string | null | undefined;
  defaultReferencePrefix?: string | null | undefined;
  defaultSalesAccountCode?: string | null | undefined;
  defaultTrackingCategory1?: string | null | undefined;
  defaultTrackingCategory2?: string | null | undefined;
  isActive: boolean;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
  scopes?: string | null | undefined;
  sendInvoicesViaXero: boolean;
  tenantId: string;
  tenantName: string;
};
export type organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation$variables = {
  input: UpdateOrganizationXeroConnectionInput;
};
export type organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation$data = {
  readonly updateOrganizationXeroConnection: {
    readonly organization: {
      readonly id: string;
      readonly xeroConnection: {
        readonly autoReconcilePayments: boolean;
        readonly billingMode: OrganizationXeroBillingMode;
        readonly defaultBrandingThemeId: string | null | undefined;
        readonly defaultReceivablesAccountCode: string | null | undefined;
        readonly defaultReferencePrefix: string | null | undefined;
        readonly defaultSalesAccountCode: string | null | undefined;
        readonly defaultTrackingCategory1: string | null | undefined;
        readonly defaultTrackingCategory2: string | null | undefined;
        readonly hasAccessToken: boolean;
        readonly hasRefreshToken: boolean;
        readonly id: string;
        readonly isActive: boolean;
        readonly lastError: string | null | undefined;
        readonly lastSuccessfulSyncAt: any | null | undefined;
        readonly scopes: string | null | undefined;
        readonly sendInvoicesViaXero: boolean;
        readonly tenantId: string;
        readonly tenantName: string;
      } | null | undefined;
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation$rawResponse = {
  readonly updateOrganizationXeroConnection: {
    readonly organization: {
      readonly id: string;
      readonly xeroConnection: {
        readonly autoReconcilePayments: boolean;
        readonly billingMode: OrganizationXeroBillingMode;
        readonly defaultBrandingThemeId: string | null | undefined;
        readonly defaultReceivablesAccountCode: string | null | undefined;
        readonly defaultReferencePrefix: string | null | undefined;
        readonly defaultSalesAccountCode: string | null | undefined;
        readonly defaultTrackingCategory1: string | null | undefined;
        readonly defaultTrackingCategory2: string | null | undefined;
        readonly hasAccessToken: boolean;
        readonly hasRefreshToken: boolean;
        readonly id: string;
        readonly isActive: boolean;
        readonly lastError: string | null | undefined;
        readonly lastSuccessfulSyncAt: any | null | undefined;
        readonly scopes: string | null | undefined;
        readonly sendInvoicesViaXero: boolean;
        readonly tenantId: string;
        readonly tenantName: string;
      } | null | undefined;
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation = {
  rawResponse: organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation$rawResponse;
  response: organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation$data;
  variables: organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation$variables;
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
  "name": "id",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "updateOrganizationXeroConnection",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationXeroConnection",
            "kind": "LinkedField",
            "name": "xeroConnection",
            "plural": false,
            "selections": [
              (v1/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "tenantId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "tenantName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "billingMode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "scopes",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "isActive",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "sendInvoicesViaXero",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "autoReconcilePayments",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultSalesAccountCode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultReceivablesAccountCode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultTrackingCategory1",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultTrackingCategory2",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultBrandingThemeId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultReferencePrefix",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "lastSuccessfulSyncAt",
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
                "name": "hasAccessToken",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "hasRefreshToken",
                "storageKey": null
              }
            ],
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
    "name": "organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "c1fcb0a41a9ac1c0f78c767638e2010d",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_updateOrganizationXeroConnectionMutation(\n  $input: UpdateOrganizationXeroConnectionInput!\n) {\n  updateOrganizationXeroConnection(input: $input) {\n    organization {\n      id\n      xeroConnection {\n        id\n        tenantId\n        tenantName\n        billingMode\n        scopes\n        isActive\n        sendInvoicesViaXero\n        autoReconcilePayments\n        defaultSalesAccountCode\n        defaultReceivablesAccountCode\n        defaultTrackingCategory1\n        defaultTrackingCategory2\n        defaultBrandingThemeId\n        defaultReferencePrefix\n        lastSuccessfulSyncAt\n        lastError\n        hasAccessToken\n        hasRefreshToken\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "45b1a177037e9a80f9f91f4fe75f8615";

export default node;
