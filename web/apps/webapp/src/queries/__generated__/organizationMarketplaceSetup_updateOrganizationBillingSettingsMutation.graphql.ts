/**
 * @generated SignedSource<<c9b7b81450795de1fa4f79dcb6761699>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type UpdateOrganizationBillingSettingsInput = {
  billingCycle: OrganizationBillingCycle;
  clientMutationId?: string | null | undefined;
  customDomain?: string | null | undefined;
  id?: string | null | undefined;
  invoiceDueInDays: number;
};
export type organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$variables = {
  input: UpdateOrganizationBillingSettingsInput;
};
export type organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$data = {
  readonly updateOrganizationBillingSettings: {
    readonly organization: {
      readonly billingCycle: {
        readonly name: string;
        readonly type: OrganizationBillingCycle;
      };
      readonly id: string;
      readonly invoiceDueInDays: number;
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$rawResponse = {
  readonly updateOrganizationBillingSettings: {
    readonly organization: {
      readonly billingCycle: {
        readonly name: string;
        readonly type: OrganizationBillingCycle;
      };
      readonly id: string;
      readonly invoiceDueInDays: number;
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation = {
  rawResponse: organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$rawResponse;
  response: organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$data;
  variables: organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation$variables;
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
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "updateOrganizationBillingSettings",
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
            "concreteType": "OrganizationBillingCycleDetails",
            "kind": "LinkedField",
            "name": "billingCycle",
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
            "name": "invoiceDueInDays",
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
    "name": "organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "5faaa7bfd254504ce993a44719844018",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_updateOrganizationBillingSettingsMutation(\n  $input: UpdateOrganizationBillingSettingsInput!\n) {\n  updateOrganizationBillingSettings(input: $input) {\n    organization {\n      id\n      billingCycle {\n        type\n        name\n      }\n      invoiceDueInDays\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1bfcb77a6f11d64e3f341a4501419d52";

export default node;
