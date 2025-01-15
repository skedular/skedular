/**
 * @generated SignedSource<<ef73c0cfb938d5299cc821d0becb13cc>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type editOrganizationZoneDialog_rootQuery$variables = {
  zoneId: string;
};
export type editOrganizationZoneDialog_rootQuery$data = {
  readonly zone: {
    readonly color: string | null | undefined;
    readonly description: string | null | undefined;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type editOrganizationZoneDialog_rootQuery = {
  response: editOrganizationZoneDialog_rootQuery$data;
  variables: editOrganizationZoneDialog_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "zoneId"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "zoneId"
      }
    ],
    "concreteType": "OrganizationTagDetails",
    "kind": "LinkedField",
    "name": "zone",
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
    "name": "editOrganizationZoneDialog_rootQuery",
    "selections": (v1/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editOrganizationZoneDialog_rootQuery",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "35b86e0034f74662383871fc84fe754c",
    "id": null,
    "metadata": {},
    "name": "editOrganizationZoneDialog_rootQuery",
    "operationKind": "query",
    "text": "query editOrganizationZoneDialog_rootQuery(\n  $zoneId: String!\n) {\n  zone(id: $zoneId) {\n    id\n    name\n    description\n    color\n  }\n}\n"
  }
};
})();

(node as any).hash = "7abd44c236d23d088622e7d7e9906b1a";

export default node;
