/**
 * @generated SignedSource<<8a7b7107e3ab4eb174a14a027c78f53f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type oldBookingCard_query$data = {
  readonly me: {
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly id: string;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
    readonly preferredDesks: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
  } | null | undefined;
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }> | null | undefined;
  readonly myOrganizations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }> | null | undefined;
  readonly organizationBookingPermissions: {
    readonly canDeleteBookingOnBehalf: boolean;
    readonly canUpdateBookingOnBehalf: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"bookingDetailsSelector_availableLocationDesks_query" | "bookingDetailsSelector_organizationMembers_query" | "bookingDetailsSelector_query">;
  readonly " $fragmentType": "oldBookingCard_query";
};
export type oldBookingCard_query$key = {
  readonly " $data"?: oldBookingCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"oldBookingCard_query">;
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
  (v0/*: any*/),
  (v1/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "nullableOrganizationId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "oldBookingCard_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
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
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "CustomerDeskDetails",
          "kind": "LinkedField",
          "name": "preferredDesks",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "uniqueId",
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
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "myOrganizations",
      "plural": true,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "organizationId",
          "variableName": "nullableOrganizationId"
        }
      ],
      "concreteType": "LocationDetails",
      "kind": "LinkedField",
      "name": "myLocations",
      "plural": true,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "organizationId",
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationBookingPermissions",
      "kind": "LinkedField",
      "name": "organizationBookingPermissions",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canUpdateBookingOnBehalf",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canDeleteBookingOnBehalf",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingDetailsSelector_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingDetailsSelector_organizationMembers_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingDetailsSelector_availableLocationDesks_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "c595c5e1c7dda69e388c94efae5f550f";

export default node;
