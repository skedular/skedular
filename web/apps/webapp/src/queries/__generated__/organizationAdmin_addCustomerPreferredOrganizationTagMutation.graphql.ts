/**
 * @generated SignedSource<<c39d3e9a8d4378eae8fc1bf1fb6cb82f>>
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
export type organizationAdmin_addCustomerPreferredOrganizationTagMutation$variables = {
  input: AddCustomerPreferredOrganizationTagInput;
};
export type organizationAdmin_addCustomerPreferredOrganizationTagMutation$data = {
  readonly addCustomerPreferredOrganizationTag: {
    readonly customer: {
      readonly id: string;
      readonly preferredZones: ReadonlyArray<{
        readonly id: string;
      }>;
    };
  };
};
export type organizationAdmin_addCustomerPreferredOrganizationTagMutation = {
  response: organizationAdmin_addCustomerPreferredOrganizationTagMutation$data;
  variables: organizationAdmin_addCustomerPreferredOrganizationTagMutation$variables;
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
    "name": "organizationAdmin_addCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_addCustomerPreferredOrganizationTagMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "dcf0e5723416fdb44df60250d72d34df",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_addCustomerPreferredOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_addCustomerPreferredOrganizationTagMutation(\n  $input: AddCustomerPreferredOrganizationTagInput!\n) {\n  addCustomerPreferredOrganizationTag(input: $input) {\n    customer {\n      id\n      preferredZones {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "83aa42662f3a3c7d2d5975ab205a5707";

export default node;
