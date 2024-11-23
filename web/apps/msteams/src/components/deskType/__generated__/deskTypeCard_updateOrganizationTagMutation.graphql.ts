/**
 * @generated SignedSource<<6e8c9264dd03e6c5a721956fb22e85e6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
  tagType: string;
};
export type deskTypeCard_updateOrganizationTagMutation$variables = {
  input: UpdateOrganizationTagInput;
};
export type deskTypeCard_updateOrganizationTagMutation$data = {
  readonly updateOrganizationTag: {
    readonly organizationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type deskTypeCard_updateOrganizationTagMutation = {
  response: deskTypeCard_updateOrganizationTagMutation$data;
  variables: deskTypeCard_updateOrganizationTagMutation$variables;
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
    "name": "updateOrganizationTag",
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
    "name": "deskTypeCard_updateOrganizationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "deskTypeCard_updateOrganizationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "968bb57151efd3df9615e63b4e63eaf9",
    "id": null,
    "metadata": {},
    "name": "deskTypeCard_updateOrganizationTagMutation",
    "operationKind": "mutation",
    "text": "mutation deskTypeCard_updateOrganizationTagMutation(\n  $input: UpdateOrganizationTagInput!\n) {\n  updateOrganizationTag(input: $input) {\n    organizationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a074b8dde5dd28d60abb3d14581c0107";

export default node;
