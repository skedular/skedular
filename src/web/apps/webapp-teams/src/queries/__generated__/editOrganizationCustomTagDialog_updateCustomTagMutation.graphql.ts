/**
 * @generated SignedSource<<b8bdb5a170bb6b2891f2f1f31094d440>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationTagPatchField = "COLOR" | "DESCRIPTION" | "NAME" | "%future added value";
export type UpdateOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  description?: string | null | undefined;
  fieldsToUpdate: ReadonlyArray<OrganizationTagPatchField>;
  id: string;
  name?: string | null | undefined;
};
export type editOrganizationCustomTagDialog_updateCustomTagMutation$variables = {
  input: UpdateOrganizationTagInput;
};
export type editOrganizationCustomTagDialog_updateCustomTagMutation$data = {
  readonly updateCustomTag: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  };
};
export type editOrganizationCustomTagDialog_updateCustomTagMutation$rawResponse = {
  readonly updateCustomTag: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  };
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
    "name": "editOrganizationCustomTagDialog_updateCustomTagMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editOrganizationCustomTagDialog_updateCustomTagMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "78e50e1afebd5bedd17c3999aeb25361",
    "id": null,
    "metadata": {},
    "name": "editOrganizationCustomTagDialog_updateCustomTagMutation",
    "operationKind": "mutation",
    "text": "mutation editOrganizationCustomTagDialog_updateCustomTagMutation(\n  $input: UpdateOrganizationTagInput!\n) {\n  updateCustomTag(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "dfbb38fb7ce21164a14146b9e8bfe689";

export default node;
