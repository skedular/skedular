/**
 * @generated SignedSource<<7a20adc2ff4d19f65fbf0279a30fb690>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CompleteTeamOnboardingInput = {
  clientMutationId?: string | null | undefined;
};
export type addTeam_completeTeamOnboardingMutation$variables = {
  input: CompleteTeamOnboardingInput;
};
export type addTeam_completeTeamOnboardingMutation$data = {
  readonly completeTeamOnboarding: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type addTeam_completeTeamOnboardingMutation = {
  response: addTeam_completeTeamOnboardingMutation$data;
  variables: addTeam_completeTeamOnboardingMutation$variables;
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
    "name": "completeTeamOnboarding",
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
    "name": "addTeam_completeTeamOnboardingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addTeam_completeTeamOnboardingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "2f16e95a049bc2ac2780e010cd8bf4c3",
    "id": null,
    "metadata": {},
    "name": "addTeam_completeTeamOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation addTeam_completeTeamOnboardingMutation(\n  $input: CompleteTeamOnboardingInput!\n) {\n  completeTeamOnboarding(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "057cc462d5353882ff10b0056270ae21";

export default node;
