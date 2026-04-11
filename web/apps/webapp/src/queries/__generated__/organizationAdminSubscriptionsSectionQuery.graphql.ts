/**
 * @generated SignedSource<<e7f2df5b4e1473bdcbb6a2408d8557d3>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationAdminSubscriptionsSectionQuery$variables = {
  organizationCustomDomain: string;
};
export type organizationAdminSubscriptionsSectionQuery$data = {
  readonly organization: {
    readonly activeOffering: {
      readonly end: any;
      readonly featureSet: ReadonlyArray<string>;
      readonly free: boolean;
      readonly id: string;
      readonly isEnterprise: boolean;
      readonly name: string;
      readonly start: any;
      readonly underPriceLines: ReadonlyArray<string>;
      readonly unitPrice: number;
    };
    readonly availableOfferings: ReadonlyArray<{
      readonly code: string;
      readonly featureSet: ReadonlyArray<string>;
      readonly free: boolean;
      readonly isEnterprise: boolean;
      readonly name: string;
      readonly underPriceLines: ReadonlyArray<string>;
      readonly unitPrice: number;
    }>;
    readonly hasAttachedPaymentMethod: boolean;
    readonly id: string;
    readonly name: string;
    readonly paymentMethods: ReadonlyArray<{
      readonly cardBrand: string | null | undefined;
      readonly cardExpiryMonth: number | null | undefined;
      readonly cardExpiryYear: number | null | undefined;
      readonly cardLastFourDigit: string | null | undefined;
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationAdminSubscriptionsSectionQuery = {
  response: organizationAdminSubscriptionsSectionQuery$data;
  variables: organizationAdminSubscriptionsSectionQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isEnterprise",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "unitPrice",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "featureSet",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "underPriceLines",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "free",
  "storageKey": null
},
v8 = [
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
      (v1/*: any*/),
      (v2/*: any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "hasAttachedPaymentMethod",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationPaymentMethod",
        "kind": "LinkedField",
        "name": "paymentMethods",
        "plural": true,
        "selections": [
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardBrand",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardExpiryMonth",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardExpiryYear",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "cardLastFourDigit",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationActiveOfferingDetails",
        "kind": "LinkedField",
        "name": "activeOffering",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          (v3/*: any*/),
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "start",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "end",
            "storageKey": null
          },
          (v4/*: any*/),
          (v5/*: any*/),
          (v6/*: any*/),
          (v7/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationOfferingDetails",
        "kind": "LinkedField",
        "name": "availableOfferings",
        "plural": true,
        "selections": [
          (v3/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "code",
            "storageKey": null
          },
          (v2/*: any*/),
          (v4/*: any*/),
          (v5/*: any*/),
          (v6/*: any*/),
          (v7/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdminSubscriptionsSectionQuery",
    "selections": (v8/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdminSubscriptionsSectionQuery",
    "selections": (v8/*: any*/)
  },
  "params": {
    "cacheID": "0e9757e4004d7c6254bf3c000e80ca67",
    "id": null,
    "metadata": {},
    "name": "organizationAdminSubscriptionsSectionQuery",
    "operationKind": "query",
    "text": "query organizationAdminSubscriptionsSectionQuery(\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n    hasAttachedPaymentMethod\n    paymentMethods {\n      id\n      cardBrand\n      cardExpiryMonth\n      cardExpiryYear\n      cardLastFourDigit\n    }\n    activeOffering {\n      id\n      isEnterprise\n      name\n      start\n      end\n      unitPrice\n      featureSet\n      underPriceLines\n      free\n    }\n    availableOfferings {\n      isEnterprise\n      code\n      name\n      unitPrice\n      featureSet\n      underPriceLines\n      free\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ad5ed505bd36852599248992371d838f";

export default node;
