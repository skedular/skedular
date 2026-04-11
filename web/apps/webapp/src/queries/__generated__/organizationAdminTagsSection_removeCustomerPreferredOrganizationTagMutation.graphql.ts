/**
 * @generated SignedSource<<9fde48a9a938b703ee87ecfdf605929c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveCustomerPreferredOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  organizationTagId: string;
};
export type organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation$variables = {
  input: RemoveCustomerPreferredOrganizationTagInput;
};
export type organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation$data = {
  readonly removeCustomerPreferredOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredCustomTags: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation = {
  response: organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation$data;
  variables: organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation$variables;
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "preferredCustomTags",
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
    "name": "organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "4318740272b803da87851a367171de91",
    "id": null,
    "metadata": {},
    "name": "organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminTagsSection_removeCustomerPreferredOrganizationTagMutation(\n  $input: RemoveCustomerPreferredOrganizationTagInput!\n) {\n  removeCustomerPreferredOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredCustomTags {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5d1c8ca45a83482196b5e5abfb42a0a7";

export default node;
