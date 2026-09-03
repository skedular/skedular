/**
 * @generated SignedSource<<1faa1cd6b0b3caacce120fcb3a222462>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type EntitlementStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PENDING" | "%future added value";
export type entitlementBookingPage_rootQuery$variables = {
  entitlementId: string;
};
export type entitlementBookingPage_rootQuery$data = {
  readonly entitlement: {
    readonly activatesAt: any;
    readonly availableQuantity: number;
    readonly expiresAt: any;
    readonly grantedQuantity: number;
    readonly id: string;
    readonly organizationCustomDomain: string;
    readonly pricingId: string;
    readonly productId: string;
    readonly remainingWeeklyRedemptions: number | null | undefined;
    readonly restrictions: {
      readonly availableDays: ReadonlyArray<DayOfWeek>;
      readonly maxDurationMinutes: number | null | undefined;
      readonly minDurationMinutes: number | null | undefined;
      readonly numberOfResourcesToBook: number;
      readonly productVersionId: string;
    } | null | undefined;
    readonly status: EntitlementStatus;
  } | null | undefined;
  readonly me: {
    readonly id: string;
  };
};
export type entitlementBookingPage_rootQuery = {
  response: entitlementBookingPage_rootQuery$data;
  variables: entitlementBookingPage_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "entitlementId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "CustomerDetails",
    "kind": "LinkedField",
    "name": "me",
    "plural": false,
    "selections": [
      (v1/*:: as any*/)
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "entitlementId"
      }
    ],
    "concreteType": "EntitlementDetails",
    "kind": "LinkedField",
    "name": "entitlement",
    "plural": false,
    "selections": [
      (v1/*:: as any*/),
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
        "name": "pricingId",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "organizationCustomDomain",
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
        "name": "remainingWeeklyRedemptions",
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
        "name": "activatesAt",
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
            "name": "productVersionId",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "availableDays",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "minDurationMinutes",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "maxDurationMinutes",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "numberOfResourcesToBook",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "entitlementBookingPage_rootQuery",
    "selections": (v2/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "entitlementBookingPage_rootQuery",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "403231bc66378850c2868ab6edf18dba",
    "id": null,
    "metadata": {},
    "name": "entitlementBookingPage_rootQuery",
    "operationKind": "query",
    "text": "query entitlementBookingPage_rootQuery(\n  $entitlementId: String!\n) {\n  me {\n    id\n  }\n  entitlement(id: $entitlementId) {\n    id\n    productId\n    pricingId\n    organizationCustomDomain\n    availableQuantity\n    remainingWeeklyRedemptions\n    grantedQuantity\n    activatesAt\n    expiresAt\n    status\n    restrictions {\n      productVersionId\n      availableDays\n      minDurationMinutes\n      maxDurationMinutes\n      numberOfResourcesToBook\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "97541754334cfc688e6a9b4d02f5fe00";

export default node;
