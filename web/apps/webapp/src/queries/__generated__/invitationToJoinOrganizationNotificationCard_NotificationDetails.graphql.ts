/**
 * @generated SignedSource<<163bb08f54952a15176bfea4317c676f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type invitationToJoinOrganizationNotificationCard_NotificationDetails$data = {
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
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly sourceId: string;
  readonly " $fragmentType": "invitationToJoinOrganizationNotificationCard_NotificationDetails";
};
export type invitationToJoinOrganizationNotificationCard_NotificationDetails$key = {
  readonly " $data"?: invitationToJoinOrganizationNotificationCard_NotificationDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"invitationToJoinOrganizationNotificationCard_NotificationDetails">;
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
  "name": "invitationToJoinOrganizationNotificationCard_NotificationDetails",
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
      "concreteType": "NotificationOrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
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

(node as any).hash = "42e5207b174c7a40461322ef4323b172";

export default node;
