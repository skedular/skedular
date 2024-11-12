/**
 * @generated SignedSource<<f71ebecd6dc3b4ba6d8ded23b002a103>>
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
export type addOrganization_completeOrganizationOnboardingMutation$variables = {
  input: CompleteOrganizationOnboardingInput;
};
export type addOrganization_completeOrganizationOnboardingMutation$data = {
  readonly completeOrganizationOnboarding: {
    readonly customer: {
      readonly id: string;
      readonly isOrganizationOnboardingDone: boolean;
    };
  } | null | undefined;
};
export type addOrganization_completeOrganizationOnboardingMutation$rawResponse = {
  readonly completeOrganizationOnboarding: {
    readonly customer: {
      readonly id: string;
      readonly isOrganizationOnboardingDone: boolean;
    };
  } | null | undefined;
};
export type addOrganization_completeOrganizationOnboardingMutation = {
  rawResponse: addOrganization_completeOrganizationOnboardingMutation$rawResponse;
  response: addOrganization_completeOrganizationOnboardingMutation$data;
  variables: addOrganization_completeOrganizationOnboardingMutation$variables;
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
    "name": "addOrganization_completeOrganizationOnboardingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addOrganization_completeOrganizationOnboardingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "1ca924e4a2e06687cd18de0bc298ab1f",
    "id": null,
    "metadata": {},
    "name": "addOrganization_completeOrganizationOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation addOrganization_completeOrganizationOnboardingMutation(\n  $input: CompleteOrganizationOnboardingInput!\n) {\n  completeOrganizationOnboarding(input: $input) {\n    customer {\n      id\n      isOrganizationOnboardingDone\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6dc31966ad524fa3de86a272c7dbfab3";

export default node;
