/**
 * @generated SignedSource<<68927898390e32cbb127dae23155df9d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteResourcesInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationLocationManageResourcesSection_deleteResourcesMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteResourcesInput;
};
export type organizationLocationManageResourcesSection_deleteResourcesMutation$data = {
  readonly deleteResources: {
    readonly resources: ReadonlyArray<{
      readonly id: string;
    }>;
  };
};
export type organizationLocationManageResourcesSection_deleteResourcesMutation = {
  response: organizationLocationManageResourcesSection_deleteResourcesMutation$data;
  variables: organizationLocationManageResourcesSection_deleteResourcesMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationLocationManageResourcesSection_deleteResourcesMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ResourcesPayload",
        "kind": "LinkedField",
        "name": "deleteResources",
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
              (v2/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationLocationManageResourcesSection_deleteResourcesMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ResourcesPayload",
        "kind": "LinkedField",
        "name": "deleteResources",
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
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "39e0833d7b8c3e7a6aa541b77dd13818",
    "id": null,
    "metadata": {},
    "name": "organizationLocationManageResourcesSection_deleteResourcesMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocationManageResourcesSection_deleteResourcesMutation(\n  $input: DeleteResourcesInput!\n) {\n  deleteResources(input: $input) {\n    resources {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "bb591451e284528ebf564c90911bc98d";

export default node;
