/**
 * @generated SignedSource<<8f8528d93c7d235a3c782c089d31016d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type teamBookingsCard_rootQuery$variables = {
  fetchBookingPermission: boolean;
  from: any;
  organizationId: string;
  teamId: string;
  to: any;
};
export type teamBookingsCard_rootQuery$data = {
  readonly allBookings: ReadonlyArray<{
    readonly customer: {
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
    readonly id: string;
  } | null | undefined;
  readonly organizationBookingPermissions?: {
    readonly canAddBookingOnBehalf: boolean;
  };
  readonly team: {
    readonly members: ReadonlyArray<{
      readonly customer: {
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
        readonly uniqueId: string;
      } | null | undefined;
      readonly id: string;
    }>;
  } | null | undefined;
};
export type teamBookingsCard_rootQuery = {
  response: teamBookingsCard_rootQuery$data;
  variables: teamBookingsCard_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "fetchBookingPermission"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "from"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamId"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "to"
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v5/*: any*/)
  ],
  "storageKey": null
},
v7 = {
  "condition": "fetchBookingPermission",
  "kind": "Condition",
  "passingValue": true,
  "selections": [
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
          "name": "canAddBookingOnBehalf",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ]
},
v8 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "teamId"
  }
],
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamMemberDetails",
  "kind": "LinkedField",
  "name": "members",
  "plural": true,
  "selections": [
    (v5/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "TeamCustomerDetails",
      "kind": "LinkedField",
      "name": "customer",
      "plural": false,
      "selections": [
        (v9/*: any*/),
        (v10/*: any*/),
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
v12 = {
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
    (v5/*: any*/),
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
      "selections": [
        (v9/*: any*/)
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
      "selections": [
        (v10/*: any*/)
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
        (v10/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingLocationTagDetails",
          "kind": "LinkedField",
          "name": "locationTags",
          "plural": true,
          "selections": [
            (v9/*: any*/),
            (v10/*: any*/),
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
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "teamBookingsCard_rootQuery",
    "selections": [
      (v6/*: any*/),
      (v7/*: any*/),
      {
        "alias": null,
        "args": (v8/*: any*/),
        "concreteType": "TeamDetails",
        "kind": "LinkedField",
        "name": "team",
        "plural": false,
        "selections": [
          (v11/*: any*/)
        ],
        "storageKey": null
      },
      (v12/*: any*/)
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v1/*: any*/),
      (v4/*: any*/)
    ],
    "kind": "Operation",
    "name": "teamBookingsCard_rootQuery",
    "selections": [
      (v6/*: any*/),
      (v7/*: any*/),
      {
        "alias": null,
        "args": (v8/*: any*/),
        "concreteType": "TeamDetails",
        "kind": "LinkedField",
        "name": "team",
        "plural": false,
        "selections": [
          (v11/*: any*/),
          (v5/*: any*/)
        ],
        "storageKey": null
      },
      (v12/*: any*/)
    ]
  },
  "params": {
    "cacheID": "831ebdcbe3206a346d5214da5134c2d6",
    "id": null,
    "metadata": {},
    "name": "teamBookingsCard_rootQuery",
    "operationKind": "query",
    "text": "query teamBookingsCard_rootQuery(\n  $fetchBookingPermission: Boolean!\n  $organizationId: String!\n  $teamId: String!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  me {\n    id\n  }\n  organizationBookingPermissions(organizationId: $organizationId) @include(if: $fetchBookingPermission) {\n    canAddBookingOnBehalf\n  }\n  team(id: $teamId) {\n    members {\n      id\n      customer {\n        uniqueId\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n    }\n    id\n  }\n  allBookings(where: {teamIds: [$teamId], fromGTE: $from, toLT: $to}) {\n    id\n    from\n    customer {\n      uniqueId\n    }\n    location {\n      name\n    }\n    desks {\n      name\n      locationTags {\n        uniqueId\n        name\n        tagType\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "69c9f2e8830c709ba76eb7e2eec19f66";

export default node;
