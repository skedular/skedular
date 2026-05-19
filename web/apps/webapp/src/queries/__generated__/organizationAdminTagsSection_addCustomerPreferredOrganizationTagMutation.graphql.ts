/**
 * @generated SignedSource<<80945ab4ef387ce8c81884616629fda9>>
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
export type organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation$variables = {
  input: AddCustomerPreferredOrganizationTagInput;
};
export type organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation$data = {
  readonly addCustomerPreferredOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredCustomTags: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation = {
  response: organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation$data;
  variables: organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation$variables;
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
    "name": "organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "04360cfabb0c92577f5e5365a8ffeba4",
    "id": null,
    "metadata": {},
    "name": "organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminTagsSection_addCustomerPreferredOrganizationTagMutation(\n  $input: AddCustomerPreferredOrganizationTagInput!\n) {\n  addCustomerPreferredOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredCustomTags {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "927fb78283ec6fec1e3af022dead8eb5";

export default node;
