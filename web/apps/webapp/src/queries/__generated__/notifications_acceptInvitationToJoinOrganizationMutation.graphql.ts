/**
 * @generated SignedSource<<62a362079dcfbe2599a1775ac64b01af>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type InvitationStatus = "ACCEPTED" | "CANCELLED" | "EXPIRED" | "PENDING" | "REJECTED" | "%future added value";
export type AcceptInvitationToJoinOrganizationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type notifications_acceptInvitationToJoinOrganizationMutation$variables = {
  input: AcceptInvitationToJoinOrganizationInput;
};
export type notifications_acceptInvitationToJoinOrganizationMutation$data = {
  readonly acceptInvitationToJoinOrganization: {
    readonly inviteCustomerToJoinOrganization: {
      readonly id: string;
      readonly status: {
        readonly name: string;
        readonly type: InvitationStatus;
      };
    };
  };
};
export type notifications_acceptInvitationToJoinOrganizationMutation$rawResponse = {
  readonly acceptInvitationToJoinOrganization: {
    readonly inviteCustomerToJoinOrganization: {
      readonly id: string;
      readonly status: {
        readonly name: string;
        readonly type: InvitationStatus;
      };
    };
  };
};
export type notifications_acceptInvitationToJoinOrganizationMutation = {
  rawResponse: notifications_acceptInvitationToJoinOrganizationMutation$rawResponse;
  response: notifications_acceptInvitationToJoinOrganizationMutation$data;
  variables: notifications_acceptInvitationToJoinOrganizationMutation$variables;
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
    "name": "acceptInvitationToJoinOrganization",
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
    "name": "notifications_acceptInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "notifications_acceptInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "7e029c516243e2f93180caf51d83e937",
    "id": null,
    "metadata": {},
    "name": "notifications_acceptInvitationToJoinOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation notifications_acceptInvitationToJoinOrganizationMutation(\n  $input: AcceptInvitationToJoinOrganizationInput!\n) {\n  acceptInvitationToJoinOrganization(input: $input) {\n    inviteCustomerToJoinOrganization {\n      id\n      status {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0bb3070635a2f8d38664d7f924c93d99";

export default node;
