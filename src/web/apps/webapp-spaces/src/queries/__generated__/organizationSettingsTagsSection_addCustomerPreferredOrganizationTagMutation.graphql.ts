/**
 * @generated SignedSource<<69f97d37b4fb9d50b9d83316d86c881e>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomerPreferredOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  organizationTagId: string;
};
export type organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation$variables = {
  input: AddCustomerPreferredOrganizationTagInput;
};
export type organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation$data = {
  readonly addCustomerPreferredOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredCustomTags: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation = {
  response: organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation$data;
  variables: organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation$variables;
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
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredCustomTags",
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
    "name": "organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "33439ba7cbc6e37eb43cd7b1d9dfe63b",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsTagsSection_addCustomerPreferredOrganizationTagMutation(\n  $input: AddCustomerPreferredOrganizationTagInput!\n) {\n  addCustomerPreferredOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredCustomTags {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "734a3d26a6eeeca395de865f832021f3";

export default node;
