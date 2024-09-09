/**
 * @generated SignedSource<<a66eb8199f751dfe4bc761edf06a9383>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type invitationToJoinTeamNotificationCard_NotificationDetails$data = {
  readonly id: string;
  readonly invitedBy: {
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  } | null | undefined;
  readonly invitee: {
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  } | null | undefined;
  readonly sourceId: string;
  readonly team: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentType": "invitationToJoinTeamNotificationCard_NotificationDetails";
};
export type invitationToJoinTeamNotificationCard_NotificationDetails$key = {
  readonly " $data"?: invitationToJoinTeamNotificationCard_NotificationDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"invitationToJoinTeamNotificationCard_NotificationDetails">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v1 = [
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
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "invitationToJoinTeamNotificationCard_NotificationDetails",
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
      "concreteType": "NotificationCustomerDetails",
      "kind": "LinkedField",
      "name": "invitedBy",
      "plural": false,
      "selections": (v1/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "NotificationCustomerDetails",
      "kind": "LinkedField",
      "name": "invitee",
      "plural": false,
      "selections": (v1/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "NotificationTeamDetails",
      "kind": "LinkedField",
      "name": "team",
      "plural": false,
      "selections": [
        (v0/*: any*/)
      ],
      "storageKey": null
    }
  ],
  "type": "Notification",
  "abstractKey": null
};
})();

(node as any).hash = "1c7b70ed5e3f6a0e8e2bf2c5e0f95547";

export default node;
