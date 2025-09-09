/**
 * @generated SignedSource<<22ac0b9bcaa920ac80c4ad65a0eb7720>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagOrderField = "DESCRIPTION" | "NAME" | "TYPE" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type addProduct_rootQuery$variables = {
  multipleChoicesLocationTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  multipleChoicesProductTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationUniqueAlphanumericName: string;
};
export type addProduct_rootQuery$data = {
  readonly defaultMaxAllowedResourcesLockTimePaidViaBankTransfer: number;
  readonly defaultMaxAllowedResourcesLockTimePaidViaCard: number;
  readonly openingHoursMinutesStep: number;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesBookingPaymentMethodTypes_query" | "multipleChoicesLocationTags_query" | "multipleChoicesProductTags_query" | "singleChoiceCurrency_query" | "singleChoicePriceUnit_query">;
};
export type addProduct_rootQuery = {
  response: addProduct_rootQuery$data;
  variables: addProduct_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesLocationTagsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesProductTagsSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "openingHoursMinutesStep",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "defaultMaxAllowedResourcesLockTimePaidViaCard",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "defaultMaxAllowedResourcesLockTimePaidViaBankTransfer",
  "storageKey": null
},
v6 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesProductTagsSortingValues"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v9 = [
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
          (v7/*: any*/),
          (v8/*: any*/),
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
v10 = [
  "orderBy"
],
v11 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesLocationTagsSortingValues"
  }
],
v12 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v8/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "addProduct_rootQuery",
    "selections": [
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesProductTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesLocationTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoicePriceUnit_query"
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
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "addProduct_rootQuery",
    "selections": [
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
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
            "args": (v6/*: any*/),
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": false,
            "selections": (v9/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v6/*: any*/),
            "filters": (v10/*: any*/),
            "handle": "connection",
            "key": "multipleChoicesProductTags_productTags",
            "kind": "LinkedHandle",
            "name": "productTags"
          },
          (v7/*: any*/),
          {
            "alias": null,
            "args": (v11/*: any*/),
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "locationTags",
            "plural": false,
            "selections": (v9/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v11/*: any*/),
            "filters": (v10/*: any*/),
            "handle": "connection",
            "key": "multipleChoicesLocationTags_locationTags",
            "kind": "LinkedHandle",
            "name": "locationTags"
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "PriceUnitDetails",
        "kind": "LinkedField",
        "name": "priceUnits",
        "plural": true,
        "selections": (v12/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v12/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "PaymentMethodTypeDetails",
        "kind": "LinkedField",
        "name": "paymentMethodTypes",
        "plural": true,
        "selections": (v12/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "975c142504dd3b908606113cda6a6861",
    "id": null,
    "metadata": {},
    "name": "addProduct_rootQuery",
    "operationKind": "query",
    "text": "query addProduct_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]\n) {\n  openingHoursMinutesStep\n  defaultMaxAllowedResourcesLockTimePaidViaCard\n  defaultMaxAllowedResourcesLockTimePaidViaBankTransfer\n  ...multipleChoicesProductTags_query\n  ...multipleChoicesLocationTags_query\n  ...singleChoicePriceUnit_query\n  ...singleChoiceCurrency_query\n  ...multipleChoicesBookingPaymentMethodTypes_query\n}\n\nfragment multipleChoicesBookingPaymentMethodTypes_query on Query {\n  paymentMethodTypes {\n    type\n    name\n  }\n}\n\nfragment multipleChoicesLocationTags_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    locationTags(orderBy: $multipleChoicesLocationTagsSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment multipleChoicesProductTags_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    productTags(orderBy: $multipleChoicesProductTagsSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment singleChoiceCurrency_query on Query {\n  currencies {\n    type\n    name\n  }\n}\n\nfragment singleChoicePriceUnit_query on Query {\n  priceUnits {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "0ad3ba3886b016859deb9d142b32d252";

export default node;
