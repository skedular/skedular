/**
 * @generated SignedSource<<3705a48f9136b2230ba9d74d61c272b8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type InvitationStatus = "ACCEPTED" | "CANCELLED" | "EXPIRED" | "PENDING" | "REJECTED" | "%future added value";
export type RejectInvitationToJoinOrganizationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type notifications_rejectInvitationToJoinOrganizationMutation$variables = {
  input: RejectInvitationToJoinOrganizationInput;
};
export type notifications_rejectInvitationToJoinOrganizationMutation$data = {
  readonly rejectInvitationToJoinOrganization: {
    readonly inviteCustomerToJoinOrganization: {
      readonly id: string;
      readonly status: {
        readonly name: string;
        readonly type: InvitationStatus;
      };
    };
  };
};
export type notifications_rejectInvitationToJoinOrganizationMutation$rawResponse = {
  readonly rejectInvitationToJoinOrganization: {
    readonly inviteCustomerToJoinOrganization: {
      readonly id: string;
      readonly status: {
        readonly name: string;
        readonly type: InvitationStatus;
      };
    };
  };
};
export type notifications_rejectInvitationToJoinOrganizationMutation = {
  rawResponse: notifications_rejectInvitationToJoinOrganizationMutation$rawResponse;
  response: notifications_rejectInvitationToJoinOrganizationMutation$data;
  variables: notifications_rejectInvitationToJoinOrganizationMutation$variables;
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
    "concreteType": "InvitationToJoinOrganizationPayload",
    "kind": "LinkedField",
    "name": "rejectInvitationToJoinOrganization",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "InviteCustomerToJoinOrganizationDetails",
        "kind": "LinkedField",
        "name": "inviteCustomerToJoinOrganization",
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
            "concreteType": "OrganizationInvitationStatusDetails",
            "kind": "LinkedField",
            "name": "status",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "type",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "name",
                "storageKey": null
              }
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
    "name": "notifications_rejectInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "notifications_rejectInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "e8353e15a0ee6623f56813dec4c134e3",
    "id": null,
    "metadata": {},
    "name": "notifications_rejectInvitationToJoinOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation notifications_rejectInvitationToJoinOrganizationMutation(\n  $input: RejectInvitationToJoinOrganizationInput!\n) {\n  rejectInvitationToJoinOrganization(input: $input) {\n    inviteCustomerToJoinOrganization {\n      id\n      status {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5e18817e63520eea34365ce284ff8a45";

export default node;
