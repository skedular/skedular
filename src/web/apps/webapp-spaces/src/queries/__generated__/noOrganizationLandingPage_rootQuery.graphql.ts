/**
 * @generated SignedSource<<c3cc20494ec9cdff8afe64e5f6ef1b44>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type noOrganizationLandingPage_rootQuery$variables = Record<PropertyKey, never>;
export type noOrganizationLandingPage_rootQuery$data = {
  readonly me: {
    readonly id: string;
    readonly isOnboardingDone: boolean;
  };
  readonly " $fragmentSpreads": FragmentRefs<"noOrganizationLandingContent_query">;
};
export type noOrganizationLandingPage_rootQuery = {
  response: noOrganizationLandingPage_rootQuery$data;
  variables: noOrganizationLandingPage_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
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
    "name": "noOrganizationLandingPage_rootQuery",
    "selections": [
      (v0/*:: as any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "noOrganizationLandingContent_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "noOrganizationLandingPage_rootQuery",
    "selections": [
      (v0/*:: as any*/),
      {
        "alias": null,
        "args": [
          {
            "kind": "Literal",
            "name": "types",
            "value": [
              "MARKETPLACE"
            ]
          }
        ],
        "concreteType": "MyOrganizationDetails",
        "kind": "LinkedField",
        "name": "myOrganizations",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "uniqueId",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "customDomain",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "logoUrl",
            "storageKey": null
          }
        ],
        "storageKey": "myOrganizations(types:[\"MARKETPLACE\"])"
      }
    ]
  },
  "params": {
    "cacheID": "8fb29b245d0dcc177a89421dbb9af3ac",
    "id": null,
    "metadata": {},
    "name": "noOrganizationLandingPage_rootQuery",
    "operationKind": "query",
    "text": "query noOrganizationLandingPage_rootQuery {\n  me {\n    id\n    isOnboardingDone\n  }\n  ...noOrganizationLandingContent_query\n}\n\nfragment noOrganizationLandingContent_query on Query {\n  myOrganizations(types: [MARKETPLACE]) {\n    name\n    uniqueId\n    customDomain\n    logoUrl\n  }\n}\n"
  }
};
})();

(node as any).hash = "6695274b99695828d6b5e77f9b53e31f";

export default node;
