/**
 * @generated SignedSource<<0a371229acff7a0535c28177cd18c8cf>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type entitlementBookingPage_availableResourcesQuery$variables = {
  from: any;
  locationId?: string | null | undefined;
  organizationCustomDomain: string;
  productId: string;
  until: any;
};
export type entitlementBookingPage_availableResourcesQuery$data = {
  readonly availableResources: ReadonlyArray<{
    readonly location: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
    readonly resource: {
      readonly id: string;
      readonly name: string;
    };
  }>;
  readonly marketplaceLocations: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
      };
    }>;
  };
};
export type entitlementBookingPage_availableResourcesQuery = {
  response: entitlementBookingPage_availableResourcesQuery$data;
  variables: entitlementBookingPage_availableResourcesQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "from"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productId"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "until"
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v6 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "id",
    "storageKey": null
  },
  (v5/*:: as any*/)
],
v7 = [
  {
    "alias": null,
    "args": [
      {
        "fields": [
          {
            "items": [
              {
                "kind": "Variable",
                "name": "productIds.0",
                "variableName": "productId"
              }
            ],
            "kind": "ListValue",
            "name": "productIds"
          }
        ],
        "kind": "ObjectValue",
        "name": "where"
      }
    ],
    "concreteType": "ConnectionOfLocationEdge",
    "kind": "LinkedField",
    "name": "marketplaceLocations",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationEdge",
        "kind": "LinkedField",
        "name": "edges",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "node",
            "plural": false,
            "selections": (v6/*:: as any*/),
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
    "args": [
      {
        "fields": [
          {
            "kind": "Variable",
            "name": "from",
            "variableName": "from"
          },
          {
            "kind": "Variable",
            "name": "locationId",
            "variableName": "locationId"
          },
          {
            "kind": "Variable",
            "name": "organizationCustomDomain",
            "variableName": "organizationCustomDomain"
          },
          {
            "kind": "Variable",
            "name": "productId",
            "variableName": "productId"
          },
          {
            "kind": "Variable",
            "name": "until",
            "variableName": "until"
          }
        ],
        "kind": "ObjectValue",
        "name": "where"
      }
    ],
    "concreteType": "BookingResourceDetails",
    "kind": "LinkedField",
    "name": "availableResources",
    "plural": true,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "ResourceDetails",
        "kind": "LinkedField",
        "name": "resource",
        "plural": false,
        "selections": (v6/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "Booking_LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "uniqueId",
            "storageKey": null
          },
          (v5/*:: as any*/)
        ],
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
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "entitlementBookingPage_availableResourcesQuery",
    "selections": (v7/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v1/*:: as any*/),
      (v0/*:: as any*/),
      (v4/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "entitlementBookingPage_availableResourcesQuery",
    "selections": (v7/*:: as any*/)
  },
  "params": {
    "cacheID": "76c1630eb2d5150950fdecd3a9b8f076",
    "id": null,
    "metadata": {},
    "name": "entitlementBookingPage_availableResourcesQuery",
    "operationKind": "query",
    "text": "query entitlementBookingPage_availableResourcesQuery(\n  $organizationCustomDomain: String!\n  $productId: String!\n  $locationId: String\n  $from: DateTime!\n  $until: DateTime!\n) {\n  marketplaceLocations(where: {productIds: [$productId]}) {\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  availableResources(where: {organizationCustomDomain: $organizationCustomDomain, productId: $productId, locationId: $locationId, from: $from, until: $until}) {\n    resource {\n      id\n      name\n    }\n    location {\n      uniqueId\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "78d8fb375a818bc89f50994466b4f40e";

export default node;
