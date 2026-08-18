/**
 * @generated SignedSource<<3f49b06cbaee9cdc823a272105e0e686>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type customerEntitlementsStrip_query$variables = Record<PropertyKey, never>;
export type customerEntitlementsStrip_query$data = {
  readonly myEntitlements: ReadonlyArray<{
    readonly availableQuantity: number;
    readonly expiresAt: any;
    readonly grantedQuantity: number;
    readonly id: string;
    readonly pricingId: string;
    readonly restrictions: {
      readonly availableDays: ReadonlyArray<DayOfWeek>;
      readonly productId: string;
    } | null | undefined;
    readonly status: EntitlementStatus;
  }>;
};
export type customerEntitlementsStrip_query = {
  response: customerEntitlementsStrip_query$data;
  variables: customerEntitlementsStrip_query$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
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
        "name": "pricingId",
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
        "name": "grantedQuantity",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "expiresAt",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "status",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "EntitlementRestrictionsDetails",
        "kind": "LinkedField",
        "name": "restrictions",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "productId",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "availableDays",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "customerEntitlementsStrip_query",
    "selections": (v0/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "customerEntitlementsStrip_query",
    "selections": (v0/*:: as any*/)
  },
  "params": {
    "cacheID": "71a0f61bc50e0a0426edf373d4759b25",
    "id": null,
    "metadata": {},
    "name": "customerEntitlementsStrip_query",
    "operationKind": "query",
    "text": "query customerEntitlementsStrip_query {\n  myEntitlements {\n    id\n    pricingId\n    availableQuantity\n    grantedQuantity\n    expiresAt\n    status\n    restrictions {\n      productId\n      availableDays\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b419f5858961ef2c924182321d670010";

export default node;
