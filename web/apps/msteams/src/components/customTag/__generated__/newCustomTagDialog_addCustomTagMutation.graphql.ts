/**
 * @generated SignedSource<<9d97d5a61433aabd3644a64194e55db7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddCustomTagInput = {
  clientMutationId?: string | null | undefined;
  description?: string | null | undefined;
  id?: string | null | undefined;
  name: string;
  organizationId: string;
};
export type newCustomTagDialog_addCustomTagMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddCustomTagInput;
};
export type newCustomTagDialog_addCustomTagMutation$data = {
  readonly addCustomTag: {
    readonly organizationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type newCustomTagDialog_addCustomTagMutation$rawResponse = {
  readonly addCustomTag: {
    readonly organizationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type newCustomTagDialog_addCustomTagMutation = {
  rawResponse: newCustomTagDialog_addCustomTagMutation$rawResponse;
  response: newCustomTagDialog_addCustomTagMutation$data;
  variables: newCustomTagDialog_addCustomTagMutation$variables;
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
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "organizationTag",
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
      "name": "name",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "newCustomTagDialog_addCustomTagMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "addCustomTag",
        "plural": false,
        "selections": [
          (v2/*: any*/)
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
    "name": "newCustomTagDialog_addCustomTagMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "addCustomTag",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "filters": null,
            "handle": "appendNode",
            "key": "",
            "kind": "LinkedHandle",
            "name": "organizationTag",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "OrganizationTagDetails"
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "1ec0c5aa5b60169462d90658c8d998f9",
    "id": null,
    "metadata": {},
    "name": "newCustomTagDialog_addCustomTagMutation",
    "operationKind": "mutation",
    "text": "mutation newCustomTagDialog_addCustomTagMutation(\n  $input: AddCustomTagInput!\n) {\n  addCustomTag(input: $input) {\n    organizationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2ddc6bdbc6cd2938f1acc42030752b7e";

export default node;
