/**
 * @generated SignedSource<<69de911708f4c37284c3baa59d8a38f4>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type NotificationOrderField = "eventRaisedAt" | "notificationType" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type NotificationOrderInput = {
  direction: OrderDirection;
  field: NotificationOrderField;
};
export type notifications_PaginationQuery$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  myNotificationsSortingValues?: ReadonlyArray<NotificationOrderInput> | null | undefined;
};
export type notifications_PaginationQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"notifications_query">;
};
export type notifications_PaginationQuery = {
  response: notifications_PaginationQuery$data;
  variables: notifications_PaginationQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": 50,
    "kind": "LocalArgument",
    "name": "count"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "cursor"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "myNotificationsSortingValues"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "after",
    "variableName": "cursor"
  },
  {
    "kind": "Variable",
    "name": "first",
    "variableName": "count"
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "myNotificationsSortingValues"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  (v2/*: any*/),
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
v4 = [
  (v2/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "notifications_PaginationQuery",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "count",
            "variableName": "count"
          },
          {
            "kind": "Variable",
            "name": "cursor",
            "variableName": "cursor"
          }
        ],
        "kind": "FragmentSpread",
        "name": "notifications_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "notifications_PaginationQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
                    "name": "notificationType",
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
                    "concreteType": "NotificationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "invitedBy",
                    "plural": false,
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "NotificationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "invitee",
                    "plural": false,
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "NotificationOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v4/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "NotificationLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v4/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "NotificationTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v4/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "__typename",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "cursor",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "PageInfo",
            "kind": "LinkedField",
            "name": "pageInfo",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "endCursor",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "hasNextPage",
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
      },
      {
        "alias": null,
        "args": (v1/*: any*/),
        "filters": [
          "orderBy"
        ],
        "handle": "connection",
        "key": "notifications_myNotifications",
        "kind": "LinkedHandle",
        "name": "myNotifications"
      }
    ]
  },
  "params": {
    "cacheID": "ca45c8a65c82b431964cc93e5c8424fe",
    "id": null,
    "metadata": {},
    "name": "notifications_PaginationQuery",
    "operationKind": "query",
    "text": "query notifications_PaginationQuery(\n  $count: Int = 50\n  $cursor: String\n  $myNotificationsSortingValues: [NotificationOrderInput!]\n) {\n  ...notifications_query_1G22uz\n}\n\nfragment invitationToJoinLocationNotificationCard_NotificationDetails on Notification {\n  id\n  sourceId\n  invitedBy {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  invitee {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  location {\n    name\n  }\n}\n\nfragment invitationToJoinOrganizationNotificationCard_NotificationDetails on Notification {\n  id\n  sourceId\n  invitedBy {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  invitee {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    name\n  }\n}\n\nfragment invitationToJoinTeamNotificationCard_NotificationDetails on Notification {\n  id\n  sourceId\n  invitedBy {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  invitee {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  team {\n    name\n  }\n}\n\nfragment notificationCard_NotificationDetails on Notification {\n  id\n  notificationType\n  ...invitationToJoinOrganizationNotificationCard_NotificationDetails\n  ...invitationToJoinLocationNotificationCard_NotificationDetails\n  ...invitationToJoinTeamNotificationCard_NotificationDetails\n}\n\nfragment notifications_query_1G22uz on Query {\n  myNotifications(first: $count, after: $cursor, orderBy: $myNotificationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...notificationCard_NotificationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e7e56a8a95a21df607c7e3aae6c12d5a";

export default node;
