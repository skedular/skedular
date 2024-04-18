/**
 * @generated SignedSource<<2cf0005758c323f73355ffb722d3eb2b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Query } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type NotificationOrderField = "eventRaisedAt" | "notificationType" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type NotificationOrderInput = {
  direction: OrderDirection;
  field?: NotificationOrderField | null | undefined;
};
export type notificationsPaginationQuery$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  myNotificationsSortingValues?: ReadonlyArray<NotificationOrderInput> | null | undefined;
};
export type notificationsPaginationQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"notifications_query">;
};
export type notificationsPaginationQuery = {
  response: notificationsPaginationQuery$data;
  variables: notificationsPaginationQuery$variables;
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
    "name": "notificationsPaginationQuery",
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
    "name": "notificationsPaginationQuery",
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
    "cacheID": "6a90b39d53b8f68edb6771b5e8256260",
    "id": null,
    "metadata": {},
    "name": "notificationsPaginationQuery",
    "operationKind": "query",
    "text": "query notificationsPaginationQuery(\n  $count: Int = 50\n  $cursor: String\n  $myNotificationsSortingValues: [NotificationOrderInput!]\n) {\n  ...notifications_query_1G22uz\n}\n\nfragment invitationToJoinLocationNotificationCard_NotificationDetails on Notification {\n  id\n  sourceId\n  invitedBy {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  invitee {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  location {\n    name\n  }\n}\n\nfragment invitationToJoinOrganizationNotificationCard_NotificationDetails on Notification {\n  id\n  sourceId\n  invitedBy {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  invitee {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    name\n  }\n}\n\nfragment invitationToJoinTeamNotificationCard_NotificationDetails on Notification {\n  id\n  sourceId\n  invitedBy {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  invitee {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  team {\n    name\n  }\n}\n\nfragment notificationCard_NotificationDetails on Notification {\n  id\n  notificationType\n  ...invitationToJoinOrganizationNotificationCard_NotificationDetails\n  ...invitationToJoinLocationNotificationCard_NotificationDetails\n  ...invitationToJoinTeamNotificationCard_NotificationDetails\n}\n\nfragment notifications_query_1G22uz on Query {\n  myNotifications(first: $count, after: $cursor, orderBy: $myNotificationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...notificationCard_NotificationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3088f362f15f9ec98dec8581a6602456";

export default node;
