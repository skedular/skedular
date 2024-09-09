/**
 * @generated SignedSource<<ffb292bc39e2635df474f303c9892aa4>>
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
export type organizationOnboarding_completeOrganizationOnboardingMutation$variables = {
  input: CompleteOrganizationOnboardingInput;
};
export type organizationOnboarding_completeOrganizationOnboardingMutation$data = {
  readonly completeOrganizationOnboarding: {
    readonly customer: {
      readonly id: string;
      readonly isLocationOnboardingDone: boolean;
      readonly isOrganizationOnboardingDone: boolean;
    };
  } | null | undefined;
};
export type organizationOnboarding_completeOrganizationOnboardingMutation$rawResponse = {
  readonly completeOrganizationOnboarding: {
    readonly customer: {
      readonly id: string;
      readonly isLocationOnboardingDone: boolean;
      readonly isOrganizationOnboardingDone: boolean;
    };
  } | null | undefined;
};
export type organizationOnboarding_completeOrganizationOnboardingMutation = {
  rawResponse: organizationOnboarding_completeOrganizationOnboardingMutation$rawResponse;
  response: organizationOnboarding_completeOrganizationOnboardingMutation$data;
  variables: organizationOnboarding_completeOrganizationOnboardingMutation$variables;
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
    "name": "completeOrganizationOnboarding",
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
            "name": "isOrganizationOnboardingDone",
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
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationOnboarding_completeOrganizationOnboardingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationOnboarding_completeOrganizationOnboardingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "32d0db889160c931f53829fc00d4a979",
    "id": null,
    "metadata": {},
    "name": "organizationOnboarding_completeOrganizationOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation organizationOnboarding_completeOrganizationOnboardingMutation(\n  $input: CompleteOrganizationOnboardingInput!\n) {\n  completeOrganizationOnboarding(input: $input) {\n    customer {\n      id\n      isOrganizationOnboardingDone\n      isLocationOnboardingDone\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6dfeb828ae140ff2d1778aeee4301c1c";

export default node;
