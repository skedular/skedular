/**
 * @generated SignedSource<<4d1cd3915e3b958585fa3c8fe86a02c0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMembershipType = "Administrator" | "Member" | "Owner" | "%future added value";
export type ChangeOrganizationMemberOwnershipTypeInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  membershipType: OrganizationMembershipType;
};
export type organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation$variables = {
  input: ChangeOrganizationMemberOwnershipTypeInput;
};
export type organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation$data = {
  readonly changeOrganizationMemberOwnershipType: {
    readonly member: {
      readonly id: string;
      readonly membershipType: OrganizationMembershipType | null | undefined;
    } | null | undefined;
  } | null | undefined;
};
export type organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation$rawResponse = {
  readonly changeOrganizationMemberOwnershipType: {
    readonly member: {
      readonly id: string;
      readonly membershipType: OrganizationMembershipType | null | undefined;
    } | null | undefined;
  } | null | undefined;
};
export type organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation = {
  rawResponse: organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation$rawResponse;
  response: organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation$data;
  variables: organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation$variables;
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
    "name": "changeOrganizationMemberOwnershipType",
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
            "name": "membershipType",
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
    "name": "organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "0a77ff2fece03243436e5d4e2e764ff4",
    "id": null,
    "metadata": {},
    "name": "organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMemberCard_changeOrganizationMemberOwnershipTypeMutation(\n  $input: ChangeOrganizationMemberOwnershipTypeInput!\n) {\n  changeOrganizationMemberOwnershipType(input: $input) {\n    member {\n      id\n      membershipType\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c83f784dfbe79a7fe75335836365d3ed";

export default node;
