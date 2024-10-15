/**
 * @generated SignedSource<<af8036c060f66e5c60fb0618dcdd951c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamPeopleBookings_query$data = {
  readonly me: {
    readonly defaultTeams: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
    readonly id: string;
  } | null | undefined;
  readonly team: {
    readonly canDelete: boolean;
    readonly canModify: boolean;
    readonly hasFutureBooking: boolean;
    readonly name: string;
    readonly organization: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
  } | null | undefined;
  readonly teamMembers: ReadonlyArray<{
    readonly customer: {
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
      readonly photoUrl: string | null | undefined;
      readonly uniqueId: string;
    };
    readonly id: string;
  }> | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"bookingsWeekGrid_allBookings_query" | "bookingsWeekGrid_query">;
  readonly " $fragmentType": "teamPeopleBookings_query";
};
export type teamPeopleBookings_query$key = {
  readonly " $data"?: teamPeopleBookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamPeopleBookings_query">;
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
  "name": "uniqueId",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "peopleSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "teamId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "teamPeopleBookings_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "peopleSortingValues"
        },
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "teamId",
              "variableName": "teamId"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "TeamMemberDetails",
      "kind": "LinkedField",
      "name": "teamMembers",
      "plural": true,
      "selections": [
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "TeamCustomerDetails",
          "kind": "LinkedField",
          "name": "customer",
          "plural": false,
          "selections": [
            (v1/*: any*/),
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
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "CustomerTeamDetails",
          "kind": "LinkedField",
          "name": "defaultTeams",
          "plural": true,
          "selections": [
            (v1/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "teamId"
        }
      ],
      "concreteType": "TeamDetails",
      "kind": "LinkedField",
      "name": "team",
      "plural": false,
      "selections": [
        (v2/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "hasFutureBooking",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canModify",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canDelete",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "TeamOrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
          "plural": false,
          "selections": [
            (v1/*: any*/),
            (v2/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingsWeekGrid_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingsWeekGrid_allBookings_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "878c27168fed06b22035579f8468fb52";

export default node;
