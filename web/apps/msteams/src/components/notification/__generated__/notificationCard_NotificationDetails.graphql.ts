/**
 * @generated SignedSource<<dbc1666b6f9aa675df3aa1a369c96506>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
export type NotificationNotificationType = "INVITATION_TO_JOIN_LOCATION" | "INVITATION_TO_JOIN_ORGANIZATION" | "INVITATION_TO_JOIN_TEAM" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type notificationCard_NotificationDetails$data = {
  readonly id: string;
  readonly notificationType: NotificationNotificationType;
  readonly " $fragmentSpreads": FragmentRefs<"invitationToJoinLocationNotificationCard_NotificationDetails" | "invitationToJoinOrganizationNotificationCard_NotificationDetails" | "invitationToJoinTeamNotificationCard_NotificationDetails">;
  readonly " $fragmentType": "notificationCard_NotificationDetails";
};
export type notificationCard_NotificationDetails$key = {
  readonly " $data"?: notificationCard_NotificationDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"notificationCard_NotificationDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "notificationCard_NotificationDetails",
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
      "args": null,
      "kind": "FragmentSpread",
      "name": "invitationToJoinOrganizationNotificationCard_NotificationDetails"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "invitationToJoinLocationNotificationCard_NotificationDetails"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "invitationToJoinTeamNotificationCard_NotificationDetails"
    }
  ],
  "type": "Notification",
  "abstractKey": null
};

(node as any).hash = "ba62ca10e0babf53bf3a2c34efded0ec";

export default node;
