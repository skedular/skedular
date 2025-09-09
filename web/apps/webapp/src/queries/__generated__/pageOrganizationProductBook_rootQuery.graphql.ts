/**
 * @generated SignedSource<<0126f8dfa2cbdfc5ed676808997d5092>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationProductBook_rootQuery$variables = {
  dateFromToGetAvailableResources: any;
  dateUntilToGetAvailableResources: any;
  organizationUniqueAlphanumericName: string;
  productId: string;
};
export type pageOrganizationProductBook_rootQuery$data = {
  readonly product: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"bookProduct_availableResources_query" | "bookProduct_query">;
};
export type pageOrganizationProductBook_rootQuery = {
  response: pageOrganizationProductBook_rootQuery$data;
  variables: pageOrganizationProductBook_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateFromToGetAvailableResources"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateUntilToGetAvailableResources"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productId"
},
v4 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "productId"
  }
],
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v8 = [
  (v7/*: any*/),
  (v5/*: any*/)
],
v9 = [
  (v6/*: any*/),
  (v5/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationProductBook_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          (v5/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "bookProduct_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "bookProduct_availableResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v3/*: any*/),
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationProductBook_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          (v6/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "description",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "price",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "PriceUnitDetails",
            "kind": "LinkedField",
            "name": "priceUnit",
            "plural": false,
            "selections": (v8/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "currencyToDisplay",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CurrencyDetails",
            "kind": "LinkedField",
            "name": "currency",
            "plural": false,
            "selections": (v8/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "numberOfResourcesToBook",
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
            "name": "bookAllLocationResources",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "recurrenceWindowDays",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "requireConsecutiveDays",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "maxBookingSpreadDays",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "latestProductVersionId",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Marketplace_PaymentMethodTypeDetails",
            "kind": "LinkedField",
            "name": "acceptedBookingPaymentMethods",
            "plural": true,
            "selections": [
              (v7/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "isPriceTaxInclusive",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v6/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "emails",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
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
            "args": null,
            "concreteType": "OrganizationTaxDetails",
            "kind": "LinkedField",
            "name": "taxDetails",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "taxId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "taxRatePercentage",
                "storageKey": null
              },
              (v6/*: any*/)
            ],
            "storageKey": null
          },
          (v6/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "openingHoursMinutesStep",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingTypeDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingTypes",
        "plural": true,
        "selections": (v8/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "PaymentMethodTypeDetails",
        "kind": "LinkedField",
        "name": "paymentMethodTypes",
        "plural": true,
        "selections": (v8/*: any*/),
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
                "variableName": "dateFromToGetAvailableResources"
              },
              {
                "kind": "Variable",
                "name": "organizationUniqueAlphanumericName",
                "variableName": "organizationUniqueAlphanumericName"
              },
              {
                "kind": "Variable",
                "name": "productId",
                "variableName": "productId"
              },
              {
                "kind": "Variable",
                "name": "until",
                "variableName": "dateUntilToGetAvailableResources"
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
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": [
              (v6/*: any*/),
              (v5/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "ResourceDetails",
            "kind": "LinkedField",
            "name": "resource",
            "plural": false,
            "selections": [
              (v6/*: any*/),
              (v5/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagDetails",
                "kind": "LinkedField",
                "name": "customTags",
                "plural": true,
                "selections": (v9/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagDetails",
                "kind": "LinkedField",
                "name": "zones",
                "plural": true,
                "selections": (v9/*: any*/),
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
    "cacheID": "4d210f4d171a2cc63c3e44479547345f",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationProductBook_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationProductBook_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $productId: String!\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n) {\n  product(id: $productId) {\n    name\n    id\n  }\n  ...bookProduct_query\n  ...bookProduct_availableResources_query\n}\n\nfragment bookProduct_availableResources_query on Query {\n  availableResources(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, productId: $productId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    location {\n      id\n      name\n    }\n    resource {\n      id\n      name\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n\nfragment bookProduct_query on Query {\n  me {\n    id\n    emails\n  }\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    taxDetails {\n      taxId\n      taxRatePercentage\n      id\n    }\n    id\n  }\n  product(id: $productId) {\n    id\n    name\n    description\n    price\n    priceUnit {\n      type\n      name\n    }\n    currencyToDisplay\n    currency {\n      type\n      name\n    }\n    numberOfResourcesToBook\n    minDurationMinutes\n    maxDurationMinutes\n    bookAllLocationResources\n    recurrenceWindowDays\n    requireConsecutiveDays\n    maxBookingSpreadDays\n    latestProductVersionId\n    acceptedBookingPaymentMethods {\n      type\n    }\n    isPriceTaxInclusive\n  }\n  openingHoursMinutesStep\n  ...singleChoiceMarketplaceBookingType_query\n  ...singleChoiceBookingPaymentMethodType_query\n  ...multipleChoicesUserEmails_query\n}\n\nfragment multipleChoicesUserEmails_query on Query {\n  me {\n    emails\n    id\n  }\n}\n\nfragment singleChoiceBookingPaymentMethodType_query on Query {\n  paymentMethodTypes {\n    type\n    name\n  }\n}\n\nfragment singleChoiceMarketplaceBookingType_query on Query {\n  marketplaceBookingTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "678d5d9bbcb4dbc5a12317b40ef083d2";

export default node;
