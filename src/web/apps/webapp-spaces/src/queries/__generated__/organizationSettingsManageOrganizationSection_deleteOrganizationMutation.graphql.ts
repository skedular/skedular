/**
 * @generated SignedSource<<be317bba9e14a926a8f8646cafc6e961>>
 * @lightSyntaxTransform
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
export type organizationSettingsManageOrganizationSection_deleteOrganizationMutation$variables = {
  input: DeleteOrganizationInput;
};
export type organizationSettingsManageOrganizationSection_deleteOrganizationMutation$data = {
  readonly deleteOrganization: {
    readonly organization: {
      readonly id: string;
    };
  };
};
export type organizationSettingsManageOrganizationSection_deleteOrganizationMutation = {
  response: organizationSettingsManageOrganizationSection_deleteOrganizationMutation$data;
  variables: organizationSettingsManageOrganizationSection_deleteOrganizationMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationSettingsManageOrganizationSection_deleteOrganizationMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationSettingsManageOrganizationSection_deleteOrganizationMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "7ef95b59a7b5a56f8530e91aad23dc34",
    "id": null,
    "metadata": {},
    "name": "organizationSettingsManageOrganizationSection_deleteOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationSettingsManageOrganizationSection_deleteOrganizationMutation(\n  $input: DeleteOrganizationInput!\n) {\n  deleteOrganization(input: $input) {\n    organization {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4445837e3ddb23568ec775319f7c431d";

export default node;
