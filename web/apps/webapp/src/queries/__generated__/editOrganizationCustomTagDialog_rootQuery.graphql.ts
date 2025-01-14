/**
 * @generated SignedSource<<f2b77a6e4e21fd96c7b7ce6d2ce784c3>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type editOrganizationCustomTagDialog_rootQuery$variables = {
  customTagId: string;
};
export type editOrganizationCustomTagDialog_rootQuery$data = {
  readonly customTag: {
    readonly description: string | null | undefined;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type editOrganizationCustomTagDialog_rootQuery = {
  response: editOrganizationCustomTagDialog_rootQuery$data;
  variables: editOrganizationCustomTagDialog_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "customTagId"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "customTagId"
      }
    ],
    "concreteType": "OrganizationTagDetails",
    "kind": "LinkedField",
    "name": "customTag",
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
    "name": "editOrganizationCustomTagDialog_rootQuery",
    "selections": (v1/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editOrganizationCustomTagDialog_rootQuery",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "f5ef0eed8d9b0564b27e3aa25e147f47",
    "id": null,
    "metadata": {},
    "name": "editOrganizationCustomTagDialog_rootQuery",
    "operationKind": "query",
    "text": "query editOrganizationCustomTagDialog_rootQuery(\n  $customTagId: String!\n) {\n  customTag(id: $customTagId) {\n    id\n    name\n    description\n  }\n}\n"
  }
};
})();

(node as any).hash = "db61c66e9380db688b586a95e14b8159";

export default node;
