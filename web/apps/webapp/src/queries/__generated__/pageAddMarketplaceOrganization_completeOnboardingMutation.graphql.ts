/**
 * @generated SignedSource<<c76c3913f9551a4fc8a33fa515e6e5b4>>
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageAddMarketplaceOrganization_completeOnboardingMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageAddMarketplaceOrganization_completeOnboardingMutation",
    "selections": (v1/*: any*/)
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
