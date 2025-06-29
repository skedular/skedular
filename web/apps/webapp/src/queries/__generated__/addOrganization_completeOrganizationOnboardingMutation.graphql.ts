/**
 * @generated SignedSource<<7d7515eca64c2f3a6e088421f01d7440>>
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
    readonly clientMutationId: string | null | undefined;
  };
};
export type addOrganization_completeOrganizationOnboardingMutation = {
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
        "kind": "ScalarField",
        "name": "clientMutationId",
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
    "cacheID": "341b8a22857a7cf6af49b857c12ab80a",
    "id": null,
    "metadata": {},
    "name": "addOrganization_completeOrganizationOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation addOrganization_completeOrganizationOnboardingMutation(\n  $input: CompleteOrganizationOnboardingInput!\n) {\n  completeOrganizationOnboarding(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "2ef11ffc9b410736e401d60773cdda79";

export default node;
