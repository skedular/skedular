/**
 * @generated SignedSource<<01640a6b9f2781bd9900cb8709ebbf4e>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type dashboardHostDataQuery$variables = {
  organizationId: string;
};
export type dashboardHostDataQuery$data = {
  readonly bookings: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly from: any;
        readonly id: string;
        readonly involvedLocations: ReadonlyArray<{
          readonly name: string;
        }>;
        readonly marketplaceBooking: {
          readonly hostCommissionAmount: any | null | undefined;
          readonly hostCommissionRatePercentage: any | null | undefined;
          readonly hostGrossProceedsAmount: any | null | undefined;
          readonly paymentStatus: {
            readonly name: string;
          };
          readonly totalAmount: any | null | undefined;
        } | null | undefined;
        readonly until: any;
      };
    }>;
    readonly totalCount: number;
  };
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
  }> | null | undefined;
  readonly products: {
    readonly totalCount: number;
  };
};
export type dashboardHostDataQuery = {
  response: dashboardHostDataQuery$data;
  variables: dashboardHostDataQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "organizationId",
    "variableName": "organizationId"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": (v1/*:: as any*/),
  "concreteType": "LocationDetails",
  "kind": "LinkedField",
  "name": "myLocations",
  "plural": true,
  "selections": [
    (v2/*:: as any*/)
  ],
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": [
    {
      "kind": "Literal",
      "name": "first",
      "value": 1
    },
    {
      "fields": [
        {
          "kind": "Literal",
          "name": "includeInactive",
          "value": true
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
        }
      ],
      "kind": "ObjectValue",
      "name": "where"
    }
  ],
  "concreteType": "ConnectionOfProductEdge",
  "kind": "LinkedField",
  "name": "products",
  "plural": false,
  "selections": [
    (v4/*:: as any*/)
  ],
  "storageKey": null
},
v6 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 100
  },
  {
    "kind": "Literal",
    "name": "orderBy",
    "value": [
      {
        "direction": "DESCENDING",
        "field": "FROM"
      }
    ]
  },
  {
    "fields": (v1/*:: as any*/),
    "kind": "ObjectValue",
    "name": "where"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v9 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "name",
    "storageKey": null
  }
],
v10 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_LocationDetails",
  "kind": "LinkedField",
  "name": "involvedLocations",
  "plural": true,
  "selections": (v9/*:: as any*/),
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": (v9/*:: as any*/),
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalAmount",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "hostCommissionRatePercentage",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "hostCommissionAmount",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "hostGrossProceedsAmount",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "dashboardHostDataQuery",
    "selections": [
      (v3/*:: as any*/),
      (v5/*:: as any*/),
      {
        "alias": null,
        "args": (v6/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v4/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v2/*:: as any*/),
                  (v7/*:: as any*/),
                  (v8/*:: as any*/),
                  (v10/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v11/*:: as any*/),
                      (v12/*:: as any*/),
                      (v13/*:: as any*/),
                      (v14/*:: as any*/),
                      (v15/*:: as any*/)
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
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "dashboardHostDataQuery",
    "selections": [
      (v3/*:: as any*/),
      (v5/*:: as any*/),
      {
        "alias": null,
        "args": (v6/*:: as any*/),
        "concreteType": "ConnectionOfBookingEdge",
        "kind": "LinkedField",
        "name": "bookings",
        "plural": false,
        "selections": [
          (v4/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v2/*:: as any*/),
                  (v7/*:: as any*/),
                  (v8/*:: as any*/),
                  (v10/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceBookingDetails",
                    "kind": "LinkedField",
                    "name": "marketplaceBooking",
                    "plural": false,
                    "selections": [
                      (v11/*:: as any*/),
                      (v12/*:: as any*/),
                      (v13/*:: as any*/),
                      (v14/*:: as any*/),
                      (v15/*:: as any*/),
                      (v2/*:: as any*/)
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
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "fc808dfd542552ac9e20eb0319fd9016",
    "id": null,
    "metadata": {},
    "name": "dashboardHostDataQuery",
    "operationKind": "query",
    "text": "query dashboardHostDataQuery(\n  $organizationId: String!\n) {\n  myLocations(organizationId: $organizationId) {\n    id\n  }\n  products(first: 1, where: {organizationIds: [$organizationId], includeInactive: true}) {\n    totalCount\n  }\n  bookings(first: 100, where: {organizationId: $organizationId}, orderBy: [{field: FROM, direction: DESCENDING}]) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        until\n        involvedLocations {\n          name\n        }\n        marketplaceBooking {\n          paymentStatus {\n            name\n          }\n          totalAmount\n          hostCommissionRatePercentage\n          hostCommissionAmount\n          hostGrossProceedsAmount\n          id\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "103dfd4eff406f279e4d6d2b48038032";

export default node;
