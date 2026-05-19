/**
 * @generated SignedSource<<db80c968e3e71d5d756147a0621c65f3>>
 * @lightSyntaxTransform
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
    readonly color: string | null | undefined;
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editOrganizationCustomTagDialog_rootQuery",
    "selections": (v1/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editOrganizationCustomTagDialog_rootQuery",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "b21efa5a97eacd95921963bb128f8a38",
    "id": null,
    "metadata": {},
    "name": "editOrganizationCustomTagDialog_rootQuery",
    "operationKind": "query",
    "text": "query editOrganizationCustomTagDialog_rootQuery(\n  $customTagId: String!\n) {\n  customTag(id: $customTagId) {\n    id\n    name\n    description\n    color\n  }\n}\n"
  }
};
})();

(node as any).hash = "399b511f78d55a42a8f33a48a1891114";

export default node;
