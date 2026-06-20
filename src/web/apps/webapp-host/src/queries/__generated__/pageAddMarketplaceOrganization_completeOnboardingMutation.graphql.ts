/**
 * @generated SignedSource<<af8d4483ea83cc3de2e89b976dbf3f21>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CompleteOrganizationOnboardingInput = {
  clientMutationId?: string | null | undefined;
};
export type pageAddMarketplaceOrganization_completeOnboardingMutation$variables = {
  input: CompleteOrganizationOnboardingInput;
};
export type pageAddMarketplaceOrganization_completeOnboardingMutation$data = {
  readonly completeOnboarding: {
    readonly customer: {
      readonly id: string;
      readonly isOnboardingDone: boolean;
    };
  };
};
export type pageAddMarketplaceOrganization_completeOnboardingMutation$rawResponse = {
  readonly completeOnboarding: {
    readonly customer: {
      readonly id: string;
      readonly isOnboardingDone: boolean;
    };
  };
};
export type pageAddMarketplaceOrganization_completeOnboardingMutation = {
  rawResponse: pageAddMarketplaceOrganization_completeOnboardingMutation$rawResponse;
  response: pageAddMarketplaceOrganization_completeOnboardingMutation$data;
  variables: pageAddMarketplaceOrganization_completeOnboardingMutation$variables;
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
    "name": "pageAddMarketplaceOrganization_completeOnboardingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageAddMarketplaceOrganization_completeOnboardingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "0e7ff675acb380ce2f33fe7ab69103ae",
    "id": null,
    "metadata": {},
    "name": "pageAddMarketplaceOrganization_completeOnboardingMutation",
    "operationKind": "mutation",
    "text": "mutation pageAddMarketplaceOrganization_completeOnboardingMutation(\n  $input: CompleteOrganizationOnboardingInput!\n) {\n  completeOnboarding(input: $input) {\n    customer {\n      id\n      isOnboardingDone\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b848038c02aa14ff5613de6fa7b5f923";

export default node;
