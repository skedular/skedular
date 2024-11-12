/**
 * @generated SignedSource<<54a084b29ad032e37e747da55b32fb74>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CompleteLocationOnboardingInput = {
  clientMutationId?: string | null | undefined;
};
export type addLocation_completeLocationOnboardingMutation$variables = {
  input: CompleteLocationOnboardingInput;
};
export type addLocation_completeLocationOnboardingMutation$data = {
  readonly completeLocationOnboarding: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type addLocation_completeLocationOnboardingMutation = {
  response: addLocation_completeLocationOnboardingMutation$data;
  variables: addLocation_completeLocationOnboardingMutation$variables;
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
    "name": "completeLocationOnboarding",
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
    "name": "addLocation_completeLocationOnboardingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addLocation_completeLocationOnboardingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "62151c97157095ac9ff385e35cdbec3a",
    "id": null,
    "metadata": {},
    "name": "addLocation_completeLocationOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation addLocation_completeLocationOnboardingMutation(\n  $input: CompleteLocationOnboardingInput!\n) {\n  completeLocationOnboarding(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "3e878291a772b95ac510134f81676f33";

export default node;
