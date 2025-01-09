/**
 * @generated SignedSource<<c804b2fd9a0b62e8c65bdb8bc9980e3d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type editOrganizationDeskTypeDialog_rootQuery$variables = {
  deskTypeId: string;
};
export type editOrganizationDeskTypeDialog_rootQuery$data = {
  readonly deskType: {
    readonly description: string | null | undefined;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type editOrganizationDeskTypeDialog_rootQuery = {
  response: editOrganizationDeskTypeDialog_rootQuery$data;
  variables: editOrganizationDeskTypeDialog_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "deskTypeId"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "deskTypeId"
      }
    ],
    "concreteType": "OrganizationTagDetails",
    "kind": "LinkedField",
    "name": "deskType",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editOrganizationDeskTypeDialog_rootQuery",
    "selections": (v1/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editOrganizationDeskTypeDialog_rootQuery",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "d6dadffe0c4a3e78633ccc7be50d5838",
    "id": null,
    "metadata": {},
    "name": "editOrganizationDeskTypeDialog_rootQuery",
    "operationKind": "query",
    "text": "query editOrganizationDeskTypeDialog_rootQuery(\n  $deskTypeId: String!\n) {\n  deskType(id: $deskTypeId) {\n    id\n    name\n    description\n  }\n}\n"
  }
};
})();

(node as any).hash = "a7ad6e601a8a377d65a231918023eaf3";

export default node;
