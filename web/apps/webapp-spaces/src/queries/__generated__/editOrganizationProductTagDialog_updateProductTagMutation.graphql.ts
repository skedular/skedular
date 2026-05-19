/**
 * @generated SignedSource<<df281b1b16772ae7105bd9e2c7b318e7>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateProductTagInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
};
export type editOrganizationProductTagDialog_updateProductTagMutation$variables = {
  input: UpdateProductTagInput;
};
export type editOrganizationProductTagDialog_updateProductTagMutation$data = {
  readonly updateProductTag: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  };
};
export type editOrganizationProductTagDialog_updateProductTagMutation$rawResponse = {
  readonly updateProductTag: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  };
};
export type editOrganizationProductTagDialog_updateProductTagMutation = {
  rawResponse: editOrganizationProductTagDialog_updateProductTagMutation$rawResponse;
  response: editOrganizationProductTagDialog_updateProductTagMutation$data;
  variables: editOrganizationProductTagDialog_updateProductTagMutation$variables;
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
    "name": "updateProductTag",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editOrganizationProductTagDialog_updateProductTagMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editOrganizationProductTagDialog_updateProductTagMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "c3a98dc52f0b7db5ec155c2c3a975c15",
    "id": null,
    "metadata": {},
    "name": "editOrganizationProductTagDialog_updateProductTagMutation",
    "operationKind": "mutation",
    "text": "mutation editOrganizationProductTagDialog_updateProductTagMutation(\n  $input: UpdateProductTagInput!\n) {\n  updateProductTag(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3f1a52b4ff4c8e8d6978c29608b31bf6";

export default node;
