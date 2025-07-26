/**
 * @generated SignedSource<<d08d0b0c2871d3ba4aa0b3c6500a6805>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CompleteOrganizationOnboardingInput = {
  clientMutationId?: string | null | undefined;
};
export type pageAddPrivateOrganization_completeOnboardingMutation$variables = {
  input: CompleteOrganizationOnboardingInput;
};
export type pageAddPrivateOrganization_completeOnboardingMutation$data = {
  readonly completeOnboarding: {
    readonly customer: {
      readonly id: string;
      readonly isOnboardingDone: boolean;
    };
  };
};
export type pageAddPrivateOrganization_completeOnboardingMutation$rawResponse = {
  readonly completeOnboarding: {
    readonly customer: {
      readonly id: string;
      readonly isOnboardingDone: boolean;
    };
  };
};
export type pageAddPrivateOrganization_completeOnboardingMutation = {
  rawResponse: pageAddPrivateOrganization_completeOnboardingMutation$rawResponse;
  response: pageAddPrivateOrganization_completeOnboardingMutation$data;
  variables: pageAddPrivateOrganization_completeOnboardingMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "CustomerPayload",
    "kind": "LinkedField",
    "name": "completeOnboarding",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "customer",
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
    "name": "pageAddPrivateOrganization_completeOnboardingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageAddPrivateOrganization_completeOnboardingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "cd172bf5834d9d70f86d903ec87cc160",
    "id": null,
    "metadata": {},
    "name": "pageAddPrivateOrganization_completeOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation pageAddPrivateOrganization_completeOnboardingMutation(\n  $input: CompleteOrganizationOnboardingInput!\n) {\n  completeOnboarding(input: $input) {\n    customer {\n      id\n      isOnboardingDone\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c49687ba4f63cf1173817a0ab4256895";

export default node;
