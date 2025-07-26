/**
 * @generated SignedSource<<6519bdd171980d169eab0dc46259882c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageAddPrivateOrganization_rootQuery$variables = Record<PropertyKey, never>;
export type pageAddPrivateOrganization_rootQuery$data = {
  readonly me: {
    readonly id: string;
    readonly isOnboardingDone: boolean;
  };
  readonly " $fragmentSpreads": FragmentRefs<"addPrivateOrganization_query">;
};
export type pageAddPrivateOrganization_rootQuery = {
  response: pageAddPrivateOrganization_rootQuery$data;
  variables: pageAddPrivateOrganization_rootQuery$variables;
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
    "name": "pageAddPrivateOrganization_rootQuery",
    "selections": [
      (v1/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "addPrivateOrganization_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageAddPrivateOrganization_rootQuery",
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
    "cacheID": "eda0b78c16717668d1659036f3fff10d",
    "id": null,
    "metadata": {},
    "name": "pageAddPrivateOrganization_rootQuery",
    "operationKind": "query",
    "text": "query pageAddPrivateOrganization_rootQuery {\n  me {\n    id\n    isOnboardingDone\n  }\n  ...addPrivateOrganization_query\n}\n\nfragment addPrivateOrganization_query on Query {\n  activeOrganizationTermsOfUse {\n    id\n  }\n  ...organizationTermsOfUse_query\n  ...singleChoiceOrganizationMemberVisibilityPolicyquery\n}\n\nfragment organizationTermsOfUse_query on Query {\n  activeOrganizationTermsOfUse {\n    id\n    terms\n  }\n}\n\nfragment singleChoiceOrganizationMemberVisibilityPolicyquery on Query {\n  organizationMemberVisibilityPolicies {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "29322bc6c6ff58bb08f65fd7146594ba";

export default node;
