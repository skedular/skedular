/**
 * @generated SignedSource<<52c13434dce0aca8929cb505d1017d2b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingType = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type bookingCard_BookingDetails$data = {
  readonly bookingResources: ReadonlyArray<{
    readonly resource: {
      readonly color: string | null | undefined;
      readonly customTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly id: string;
      readonly name: string;
      readonly zones: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
    };
  }>;
  readonly from: any;
  readonly id: string;
  readonly invoiceUrl: string | null | undefined;
  readonly involvedCustomers: ReadonlyArray<{
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly id: string;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  }>;
  readonly involvedLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }>;
  readonly involvedOrganizations: ReadonlyArray<{
    readonly id: string;
  }>;
  readonly involvedTeams: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }>;
  readonly isPaymentRequired: boolean;
  readonly notes: string | null | undefined;
  readonly paymentStatus: {
    readonly name: string;
    readonly type: PaymentStatus;
  };
  readonly type: {
    readonly name: string;
    readonly type: BookingType;
  };
  readonly until: any;
  readonly " $fragmentType": "bookingCard_BookingDetails";
};
export type bookingCard_BookingDetails$key = {
  readonly " $data"?: bookingCard_BookingDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookingCard_BookingDetails">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
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
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v1/*: any*/)
],
v3 = [
  (v0/*: any*/),
  (v1/*: any*/)
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v5 = [
  (v0/*: any*/),
  (v1/*: any*/),
  (v4/*: any*/)
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "bookingCard_BookingDetails",
  "selections": [
    (v0/*: any*/),
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
      "name": "until",
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
      "concreteType": "BookingTypeDetails",
      "kind": "LinkedField",
      "name": "type",
      "plural": false,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "involvedCustomers",
      "plural": true,
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
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "involvedOrganizations",
      "plural": true,
      "selections": [
        (v0/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "LocationDetails",
      "kind": "LinkedField",
      "name": "involvedLocations",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "TeamDetails",
      "kind": "LinkedField",
      "name": "involvedTeams",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingResourceDetails",
      "kind": "LinkedField",
      "name": "bookingResources",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "ResourceDetails",
          "kind": "LinkedField",
          "name": "resource",
          "plural": false,
          "selections": [
            (v0/*: any*/),
            (v1/*: any*/),
            (v4/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "customTags",
              "plural": true,
              "selections": (v5/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "zones",
              "plural": true,
              "selections": (v5/*: any*/),
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "isPaymentRequired",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "PaymentStatusDetails",
      "kind": "LinkedField",
      "name": "paymentStatus",
      "plural": false,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "invoiceUrl",
      "storageKey": null
    }
  ],
  "type": "BookingDetails",
  "abstractKey": null
};
})();

(node as any).hash = "21ba6f4c4554ed2f46d8e5743dff391d";

export default node;
