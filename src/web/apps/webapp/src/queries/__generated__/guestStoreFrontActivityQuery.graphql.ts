/**
 * @generated SignedSource<<41df4531c0e2ffd0993a353807fec753>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type guestStoreFrontActivityQuery$variables = {
  bookingsSearchCriteriaFrom: any;
  bookingsSearchCriteriaTo: any;
  organizationCustomDomain: string;
};
export type guestStoreFrontActivityQuery$data = {
  readonly bookings: {
    readonly totalCount: number;
  };
  readonly marketplaceBookingSubscriptions: {
    readonly totalCount: number;
  };
  readonly myEntitlements: ReadonlyArray<{
    readonly availableQuantity: number;
    readonly id: string;
    readonly status: EntitlementStatus;
  }>;
};
export type guestStoreFrontActivityQuery = {
  response: guestStoreFrontActivityQuery$data;
  variables: guestStoreFrontActivityQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingsSearchCriteriaFrom"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingsSearchCriteriaTo"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v3 = {
  "kind": "Literal",
  "name": "first",
  "value": 0
},
v4 = {
  "kind": "Literal",
  "name": "includeMineOnly",
  "value": true
},
v5 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v6 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "totalCount",
    "storageKey": null
  }
],
v7 = [
  {
    "alias": null,
    "args": [
      (v3/*:: as any*/),
      {
        "fields": [
          {
            "kind": "Literal",
            "name": "channel",
            "value": "MARKETPLACE"
          },
          {
            "kind": "Variable",
            "name": "fromGte",
            "variableName": "bookingsSearchCriteriaFrom"
          },
          {
            "kind": "Variable",
            "name": "fromLte",
            "variableName": "bookingsSearchCriteriaTo"
          },
          (v4/*:: as any*/),
          (v5/*:: as any*/)
        ],
        "kind": "ObjectValue",
        "name": "where"
      }
    ],
    "concreteType": "ConnectionOfBookingEdge",
    "kind": "LinkedField",
    "name": "bookings",
    "plural": false,
    "selections": (v6/*:: as any*/),
    "storageKey": null
  },
  {
    "alias": null,
    "args": [
      (v3/*:: as any*/),
      {
        "fields": [
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          {
            "kind": "Literal",
            "name": "status",
            "value": "ACTIVE"
          }
        ],
        "kind": "ObjectValue",
        "name": "where"
      }
    ],
    "concreteType": "ConnectionOfMarketplaceBookingSubscriptionEdge",
    "kind": "LinkedField",
    "name": "marketplaceBookingSubscriptions",
    "plural": false,
    "selections": (v6/*:: as any*/),
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "EntitlementDetails",
    "kind": "LinkedField",
    "name": "myEntitlements",
    "plural": true,
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
        "name": "availableQuantity",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "status",
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "guestStoreFrontActivityQuery",
    "selections": (v7/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*:: as any*/),
      (v0/*:: as any*/),
      (v1/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "guestStoreFrontActivityQuery",
    "selections": (v7/*:: as any*/)
  },
  "params": {
    "cacheID": "3d04dd4cb00bfef89cfff02fe7f3d63c",
    "id": null,
    "metadata": {},
    "name": "guestStoreFrontActivityQuery",
    "operationKind": "query",
    "text": "query guestStoreFrontActivityQuery(\n  $organizationCustomDomain: String!\n  $bookingsSearchCriteriaFrom: DateTime!\n  $bookingsSearchCriteriaTo: DateTime!\n) {\n  bookings(first: 0, where: {organizationCustomDomain: $organizationCustomDomain, fromGte: $bookingsSearchCriteriaFrom, fromLte: $bookingsSearchCriteriaTo, includeMineOnly: true, channel: MARKETPLACE}) {\n    totalCount\n  }\n  marketplaceBookingSubscriptions(first: 0, where: {includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, status: ACTIVE}) {\n    totalCount\n  }\n  myEntitlements {\n    id\n    availableQuantity\n    status\n  }\n}\n"
  }
};
})();

(node as any).hash = "996d6c7169e97e83c6f602221d4ab600";

export default node;
