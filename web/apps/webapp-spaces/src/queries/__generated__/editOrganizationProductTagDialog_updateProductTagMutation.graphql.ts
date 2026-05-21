/**
 * @generated SignedSource<<c2475a59e970fb48a38007a30fd74796>>
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
export type editOrganizationProductTagDialog_updateProductTagMutation$variables = {
  input: UpdateOrganizationTagInput;
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
    "cacheID": "4d22a25f3435b0e0153b57a58b44bb7e",
    "id": null,
    "metadata": {},
    "name": "editOrganizationProductTagDialog_updateProductTagMutation",
    "operationKind": "mutation",
    "text": "mutation editOrganizationProductTagDialog_updateProductTagMutation(\n  $input: UpdateOrganizationTagInput!\n) {\n  updateProductTag(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "94cedd6632e8db3edfbeb75aa9c0057e";

export default node;
