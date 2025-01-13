/**
 * @generated SignedSource<<0a86b0be4c724285d2af5973a13abd99>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type NotificationOrderField = "Date" | "Type" | "%future added value";
export type NotificationType = "InvitationToJoinLocation" | "InvitationToJoinOrganization" | "InvitationToJoinTeam" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type NotificationOrderInput = {
  direction: OrderDirection;
  field: NotificationOrderField;
};
export type notifications_rootQuery$variables = {
  myNotificationsSortingValues?: ReadonlyArray<NotificationOrderInput> | null | undefined;
};
export type notifications_rootQuery$data = {
  readonly myNotifications: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly invitedBy: {
          readonly familyName: string | null | undefined;
          readonly givenName: string | null | undefined;
          readonly middleName: string | null | undefined;
          readonly name: string | null | undefined;
          readonly photoUrl: string | null | undefined;
        } | null | undefined;
        readonly location: {
          readonly name: string;
        } | null | undefined;
        readonly notificationType: NotificationType;
        readonly organization: {
          readonly name: string;
        } | null | undefined;
        readonly sourceId: string;
        readonly team: {
          readonly name: string;
        } | null | undefined;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
};
export type notifications_rootQuery = {
  response: notifications_rootQuery$data;
  variables: notifications_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "myNotificationsSortingValues"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  (v1/*: any*/)
],
v3 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "orderBy",
        "variableName": "myNotificationsSortingValues"
      },
      {
        "kind": "Literal",
        "name": "where",
        "value": {}
      }
    ],
    "concreteType": "NotificationConnection",
    "kind": "LinkedField",
    "name": "myNotifications",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "totalCount",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "NotificationEdge",
        "kind": "LinkedField",
        "name": "edges",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "Notification",
            "kind": "LinkedField",
            "name": "node",
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
                "name": "sourceId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "notificationType",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "NotificationCustomerDetails",
                "kind": "LinkedField",
                "name": "invitedBy",
                "plural": false,
                "selections": [
                  (v1/*: any*/),
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
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "NotificationOrganizationDetails",
                "kind": "LinkedField",
                "name": "organization",
                "plural": false,
                "selections": (v2/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "NotificationLocationDetails",
                "kind": "LinkedField",
                "name": "location",
                "plural": false,
                "selections": (v2/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "NotificationTeamDetails",
                "kind": "LinkedField",
                "name": "team",
                "plural": false,
                "selections": (v2/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
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
    "name": "notifications_rootQuery",
    "selections": (v3/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "notifications_rootQuery",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "c9e2676488e6bbc0928a4d2ab7deb177",
    "id": null,
    "metadata": {},
    "name": "notifications_rootQuery",
    "operationKind": "query",
    "text": "query notifications_rootQuery(\n  $myNotificationsSortingValues: [NotificationOrderInput!]\n) {\n  myNotifications(where: {}, orderBy: $myNotificationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        sourceId\n        notificationType\n        invitedBy {\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        organization {\n          name\n        }\n        location {\n          name\n        }\n        team {\n          name\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ada98cd6c9179a5b8cdcca3d8f1da20d";

export default node;
