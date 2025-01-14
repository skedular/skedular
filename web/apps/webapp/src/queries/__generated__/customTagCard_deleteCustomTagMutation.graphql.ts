/**
 * @generated SignedSource<<9963b0a795245f831ee492122c827610>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteCustomTagInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type customTagCard_deleteCustomTagMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteCustomTagInput;
};
export type customTagCard_deleteCustomTagMutation$data = {
  readonly deleteCustomTag: {
    readonly organizationTag: {
      readonly id: string;
    };
  } | null | undefined;
};
export type customTagCard_deleteCustomTagMutation = {
  response: customTagCard_deleteCustomTagMutation$data;
  variables: customTagCard_deleteCustomTagMutation$variables;
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
    "name": "customTagCard_deleteCustomTagMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "deleteCustomTag",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "organizationTag",
            "plural": false,
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
    "name": "customTagCard_deleteCustomTagMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "deleteCustomTag",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "organizationTag",
            "plural": false,
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
    "cacheID": "55a2819bb8113874f110bd38dc8fe7e4",
    "id": null,
    "metadata": {},
    "name": "customTagCard_deleteCustomTagMutation",
    "operationKind": "mutation",
    "text": "mutation customTagCard_deleteCustomTagMutation(\n  $input: DeleteCustomTagInput!\n) {\n  deleteCustomTag(input: $input) {\n    organizationTag {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3f4165ed645fee750bd6108ee4e54d1d";

export default node;
