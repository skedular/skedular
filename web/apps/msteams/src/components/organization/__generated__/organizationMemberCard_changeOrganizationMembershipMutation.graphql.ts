/**
 * @generated SignedSource<<476ef804ce1248912a9cca9ce437cedd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMembershipType = "Administrator" | "Member" | "Owner" | "%future added value";
export type ChangeOrganizationMembershipTypeInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  membershipType: OrganizationMembershipType;
};
export type organizationMemberCard_changeOrganizationMembershipMutation$variables = {
  input: ChangeOrganizationMembershipTypeInput;
};
export type organizationMemberCard_changeOrganizationMembershipMutation$data = {
  readonly changeOrganizationMembership: {
    readonly member: {
      readonly id: string;
      readonly membershipType: OrganizationMembershipType | null | undefined;
    } | null | undefined;
  } | null | undefined;
};
export type organizationMemberCard_changeOrganizationMembershipMutation$rawResponse = {
  readonly changeOrganizationMembership: {
    readonly member: {
      readonly id: string;
      readonly membershipType: OrganizationMembershipType | null | undefined;
    } | null | undefined;
  } | null | undefined;
};
export type organizationMemberCard_changeOrganizationMembershipMutation = {
  rawResponse: organizationMemberCard_changeOrganizationMembershipMutation$rawResponse;
  response: organizationMemberCard_changeOrganizationMembershipMutation$data;
  variables: organizationMemberCard_changeOrganizationMembershipMutation$variables;
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
    "name": "changeOrganizationMembership",
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
    "name": "organizationMemberCard_changeOrganizationMembershipMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMemberCard_changeOrganizationMembershipMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "698c79d2fadef87e231762e04fdaa367",
    "id": null,
    "metadata": {},
    "name": "organizationMemberCard_changeOrganizationMembershipMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMemberCard_changeOrganizationMembershipMutation(\n  $input: ChangeOrganizationMembershipTypeInput!\n) {\n  changeOrganizationMembership(input: $input) {\n    member {\n      id\n      membershipType\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b37000a8dbeffe465fc4fa2096eef979";

export default node;
