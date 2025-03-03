/**
 * @generated SignedSource<<e99bdbf65977f5e83c17712ba61d33b9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationResourceTypeSystemType = "Desk" | "Room" | "%future added value";
export type UpdateResourceTypeInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
};
export type editOrganizationResourceTypeDialog_updateResourceTypeMutation$variables = {
  input: UpdateResourceTypeInput;
};
export type editOrganizationResourceTypeDialog_updateResourceTypeMutation$data = {
  readonly updateResourceType: {
    readonly organizationResourceType: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly systemType: OrganizationResourceTypeSystemType | null | undefined;
    };
  } | null | undefined;
};
export type editOrganizationResourceTypeDialog_updateResourceTypeMutation$rawResponse = {
  readonly updateResourceType: {
    readonly organizationResourceType: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly systemType: OrganizationResourceTypeSystemType | null | undefined;
    };
  } | null | undefined;
};
export type editOrganizationResourceTypeDialog_updateResourceTypeMutation = {
  rawResponse: editOrganizationResourceTypeDialog_updateResourceTypeMutation$rawResponse;
  response: editOrganizationResourceTypeDialog_updateResourceTypeMutation$data;
  variables: editOrganizationResourceTypeDialog_updateResourceTypeMutation$variables;
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
    "concreteType": "OrganizationResourceTypePayload",
    "kind": "LinkedField",
    "name": "updateResourceType",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationResourceTypeDetails",
        "kind": "LinkedField",
        "name": "organizationResourceType",
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
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "systemType",
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
    "name": "editOrganizationResourceTypeDialog_updateResourceTypeMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editOrganizationResourceTypeDialog_updateResourceTypeMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "c24c9b6ea3d3cae882270db83877a065",
    "id": null,
    "metadata": {},
    "name": "editOrganizationResourceTypeDialog_updateResourceTypeMutation",
    "operationKind": "mutation",
    "text": "mutation editOrganizationResourceTypeDialog_updateResourceTypeMutation(\n  $input: UpdateResourceTypeInput!\n) {\n  updateResourceType(input: $input) {\n    organizationResourceType {\n      id\n      name\n      description\n      color\n      systemType\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "eb4c39c8891cda7e0f8abaf5a90ed314";

export default node;
