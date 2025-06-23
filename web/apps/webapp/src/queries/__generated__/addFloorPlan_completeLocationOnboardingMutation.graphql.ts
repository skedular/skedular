/**
 * @generated SignedSource<<c783afc7c17dc340e9923147d5c7ddb7>>
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
export type addFloorPlan_completeLocationOnboardingMutation$variables = {
  input: CompleteLocationOnboardingInput;
};
export type addFloorPlan_completeLocationOnboardingMutation$data = {
  readonly completeLocationOnboarding: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type addFloorPlan_completeLocationOnboardingMutation = {
  response: addFloorPlan_completeLocationOnboardingMutation$data;
  variables: addFloorPlan_completeLocationOnboardingMutation$variables;
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
    "name": "addFloorPlan_completeLocationOnboardingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addFloorPlan_completeLocationOnboardingMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "6f61418f74fe2f1cb9589613f853b375",
    "id": null,
    "metadata": {},
    "name": "addFloorPlan_completeLocationOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation addFloorPlan_completeLocationOnboardingMutation(\n  $input: CompleteLocationOnboardingInput!\n) {\n  completeLocationOnboarding(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "e2424151e2333f5eda564ccd8d9f3253";

export default node;
