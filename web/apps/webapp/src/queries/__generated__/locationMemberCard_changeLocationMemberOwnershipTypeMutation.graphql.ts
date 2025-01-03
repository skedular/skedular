/**
 * @generated SignedSource<<9b0f0f8b62f7b8309d3b83a79c67bb78>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type LocationMemberRole = "Administrator" | "Member" | "Owner" | "%future added value";
export type ChangeLocationMemberRoleInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  role: LocationMemberRole;
};
export type locationMemberCard_changeLocationMemberOwnershipTypeMutation$variables = {
  input: ChangeLocationMemberRoleInput;
};
export type locationMemberCard_changeLocationMemberOwnershipTypeMutation$data = {
  readonly changeLocationMemberRole: {
    readonly member: {
      readonly id: string;
      readonly role: LocationMemberRole | null | undefined;
    } | null | undefined;
  } | null | undefined;
};
export type locationMemberCard_changeLocationMemberOwnershipTypeMutation$rawResponse = {
  readonly changeLocationMemberRole: {
    readonly member: {
      readonly id: string;
      readonly role: LocationMemberRole | null | undefined;
    } | null | undefined;
  } | null | undefined;
};
export type locationMemberCard_changeLocationMemberOwnershipTypeMutation = {
  rawResponse: locationMemberCard_changeLocationMemberOwnershipTypeMutation$rawResponse;
  response: locationMemberCard_changeLocationMemberOwnershipTypeMutation$data;
  variables: locationMemberCard_changeLocationMemberOwnershipTypeMutation$variables;
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
    "concreteType": "LocationMemberDetailsPayload",
    "kind": "LinkedField",
    "name": "changeLocationMemberRole",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationMemberDetails",
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
    "name": "locationMemberCard_changeLocationMemberOwnershipTypeMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationMemberCard_changeLocationMemberOwnershipTypeMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "f888249f72a0799698e858bdcae71f9d",
    "id": null,
    "metadata": {},
    "name": "locationMemberCard_changeLocationMemberOwnershipTypeMutation",
    "operationKind": "mutation",
    "text": "mutation locationMemberCard_changeLocationMemberOwnershipTypeMutation(\n  $input: ChangeLocationMemberRoleInput!\n) {\n  changeLocationMemberRole(input: $input) {\n    member {\n      id\n      role\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1eea63dd924d85644a85f579fef01afb";

export default node;
