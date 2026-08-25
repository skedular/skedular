/**
 * @generated SignedSource<<7d13eee94fc03522215eabaf7c98591d>>
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
export type organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation$variables = {
  input: RemoveCustomerPreferredOrganizationTagInput;
};
export type organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation$data = {
  readonly removeCustomerPreferredOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredCustomTags: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation = {
  response: organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation$data;
  variables: organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation$variables;
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
    "name": "organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "bdfdfa20bcc67210e697269863c71749",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsTagsSection_removeCustomerPreferredOrganizationTagMutation(\n  $input: RemoveCustomerPreferredOrganizationTagInput!\n) {\n  removeCustomerPreferredOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredCustomTags {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d584c6678ee04be07a13a120c3cd7fd8";

export default node;
