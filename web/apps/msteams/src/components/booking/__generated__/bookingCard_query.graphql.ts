/**
 * @generated SignedSource<<12052d826c538440121d1b0b0a3828bf>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookingCard_query$data = {
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
  }>;
  readonly myOrganizations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }>;
  readonly organization: {
    readonly canDeleteBookingOnBehalf: boolean;
    readonly canUpdateBookingOnBehalf: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"bookingDetailsSelector_query">;
  readonly " $fragmentType": "bookingCard_query";
};
export type bookingCard_query$key = {
  readonly " $data"?: bookingCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookingCard_query">;
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
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "bookingCard_query",
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
          "variableName": "organizationId"
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
          "name": "id",
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "6542769515d9a607d0498086de36b84b";

export default node;
