/**
 * @generated SignedSource<<781da5fb6db3f49d276567c58f1bdf9c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerPreferredOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  organizationTagId: string;
};
export type organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation$variables = {
  input: AddCustomerPreferredOrganizationTagInput;
};
export type organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation$data = {
  readonly addCustomerPreferredOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation = {
  response: organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation$data;
  variables: organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation$variables;
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
    "name": "addCustomerPreferredOrganizationTag",
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredZones",
            "plural": true,
            "selections": [
              (v1/*: any*/)
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
    "name": "organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "997a84ff1d8e004e08adca09575002ab",
    "id": null,
    "metadata": {},
    "name": "organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminZonesSection_addCustomerPreferredOrganizationTagMutation(\n  $input: AddCustomerPreferredOrganizationTagInput!\n) {\n  addCustomerPreferredOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7aaf4fe49e023f0bc6bc24b47f68fc38";

export default node;
