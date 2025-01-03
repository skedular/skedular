/**
 * @generated SignedSource<<d716e3af9efe38baae2cc427eaed33c2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMemberRole = "Administrator" | "Member" | "Owner" | "%future added value";
export type ChangeOrganizationMemberRoleInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  role: OrganizationMemberRole;
};
export type organizationMemberCard_changeOrganizationMemberRoleMutation$variables = {
  input: ChangeOrganizationMemberRoleInput;
};
export type organizationMemberCard_changeOrganizationMemberRoleMutation$data = {
  readonly changeOrganizationMemberRole: {
    readonly member: {
      readonly id: string;
      readonly role: OrganizationMemberRole | null | undefined;
    } | null | undefined;
  } | null | undefined;
};
export type organizationMemberCard_changeOrganizationMemberRoleMutation$rawResponse = {
  readonly changeOrganizationMemberRole: {
    readonly member: {
      readonly id: string;
      readonly role: OrganizationMemberRole | null | undefined;
    } | null | undefined;
  } | null | undefined;
};
export type organizationMemberCard_changeOrganizationMemberRoleMutation = {
  rawResponse: organizationMemberCard_changeOrganizationMemberRoleMutation$rawResponse;
  response: organizationMemberCard_changeOrganizationMemberRoleMutation$data;
  variables: organizationMemberCard_changeOrganizationMemberRoleMutation$variables;
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
    "concreteType": "OrganizationMemberDetailsPayload",
    "kind": "LinkedField",
    "name": "changeOrganizationMemberRole",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationMemberDetails",
        "kind": "LinkedField",
        "name": "member",
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
            "kind": "ScalarField",
            "name": "role",
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
    "name": "organizationMemberCard_changeOrganizationMemberRoleMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMemberCard_changeOrganizationMemberRoleMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "2cd3f48e7b55ec5ea2dedc6a01627395",
    "id": null,
    "metadata": {},
    "name": "organizationMemberCard_changeOrganizationMemberRoleMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMemberCard_changeOrganizationMemberRoleMutation(\n  $input: ChangeOrganizationMemberRoleInput!\n) {\n  changeOrganizationMemberRole(input: $input) {\n    member {\n      id\n      role\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5ed92dd7da21f6cb710bb9dca479be82";

export default node;
