/**
 * @generated SignedSource<<20d25fb0fd46fe557f8319f34bd86cdb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageAddMarketplaceOrganization_rootQuery$variables = Record<PropertyKey, never>;
export type pageAddMarketplaceOrganization_rootQuery$data = {
  readonly me: {
    readonly id: string;
    readonly isOnboardingDone: boolean;
  };
  readonly " $fragmentSpreads": FragmentRefs<"addMarketplaceOrganization_query">;
};
export type pageAddMarketplaceOrganization_rootQuery = {
  response: pageAddMarketplaceOrganization_rootQuery$data;
  variables: pageAddMarketplaceOrganization_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v0/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "isOnboardingDone",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageAddMarketplaceOrganization_rootQuery",
    "selections": [
      (v1/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "addMarketplaceOrganization_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageAddMarketplaceOrganization_rootQuery",
    "selections": [
      (v1/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTermsOfUse",
        "kind": "LinkedField",
        "name": "activeOrganizationTermsOfUse",
        "plural": false,
        "selections": [
          (v0/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "terms",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationMemberVisibilityPolicyDetails",
        "kind": "LinkedField",
        "name": "organizationMemberVisibilityPolicies",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "type",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "209b649b10633d10e4b719f44df1eeb6",
    "id": null,
    "metadata": {},
    "name": "pageAddMarketplaceOrganization_rootQuery",
    "operationKind": "query",
    "text": "query pageAddMarketplaceOrganization_rootQuery {\n  me {\n    id\n    isOnboardingDone\n  }\n  ...addMarketplaceOrganization_query\n}\n\nfragment addMarketplaceOrganization_query on Query {\n  activeOrganizationTermsOfUse {\n    id\n  }\n  ...organizationTermsOfUse_query\n  ...singleChoiceOrganizationMemberVisibilityPolicyquery\n}\n\nfragment organizationTermsOfUse_query on Query {\n  activeOrganizationTermsOfUse {\n    id\n    terms\n  }\n}\n\nfragment singleChoiceOrganizationMemberVisibilityPolicyquery on Query {\n  organizationMemberVisibilityPolicies {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "fe4e81849dab4c8fba4f3984c5a7ecb2";

export default node;
