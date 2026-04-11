/**
 * @generated SignedSource<<4bd94a9a2b18c02e5f571731950d1160>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ActivateResourcesInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationLocationManageResourcesSection_activateResourcesMutation$variables = {
  input: ActivateResourcesInput;
};
export type organizationLocationManageResourcesSection_activateResourcesMutation$data = {
  readonly activateResources: {
    readonly resources: ReadonlyArray<{
      readonly id: string;
      readonly inactive: boolean;
    }>;
  };
};
export type organizationLocationManageResourcesSection_activateResourcesMutation = {
  response: organizationLocationManageResourcesSection_activateResourcesMutation$data;
  variables: organizationLocationManageResourcesSection_activateResourcesMutation$variables;
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
    "name": "activateResources",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocationManageResourcesSection_activateResourcesMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocationManageResourcesSection_activateResourcesMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "66abb916a4f8106e2c46bbb135caf665",
    "id": null,
    "metadata": {},
    "name": "organizationLocationManageResourcesSection_activateResourcesMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocationManageResourcesSection_activateResourcesMutation(\n  $input: ActivateResourcesInput!\n) {\n  activateResources(input: $input) {\n    resources {\n      id\n      inactive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0b390fb7cf44012722344e89e479d4ae";

export default node;
