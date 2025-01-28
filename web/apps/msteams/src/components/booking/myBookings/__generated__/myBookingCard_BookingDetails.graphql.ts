/**
 * @generated SignedSource<<399b7bcc8081aeb287b54f94fab9e401>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myBookingCard_BookingDetails$data = {
  readonly customer: {
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
    readonly uniqueId: string;
  };
  readonly desks: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly customTags: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
    readonly name: string;
    readonly uniqueId: string;
    readonly zones: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
  }>;
  readonly from: any;
  readonly id: string;
  readonly location: {
    readonly name: string;
    readonly uniqueId: string;
  } | null | undefined;
  readonly notes: string | null | undefined;
  readonly rooms: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly customTags: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
    readonly name: string;
    readonly uniqueId: string;
    readonly zones: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
  }>;
  readonly team: {
    readonly name: string;
    readonly uniqueId: string;
  } | null | undefined;
  readonly to: any;
  readonly " $fragmentType": "myBookingCard_BookingDetails";
};
export type myBookingCard_BookingDetails$key = {
  readonly " $data"?: myBookingCard_BookingDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"myBookingCard_BookingDetails">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  (v0/*: any*/),
  (v1/*: any*/)
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v4 = [
  (v0/*: any*/),
  (v1/*: any*/),
  (v3/*: any*/)
],
v5 = [
  (v0/*: any*/),
  (v1/*: any*/),
  (v3/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingOrganizationCustomTagDetails",
    "kind": "LinkedField",
    "name": "customTags",
    "plural": true,
    "selections": (v4/*: any*/),
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "BookingOrganizationZoneDetails",
    "kind": "LinkedField",
    "name": "zones",
    "plural": true,
    "selections": (v4/*: any*/),
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "myBookingCard_BookingDetails",
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
      "name": "from",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "to",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "notes",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingCustomerDetails",
      "kind": "LinkedField",
      "name": "customer",
      "plural": false,
      "selections": [
        (v0/*: any*/),
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
      "concreteType": "BookingLocationDetails",
      "kind": "LinkedField",
      "name": "location",
      "plural": false,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingTeamDetails",
      "kind": "LinkedField",
      "name": "team",
      "plural": false,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingDeskDetails",
      "kind": "LinkedField",
      "name": "desks",
      "plural": true,
      "selections": (v5/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingRoomDetails",
      "kind": "LinkedField",
      "name": "rooms",
      "plural": true,
      "selections": (v5/*: any*/),
      "storageKey": null
    }
  ],
  "type": "BookingDetails",
  "abstractKey": null
};
})();

(node as any).hash = "1ea82a0ca45299439bfb84b48ea59b8b";

export default node;
