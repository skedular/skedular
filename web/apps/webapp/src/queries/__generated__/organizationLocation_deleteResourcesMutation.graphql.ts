/**
 * @generated SignedSource<<19031d50c3628d3051ea3f5e344c4c58>>
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
export type organizationLocation_deleteResourcesMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteResourcesInput;
};
export type organizationLocation_deleteResourcesMutation$data = {
  readonly deleteResources: {
    readonly resources: ReadonlyArray<{
      readonly id: string;
    }>;
  };
};
export type organizationLocation_deleteResourcesMutation = {
  response: organizationLocation_deleteResourcesMutation$data;
  variables: organizationLocation_deleteResourcesMutation$variables;
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
    "name": "organizationLocation_deleteResourcesMutation",
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
    "name": "organizationLocation_deleteResourcesMutation",
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
    "cacheID": "a12199f6ae6ca207f71e0c9a1e875c81",
    "id": null,
    "metadata": {},
    "name": "organizationLocation_deleteResourcesMutation",
    "operationKind": "mutation",
    "text": "mutation organizationLocation_deleteResourcesMutation(\n  $input: DeleteResourcesInput!\n) {\n  deleteResources(input: $input) {\n    resources {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e879d592b8775c19c21d2b1db95c2893";

export default node;
