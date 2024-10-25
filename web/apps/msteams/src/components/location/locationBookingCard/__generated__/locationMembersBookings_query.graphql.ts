/**
 * @generated SignedSource<<b47dd5e87a2bfbc493530d76ad9dd962>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationMembersBookings_query$data = {
  readonly customersByDefaultLocation: ReadonlyArray<{
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly id: string;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  }> | null | undefined;
  readonly location: {
    readonly canDelete: boolean;
    readonly canModify: boolean;
    readonly deskCapacity: number;
    readonly hasFutureBooking: boolean;
    readonly name: string;
    readonly organization: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
  } | null | undefined;
  readonly locationMembers: ReadonlyArray<{
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
  readonly me: {
    readonly defaultLocations: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"bookingsWeekGrid_allBookings_query" | "bookingsWeekGrid_query">;
  readonly " $fragmentType": "locationMembersBookings_query";
};
export type locationMembersBookings_query$key = {
  readonly " $data"?: locationMembersBookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationMembersBookings_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "fields": [
    {
      "kind": "Variable",
      "name": "locationId",
      "variableName": "locationId"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
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
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "peopleSortingValues"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationMembersBookings_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "peopleSortingValues"
        },
        (v0/*: any*/)
      ],
      "concreteType": "LocationMemberDetails",
      "kind": "LinkedField",
      "name": "locationMembers",
      "plural": true,
      "selections": [
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationCustomerDetails",
          "kind": "LinkedField",
          "name": "customer",
          "plural": false,
          "selections": [
            (v2/*: any*/),
            (v3/*: any*/),
            (v4/*: any*/),
            (v5/*: any*/),
            (v6/*: any*/),
            (v7/*: any*/)
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
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "customersByDefaultLocation",
      "plural": true,
      "selections": [
        (v1/*: any*/),
        (v3/*: any*/),
        (v4/*: any*/),
        (v5/*: any*/),
        (v6/*: any*/),
        (v7/*: any*/)
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
          "concreteType": "CustomerLocationDetails",
          "kind": "LinkedField",
          "name": "defaultLocations",
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
          "variableName": "locationId"
        }
      ],
      "concreteType": "LocationDetails",
      "kind": "LinkedField",
      "name": "location",
      "plural": false,
      "selections": [
        (v3/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "deskCapacity",
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
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationOrganizationDetails",
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

(node as any).hash = "1261316d02d3f2673a0506c943b07145";

export default node;
