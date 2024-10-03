/**
 * @generated SignedSource<<0c78e2b671e85fae731f3d070f1c5a64>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type customerTodaySummary_rootQuery$variables = {
  from: any;
  to: any;
};
export type customerTodaySummary_rootQuery$data = {
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
      readonly uniqueId: string;
    } | null | undefined;
    readonly team: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
    readonly to: any;
  }>;
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
  }>;
  readonly myTeams: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
  }>;
};
export type customerTodaySummary_rootQuery = {
  response: customerTodaySummary_rootQuery$data;
  variables: customerTodaySummary_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "from"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "to"
  }
],
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
  (v3/*: any*/)
],
v5 = [
  (v2/*: any*/)
],
v6 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "CustomerDetails",
    "kind": "LinkedField",
    "name": "me",
    "plural": false,
    "selections": [
      (v1/*: any*/)
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
            "kind": "Variable",
            "name": "toLTE",
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
        "selections": [
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
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingLocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": (v4/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingTeamDetails",
        "kind": "LinkedField",
        "name": "team",
        "plural": false,
        "selections": (v4/*: any*/),
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
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "LocationDetails",
    "kind": "LinkedField",
    "name": "myLocations",
    "plural": true,
    "selections": [
      (v1/*: any*/),
      (v3/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationOrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v5/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "TeamDetails",
    "kind": "LinkedField",
    "name": "myTeams",
    "plural": true,
    "selections": [
      (v1/*: any*/),
      (v3/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "TeamOrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v5/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "customerTodaySummary_rootQuery",
    "selections": (v6/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "customerTodaySummary_rootQuery",
    "selections": (v6/*: any*/)
  },
  "params": {
    "cacheID": "beb15ec2ba88b98e2e16509b5f97213c",
    "id": null,
    "metadata": {},
    "name": "customerTodaySummary_rootQuery",
    "operationKind": "query",
    "text": "query customerTodaySummary_rootQuery(\n  $from: DateTime!\n  $to: DateTime!\n) {\n  me {\n    id\n  }\n  allBookings(where: {fromGTE: $from, toLTE: $to}) {\n    id\n    from\n    to\n    customer {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    location {\n      uniqueId\n      name\n    }\n    team {\n      uniqueId\n      name\n    }\n    desks {\n      name\n      locationTags {\n        uniqueId\n        name\n        tagType\n      }\n    }\n  }\n  myLocations {\n    id\n    name\n    organization {\n      uniqueId\n    }\n  }\n  myTeams {\n    id\n    name\n    organization {\n      uniqueId\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "20905cdbf4bc6a03357548d9bfbbd756";

export default node;
