/**
 * @generated SignedSource<<1e6c1fe99d17792c9b113e2f154dd2ab>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationOnboarding_rootQuery$variables = Record<PropertyKey, never>;
export type organizationOnboarding_rootQuery$data = {
  readonly me: {
    readonly id: string;
    readonly isOrganizationOnboardingDone: boolean;
  } | null | undefined;
};
export type organizationOnboarding_rootQuery = {
  response: organizationOnboarding_rootQuery$data;
  variables: organizationOnboarding_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
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
        "name": "isOrganizationOnboardingDone",
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationOnboarding_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "organizationOnboarding_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "2777a887ef80a168db71140897f81189",
    "id": null,
    "metadata": {},
    "name": "organizationOnboarding_rootQuery",
    "operationKind": "query",
    "text": "query organizationOnboarding_rootQuery {\n  me {\n    id\n    isOrganizationOnboardingDone\n  }\n}\n"
  }
};
})();

(node as any).hash = "cd5bc38b66981f02f98d6775de21f736";

export default node;
