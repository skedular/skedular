/**
 * @generated SignedSource<<b4481faca942015eee14b1de0db4027e>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CompleteOrganizationMemberOnboardingInput = {
  clientMutationId?: string | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type gettingStarted_completeOrganizationMemberOnboardingMutation$variables = {
  input: CompleteOrganizationMemberOnboardingInput;
};
export type gettingStarted_completeOrganizationMemberOnboardingMutation$data = {
  readonly completeOrganizationMemberOnboarding: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type gettingStarted_completeOrganizationMemberOnboardingMutation = {
  response: gettingStarted_completeOrganizationMemberOnboardingMutation$data;
  variables: gettingStarted_completeOrganizationMemberOnboardingMutation$variables;
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
    "concreteType": "OrganizationMemberPayload",
    "kind": "LinkedField",
    "name": "completeOrganizationMemberOnboarding",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "gettingStarted_completeOrganizationMemberOnboardingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "gettingStarted_completeOrganizationMemberOnboardingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "432123974dc5ac6a1aa276f9574e3424",
    "id": null,
    "metadata": {},
    "name": "gettingStarted_completeOrganizationMemberOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation gettingStarted_completeOrganizationMemberOnboardingMutation(\n  $input: CompleteOrganizationMemberOnboardingInput!\n) {\n  completeOrganizationMemberOnboarding(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "47e83ce3d1857773b2ee9223e1243260";

export default node;
