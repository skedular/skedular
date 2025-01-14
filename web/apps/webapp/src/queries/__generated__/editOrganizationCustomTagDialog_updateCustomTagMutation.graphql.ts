/**
 * @generated SignedSource<<8a41ffbfdc864f9722a9115ca256f50c>>
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
export type editOrganizationCustomTagDialog_updateCustomTagMutation$variables = {
  input: UpdateCustomTagInput;
};
export type editOrganizationCustomTagDialog_updateCustomTagMutation$data = {
  readonly updateCustomTag: {
    readonly organizationTag: {
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type editOrganizationCustomTagDialog_updateCustomTagMutation$rawResponse = {
  readonly updateCustomTag: {
    readonly organizationTag: {
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type editOrganizationCustomTagDialog_updateCustomTagMutation = {
  rawResponse: editOrganizationCustomTagDialog_updateCustomTagMutation$rawResponse;
  response: editOrganizationCustomTagDialog_updateCustomTagMutation$data;
  variables: editOrganizationCustomTagDialog_updateCustomTagMutation$variables;
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
    "name": "editOrganizationCustomTagDialog_updateCustomTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editOrganizationCustomTagDialog_updateCustomTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "743d1a238f27c33b96740dc7fcb3385e",
    "id": null,
    "metadata": {},
    "name": "editOrganizationCustomTagDialog_updateCustomTagMutation",
    "operationKind": "mutation",
    "text": "mutation editOrganizationCustomTagDialog_updateCustomTagMutation(\n  $input: UpdateCustomTagInput!\n) {\n  updateCustomTag(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1c4de7288b2d9d677a0d92bc0d089c9e";

export default node;
