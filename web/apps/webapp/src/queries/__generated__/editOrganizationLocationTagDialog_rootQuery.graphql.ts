/**
 * @generated SignedSource<<accecf9fbaf98e8e1cb8bcdc6f6f40c1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type editOrganizationLocationTagDialog_rootQuery$variables = {
  locationTagId: string;
};
export type editOrganizationLocationTagDialog_rootQuery$data = {
  readonly locationTag: {
    readonly color: string | null | undefined;
    readonly description: string | null | undefined;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type editOrganizationLocationTagDialog_rootQuery = {
  response: editOrganizationLocationTagDialog_rootQuery$data;
  variables: editOrganizationLocationTagDialog_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationTagId"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "locationTagId"
      }
    ],
    "concreteType": "OrganizationTagDetails",
    "kind": "LinkedField",
    "name": "locationTag",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editOrganizationLocationTagDialog_rootQuery",
    "selections": (v1/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editOrganizationLocationTagDialog_rootQuery",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "1444b858f61c4534dc3ce3baabc9675d",
    "id": null,
    "metadata": {},
    "name": "editOrganizationLocationTagDialog_rootQuery",
    "operationKind": "query",
    "text": "query editOrganizationLocationTagDialog_rootQuery(\n  $locationTagId: String!\n) {\n  locationTag(id: $locationTagId) {\n    id\n    name\n    description\n    color\n  }\n}\n"
  }
};
})();

(node as any).hash = "6e7856262f93ae606daddb1f7678ae62";

export default node;
