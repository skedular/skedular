/**
 * @generated SignedSource<<9d37953ba744f7b3c3c8360e82372f0f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type Currency = "NZD" | "USD" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagOrderField = "DESCRIPTION" | "NAME" | "TYPE" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type addProduct_rootQuery$variables = {
  multipleChoicesProductTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationUniqueAlphanumericName: string;
};
export type addProduct_rootQuery$data = {
  readonly bookingSlotSizeInMinutes: number;
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly defaultMaxAllowedResourcesLockTimePaidViaBankTransfer: number;
  readonly defaultMaxAllowedResourcesLockTimePaidViaCard: number;
  readonly paymentMethods: ReadonlyArray<{
    readonly name: string;
    readonly type: PaymentMethod;
  }>;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesBookingPaymentMethodTypes_query" | "multipleChoicesProductTags_query" | "singleChoiceCurrency_query" | "singleChoiceProductPricingCadence_query">;
};
export type addProduct_rootQuery = {
  response: addProduct_rootQuery$data;
  variables: addProduct_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesProductTagsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "bookingSlotSizeInMinutes",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "defaultMaxAllowedResourcesLockTimePaidViaCard",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "defaultMaxAllowedResourcesLockTimePaidViaBankTransfer",
  "storageKey": null
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
    "name": "type",
    "storageKey": null
  },
  (v5/*: any*/)
],
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "CurrencyDetails",
  "kind": "LinkedField",
  "name": "currencies",
  "plural": true,
  "selections": (v6/*: any*/),
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentMethodTypeDetails",
  "kind": "LinkedField",
  "name": "paymentMethods",
  "plural": true,
  "selections": (v6/*: any*/),
  "storageKey": null
},
v9 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesProductTagsSortingValues"
  }
],
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "addProduct_rootQuery",
    "selections": [
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v7/*: any*/),
      (v8/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesProductTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceCurrency_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesBookingPaymentMethodTypes_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceProductPricingCadence_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "addProduct_rootQuery",
    "selections": [
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v7/*: any*/),
      (v8/*: any*/),
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "uniqueAlphanumericName",
            "variableName": "organizationUniqueAlphanumericName"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": (v9/*: any*/),
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "totalCount",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v10/*: any*/),
                      (v5/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "color",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "__typename",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "cursor",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PageInfo",
                "kind": "LinkedField",
                "name": "pageInfo",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "endCursor",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "hasNextPage",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "kind": "ClientExtension",
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "__id",
                    "storageKey": null
                  }
                ]
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v9/*: any*/),
            "filters": [
              "orderBy"
            ],
            "handle": "connection",
            "key": "multipleChoicesProductTags_productTags",
            "kind": "LinkedHandle",
            "name": "productTags"
          },
          (v10/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "PaymentMethodTypeDetails",
        "kind": "LinkedField",
        "name": "paymentMethodTypes",
        "plural": true,
        "selections": (v6/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductPricingCadenceDetails",
        "kind": "LinkedField",
        "name": "productPricingCadences",
        "plural": true,
        "selections": (v6/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "ec3757a84f01aa4995ff75fefbff7378",
    "id": null,
    "metadata": {},
    "name": "addProduct_rootQuery",
    "operationKind": "query",
    "text": "query addProduct_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]\n) {\n  bookingSlotSizeInMinutes\n  defaultMaxAllowedResourcesLockTimePaidViaCard\n  defaultMaxAllowedResourcesLockTimePaidViaBankTransfer\n  currencies {\n    type\n    name\n  }\n  paymentMethods {\n    type\n    name\n  }\n  ...multipleChoicesProductTags_query\n  ...singleChoiceCurrency_query\n  ...multipleChoicesBookingPaymentMethodTypes_query\n  ...singleChoiceProductPricingCadence_query\n}\n\nfragment multipleChoicesBookingPaymentMethodTypes_query on Query {\n  paymentMethodTypes {\n    type\n    name\n  }\n}\n\nfragment multipleChoicesProductTags_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    productTags(orderBy: $multipleChoicesProductTagsSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment singleChoiceCurrency_query on Query {\n  currencies {\n    type\n    name\n  }\n}\n\nfragment singleChoiceProductPricingCadence_query on Query {\n  productPricingCadences {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "89d1b1ea3692c2365b298057e260f95e";

export default node;
