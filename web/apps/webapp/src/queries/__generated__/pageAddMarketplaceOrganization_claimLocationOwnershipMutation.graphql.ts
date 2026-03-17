/**
 * @generated SignedSource<<f9d603bfd276b859e8dd594e21d519c9>>
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
export type pageAddMarketplaceOrganization_claimLocationOwnershipMutation$variables = {
  input: ClaimLocationOwnershipInput;
};
export type pageAddMarketplaceOrganization_claimLocationOwnershipMutation$data = {
  readonly claimLocationOwnership: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type pageAddMarketplaceOrganization_claimLocationOwnershipMutation = {
  response: pageAddMarketplaceOrganization_claimLocationOwnershipMutation$data;
  variables: pageAddMarketplaceOrganization_claimLocationOwnershipMutation$variables;
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
    "name": "pageAddMarketplaceOrganization_claimLocationOwnershipMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageAddMarketplaceOrganization_claimLocationOwnershipMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "856ab27dbd8c9785c71eed4d7b9f448a",
    "id": null,
    "metadata": {},
    "name": "pageAddMarketplaceOrganization_claimLocationOwnershipMutation",
    "operationKind": "mutation",
    "text": "mutation pageAddMarketplaceOrganization_claimLocationOwnershipMutation(\n  $input: ClaimLocationOwnershipInput!\n) {\n  claimLocationOwnership(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "3e475c37380c2705d6a36f771a551d41";

export default node;
