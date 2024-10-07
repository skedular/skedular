/**
 * @generated SignedSource<<0409cd9960d1990744c3450e757a3854>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationPeopleBookings_query$data = {
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
    readonly team: {
      readonly name: string;
    } | null | undefined;
    readonly to: any;
  }> | null | undefined;
  readonly me: {
    readonly defaultOrganization: {
      readonly uniqueId: string;
    } | null | undefined;
    readonly id: string;
  } | null | undefined;
  readonly organization: {
    readonly canDelete: boolean;
    readonly canModify: boolean;
    readonly hasFutureBooking: boolean;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
  readonly organizationBookingPermissions?: {
    readonly canAddBookingOnBehalf: boolean;
    readonly canDeleteBookingOnBehalf: boolean;
  } | null | undefined;
  readonly organizationMembers?: ReadonlyArray<{
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
  readonly " $fragmentType": "organizationPeopleBookings_query";
};
export type organizationPeopleBookings_query$key = {
  readonly " $data"?: organizationPeopleBookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationPeopleBookings_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
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
],
v5 = [
  (v3/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "from"
    },
    {
      "kind": "RootArgument",
      "name": "organizationExists"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
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
      "name": "to"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./organizationPeopleBookingsOrganizationMembers_PaginationQuery.graphql')
    }
  },
  "name": "organizationPeopleBookings_query",
  "selections": [
    {
      "condition": "organizationExists",
      "kind": "Condition",
      "passingValue": true,
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
          "concreteType": "OrganizationMemberDetails",
          "kind": "LinkedField",
          "name": "organizationMembers",
          "plural": true,
          "selections": [
            (v1/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationCustomerDetails",
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
          "args": [
            (v0/*: any*/)
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
        }
      ]
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
          "concreteType": "CustomerOrganizationDetails",
          "kind": "LinkedField",
          "name": "defaultOrganization",
          "plural": false,
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
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        (v3/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "logoUrl",
          "storageKey": null
        },
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
                  "name": "organizationIds.0",
                  "variableName": "organizationId"
                }
              ],
              "kind": "ListValue",
              "name": "organizationIds"
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
          "kind": "ScalarField",
          "name": "to",
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
          "selections": (v5/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingTeamDetails",
          "kind": "LinkedField",
          "name": "team",
          "plural": false,
          "selections": (v5/*: any*/),
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

(node as any).hash = "ed188be8e6996e8620a670bc8a41e226";

export default node;
