/**
 * @generated SignedSource<<6fc28338d9deade03f4dc8affb508c9b>>
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
    readonly totalCount: number;
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
    readonly totalCount: number;
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
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "createdBy",
  "plural": false,
  "selections": [
    (v0/*: any*/),
    (v7/*: any*/),
    (v8/*: any*/),
    (v9/*: any*/),
    (v10/*: any*/)
  ],
  "storageKey": null
},
v12 = [
  (v0/*: any*/)
],
v13 = {
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
v14 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamInvitationStatusDetails",
  "kind": "LinkedField",
  "name": "teamInvitationStatuses",
  "plural": true,
  "selections": (v1/*: any*/),
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamInvitationStatusDetails",
  "kind": "LinkedField",
  "name": "status",
  "plural": false,
  "selections": (v1/*: any*/),
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "createdBy",
  "plural": false,
  "selections": [
    (v0/*: any*/),
    (v7/*: any*/),
    (v8/*: any*/),
    (v9/*: any*/),
    (v10/*: any*/),
    (v5/*: any*/)
  ],
  "storageKey": null
},
v17 = [
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
                  (v11/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v12/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v13/*: any*/)
        ],
        "storageKey": "myInvitationsToJoinOrganizations(orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"CREATED_AT\"}],where:{\"status\":\"PENDING\"})"
      },
      (v14/*: any*/),
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
                  (v15/*: any*/),
                  (v11/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v12/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v13/*: any*/)
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
                  (v16/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v17/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v13/*: any*/)
        ],
        "storageKey": "myInvitationsToJoinOrganizations(orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"CREATED_AT\"}],where:{\"status\":\"PENDING\"})"
      },
      (v14/*: any*/),
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
                  (v15/*: any*/),
                  (v16/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v17/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v13/*: any*/)
        ],
        "storageKey": "myInvitationsToJoinTeams(orderBy:[{\"direction\":\"ASCENDING\",\"field\":\"CREATED_AT\"}],where:{\"status\":\"PENDING\"})"
      }
    ]
  },
  "params": {
    "cacheID": "180d9aba7d3bcc4ad7bf4959fa06c0ed",
    "id": null,
    "metadata": {},
    "name": "notifications_rootQuery",
    "operationKind": "query",
    "text": "query notifications_rootQuery {\n  organizationInvitationStatuses {\n    type\n    name\n  }\n  myInvitationsToJoinOrganizations(where: {status: PENDING}, orderBy: [{field: CREATED_AT, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        status {\n          type\n          name\n        }\n        createdBy {\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n          id\n        }\n        organization {\n          name\n          id\n        }\n      }\n    }\n  }\n  teamInvitationStatuses {\n    type\n    name\n  }\n  myInvitationsToJoinTeams(where: {status: PENDING}, orderBy: [{field: CREATED_AT, direction: ASCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        status {\n          type\n          name\n        }\n        createdBy {\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n          id\n        }\n        team {\n          name\n          id\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ccfbf26c66d82e5e7faae26fff107362";

export default node;
