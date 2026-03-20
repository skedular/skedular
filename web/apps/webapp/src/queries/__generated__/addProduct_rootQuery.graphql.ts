/**
 * @generated SignedSource<<7efc766264311302e73a28c448a9873f>>
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
  organizationCustomDomain: string;
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
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesAmenities_query" | "multipleChoicesPaymentMethodTypes_query" | "multipleChoicesProductTags_query" | "singleChoiceCurrency_query" | "singleChoiceProductPricingBillingMode_query" | "singleChoiceProductPricingCadence_query" | "singleChoiceProductPricingCancellationType_query" | "singleChoiceProductType_query">;
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
  "name": "organizationCustomDomain"
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
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
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
        "name": "singleChoiceProductPricingBillingMode_query"
      },
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
        "name": "multipleChoicesPaymentMethodTypes_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceProductPricingCadence_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceProductPricingCancellationType_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesAmenities_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceProductType_query"
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
        "args": null,
        "concreteType": "ProductPricingBillingModeDetails",
        "kind": "LinkedField",
        "name": "productPricingBillingModes",
        "plural": true,
        "selections": (v6/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "customDomain",
            "variableName": "organizationCustomDomain"
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
                      (v11/*: any*/),
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
          (v10/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "amenities",
            "plural": true,
            "selections": [
              (v10/*: any*/),
              (v5/*: any*/),
              (v11/*: any*/)
            ],
            "storageKey": null
          }
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
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductPricingCancellationTypeDetails",
        "kind": "LinkedField",
        "name": "productPricingCancellationTypes",
        "plural": true,
        "selections": (v6/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductTypeDetails",
        "kind": "LinkedField",
        "name": "productTypes",
        "plural": true,
        "selections": (v6/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "12b2e2543215c38d22fb17ac9e84d43d",
    "id": null,
    "metadata": {},
    "name": "addProduct_rootQuery",
    "operationKind": "query",
    "text": "query addProduct_rootQuery(\n  $organizationCustomDomain: String!\n  $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]\n) {\n  bookingSlotSizeInMinutes\n  defaultMaxAllowedResourcesLockTimePaidViaCard\n  defaultMaxAllowedResourcesLockTimePaidViaBankTransfer\n  currencies {\n    type\n    name\n  }\n  paymentMethods {\n    type\n    name\n  }\n  ...singleChoiceProductPricingBillingMode_query\n  ...multipleChoicesProductTags_query\n  ...singleChoiceCurrency_query\n  ...multipleChoicesPaymentMethodTypes_query\n  ...singleChoiceProductPricingCadence_query\n  ...singleChoiceProductPricingCancellationType_query\n  ...multipleChoicesAmenities_query\n  ...singleChoiceProductType_query\n}\n\nfragment multipleChoicesAmenities_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    amenities {\n      id\n      name\n      color\n    }\n    id\n  }\n}\n\nfragment multipleChoicesPaymentMethodTypes_query on Query {\n  paymentMethodTypes {\n    type\n    name\n  }\n}\n\nfragment multipleChoicesProductTags_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    productTags(orderBy: $multipleChoicesProductTagsSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment singleChoiceCurrency_query on Query {\n  currencies {\n    type\n    name\n  }\n}\n\nfragment singleChoiceProductPricingBillingMode_query on Query {\n  productPricingBillingModes {\n    type\n    name\n  }\n}\n\nfragment singleChoiceProductPricingCadence_query on Query {\n  productPricingCadences {\n    type\n    name\n  }\n}\n\nfragment singleChoiceProductPricingCancellationType_query on Query {\n  productPricingCancellationTypes {\n    type\n    name\n  }\n}\n\nfragment singleChoiceProductType_query on Query {\n  productTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "8400bd95c5f41d7f85d123eb478a5ad1";

export default node;
