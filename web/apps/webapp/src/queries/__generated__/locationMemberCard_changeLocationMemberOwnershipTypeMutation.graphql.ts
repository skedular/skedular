/**
 * @generated SignedSource<<f3247229787f97e9c482c96fbb36ce35>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type LocationMembershipType = "Administrator" | "Member" | "Owner" | "%future added value";
export type ChangeLocationMembershipTypeInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  membershipType: LocationMembershipType;
};
export type locationMemberCard_changeLocationMemberOwnershipTypeMutation$variables = {
  input: ChangeLocationMembershipTypeInput;
};
export type locationMemberCard_changeLocationMemberOwnershipTypeMutation$data = {
  readonly changeLocationMembershipType: {
    readonly member: {
      readonly id: string;
      readonly membershipType: LocationMembershipType | null | undefined;
    } | null | undefined;
  } | null | undefined;
};
export type locationMemberCard_changeLocationMemberOwnershipTypeMutation$rawResponse = {
  readonly changeLocationMembershipType: {
    readonly member: {
      readonly id: string;
      readonly membershipType: LocationMembershipType | null | undefined;
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
    "name": "changeLocationMembershipType",
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
    "cacheID": "ad2b41a0ef32ac4708ddb1525aa45f70",
    "id": null,
    "metadata": {},
    "name": "locationMemberCard_changeLocationMemberOwnershipTypeMutation",
    "operationKind": "mutation",
    "text": "mutation locationMemberCard_changeLocationMemberOwnershipTypeMutation(\n  $input: ChangeLocationMembershipTypeInput!\n) {\n  changeLocationMembershipType(input: $input) {\n    member {\n      id\n      membershipType\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0210366b7166128bd984eefb5c7fcdd8";

export default node;
