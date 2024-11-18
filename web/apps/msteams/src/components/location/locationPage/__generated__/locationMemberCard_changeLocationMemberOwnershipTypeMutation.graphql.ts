/**
 * @generated SignedSource<<84e04a9a97a8bf4ad30ff9debe477bb8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type LocationMemberMembershipType = "Administrator" | "Member" | "Owner" | "%future added value";
export type ChangeLocationMemberOwnershipTypeInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  membershipType: LocationMemberMembershipType;
};
export type locationMemberCard_changeLocationMemberOwnershipTypeMutation$variables = {
  input: ChangeLocationMemberOwnershipTypeInput;
};
export type locationMemberCard_changeLocationMemberOwnershipTypeMutation$data = {
  readonly changeLocationMemberOwnershipType: {
    readonly member: {
      readonly id: string;
      readonly membershipType: LocationMemberMembershipType | null | undefined;
    } | null | undefined;
  } | null | undefined;
};
export type locationMemberCard_changeLocationMemberOwnershipTypeMutation$rawResponse = {
  readonly changeLocationMemberOwnershipType: {
    readonly member: {
      readonly id: string;
      readonly membershipType: LocationMemberMembershipType | null | undefined;
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
    "name": "changeLocationMemberOwnershipType",
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
    "cacheID": "450897abacdf11f2c7737c922e2ce3ec",
    "id": null,
    "metadata": {},
    "name": "locationMemberCard_changeLocationMemberOwnershipTypeMutation",
    "operationKind": "mutation",
    "text": "mutation locationMemberCard_changeLocationMemberOwnershipTypeMutation(\n  $input: ChangeLocationMemberOwnershipTypeInput!\n) {\n  changeLocationMemberOwnershipType(input: $input) {\n    member {\n      id\n      membershipType\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "edca40136345aec0b5d6df26f9abb2d0";

export default node;
