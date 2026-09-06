/**
 * @generated SignedSource<<102f43ef0ab80398d84fe19f2a03847a>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MembershipTerm = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "MONTHLY" | "NOT_SET" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
export type ProductPricingBillingMode = "IN_ARREARS" | "NOT_SET" | "UPFRONT" | "%future added value";
export type ProductPricingCancellationPolicyType = "FULL_REFUND_BEFORE_CUTOFF" | "NOT_SET" | "NO_CANCELLATION" | "TIERED_REFUND" | "%future added value";
export type locationsHostDataQuery$variables = {
  organizationId: string;
};
export type locationsHostDataQuery$data = {
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly physicalAddress: {
      readonly multilinesFormattedAddress: string | null | undefined;
    } | null | undefined;
    readonly products: ReadonlyArray<{
      readonly id: string;
      readonly pricingOptions: ReadonlyArray<{
        readonly billingMode: ProductPricingBillingMode;
        readonly cancellationPolicyType: ProductPricingCancellationPolicyType;
        readonly membershipTerm: MembershipTerm;
      }>;
    }>;
    readonly timezone: string | null | undefined;
  }> | null | undefined;
};
export type locationsHostDataQuery = {
  response: locationsHostDataQuery$data;
  variables: locationsHostDataQuery$variables;
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
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "timezone",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "multilinesFormattedAddress",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "ProductDetails",
  "kind": "LinkedField",
  "name": "products",
  "plural": true,
  "selections": [
    (v2/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "ProductPricing",
      "kind": "LinkedField",
      "name": "pricingOptions",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "billingMode",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "membershipTerm",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "cancellationPolicyType",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "locationsHostDataQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "myLocations",
        "plural": true,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationPhysicalAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              (v5/*:: as any*/)
            ],
            "storageKey": null
          },
          (v6/*:: as any*/)
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
    "name": "locationsHostDataQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "myLocations",
        "plural": true,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationPhysicalAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              (v5/*:: as any*/),
              (v2/*:: as any*/)
            ],
            "storageKey": null
          },
          (v6/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "f13d87b7a44dc09775a382a8c27b14d8",
    "id": null,
    "metadata": {},
    "name": "locationsHostDataQuery",
    "operationKind": "query",
    "text": "query locationsHostDataQuery(\n  $organizationId: String!\n) {\n  myLocations(organizationId: $organizationId) {\n    id\n    name\n    timezone\n    physicalAddress {\n      multilinesFormattedAddress\n      id\n    }\n    products {\n      id\n      pricingOptions {\n        billingMode\n        membershipTerm\n        cancellationPolicyType\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3fbe0ee54922b3c83710f6aa39860ec0";

export default node;
