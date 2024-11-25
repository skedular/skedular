/**
 * @generated SignedSource<<91c573ed2d7a3521f5b4bf357feb74d4>>
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
    readonly canViewAnalytics: boolean;
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
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "canViewAnalytics",
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
    "cacheID": "3113d824ba53969ae9aa1a3a4e4823ea",
    "id": null,
    "metadata": {},
    "name": "oldOrganization_rootQuery",
    "operationKind": "query",
    "text": "query oldOrganization_rootQuery(\n  $organizationId: String!\n) {\n  organization(id: $organizationId) {\n    id\n    name\n    logoUrl\n    canModify\n    canViewAnalytics\n  }\n}\n"
  }
};
})();

(node as any).hash = "477acec354e194a94501045b4eae263b";

export default node;
