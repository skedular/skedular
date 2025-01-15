/**
 * @generated SignedSource<<5341c96b652a2437b43fe06080f1b0ce>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateCustomTagInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
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
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
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
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "description",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "color",
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
    "cacheID": "1fe7b7a69f64995e8e882ee9c9839208",
    "id": null,
    "metadata": {},
    "name": "customTagCard_updateCustomTagMutation",
    "operationKind": "mutation",
    "text": "mutation customTagCard_updateCustomTagMutation(\n  $input: UpdateCustomTagInput!\n) {\n  updateCustomTag(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b14bc4602316a92e9384de668a65be9c";

export default node;
