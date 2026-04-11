/**
 * @generated SignedSource<<35f57a52941bc640048e5ac8320ac5da>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteOrganizationInput = {
  clientMutationId?: string | null | undefined;
  customDomain?: string | null | undefined;
  id?: string | null | undefined;
};
export type organizationAdminManageOrganizationSection_deleteOrganizationMutation$variables = {
  input: DeleteOrganizationInput;
};
export type organizationAdminManageOrganizationSection_deleteOrganizationMutation$data = {
  readonly deleteOrganization: {
    readonly organization: {
      readonly id: string;
    };
  };
};
export type organizationAdminManageOrganizationSection_deleteOrganizationMutation = {
  response: organizationAdminManageOrganizationSection_deleteOrganizationMutation$data;
  variables: organizationAdminManageOrganizationSection_deleteOrganizationMutation$variables;
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
    "name": "deleteOrganization",
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
    "name": "organizationAdminManageOrganizationSection_deleteOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdminManageOrganizationSection_deleteOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "c2090547f7aad4ce9861c3753797e062",
    "id": null,
    "metadata": {},
    "name": "organizationAdminManageOrganizationSection_deleteOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdminManageOrganizationSection_deleteOrganizationMutation(\n  $input: DeleteOrganizationInput!\n) {\n  deleteOrganization(input: $input) {\n    organization {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8ca96377233b454437d23c0cc856ff22";

export default node;
