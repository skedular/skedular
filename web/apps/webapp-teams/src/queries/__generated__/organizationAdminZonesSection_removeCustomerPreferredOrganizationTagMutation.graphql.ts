/**
 * @generated SignedSource<<f9e16e06dc9855b82327dc77b75b0636>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPreferredOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  organizationTagId: string;
};
export type organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation$variables = {
  input: RemoveCustomerPreferredOrganizationTagInput;
};
export type organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation$data = {
  readonly removeCustomerPreferredOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation = {
  response: organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation$data;
  variables: organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation$variables;
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
    "concreteType": "CustomerPayload",
    "kind": "LinkedField",
    "name": "removeCustomerPreferredOrganizationTag",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "customer",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredZones",
            "plural": true,
            "selections": [
              (v1/*:: as any*/)
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
    "name": "organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "33b86cd95852dc23f05b783434b2da37",
    "id": null,
    "metadata": {},
    "name": "organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminZonesSection_removeCustomerPreferredOrganizationTagMutation(\n  $input: RemoveCustomerPreferredOrganizationTagInput!\n) {\n  removeCustomerPreferredOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "db61573d9b3e36106061a93ba2e63a74";

export default node;
