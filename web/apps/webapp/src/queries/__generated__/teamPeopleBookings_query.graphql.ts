/**
 * @generated SignedSource<<921dd0f603bc0f78587bb5280f49e018>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamPeopleBookings_query$data = {
  readonly allBookings: ReadonlyArray<{
    readonly customer: {
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
      readonly photoUrl: string | null | undefined;
      readonly uniqueId: string;
    };
    readonly desks: ReadonlyArray<{
      readonly locationTags: ReadonlyArray<{
        readonly name: string;
        readonly tagType: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly name: string;
    }>;
    readonly from: any;
    readonly id: string;
    readonly location: {
      readonly name: string;
    } | null | undefined;
  }>;
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
  readonly teamBookingPermissions: {
    readonly canAddBookingOnBehalf: boolean;
    readonly canDeleteBookingOnBehalf: boolean;
  };
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
  }>;
  readonly " $fragmentType": "teamPeopleBookings_query";
};
export type teamPeopleBookings_query$key = {
  readonly " $data"?: teamPeopleBookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamPeopleBookings_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "kind": "Variable",
  "name": "teamId",
  "variableName": "teamId"
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = [
  (v2/*: any*/),
  (v3/*: any*/),
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
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "from"
    },
    {
      "kind": "RootArgument",
      "name": "peopleNameSearchText"
    },
    {
      "kind": "RootArgument",
      "name": "peopleSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "teamId"
    },
    {
      "kind": "RootArgument",
      "name": "to"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./teamPeopleBookingsTeamMembers_PaginationQuery.graphql')
    }
  },
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
              "name": "nameContains",
              "variableName": "peopleNameSearchText"
            },
            (v0/*: any*/)
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
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "TeamCustomerDetails",
          "kind": "LinkedField",
          "name": "customer",
          "plural": false,
          "selections": (v4/*: any*/),
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
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "CustomerTeamDetails",
          "kind": "LinkedField",
          "name": "defaultTeams",
          "plural": true,
          "selections": [
            (v2/*: any*/)
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
        (v3/*: any*/),
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
            (v2/*: any*/),
            (v3/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        (v0/*: any*/)
      ],
      "concreteType": "TeamBookingPermissions",
      "kind": "LinkedField",
      "name": "teamBookingPermissions",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canAddBookingOnBehalf",
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
      "alias": null,
      "args": [
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "fromGTE",
              "variableName": "from"
            },
            {
              "items": [
                {
                  "kind": "Variable",
                  "name": "teamIds.0",
                  "variableName": "teamId"
                }
              ],
              "kind": "ListValue",
              "name": "teamIds"
            },
            {
              "kind": "Variable",
              "name": "toLT",
              "variableName": "to"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "BookingDetails",
      "kind": "LinkedField",
      "name": "allBookings",
      "plural": true,
      "selections": [
        (v1/*: any*/),
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
          "concreteType": "BookingCustomerDetails",
          "kind": "LinkedField",
          "name": "customer",
          "plural": false,
          "selections": (v4/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingLocationDetails",
          "kind": "LinkedField",
          "name": "location",
          "plural": false,
          "selections": [
            (v3/*: any*/)
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingDeskDetails",
          "kind": "LinkedField",
          "name": "desks",
          "plural": true,
          "selections": [
            (v3/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "BookingLocationTagDetails",
              "kind": "LinkedField",
              "name": "locationTags",
              "plural": true,
              "selections": [
                (v2/*: any*/),
                (v3/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "tagType",
                  "storageKey": null
                }
              ],
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "0943b3f1356c39a03c81e442f9af1708";

export default node;
