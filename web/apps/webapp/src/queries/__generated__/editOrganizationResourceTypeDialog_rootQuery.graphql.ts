/**
 * @generated SignedSource<<18f9ce4ead2d93abdc6cdb0f488e9450>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationResourceTypeSystemType = "Desk" | "Room" | "%future added value";
export type editOrganizationResourceTypeDialog_rootQuery$variables = {
  resourceTypeId: string;
};
export type editOrganizationResourceTypeDialog_rootQuery$data = {
  readonly resourceType: {
    readonly color: string | null | undefined;
    readonly description: string | null | undefined;
    readonly id: string;
    readonly name: string;
    readonly systemType: OrganizationResourceTypeSystemType | null | undefined;
  } | null | undefined;
};
export type editOrganizationResourceTypeDialog_rootQuery = {
  response: editOrganizationResourceTypeDialog_rootQuery$data;
  variables: editOrganizationResourceTypeDialog_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "resourceTypeId"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "resourceTypeId"
      }
    ],
    "concreteType": "OrganizationResourceTypeDetails",
    "kind": "LinkedField",
    "name": "resourceType",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editOrganizationResourceTypeDialog_rootQuery",
    "selections": (v1/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editOrganizationResourceTypeDialog_rootQuery",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "18d9ada44efb6e0514b6b5ce1dc89e84",
    "id": null,
    "metadata": {},
    "name": "editOrganizationResourceTypeDialog_rootQuery",
    "operationKind": "query",
    "text": "query editOrganizationResourceTypeDialog_rootQuery(\n  $resourceTypeId: String!\n) {\n  resourceType(id: $resourceTypeId) {\n    id\n    name\n    description\n    color\n    systemType\n  }\n}\n"
  }
};
})();

(node as any).hash = "681ee6dd244183814b82b28425c8145b";

export default node;
