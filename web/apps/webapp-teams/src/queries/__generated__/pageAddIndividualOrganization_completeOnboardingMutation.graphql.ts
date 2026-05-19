/**
 * @generated SignedSource<<31869c3eb2c8220a2cf400f7a04803fe>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CompleteOrganizationOnboardingInput = {
  clientMutationId?: string | null | undefined;
};
export type pageAddIndividualOrganization_completeOnboardingMutation$variables = {
  input: CompleteOrganizationOnboardingInput;
};
export type pageAddIndividualOrganization_completeOnboardingMutation$data = {
  readonly completeOnboarding: {
    readonly customer: {
      readonly id: string;
      readonly isOnboardingDone: boolean;
    };
  };
};
export type pageAddIndividualOrganization_completeOnboardingMutation$rawResponse = {
  readonly completeOnboarding: {
    readonly customer: {
      readonly id: string;
      readonly isOnboardingDone: boolean;
    };
  };
};
export type pageAddIndividualOrganization_completeOnboardingMutation = {
  rawResponse: pageAddIndividualOrganization_completeOnboardingMutation$rawResponse;
  response: pageAddIndividualOrganization_completeOnboardingMutation$data;
  variables: pageAddIndividualOrganization_completeOnboardingMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageAddIndividualOrganization_completeOnboardingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageAddIndividualOrganization_completeOnboardingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "372348933017f5816ca7fe5717aa3a8d",
    "id": null,
    "metadata": {},
    "name": "pageAddIndividualOrganization_completeOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation pageAddIndividualOrganization_completeOnboardingMutation(\n  $input: CompleteOrganizationOnboardingInput!\n) {\n  completeOnboarding(input: $input) {\n    customer {\n      id\n      isOnboardingDone\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a043e32a9ac42b8543759688c96008ef";

export default node;
