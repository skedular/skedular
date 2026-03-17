/**
 * @generated SignedSource<<f8291404eed18c00492a34b2dcaf4fed>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type UpdateOrganizationBillingCycleInput = {
  billingCycle: OrganizationBillingCycle;
  clientMutationId?: string | null | undefined;
  customDomain?: string | null | undefined;
  id?: string | null | undefined;
};
export type organizationMarketplaceSetup_updateOrganizationBillingCycleMutation$variables = {
  input: UpdateOrganizationBillingCycleInput;
};
export type organizationMarketplaceSetup_updateOrganizationBillingCycleMutation$data = {
  readonly updateOrganizationBillingCycle: {
    readonly organization: {
      readonly billingCycle: {
        readonly name: string;
        readonly type: OrganizationBillingCycle;
      };
      readonly id: string;
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationBillingCycleMutation$rawResponse = {
  readonly updateOrganizationBillingCycle: {
    readonly organization: {
      readonly billingCycle: {
        readonly name: string;
        readonly type: OrganizationBillingCycle;
      };
      readonly id: string;
    };
  };
};
export type organizationMarketplaceSetup_updateOrganizationBillingCycleMutation = {
  rawResponse: organizationMarketplaceSetup_updateOrganizationBillingCycleMutation$rawResponse;
  response: organizationMarketplaceSetup_updateOrganizationBillingCycleMutation$data;
  variables: organizationMarketplaceSetup_updateOrganizationBillingCycleMutation$variables;
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
    "name": "updateOrganizationBillingCycle",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMarketplaceSetup_updateOrganizationBillingCycleMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMarketplaceSetup_updateOrganizationBillingCycleMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "630b4b52ce4bddd1eaca93e2d69aeed3",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_updateOrganizationBillingCycleMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_updateOrganizationBillingCycleMutation(\n  $input: UpdateOrganizationBillingCycleInput!\n) {\n  updateOrganizationBillingCycle(input: $input) {\n    organization {\n      id\n      billingCycle {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "64ccbb0d74d0a9c1e57a1c7728194d4c";

export default node;
