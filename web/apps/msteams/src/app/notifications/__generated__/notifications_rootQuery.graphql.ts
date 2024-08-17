/**
 * @generated SignedSource<<08b28d76d344f25e0e89661b0d2a8db9>>
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
export type notifications_rootQuery$variables = {
  myNotificationsSortingValues: ReadonlyArray<NotificationOrderInput>;
};
export type notifications_rootQuery$data = {
  readonly notificationCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"notifications_query" | "rootShell_query">;
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
  "name": "notificationCustomerRecordSynced",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v8 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "myNotificationsSortingValues"
  }
],
v9 = [
  (v7/*: any*/),
  (v3/*: any*/),
  (v4/*: any*/),
  (v5/*: any*/),
  (v6/*: any*/)
],
v10 = [
  (v7/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "notifications_rootQuery",
    "selections": [
      (v1/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "rootShell_query"
      },
      {
        "args": null,
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
    "name": "notifications_rootQuery",
    "selections": [
      (v1/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerEmail",
            "kind": "LinkedField",
            "name": "email",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "verified",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "title",
            "storageKey": null
          },
          (v3/*: any*/),
          (v4/*: any*/),
          (v5/*: any*/),
          (v6/*: any*/),
          (v7/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "isMsTeamsTenantInstalled",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "adminConsentUrl",
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v8/*: any*/),
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
                  (v2/*: any*/),
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
                    "selections": (v9/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "NotificationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "invitee",
                    "plural": false,
                    "selections": (v9/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "NotificationOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v10/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "NotificationLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v10/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "NotificationTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v10/*: any*/),
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
        "args": (v8/*: any*/),
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
    "cacheID": "9ceaf0e6a4405fef376d8459a103c9cf",
    "id": null,
    "metadata": {},
    "name": "notifications_rootQuery",
    "operationKind": "query",
    "text": "query notifications_rootQuery(\n  $myNotificationsSortingValues: [NotificationOrderInput!]!\n) {\n  notificationCustomerRecordSynced\n  ...rootShell_query\n  ...notifications_query\n}\n\nfragment invitationToJoinLocationNotificationCard_NotificationDetails on Notification {\n  id\n  sourceId\n  invitedBy {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  invitee {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  location {\n    name\n  }\n}\n\nfragment invitationToJoinOrganizationNotificationCard_NotificationDetails on Notification {\n  id\n  sourceId\n  invitedBy {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  invitee {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organization {\n    name\n  }\n}\n\nfragment invitationToJoinTeamNotificationCard_NotificationDetails on Notification {\n  id\n  sourceId\n  invitedBy {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  invitee {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  team {\n    name\n  }\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment mainRootLayout_query on Query {\n  me {\n    email {\n      email\n      verified\n      id\n    }\n    givenName\n    middleName\n    familyName\n    photoUrl\n    id\n  }\n  ...newFeedbackDialog_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment notificationCard_NotificationDetails on Notification {\n  id\n  notificationType\n  ...invitationToJoinOrganizationNotificationCard_NotificationDetails\n  ...invitationToJoinLocationNotificationCard_NotificationDetails\n  ...invitationToJoinTeamNotificationCard_NotificationDetails\n}\n\nfragment notifications_query on Query {\n  myNotifications(first: 50, orderBy: $myNotificationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...notificationCard_NotificationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n\nfragment rootShell_query on Query {\n  me {\n    id\n  }\n  isMsTeamsTenantInstalled\n  adminConsentUrl\n  ...observability_query\n  ...mainRootLayout_query\n}\n"
  }
};
})();

(node as any).hash = "28af84efc15535d964b260285db3199c";

export default node;
