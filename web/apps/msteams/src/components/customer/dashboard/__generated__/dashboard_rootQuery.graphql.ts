/**
 * @generated SignedSource<<a1ce0e23a6c5388c4ab521313b7be755>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type dashboard_rootQuery$variables = {
  nullableOrganizationId?: string | null | undefined;
  organizationId: string;
};
export type dashboard_rootQuery$data = {
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
  }> | null | undefined;
  readonly myTeams: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
  }> | null | undefined;
  readonly organization: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type dashboard_rootQuery = {
  response: dashboard_rootQuery$data;
  variables: dashboard_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "nullableOrganizationId"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = [
  {
    "kind": "Variable",
    "name": "organizationId",
    "variableName": "nullableOrganizationId"
  }
],
v5 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v3/*: any*/)
],
v6 = [
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
      (v2/*: any*/),
      (v3/*: any*/)
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": (v4/*: any*/),
    "concreteType": "LocationDetails",
    "kind": "LinkedField",
    "name": "myLocations",
    "plural": true,
    "selections": [
      (v2/*: any*/),
      (v3/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationOrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v5/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": (v4/*: any*/),
    "concreteType": "TeamDetails",
    "kind": "LinkedField",
    "name": "myTeams",
    "plural": true,
    "selections": [
      (v2/*: any*/),
      (v3/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "TeamOrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v5/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "dashboard_rootQuery",
    "selections": (v6/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "dashboard_rootQuery",
    "selections": (v6/*: any*/)
  },
  "params": {
    "cacheID": "39b8613a637d2ce459fc923c278af9ed",
    "id": null,
    "metadata": {},
    "name": "dashboard_rootQuery",
    "operationKind": "query",
    "text": "query dashboard_rootQuery(\n  $organizationId: String!\n  $nullableOrganizationId: String\n) {\n  organization(id: $organizationId) {\n    id\n    name\n  }\n  myLocations(organizationId: $nullableOrganizationId) {\n    id\n    name\n    organization {\n      uniqueId\n      name\n    }\n  }\n  myTeams(organizationId: $nullableOrganizationId) {\n    id\n    name\n    organization {\n      uniqueId\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4ab56b36f805ba716e2437babef795a5";

export default node;
