/**
 * @generated SignedSource<<f8e2d4a9f86d0838d0820598f06bba97>>
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
    readonly isLocationOnboardingDone: boolean;
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
        "name": "isLocationOnboardingDone",
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
    "cacheID": "e01bcf4b69c41d510a7c24b0cd7858ee",
    "id": null,
    "metadata": {},
    "name": "organizationOnboarding_rootQuery",
    "operationKind": "query",
    "text": "query organizationOnboarding_rootQuery {\n  me {\n    id\n    isLocationOnboardingDone\n  }\n}\n"
  }
};
})();

(node as any).hash = "cc2760fbbc97513d86bdd8aa80193dec";

export default node;
