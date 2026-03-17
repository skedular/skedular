/**
 * @generated SignedSource<<d69eca3091776b04afdd6592718abfc4>>
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
export type pageAddPrivateOrganization_claimLocationOwnershipMutation$variables = {
  input: ClaimLocationOwnershipInput;
};
export type pageAddPrivateOrganization_claimLocationOwnershipMutation$data = {
  readonly claimLocationOwnership: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type pageAddPrivateOrganization_claimLocationOwnershipMutation = {
  response: pageAddPrivateOrganization_claimLocationOwnershipMutation$data;
  variables: pageAddPrivateOrganization_claimLocationOwnershipMutation$variables;
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
    "name": "pageAddPrivateOrganization_claimLocationOwnershipMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageAddPrivateOrganization_claimLocationOwnershipMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "23bb16a0da7ab875b1503ba1050243ce",
    "id": null,
    "metadata": {},
    "name": "pageAddPrivateOrganization_claimLocationOwnershipMutation",
    "operationKind": "mutation",
    "text": "mutation pageAddPrivateOrganization_claimLocationOwnershipMutation(\n  $input: ClaimLocationOwnershipInput!\n) {\n  claimLocationOwnership(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "4eb73298407fbcf1777bd24306dbf12d";

export default node;
