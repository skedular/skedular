/**
 * @generated SignedSource<<5ae7e6e1dbb333c71385790d0f0f7ec4>>
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
  "kind": "ScalarField",
  "name": "isOnboardingDone",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageAddPrivateOrganization_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v0/*: any*/),
          (v1/*: any*/)
        ],
        "storageKey": null
      },
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
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v0/*: any*/),
          (v1/*: any*/),
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
      }
    ]
  },
  "params": {
    "cacheID": "7e8635f1928c4ca3f3a1c6d1a311a829",
    "id": null,
    "metadata": {},
    "name": "pageAddPrivateOrganization_rootQuery",
    "operationKind": "query",
    "text": "query pageAddPrivateOrganization_rootQuery {\n  me {\n    id\n    isOnboardingDone\n  }\n  ...addPrivateOrganization_query\n}\n\nfragment addPrivateOrganization_query on Query {\n  me {\n    emails\n    id\n  }\n  activeOrganizationTermsOfUse {\n    id\n  }\n  ...organizationTermsOfUse_query\n}\n\nfragment organizationTermsOfUse_query on Query {\n  activeOrganizationTermsOfUse {\n    id\n    terms\n  }\n}\n"
  }
};
})();

(node as any).hash = "29322bc6c6ff58bb08f65fd7146594ba";

export default node;
