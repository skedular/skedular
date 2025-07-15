/**
 * @generated SignedSource<<db77e7b7cf6abf1f8dcfcf57333c26b6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type InvitationStatus = "ACCEPTED" | "CANCELLED" | "PENDING" | "REJECTED" | "%future added value";
export type notifications_rootQuery$variables = Record<PropertyKey, never>;
export type notifications_rootQuery$data = {
  readonly myInvitationsToJoinOrganizations: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly createdBy: {
          readonly familyName: string | null | undefined;
          readonly givenName: string | null | undefined;
          readonly middleName: string | null | undefined;
          readonly name: string | null | undefined;
          readonly photoUrl: string | null | undefined;
        };
        readonly id: string;
        readonly organization: {
          readonly name: string;
        };
        readonly status: {
          readonly name: string;
          readonly type: InvitationStatus;
        };
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly myInvitationsToJoinTeams: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly createdBy: {
          readonly familyName: string | null | undefined;
          readonly givenName: string | null | undefined;
          readonly middleName: string | null | undefined;
          readonly name: string | null | undefined;
          readonly photoUrl: string | null | undefined;
        };
        readonly id: string;
        readonly status: {
          readonly name: string;
          readonly type: InvitationStatus;
        };
        readonly team: {
          readonly name: string;
        };
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly organizationInvitationStatuses: ReadonlyArray<{
    readonly name: string;
    readonly type: InvitationStatus;
  }>;
  readonly teamInvitationStatuses: ReadonlyArray<{
    readonly name: string;
    readonly type: InvitationStatus;
  }>;
};
export type notifications_rootQuery = {
  response: notifications_rootQuery$data;
  variables: notifications_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v1 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v0/*: any*/)
],
v2 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationInvitationStatusDetails",
  "kind": "LinkedField",
  "name": "organizationInvitationStatuses",
  "plural": true,
  "selections": (v1/*: any*/),
  "storageKey": null
},
v3 = [
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "ASCENDING",
        "field": "CREATED_AT"
      }
    ]
  },
  {
    "kind": "Literal",
    "name": "where",
    "value": {
      "status": "PENDING"
    }
  }
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationInvitationStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v1/*: any*/),
  "storageKey": null
},
v7 = [
  (v0/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "givenName",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "middleName",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "familyName",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "photoUrl",
    "storageKey": null
  }
],
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "Organization_CustomerDetails",
  "kind": "LinkedField",
  "name": "createdBy",
  "plural": false,
  "selections": (v7/*: any*/),
  "storageKey": null
},
v9 = [
  (v0/*: any*/)
],
v10 = {
  "kind": "ClientExtension",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "__id",
      "storageKey": null
    }
  ]
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamInvitationStatusDetails",
  "kind": "LinkedField",
  "name": "teamInvitationStatuses",
  "plural": true,
  "selections": (v1/*: any*/),
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamInvitationStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v1/*: any*/),
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "concreteType": "Team_CustomerDetails",
  "kind": "LinkedField",
  "name": "createdBy",
  "plural": false,
  "selections": (v7/*: any*/),
  "storageKey": null
},
v14 = [
  (v0/*: any*/),
  (v5/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "notifications_rootQuery",
    "selections": [
      (v2/*: any*/),
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "ConnectionOfOrganizationJoinInvitationEdge",
        "kind": "LinkedField",
        "name": "myInvitationsToJoinOrganizations",
        "plural": false,
        "selections": [
          (v4/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationJoinInvitationEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "InviteCustomerToJoinOrganizationDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v5/*: any*/),
                  (v6/*: any*/),
                  (v8/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v9/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v10/*: any*/)
        ],
        "storageKey": "myInvitationsToJoinOrganizations(orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"CREATED_AT\"}],where:{\"status\":\"PENDING\"})"
      },
      (v11/*: any*/),
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "ConnectionOfTeamJoinInvitationEdge",
        "kind": "LinkedField",
        "name": "myInvitationsToJoinTeams",
        "plural": false,
        "selections": [
          (v4/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamJoinInvitationEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "InviteCustomerToJoinTeamDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v5/*: any*/),
                  (v12/*: any*/),
                  (v13/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v9/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v10/*: any*/)
        ],
        "storageKey": "myInvitationsToJoinTeams(orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"CREATED_AT\"}],where:{\"status\":\"PENDING\"})"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "notifications_rootQuery",
    "selections": [
      (v2/*: any*/),
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "ConnectionOfOrganizationJoinInvitationEdge",
        "kind": "LinkedField",
        "name": "myInvitationsToJoinOrganizations",
        "plural": false,
        "selections": [
          (v4/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationJoinInvitationEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "InviteCustomerToJoinOrganizationDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v5/*: any*/),
                  (v6/*: any*/),
                  (v8/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v14/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v10/*: any*/)
        ],
        "storageKey": "myInvitationsToJoinOrganizations(orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"CREATED_AT\"}],where:{\"status\":\"PENDING\"})"
      },
      (v11/*: any*/),
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "ConnectionOfTeamJoinInvitationEdge",
        "kind": "LinkedField",
        "name": "myInvitationsToJoinTeams",
        "plural": false,
        "selections": [
          (v4/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamJoinInvitationEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "InviteCustomerToJoinTeamDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v5/*: any*/),
                  (v12/*: any*/),
                  (v13/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v14/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v10/*: any*/)
        ],
        "storageKey": "myInvitationsToJoinTeams(orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"CREATED_AT\"}],where:{\"status\":\"PENDING\"})"
      }
    ]
  },
  "params": {
    "cacheID": "a8d0162419473305286a7fa21906ddc4",
    "id": null,
    "metadata": {},
    "name": "notifications_rootQuery",
    "operationKind": "query",
    "text": "query notifications_rootQuery {\n  organizationInvitationStatuses {\n    type\n    name\n  }\n  myInvitationsToJoinOrganizations(where: {status: PENDING}, orderBy: [{field: CREATED_AT, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        status {\n          type\n          name\n        }\n        createdBy {\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        organization {\n          name\n          id\n        }\n      }\n    }\n  }\n  teamInvitationStatuses {\n    type\n    name\n  }\n  myInvitationsToJoinTeams(where: {status: PENDING}, orderBy: [{field: CREATED_AT, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        status {\n          type\n          name\n        }\n        createdBy {\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        team {\n          name\n          id\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ccfbf26c66d82e5e7faae26fff107362";

export default node;
