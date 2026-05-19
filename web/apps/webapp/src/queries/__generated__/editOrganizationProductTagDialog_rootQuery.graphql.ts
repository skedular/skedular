/**
 * @generated SignedSource<<469887e28bfd024f57e69c5e202b466e>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type editOrganizationProductTagDialog_rootQuery$variables = {
  productTagId: string;
};
export type editOrganizationProductTagDialog_rootQuery$data = {
  readonly productTag: {
    readonly color: string | null | undefined;
    readonly description: string | null | undefined;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type editOrganizationProductTagDialog_rootQuery = {
  response: editOrganizationProductTagDialog_rootQuery$data;
  variables: editOrganizationProductTagDialog_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "productTagId"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "productTagId"
      }
    ],
    "concreteType": "OrganizationTagDetails",
    "kind": "LinkedField",
    "name": "productTag",
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
    "name": "editOrganizationProductTagDialog_rootQuery",
    "selections": (v1/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editOrganizationProductTagDialog_rootQuery",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "c1a1a5db6b1410216554b25add3d9387",
    "id": null,
    "metadata": {},
    "name": "editOrganizationProductTagDialog_rootQuery",
    "operationKind": "query",
    "text": "query editOrganizationProductTagDialog_rootQuery(\n  $productTagId: String!\n) {\n  productTag(id: $productTagId) {\n    id\n    name\n    description\n    color\n  }\n}\n"
  }
};
})();

(node as any).hash = "0f78de2eb33f5a638733e84f3baba933";

export default node;
