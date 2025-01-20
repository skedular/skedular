/**
 * @generated SignedSource<<be185fef56be6011131edd0cd1033702>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type oldOrganization_rootQuery$variables = {
  organizationId: string;
};
export type oldOrganization_rootQuery$data = {
  readonly organization: {
    readonly canModify: boolean;
    readonly id: string;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
};
export type oldOrganization_rootQuery = {
  response: oldOrganization_rootQuery$data;
  variables: oldOrganization_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "organizationId"
      }
    ],
    "concreteType": "OrganizationDetails",
    "kind": "LinkedField",
    "name": "organization",
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
        "name": "logoUrl",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "canModify",
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
    "name": "oldOrganization_rootQuery",
    "selections": (v1/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "oldOrganization_rootQuery",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "bc69396f4c1125081333d970c321f0ec",
    "id": null,
    "metadata": {},
    "name": "oldOrganization_rootQuery",
    "operationKind": "query",
    "text": "query oldOrganization_rootQuery(\n  $organizationId: String!\n) {\n  organization(id: $organizationId) {\n    id\n    name\n    logoUrl\n    canModify\n  }\n}\n"
  }
};
})();

(node as any).hash = "731458a89c9192d5151b24acd29d7647";

export default node;
