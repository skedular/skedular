/**
 * @generated SignedSource<<0d217529c095439f7c911c71310d5fec>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateCustomTagInput = {
  clientMutationId?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
};
export type customTagCard_updateCustomTagMutation$variables = {
  input: UpdateCustomTagInput;
};
export type customTagCard_updateCustomTagMutation$data = {
  readonly updateCustomTag: {
    readonly organizationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type customTagCard_updateCustomTagMutation = {
  response: customTagCard_updateCustomTagMutation$data;
  variables: customTagCard_updateCustomTagMutation$variables;
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
    "concreteType": "OrganizationTagPayload",
    "kind": "LinkedField",
    "name": "updateCustomTag",
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
    "name": "customTagCard_updateCustomTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "customTagCard_updateCustomTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "6ffa1c7a1c6f21749846cf063ce665a2",
    "id": null,
    "metadata": {},
    "name": "customTagCard_updateCustomTagMutation",
    "operationKind": "mutation",
    "text": "mutation customTagCard_updateCustomTagMutation(\n  $input: UpdateCustomTagInput!\n) {\n  updateCustomTag(input: $input) {\n    organizationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1e14728cf7ab23f940b9e0a5509c8545";

export default node;
