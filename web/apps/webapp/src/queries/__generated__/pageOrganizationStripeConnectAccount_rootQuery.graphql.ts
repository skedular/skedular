/**
 * @generated SignedSource<<1e1791b53bd6e46e6a66eb9d50786888>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationStripeConnectAccount_rootQuery$variables = {
  organizationStripeConnectAccountId: string;
};
export type pageOrganizationStripeConnectAccount_rootQuery$data = {
  readonly organizationStripeConnectAccount: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"editStripeConnectAccount_query">;
};
export type pageOrganizationStripeConnectAccount_rootQuery = {
  response: pageOrganizationStripeConnectAccount_rootQuery$data;
  variables: pageOrganizationStripeConnectAccount_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationStripeConnectAccountId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationStripeConnectAccountId"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationStripeConnectAccount_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationStripeConnectAccountDetails",
        "kind": "LinkedField",
        "name": "organizationStripeConnectAccount",
        "plural": false,
        "selections": [
          (v2/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editStripeConnectAccount_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageOrganizationStripeConnectAccount_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationStripeConnectAccountDetails",
        "kind": "LinkedField",
        "name": "organizationStripeConnectAccount",
        "plural": false,
        "selections": [
          (v2/*: any*/),
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
            "name": "country",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "defaultCurrency",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "businessType",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "companyName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "url",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "supportUrl",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "contactEmail",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "contactPhone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "onboardingUrl",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "onboardingCompleted",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "1f2d3b276f55e853e9d9237b8f09b28e",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationStripeConnectAccount_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationStripeConnectAccount_rootQuery(\n  $organizationStripeConnectAccountId: String!\n) {\n  organizationStripeConnectAccount(id: $organizationStripeConnectAccountId) {\n    name\n    id\n  }\n  ...editStripeConnectAccount_query\n}\n\nfragment editStripeConnectAccount_query on Query {\n  organizationStripeConnectAccount(id: $organizationStripeConnectAccountId) {\n    id\n    name\n    country\n    defaultCurrency\n    businessType\n    companyName\n    url\n    supportUrl\n    contactEmail\n    contactPhone\n    onboardingUrl\n    onboardingCompleted\n  }\n}\n"
  }
};
})();

(node as any).hash = "82f6c0fc3a8f03ec672bf629ca6bf137";

export default node;
