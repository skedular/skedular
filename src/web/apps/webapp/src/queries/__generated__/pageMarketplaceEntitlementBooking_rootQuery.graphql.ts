/**
 * @generated SignedSource<<2decd58482f6145205544807f20c344d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageMarketplaceEntitlementBooking_rootQuery$variables = {
  entitlementId: string;
};
export type pageMarketplaceEntitlementBooking_rootQuery$data = {
  readonly entitlement: {
    readonly id: string;
    readonly organizationCustomDomain: string;
    readonly productId: string;
  } | null | undefined;
};
export type pageMarketplaceEntitlementBooking_rootQuery = {
  response: pageMarketplaceEntitlementBooking_rootQuery$data;
  variables: pageMarketplaceEntitlementBooking_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "entitlementId"
  }
],
v1 = [
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
        "name": "productId",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "organizationCustomDomain",
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
    "name": "pageMarketplaceEntitlementBooking_rootQuery",
    "selections": (v1/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageMarketplaceEntitlementBooking_rootQuery",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "27a04551844e1c5ff0bbb1cb04e31bec",
    "id": null,
    "metadata": {},
    "name": "pageMarketplaceEntitlementBooking_rootQuery",
    "operationKind": "query",
    "text": "query pageMarketplaceEntitlementBooking_rootQuery(\n  $entitlementId: String!\n) {\n  entitlement(id: $entitlementId) {\n    id\n    productId\n    organizationCustomDomain\n  }\n}\n"
  }
};
})();

(node as any).hash = "e91c6362525733a283152e5d4f73ad58";

export default node;
