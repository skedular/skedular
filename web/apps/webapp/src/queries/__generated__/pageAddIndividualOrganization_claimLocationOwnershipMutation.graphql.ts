/**
 * @generated SignedSource<<73413f337247468b96077b0da83187df>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ClaimLocationOwnershipInput = {
  clientMutationId?: string | null | undefined;
  id?: string | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
  uniqueClaimCode: string;
};
export type pageAddIndividualOrganization_claimLocationOwnershipMutation$variables = {
  input: ClaimLocationOwnershipInput;
};
export type pageAddIndividualOrganization_claimLocationOwnershipMutation$data = {
  readonly claimLocationOwnership: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type pageAddIndividualOrganization_claimLocationOwnershipMutation = {
  response: pageAddIndividualOrganization_claimLocationOwnershipMutation$data;
  variables: pageAddIndividualOrganization_claimLocationOwnershipMutation$variables;
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
    "concreteType": "LocationPayload",
    "kind": "LinkedField",
    "name": "claimLocationOwnership",
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
    "name": "pageAddIndividualOrganization_claimLocationOwnershipMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageAddIndividualOrganization_claimLocationOwnershipMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "3f716baef6a77e8ba0b95490b5a9a154",
    "id": null,
    "metadata": {},
    "name": "pageAddIndividualOrganization_claimLocationOwnershipMutation",
    "operationKind": "mutation",
    "text": "mutation pageAddIndividualOrganization_claimLocationOwnershipMutation(\n  $input: ClaimLocationOwnershipInput!\n) {\n  claimLocationOwnership(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "178c80e8fb3281fd3d34ffbc02db8a15";

export default node;
