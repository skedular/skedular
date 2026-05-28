/**
 * @generated SignedSource<<9023c07add164a75521d9c6945b99760>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeactivateResourcesInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationLocationManageResourcesSection_deactivateResourcesMutation$variables = {
  input: DeactivateResourcesInput;
};
export type organizationLocationManageResourcesSection_deactivateResourcesMutation$data = {
  readonly deactivateResources: {
    readonly resources: ReadonlyArray<{
      readonly id: string;
      readonly inactive: boolean;
    }>;
  };
};
export type organizationLocationManageResourcesSection_deactivateResourcesMutation = {
  response: organizationLocationManageResourcesSection_deactivateResourcesMutation$data;
  variables: organizationLocationManageResourcesSection_deactivateResourcesMutation$variables;
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
    "concreteType": "ResourcesPayload",
    "kind": "LinkedField",
    "name": "deactivateResources",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "ResourceDetails",
        "kind": "LinkedField",
        "name": "resources",
        "plural": true,
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
            "name": "inactive",
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
    "name": "organizationLocationManageResourcesSection_deactivateResourcesMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationLocationManageResourcesSection_deactivateResourcesMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "2ed5907a4a82f6a2b2e54c7bcb238b57",
    "id": null,
    "metadata": {},
    "name": "organizationLocationManageResourcesSection_deactivateResourcesMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocationManageResourcesSection_deactivateResourcesMutation(\n  $input: DeactivateResourcesInput!\n) {\n  deactivateResources(input: $input) {\n    resources {\n      id\n      inactive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cd7fefd15d0b52a0b0f1ebc91ae3035c";

export default node;
